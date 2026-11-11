using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;

namespace BetweenSus
{
    /// <summary>The red button of the cafeteria: calls a meeting.</summary>
    public class Building_EmergencyButton : Building
    {
        public override IEnumerable<Gizmo> GetGizmos()
        {
            foreach (var g in base.GetGizmos()) yield return g;
            yield return new Command_Action
            {
                defaultLabel = "BS_CallMeeting".Translate(),
                defaultDesc = "BS_CallMeetingDesc".Translate(),
                icon = ContentFinder<Texture2D>.Get("UI/Commands/Attack", false) ?? BaseContent.BadTex,
                action = () =>
                {
                    var ship = GameComponent_Ship.Instance;
                    if (ship == null) return;
                    if (!ship.CanCallMeeting)
                        Messages.Message("BS_MeetingCooldown".Translate(), MessageTypeDefOf.RejectInput, false);
                    else
                        ship.StartMeeting(null, null);
                }
            };
        }
    }

    /// <summary>
    /// Security console (cameras: rooms with the names of who is inside) and admin console (rooms with a head count).
    /// </summary>
    public class Building_ShipConsole : Building
    {
        public override IEnumerable<Gizmo> GetGizmos()
        {
            foreach (var g in base.GetGizmos()) yield return g;
            bool security = def.defName == "BS_SecurityConsole";
            yield return new Command_Action
            {
                defaultLabel = (security ? "BS_Cameras" : "BS_ShipMap").Translate(),
                defaultDesc = (security ? "BS_CamerasDesc" : "BS_ShipMapDesc").Translate(),
                icon = ContentFinder<Texture2D>.Get("UI/Commands/Attack", false) ?? BaseContent.BadTex,
                action = () => Find.WindowStack.Add(new Dialog_MessageBox(Report(security)))
            };
        }

        private static string Report(bool names)
        {
            var ship = GameComponent_Ship.Instance;
            if (ship == null) return "";
            var lines = new List<string>();
            foreach (var room in ShipLayout.Rooms)
            {
                var inside = Find.CurrentMap.mapPawns.FreeColonistsSpawned
                    .Where(p => ship.RoomName(p.Position) == room.Name).ToList();
                if (inside.Count == 0) continue;
                string who = names ? string.Join(", ", inside.Select(p => p.LabelShort)) : inside.Count.ToString();
                lines.Add(GenText.SplitCamelCase(room.Name) + ": " + who);
            }
            return lines.Count == 0 ? "BS_NobodyAround".Translate().ToString() : string.Join("\n", lines);
        }
    }
}
