using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using RimWorld.Planet;
using UnityEngine;
using Verse;
using Verse.AI;
using Verse.Sound;

namespace BetweenSus
{
    public enum SabotageKind { None, Reactor, O2, Doors }

    /// <summary>
    /// The game loop of Between Sus: tasks, sabotages, kills, bodies, meetings, votes and victory.
    /// Every choice here is a default made without the pitch (see Docs/DESIGN.md) and can change.
    /// </summary>
    public class GameComponent_Ship : GameComponent
    {
        public const int KillCooldown = 15000;
        public const int SabotageCooldown = 30000;
        public const int MeetingCooldown = 8000;

        public IntVec3 origin;
        private bool ended;
        private SabotageKind sabotage;
        private int sabotageEnd;
        private int sabotageCooldownEnd;
        private int meetingCooldownEnd;
        private List<int> fixedStationIds = new List<int>();
        private Dictionary<Pawn, int> killCooldownEnd = new Dictionary<Pawn, int>();
        private List<Pawn> keysWorking;
        private List<int> valuesWorking;
        private List<Pawn> ghosts = new List<Pawn>();
        private List<Pawn> guardianGhosts = new List<Pawn>();
        private Pawn protectedPawn;
        private List<Thing> lockedDoors = new List<Thing>();

        // Transient state, lost on reload.
        private readonly Dictionary<Pawn, int> evidence = new Dictionary<Pawn, int>();
        private readonly Dictionary<Pawn, int> suspicion = new Dictionary<Pawn, int>();
        private readonly Dictionary<Pawn, int> vanishUntil = new Dictionary<Pawn, int>();
        private readonly Dictionary<Pawn, KeyValuePair<Color, int>> shifted = new Dictionary<Pawn, KeyValuePair<Color, int>>();
        private readonly HashSet<int> reportedCorpses = new HashSet<int>();
        private Pawn tracked;
        private int trackUntil;

        public GameComponent_Ship(Game game) { }

        public static GameComponent_Ship Instance => Current.Game?.GetComponent<GameComponent_Ship>();

        public bool Ended => ended;
        public SabotageKind ActiveSabotage => sabotage;
        /// <summary>The emergency button is disabled during a sabotage, and bodies cannot be reported either.</summary>
        public bool CanCallMeeting => Find.TickManager.TicksGame >= meetingCooldownEnd && sabotage == SabotageKind.None;
        public bool IsGhost(Pawn p) => ghosts.Contains(p);
        public bool IsGuardianGhost(Pawn p) => guardianGhosts.Contains(p);
        public int EvidenceOn(Pawn p) => evidence.TryGetValue(p, out int n) ? n : 0;
        public const int EvidenceNeeded = 3;

        private static GameComponent_Roles Roles => GameComponent_Roles.Instance;
        private static Map Map => Find.CurrentMap;
        private static int Now => Find.TickManager.TicksGame;

        // ---------------------------------------------------------------- start

        public void BeginGame()
        {
            ended = false;
            sabotage = SabotageKind.None;
            fixedStationIds.Clear();
            killCooldownEnd.Clear();
            meetingCooldownEnd = Now + 2500;
            sabotageCooldownEnd = Now + 15000;
            foreach (var p in Living())
                if (Roles.TeamOf(p) == Team.Impostor)
                    killCooldownEnd[p] = Now + 12000;
        }

        // ---------------------------------------------------------------- queries

        public IEnumerable<Pawn> Living() =>
            Map?.mapPawns.FreeColonistsSpawned.Where(p => !p.Dead && !ghosts.Contains(p)) ?? Enumerable.Empty<Pawn>();

        private IEnumerable<Pawn> LivingCrew() => Living().Where(p => Roles.TeamOf(p) == Team.Crewmate);
        private IEnumerable<Pawn> LivingImpostors() => Living().Where(p => Roles.TeamOf(p) == Team.Impostor);

        public IEnumerable<Building_Station> Stations() =>
            Map.listerBuildings.allBuildingsColonist.OfType<Building_Station>();

        public bool IsFixed(Building_Station s) => fixedStationIds.Contains(s.thingIDNumber);

        public string RoomName(IntVec3 cell)
        {
            foreach (var room in ShipLayout.Rooms)
            {
                var r = new CellRect(origin.x + room.Rect.minX, origin.z + room.Rect.minZ, room.Rect.Width, room.Rect.Height);
                if (r.Contains(cell)) return room.Name;
            }
            return "";
        }

