using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;

namespace BetweenSus
{
    /// <summary>
    /// At game start: draws teams and roles, dresses everyone, opens the two screens for the main pawn.
    /// </summary>
    public class ScenPart_AssignRoles : ScenPart
    {
        public int impostorCount = 2;
        // Test hook, empty in the shipped scenario: roles forced by pawn spawn order, and which of them is the main pawn.
        public List<RoleDef> forcedRoles;
        public int forcedMainIndex;
        // Test hook, 0 in the shipped scenario: evidence given at start to the first impostor by spawn order.
        public int startEvidence;
        // Test hook, false in the shipped scenario: no meeting from a random button press (an open meeting window stops the logic).
        public bool noAutoButton;
        // Test hook, false in the shipped scenario: AI impostors never kill (a body report opens a meeting that stops the logic).
        public bool noKill;

        public override void PostGameStart()
        {
            var roles = GameComponent_Roles.Instance;
            var pawns = Find.CurrentMap?.mapPawns.FreeColonistsSpawned.ToList();
            if (roles == null || pawns == null || pawns.Count == 0) return;

            roles.Assign(pawns, Math.Min(impostorCount, Math.Max(1, pawns.Count / 4)), forcedRoles, forcedMainIndex);
            DressAll(pawns);
            Start(pawns);
            if (GameComponent_Ship.Instance != null)
            {
                GameComponent_Ship.Instance.noAutoButton = noAutoButton;
                GameComponent_Ship.Instance.noKill = noKill;
            }
            if (startEvidence > 0)
            {
                var suspect = pawns.FirstOrDefault(p => roles.TeamOf(p) == Team.Impostor);
                if (suspect != null) GameComponent_Ship.Instance?.AddEvidence(suspect, startEvidence);
            }
            Find.WindowStack.Add(new Window_TeamReveal(roles));
        }

        private static void Start(List<Pawn> pawns)
        {
            var tasks = DefDatabase<WorkTypeDef>.GetNamedSilentFail("BS_Tasks");
            if (tasks != null)
                foreach (var p in pawns)
                    if (p.workSettings != null && !p.WorkTypeIsDisabled(tasks))
                        p.workSettings.SetPriority(tasks, 1);
            GameComponent_Ship.Instance?.BeginGame();
        }

        private static void DressAll(List<Pawn> pawns)
        {
            var colors = Palette.SuitColors.InRandomOrder().ToList();
            for (int i = 0; i < pawns.Count; i++)
            {
                var pawn = pawns[i];
                pawn.apparel.DestroyAll();
                var color = colors[i % colors.Count];
                Wear(pawn, SuitDefOf.Apparel_ArmorRecon, color);
                // The helmet belongs to the Odyssey DLC: no DLC, no helmet, never a dependency.
                var helmet = DefDatabase<ThingDef>.GetNamedSilentFail("Apparel_VacsuitHelmet");
                if (helmet != null) Wear(pawn, helmet, color);
            }
        }

        private static void Wear(Pawn pawn, ThingDef def, Color color)
        {
            var apparel = (Apparel)ThingMaker.MakeThing(def, GenStuff.DefaultStuffFor(def));
            apparel.SetColor(color);
            pawn.apparel.Wear(apparel, dropReplacedApparel: false);
            pawn.apparel.Lock(apparel);
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref impostorCount, "impostorCount", 2);
            Scribe_Collections.Look(ref forcedRoles, "forcedRoles", LookMode.Def);
            Scribe_Values.Look(ref forcedMainIndex, "forcedMainIndex", 0);
            Scribe_Values.Look(ref startEvidence, "startEvidence", 0);
            Scribe_Values.Look(ref noAutoButton, "noAutoButton", false);
            Scribe_Values.Look(ref noKill, "noKill", false);
        }
    }

    [DefOf]
    public static class SuitDefOf
    {
        public static ThingDef Apparel_ArmorRecon;

        static SuitDefOf() => DefOfHelper.EnsureInitializedInCtor(typeof(SuitDefOf));
    }
}
