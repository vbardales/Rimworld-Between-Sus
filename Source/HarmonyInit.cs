using HarmonyLib;
using Verse;

namespace BetweenSus
{
    [StaticConstructorOnStartup]
    public static class HarmonyInit
    {
        static HarmonyInit()
        {
            new Harmony("nelim.betweensus").PatchAll();
        }
    }
}