        private string RoomLabel(IntVec3 cell)
        {
            string name = RoomName(cell);
            return name.NullOrEmpty() ? "?" : GenText.SplitCamelCase(name);
        }

        // ---------------------------------------------------------------- tasks and sabotage

        public void OnTaskDone()
        {
            var all = Stations().Where(s => !s.IsFix).ToList();
            int done = all.Count(s => s.Done);
            Messages.Message("BS_TaskProgress".Translate(done, all.Count), MessageTypeDefOf.PositiveEvent, false);
        }

        public void MarkFixed(Building_Station s)
        {
            if (!fixedStationIds.Contains(s.thingIDNumber)) fixedStationIds.Add(s.thingIDNumber);
        }

        public bool StartSabotage(SabotageKind kind, Pawn by)
        {
            if (ended || sabotage != SabotageKind.None || Now < sabotageCooldownEnd) return false;

            if (kind == SabotageKind.Doors)
            {
                // Every door is locked for a while: nobody passes, the button is off, bodies cannot be reported.
                sabotage = kind;
                sabotageEnd = Now + 1800;
                lockedDoors.Clear();
                foreach (var door in Map.listerBuildings.allBuildingsColonist.OfType<Building_Door>())
                {
                    door.SetForbidden(true, false);
                    lockedDoors.Add(door);
                }
                Find.LetterStack.ReceiveLetter("BS_SabotageDoors".Translate(), "BS_SabotageDoorsText".Translate(), LetterDefOf.ThreatBig);
                return true;
            }

            var stations = Stations().Where(s => s.IsFix && s.FixKind == kind).ToList();
            if (stations.Count == 0) return false;

            sabotage = kind;
            fixedStationIds.Clear();
            sabotageEnd = Now + (kind == SabotageKind.Reactor ? 5400 : 7200);
            if (by != null && Roles.Is(by, "BS_Impostor_Pro")) sabotageEnd += 1800;
            string where = string.Join(", ", stations.Select(s => RoomLabel(s.Position)));
            Find.LetterStack.ReceiveLetter(
                (kind == SabotageKind.Reactor ? "BS_SabotageReactor" : "BS_SabotageO2").Translate(),
                "BS_SabotageText".Translate(where, ((sabotageEnd - Now) / 60).ToString()),
                LetterDefOf.ThreatBig);
            return true;
        }

        // ---------------------------------------------------------------- kills

        private bool Observed(Pawn killer, Pawn victim)
        {
            if (vanishUntil.TryGetValue(killer, out int until) && Now < until) return false;
            foreach (var p in LivingCrew())
            {
                if (p == killer || p == victim) continue;
                if (p.Position.DistanceTo(killer.Position) <= 14f
                    && GenSight.LineOfSight(p.Position, killer.Position, killer.Map))
                    return true;
            }
            return false;
        }

        private int WitnessCount(Pawn actor, Pawn except)
        {
            int n = 0;
            foreach (var p in LivingCrew())
                if (p != actor && p != except && p.Position.DistanceTo(actor.Position) <= 14f
                    && GenSight.LineOfSight(p.Position, actor.Position, actor.Map))
                    n++;
            return n;
        }

        /// <summary>Evidence against an impostor. At EvidenceNeeded, the crew can eliminate it without a vote.</summary>
        public void AddEvidence(Pawn impostor, int amount)
        {
            evidence[impostor] = EvidenceOn(impostor) + amount;
            var main = Roles.MainPawn;
            if (main != null && Roles.TeamOf(main) == Team.Crewmate && EvidenceOn(impostor) >= EvidenceNeeded)
                Messages.Message("BS_EvidenceReady".Translate(impostor.LabelShort), MessageTypeDefOf.PositiveEvent, false);
        }

        /// <summary>A fake task seen by crewmates is evidence: a real task would have shown progress.</summary>
        public void OnFakeTask(Pawn impostor)
        {
            int seen = WitnessCount(impostor, null);
            if (seen > 0) AddEvidence(impostor, 1);
        }

        /// <summary>Eliminates an impostor with enough evidence, without a vote.</summary>
        public void Accuse(Pawn suspect)
        {
            if (ended || suspect == null || EvidenceOn(suspect) < EvidenceNeeded || Roles.TeamOf(suspect) != Team.Impostor) return;
            Eject(suspect, "BS_EjectEvidence");
            CheckWin();
        }

        public IEnumerable<Pawn> Accusable() =>
            LivingImpostors().Where(p => EvidenceOn(p) >= EvidenceNeeded);

