using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;

namespace BetweenSus
{
    /// <summary>
    /// At game start: draws teams and roles, dresses everyone, opens the two screens for the main pawn.
    /// </summary>
    public class ScenPart_AssignRoles : ScenPart
    {
        public int impostorCount = 2;

        public override void PostGameStart()
        {
            var roles = GameComponent_Roles.Instance;
            var pawns = Find.CurrentMap?.mapPawns.FreeColonistsSpawned.ToList();
            if (roles == null || pawns == null || pawns.Count == 0) return;

            roles.Assign(pawns, Math.Min(impostorCount, Math.Max(1, pawns.Count / 4)));
            DressAll(pawns);
            Find.WindowStack.Add(new Window_TeamReveal(roles));
        }

        private static void DressAll(List<Pawn> pawns)
        {
            var colors = Palette.SuitColors.InRandomOrder().ToList();
            for (int i = 0; i < pawns.Count; i++)
            {
                var pawn = pawns[i];
                pawn.apparel.DestroyAll();
                var suit = (Apparel)ThingMaker.MakeThing(SuitDefOf.BS_CrewSuit);
                suit.SetColor(colors[i % colors.Count]);
                pawn.apparel.Wear(suit, dropReplacedApparel: false);
                pawn.apparel.Lock(suit);
            }
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref impostorCount, "impostorCount", 2);
        }
    }

    [DefOf]
    public static class SuitDefOf
    {
        public static ThingDef BS_CrewSuit;

        static SuitDefOf() => DefOfHelper.EnsureInitializedInCtor(typeof(SuitDefOf));
    }
}
