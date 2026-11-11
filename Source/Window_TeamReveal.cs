using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;

namespace BetweenSus
{
    /// <summary>Screen 1: the team of the main pawn.</summary>
    public class Window_TeamReveal : Window
    {
        private readonly GameComponent_Roles roles;
        private readonly Team team;
        private readonly List<Pawn> shown;

        public override Vector2 InitialSize => new Vector2(900f, 520f);

        public Window_TeamReveal(GameComponent_Roles roles)
        {
            this.roles = roles;
            team = roles.TeamOf(roles.MainPawn) ?? Team.Crewmate;
            // Crewmate: the whole crew. Impostor: only the fellow impostors.
            shown = team == Team.Crewmate
                ? Find.CurrentMap.mapPawns.FreeColonists.ToList()
                : roles.Teammates(roles.MainPawn).Prepend(roles.MainPawn).ToList();
            forcePause = true;
            absorbInputAroundWindow = true;
            closeOnClickedOutside = false;
            closeOnAccept = false;
            closeOnCancel = false;
            doCloseX = false;
        }

        public override void DoWindowContents(Rect inRect)
        {
            bool impostor = team == Team.Impostor;
            Color accent = impostor ? new Color(0.85f, 0.15f, 0.15f) : new Color(0.2f, 0.85f, 0.95f);

            Text.Font = GameFont.Medium;
            Text.Anchor = TextAnchor.MiddleCenter;
            GUI.color = accent;
            Widgets.Label(new Rect(0f, 10f, inRect.width, 60f),
                (impostor ? "BS_TeamImpostor" : "BS_TeamCrewmate").Translate().ToString().ToUpper());
            GUI.color = Color.white;

            Text.Font = GameFont.Small;
            string subtitle = impostor
                ? "BS_TeamSubtitleImpostor".Translate().ToString()
                : "BS_TeamSubtitleCrewmate".Translate(roles.ImpostorCount).ToString();
            Widgets.Label(new Rect(0f, 70f, inRect.width, 30f), subtitle);
            Text.Anchor = TextAnchor.UpperLeft;

            const float cell = 80f;
            int perRow = Mathf.Max(1, (int)(inRect.width / cell));
            float rowStart = (inRect.width - Mathf.Min(shown.Count, perRow) * cell) / 2f;
            for (int i = 0; i < shown.Count; i++)
            {
                var r = new Rect(rowStart + (i % perRow) * cell, 120f + (i / perRow) * (cell + 24f), cell, cell);
                Widgets.ThingIcon(r, shown[i]);
                Text.Anchor = TextAnchor.UpperCenter;
                Widgets.Label(new Rect(r.x - 6f, r.yMax, r.width + 12f, 24f), shown[i].LabelShort);
                Text.Anchor = TextAnchor.UpperLeft;
            }

            if (Widgets.ButtonText(new Rect(inRect.width / 2f - 100f, inRect.height - 50f, 200f, 40f),
                    "BS_Continue".Translate()))
            {
                Close();
                Find.WindowStack.Add(new Window_RoleReveal(roles.RoleOf(roles.MainPawn)));
            }
        }
    }
}