        public float KillCooldownFor(Pawn killer)
        {
            if (Roles.Is(killer, "BS_Impostor_Ninja")) return KillCooldown * 0.5f;
            if (Roles.Is(killer, "BS_Impostor_Pro")) return KillCooldown * 0.25f;
            return KillCooldown;
        }

        public bool KillReady(Pawn killer) =>
            !killCooldownEnd.TryGetValue(killer, out int t) || Now >= t;

        public void Kill(Pawn killer, Pawn victim)
        {
            killCooldownEnd[killer] = Now + (int)KillCooldownFor(killer);
            if (victim == protectedPawn)
            {
                // A guardian ghost shields one pawn from the next kill.
                protectedPawn = null;
                Messages.Message("BS_KillBlocked".Translate(), MessageTypeDefOf.NeutralEvent, false);
                return;
            }
            var pos = victim.Position;
            int witnesses = WitnessCount(killer, victim);

            victim.Kill(new DamageInfo(DamageDefOf.Cut, 9999f, 999f, -1f, killer));
            if (witnesses > 0) AddEvidence(killer, EvidenceNeeded * witnesses);

            foreach (var p in Living())
                if (p != victim && p.Position.DistanceTo(pos) <= 15f)
                    suspicion[p] = (suspicion.TryGetValue(p, out int s) ? s : 0) + 1;

            var main = Roles.MainPawn;
            if (main != null && !main.Dead && Roles.Is(main, "BS_Crewmate_Necromancer"))
                Messages.Message("BS_NecroHears".Translate(victim.LabelShort, RoomLabel(pos)), MessageTypeDefOf.NegativeEvent, false);

            if (Roles.Is(killer, "BS_Impostor_Acid"))
                victim.Corpse?.Destroy();

            if (killer == main)
                Messages.Message("BS_YouKilled".Translate(victim.LabelShort), MessageTypeDefOf.NeutralEvent, false);
            CheckWin();
        }

        /// <summary>Gizmo action for the main pawn: kills the nearest crewmate within 2 cells.</summary>
        public void TryKillNearest(Pawn killer)
        {
            if (!KillReady(killer)) { Messages.Message("BS_KillNotReady".Translate(), MessageTypeDefOf.RejectInput, false); return; }
            var victim = LivingCrew().Where(p => p != killer && p.Position.DistanceTo(killer.Position) <= 2.5f)
                .OrderBy(p => p.Position.DistanceTo(killer.Position)).FirstOrDefault();
            if (victim == null) { Messages.Message("BS_NoVictim".Translate(), MessageTypeDefOf.RejectInput, false); return; }
            Kill(killer, victim);
        }

        // ---------------------------------------------------------------- role abilities

        public void Shapeshift(Pawn pawn)
        {
            var suit = pawn.apparel.WornApparel.FirstOrDefault();
            var other = Living().Where(p => p != pawn).RandomElementWithFallback();
            var otherSuit = other?.apparel.WornApparel.FirstOrDefault();
            if (suit == null || otherSuit == null || shifted.ContainsKey(pawn)) return;
            shifted[pawn] = new KeyValuePair<Color, int>(suit.DrawColor, Now + 60000);
            suit.SetColor(otherSuit.DrawColor);
            pawn.Drawer.renderer.SetAllGraphicsDirty();
            Messages.Message("BS_Shapeshifted".Translate(other.LabelShort), MessageTypeDefOf.NeutralEvent, false);
        }

        public void Vanish(Pawn pawn)
        {
            vanishUntil[pawn] = Now + 900;
            Messages.Message("BS_Vanished".Translate(), MessageTypeDefOf.NeutralEvent, false);
        }

        public void Vitals()
        {
            string alive = string.Join(", ", Living().Select(p => p.LabelShort));
            Find.WindowStack.Add(new Dialog_MessageBox("BS_VitalsText".Translate(alive)));
        }

        public void Track(Pawn target)
        {
            tracked = target;
            trackUntil = Now + 60000;
            Messages.Message("BS_Tracking".Translate(target.LabelShort), MessageTypeDefOf.NeutralEvent, false);
        }

        // ---------------------------------------------------------------- meetings

