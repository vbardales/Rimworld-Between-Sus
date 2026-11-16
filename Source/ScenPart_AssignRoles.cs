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
        // Test hook, empty in the shipped scenario: roles forced by pawn name order, and which of them is the main pawn.
        public List<RoleDef> forcedRoles;
        public int forcedMainIndex;

        public override void PostGameStart()
        {
            var roles = GameComponent_Roles.Instance;
            var pawns = Find.CurrentMap?.mapPawns.FreeColonistsSpawned.ToList();
            if (roles == null || pawns == null || pawns.Count == 0) return;

            roles.Assign(pawns, Math.Min(impostorCount, Math.Max(1, pawns.Count / 4)), forcedRoles, forcedMainIndex);
            DressAll(pawns);
            Start(pawns);
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
        }
    }

    [DefOf]
    public static class SuitDefOf
    {
        public static ThingDef Apparel_ArmorRecon;

        static SuitDefOf() => DefOfHelper.EnsureInitializedInCtor(typeof(SuitDefOf));
    }
}
