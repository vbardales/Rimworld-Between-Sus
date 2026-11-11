using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.AI;

namespace BetweenSus
{
    /// <summary>
    /// A pawn may refuse a player order. No reason is shown: the player deduces from actions.
    /// </summary>
    [HarmonyPatch(typeof(Pawn_JobTracker), nameof(Pawn_JobTracker.TryTakeOrderedJob))]
    public static class Patch_OrderRefusal
    {
        public static bool Prefix(Job job, ref bool __result, Pawn ___pawn)
        {
            var roles = GameComponent_Roles.Instance;
            if (roles == null || !roles.Active || ___pawn == null || !___pawn.IsColonistPlayerControlled)
                return true;
            // A ghost can only do ship tasks and walk.
            var ship = GameComponent_Ship.Instance;
            if (ship != null && ship.IsGhost(___pawn) && job.def != StationJobDefOf.BS_DoStation && job.def != JobDefOf.Goto)
            {
                MoteMaker.ThrowText(___pawn.DrawPos, ___pawn.MapHeld, "BS_GhostCannot".Translate(), Color.white, 3.5f);
                __result = false;
                return false;
            }
            if (Rand.Value >= roles.RefusalChance(___pawn, job))
                return true;

            MoteMaker.ThrowText(___pawn.DrawPos, ___pawn.MapHeld, "BS_Refuses".Translate(), Color.white, 3.5f);
            __result = false;
            return false;
        }
    }
}