        public void StartMeeting(Pawn reporter, Corpse body)
        {
            if (ended || Find.WindowStack.IsOpen<Window_Meeting>()) return;
            var roles = Roles;
            var cafeteria = ShipLayout.Rooms.First(r => r.Name == "Cafeteria").Rect.CenterCell;
            var center = new IntVec3(origin.x + cafeteria.x, 0, origin.z + cafeteria.z);
            foreach (var p in Living().ToList())
            {
                p.Position = CellFinder.RandomClosewalkCellNear(center, Map, 10);
                p.Notify_Teleported(true, true);
            }
            string title = body != null
                ? "BS_BodyReported".Translate(reporter?.LabelShort ?? "?", body.InnerPawn.LabelShort).ToString()
                : reporter != null
                    ? "BS_ButtonPressed".Translate(reporter.LabelShort).ToString()
                    : "BS_EmergencyMeeting".Translate().ToString();
            Find.WindowStack.Add(new Window_Meeting(title, Living().ToList(), roles.MainPawn));
        }

        /// <summary>Counts the votes (player choice plus AI) and ejects the pawn with the plurality.</summary>
        public void ResolveMeeting(Pawn playerChoice, string argument = null)
        {
            // The player's argument makes the others more suspicious of the accused.
            if (playerChoice != null && !argument.NullOrEmpty())
            {
                suspicion[playerChoice] = (suspicion.TryGetValue(playerChoice, out int bonus) ? bonus : 0) + 2;
                Messages.Message("BS_YouArgue".Translate(playerChoice.LabelShort, argument), MessageTypeDefOf.NeutralEvent, false);
            }
            var candidates = Living().ToList();
            var votes = new Dictionary<Pawn, int>();
            int skips = 0;
            foreach (var voter in candidates)
            {
                Pawn choice = voter == Roles.MainPawn ? playerChoice : AIVote(voter, candidates);
                if (choice == null) skips++;
                else votes[choice] = (votes.TryGetValue(choice, out int n) ? n : 0) + 1;
            }

            var ranked = votes.OrderByDescending(kv => kv.Value).ToList();
            Pawn ejected = null;
            if (ranked.Count > 0 && ranked[0].Value > skips && (ranked.Count == 1 || ranked[0].Value > ranked[1].Value))
                ejected = ranked[0].Key;

            foreach (var corpse in Map.listerThings.ThingsInGroup(ThingRequestGroup.Corpse).OfType<Corpse>().ToList())
                if (corpse.InnerPawn.IsColonist) corpse.Destroy();
            suspicion.Clear();
            meetingCooldownEnd = Now + MeetingCooldown;

            if (ejected == null)
                Messages.Message("BS_NobodyEjected".Translate(), MessageTypeDefOf.NeutralEvent, false);
            else
                Eject(ejected, "BS_EjectVote");
            CheckWin();
        }

        /// <summary>
        /// Ejects a pawn into the fuel of an engine, with a flash, a sound and a text about its team.
        /// The main pawn is not removed: it becomes a ghost, either a task ghost or a winged guardian ghost.
        /// </summary>
        private void Eject(Pawn pawn, string causeKey)
        {
            bool impostor = Roles.TeamOf(pawn) == Team.Impostor;
            var map = pawn.Map ?? Map;
            var engine = ShipLayout.Rooms.Where(r => r.Name == "UpperEngine" || r.Name == "LowerEngine").RandomElement();
            var c = engine.Rect.CenterCell;
            var cell = new IntVec3(origin.x + c.x, 0, origin.z + c.z);

            if (pawn.Spawned)
            {
                pawn.Position = cell;
                pawn.Notify_Teleported(true, true);
            }
            FleckMaker.Static(cell, map, FleckDefOf.ExplosionFlash, 12f);
            FleckMaker.ThrowSmoke(cell.ToVector3Shifted(), map, 2f);
            MoteMaker.ThrowText(cell.ToVector3Shifted(), map,
                (impostor ? "BS_WasImpostor" : "BS_WasNotImpostor").Translate(pawn.LabelShort),
                impostor ? Color.green : Color.red, 6f);
            DefDatabase<SoundDef>.GetNamedSilentFail(impostor ? "Quest_Succeded" : "Quest_Failed")?.PlayOneShotOnCamera(map);

            evidence.Remove(pawn);
            if (pawn == Roles.MainPawn)
            {
                ghosts.Add(pawn);
                bool guardian = Rand.Bool;
                if (guardian) guardianGhosts.Add(pawn);
                Messages.Message((guardian ? "BS_BecameGuardianGhost" : "BS_BecameTaskGhost").Translate(),
                    MessageTypeDefOf.NeutralEvent, false);
            }
            else
            {
                pawn.DeSpawn();
                Find.WorldPawns.PassToWorld(pawn, PawnDiscardDecideMode.Decide);
            }

            Find.LetterStack.ReceiveLetter(causeKey.Translate(pawn.LabelShort),
                (impostor ? "BS_WasImpostor" : "BS_WasNotImpostor").Translate(pawn.LabelShort) + "\n" +
                "BS_ImpostorsLeft".Translate(LivingImpostors().Count()),
                impostor ? LetterDefOf.PositiveEvent : LetterDefOf.NegativeEvent);
        }

