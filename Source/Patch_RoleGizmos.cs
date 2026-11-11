using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;

namespace BetweenSus
{
    /// <summary>Role gizmos, shown on the main pawn only.</summary>
    [HarmonyPatch(typeof(Pawn), nameof(Pawn.GetGizmos))]
    public static class Patch_RoleGizmos
    {
        public static IEnumerable<Gizmo> Postfix(IEnumerable<Gizmo> __result, Pawn __instance)
        {
            foreach (var g in __result) yield return g;

            var roles = GameComponent_Roles.Instance;
            var ship = GameComponent_Ship.Instance;
            if (roles == null || ship == null || !roles.Active || ship.Ended) yield break;
            if (__instance != roles.MainPawn || __instance.Dead || !__instance.Spawned) yield break;

            var role = roles.RoleOf(__instance);
            if (role == null) yield break;

            if (role.team == Team.Impostor)
            {
                yield return Make("BS_Kill", "BS_KillDesc", "UI/Commands/Attack", () => ship.TryKillNearest(__instance));
                yield return Make("BS_Sabotage", "BS_SabotageDesc", "UI/Commands/Detonate", () =>
                    Find.WindowStack.Add(new FloatMenu(new List<FloatMenuOption>
                    {
                        new FloatMenuOption("BS_SabotageReactor".Translate(), () => Sabotage(ship, SabotageKind.Reactor, __instance)),
                        new FloatMenuOption("BS_SabotageO2".Translate(), () => Sabotage(ship, SabotageKind.O2, __instance)),
                    })));
            }

            switch (role.defName)
            {
                case "BS_Crewmate_Scientist":
                    yield return Make("BS_Vitals", "BS_VitalsDesc", "UI/Commands/Draft", ship.Vitals);
                    break;
                case "BS_Crewmate_Tracker":
                    yield return Make("BS_TrackGizmo", "BS_TrackDesc", "UI/Commands/Halt", () =>
                        Find.WindowStack.Add(new FloatMenu(ship.Living().Where(p => p != __instance)
                            .Select(p => new FloatMenuOption(p.LabelShort, () => ship.Track(p))).ToList())));
                    break;
                case "BS_Impostor_Shapeshifter":
                    yield return Make("BS_ShapeshiftGizmo", "BS_ShapeshiftDesc", "UI/Commands/Draft", () => ship.Shapeshift(__instance));
                    break;
                case "BS_Impostor_Phantom":
                    yield return Make("BS_VanishGizmo", "BS_VanishDesc", "UI/Commands/Halt", () => ship.Vanish(__instance));
                    break;
            }
        }

        private static void Sabotage(GameComponent_Ship ship, SabotageKind kind, Pawn by)
        {
            if (!ship.StartSabotage(kind, by))
                Messages.Message("BS_SabotageNotReady".Translate(), MessageTypeDefOf.RejectInput, false);
        }

        private static Command_Action Make(string label, string desc, string iconPath, System.Action action) =>
            new Command_Action
            {
                defaultLabel = label.Translate(),
                defaultDesc = desc.Translate(),
                icon = ContentFinder<Texture2D>.Get(iconPath, false) ?? BaseContent.BadTex,
                action = action
            };
    }
}
