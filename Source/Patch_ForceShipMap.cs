using System;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using RimWorld;
using RimWorld.Planet;
using Verse;

namespace BetweenSus
{
    /// <summary>
    /// The Between Sus scenario starts on the ship instead of a natural map.
    /// Arguments are found by type, so the patch does not depend on parameter names, and a null generator is replaced too.
    /// </summary>
    [HarmonyPatch(typeof(MapGenerator), nameof(MapGenerator.GenerateMap))]
    public static class Patch_ForceShipMap
    {
        public static void Prefix(MethodBase __originalMethod, object[] __args)
        {
            var parent = __args.OfType<MapParent>().FirstOrDefault();
            if (parent == null || parent.Faction != Faction.OfPlayer) return;
            if (Find.Scenario == null || !Find.Scenario.AllParts.Any(p => p is ScenPart_AssignRoles)) return;
            if (Find.Maps.Count > 0) return;

            var ship = DefDatabase<MapGeneratorDef>.GetNamedSilentFail("BS_ShipMap");
            if (ship == null) return;
            int index = Array.FindIndex(__originalMethod.GetParameters(), p => p.ParameterType == typeof(MapGeneratorDef));
            if (index >= 0) __args[index] = ship;
        }
    }
}