        /// <summary>A guardian ghost shields one pawn from the next kill.</summary>
        public void Protect(Pawn target)
        {
            protectedPawn = target;
            Messages.Message("BS_Protecting".Translate(target.LabelShort), MessageTypeDefOf.PositiveEvent, false);
        }

        private Pawn AIVote(Pawn voter, List<Pawn> candidates)
        {
            if (Rand.Chance(0.2f)) return null;
            var pool = candidates.Where(p => p != voter);
            if (Roles.TeamOf(voter) == Team.Impostor)
            {
                if (Rand.Chance(0.3f)) return null;
                pool = pool.Where(p => Roles.TeamOf(p) == Team.Crewmate);
            }
            var list = pool.ToList();
            return list.Count == 0 ? null : list.RandomElementByWeight(p => 1f + (suspicion.TryGetValue(p, out int s) ? s : 0));
        }

        // ---------------------------------------------------------------- victory

        public bool CheckWin()
        {
            if (ended || !Roles.Active) return ended;
            int crew = LivingCrew().Count();
            int imps = LivingImpostors().Count();

            // Impostors win when only they and at most one crewmate remain.
            if (imps > 0 && crew <= 1) End(false, "BS_WinOutnumbered".Translate());
            // The crew wins when every impostor is gone, but not during a sabotage (except doors).
            else if (imps == 0 && (sabotage == SabotageKind.None || sabotage == SabotageKind.Doors))
                End(true, "BS_WinEjected".Translate());
            return ended;
        }

        private void End(bool crewWins, string reason)
        {
            ended = true;
            var roleLines = Roles.AllPawns().Select(p => p.LabelShort + ": " + Roles.RoleOf(p)?.LabelCap.ToString());
            string title = (crewWins ? "BS_CrewWins" : "BS_ImpostorsWin").Translate();
            string text = reason + "\n\n" + string.Join("\n", roleLines);
            Find.LetterStack.ReceiveLetter(title, text, crewWins ? LetterDefOf.PositiveEvent : LetterDefOf.ThreatBig);
            Find.WindowStack.Add(new Dialog_MessageBox(title + "\n\n" + text));
            Find.TickManager.Pause();
        }

        // ---------------------------------------------------------------- tick

        public override void GameComponentTick()
        {
            if (Now % 250 != 0 || ended || Roles == null || !Roles.Active || Map == null) return;
            Logic();
        }

        private void Logic()
        {
            if (CheckWin()) return;

            if (sabotage == SabotageKind.Doors && Now >= sabotageEnd)
            {
                foreach (var door in lockedDoors.OfType<Building_Door>())
                    door.SetForbidden(false, false);
                lockedDoors.Clear();
                sabotage = SabotageKind.None;
                sabotageCooldownEnd = Now + SabotageCooldown;
                Messages.Message("BS_SabotageFixed".Translate(), MessageTypeDefOf.PositiveEvent, false);
            }
            else if (sabotage == SabotageKind.Reactor || sabotage == SabotageKind.O2)
            {
                if (Now >= sabotageEnd) { End(false, "BS_WinSabotage".Translate()); return; }
                var fix = Stations().Where(s => s.IsFix && s.FixKind == sabotage).ToList();
                if (fix.Count > 0 && fix.All(IsFixed))
                {
                    sabotage = SabotageKind.None;
                    fixedStationIds.Clear();
                    sabotageCooldownEnd = Now + SabotageCooldown;
                    Messages.Message("BS_SabotageFixed".Translate(), MessageTypeDefOf.PositiveEvent, false);
                }
            }

            foreach (var kv in shifted.ToList())
                if (Now >= kv.Value.Value)
                {
                    var suit = kv.Key.apparel?.WornApparel.FirstOrDefault();
                    if (suit != null) { suit.SetColor(kv.Value.Key); kv.Key.Drawer.renderer.SetAllGraphicsDirty(); }
                    shifted.Remove(kv.Key);
                }

            if (tracked != null && Now < trackUntil && Now % 2500 < 250 && !tracked.Dead && tracked.Spawned)
                Messages.Message("BS_TrackReport".Translate(tracked.LabelShort, RoomLabel(tracked.Position)), MessageTypeDefOf.NeutralEvent, false);

            // The crew eliminates an impostor with enough evidence, without waiting for a vote.
            foreach (var suspect in Accusable().ToList())
                if (Rand.Chance(0.5f)) Accuse(suspect);
            if (ended) return;

            // Bodies cannot be reported during a sabotage, and the button is off.
            if ((sabotage == SabotageKind.None && ReportBodies()) || Find.WindowStack.IsOpen<Window_Meeting>()) return;

            // Colonists may press the emergency button on their own.
            if (CanCallMeeting && Rand.Chance(0.02f))
            {
                var presser = LivingCrew().RandomElementWithFallback();
                if (presser != null) { StartMeeting(presser, null); return; }
            }

            foreach (var imp in LivingImpostors().Where(p => p != Roles.MainPawn).ToList())
                AIImpostor(imp);
        }

