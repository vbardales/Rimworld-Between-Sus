using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using RimWorld.Planet;
using UnityEngine;
using Verse;
using Verse.AI;

namespace BetweenSus
{
    public enum SabotageKind { None, Reactor, O2 }

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

        // Transient state, lost on reload.
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
        public bool CanCallMeeting => Find.TickManager.TicksGame >= meetingCooldownEnd;

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
            Map?.mapPawns.FreeColonistsSpawned.Where(p => !p.Dead) ?? Enumerable.Empty<Pawn>();

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
            var pos = victim.Position;
            var map = victim.Map;

            victim.Kill(new DamageInfo(DamageDefOf.Cut, 9999f, 999f, -1f, killer));

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
                : "BS_EmergencyMeeting".Translate().ToString();
            Find.WindowStack.Add(new Window_Meeting(title, Living().ToList(), roles.MainPawn));
        }

        /// <summary>Counts the votes (player choice plus AI) and ejects the pawn with the plurality.</summary>
        public void ResolveMeeting(Pawn playerChoice)
        {
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
            {
                bool impostor = Roles.TeamOf(ejected) == Team.Impostor;
                ejected.DeSpawn();
                Find.WorldPawns.PassToWorld(ejected, PawnDiscardDecideMode.Decide);
                int left = LivingImpostors().Count();
                Find.LetterStack.ReceiveLetter("BS_Ejected".Translate(ejected.LabelShort),
                    (impostor ? "BS_WasImpostor" : "BS_WasNotImpostor").Translate(ejected.LabelShort) + "\n" +
                    "BS_ImpostorsLeft".Translate(left),
                    impostor ? LetterDefOf.PositiveEvent : LetterDefOf.NegativeEvent);
            }
            CheckWin();
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
            var tasks = Stations().Where(s => !s.IsFix).ToList();

            if (imps == 0) End(true, "BS_WinEjected".Translate());
            else if (imps >= crew) End(false, "BS_WinOutnumbered".Translate());
            else if (tasks.Count > 0 && tasks.All(s => s.Done)) End(true, "BS_WinTasks".Translate());
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

            if (sabotage != SabotageKind.None)
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

            if (ReportBodies() || Find.WindowStack.IsOpen<Window_Meeting>()) return;

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
                StartSabotage(Rand.Bool ? SabotageKind.Reactor : SabotageKind.O2, imp);

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
            Scribe_Collections.Look(ref killCooldownEnd, "killCooldownEnd", LookMode.Reference, LookMode.Value,
                ref keysWorking, ref valuesWorking);
            if (Scribe.mode == LoadSaveMode.PostLoadInit)
            {
                fixedStationIds = fixedStationIds ?? new List<int>();
                killCooldownEnd = (killCooldownEnd ?? new Dictionary<Pawn, int>())
                    .Where(kv => kv.Key != null).ToDictionary(kv => kv.Key, kv => kv.Value);
            }
        }
    }
}