        private bool ReportBodies()
        {
            foreach (var corpse in Map.listerThings.ThingsInGroup(ThingRequestGroup.Corpse).OfType<Corpse>())
            {
                if (!corpse.InnerPawn.IsColonist || reportedCorpses.Contains(corpse.thingIDNumber)) continue;
                var reporter = LivingCrew().FirstOrDefault(p =>
                    p.Position.DistanceTo(corpse.Position) <= 10f && GenSight.LineOfSight(p.Position, corpse.Position, Map));
                if (reporter == null) continue;
                reportedCorpses.Add(corpse.thingIDNumber);
                StartMeeting(reporter, corpse);
                return true;
            }
            return false;
        }

        private void AIImpostor(Pawn imp)
        {
            if (imp.Downed || !imp.Spawned) return;

            if (sabotage == SabotageKind.None && Now >= sabotageCooldownEnd && Rand.Chance(0.15f))
                StartSabotage(new[] { SabotageKind.Reactor, SabotageKind.O2, SabotageKind.Doors }.RandomElement(), imp);

            if (!KillReady(imp)) return;
            var victim = LivingCrew().Where(p => p != imp)
                .OrderBy(p => p.Position.DistanceTo(imp.Position)).FirstOrDefault();
            if (victim == null) return;

            if (imp.Position.DistanceTo(victim.Position) <= 2.5f)
            {
                if (!Observed(imp, victim)) Kill(imp, victim);
                return;
            }
            if (imp.CurJobDef != JobDefOf.Goto || Rand.Chance(0.3f))
                imp.jobs.StartJob(JobMaker.MakeJob(JobDefOf.Goto, victim.Position), JobCondition.InterruptForced);
        }

        // ---------------------------------------------------------------- save

        public override void ExposeData()
        {
            Scribe_Values.Look(ref origin, "origin");
            Scribe_Values.Look(ref ended, "ended");
            Scribe_Values.Look(ref sabotage, "sabotage");
            Scribe_Values.Look(ref sabotageEnd, "sabotageEnd");
            Scribe_Values.Look(ref sabotageCooldownEnd, "sabotageCooldownEnd");
            Scribe_Values.Look(ref meetingCooldownEnd, "meetingCooldownEnd");
            Scribe_Collections.Look(ref fixedStationIds, "fixedStationIds", LookMode.Value);
            Scribe_Collections.Look(ref ghosts, "ghosts", LookMode.Reference);
            Scribe_Collections.Look(ref guardianGhosts, "guardianGhosts", LookMode.Reference);
            Scribe_Collections.Look(ref lockedDoors, "lockedDoors", LookMode.Reference);
            Scribe_References.Look(ref protectedPawn, "protectedPawn");
            Scribe_Collections.Look(ref killCooldownEnd, "killCooldownEnd", LookMode.Reference, LookMode.Value,
                ref keysWorking, ref valuesWorking);
            if (Scribe.mode == LoadSaveMode.PostLoadInit)
            {
                fixedStationIds = fixedStationIds ?? new List<int>();
                ghosts = (ghosts ?? new List<Pawn>()).Where(p => p != null).ToList();
                guardianGhosts = (guardianGhosts ?? new List<Pawn>()).Where(p => p != null).ToList();
                lockedDoors = (lockedDoors ?? new List<Thing>()).Where(t => t != null).ToList();
                killCooldownEnd = (killCooldownEnd ?? new Dictionary<Pawn, int>())
                    .Where(kv => kv.Key != null).ToDictionary(kv => kv.Key, kv => kv.Value);
            }
        }
    }
}
