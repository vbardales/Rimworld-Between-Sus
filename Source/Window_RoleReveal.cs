using RimWorld;
using UnityEngine;
using Verse;

namespace BetweenSus
{
    /// <summary>Screen 2: the small role of the main pawn.</summary>
    public class Window_RoleReveal : Window
    {
        private readonly RoleDef role;

        public override Vector2 InitialSize => new Vector2(760f, 520f);

        public Window_RoleReveal(RoleDef role)
        {
            this.role = role;
            forcePause = true;
            absorbInputAroundWindow = true;
            closeOnClickedOutside = false;
            closeOnAccept = false;
            closeOnCancel = false;
            doCloseX = false;
        }

        public override void DoWindowContents(Rect inRect)
        {
            if (role == null) { Close(); return; }
            Color accent = role.team == Team.Impostor ? new Color(0.85f, 0.15f, 0.15f) : new Color(0.2f, 0.85f, 0.95f);

            Text.Font = GameFont.Medium;
            Text.Anchor = TextAnchor.MiddleCenter;
            if (role.Icon != null) GUI.DrawTexture(new Rect(inRect.width / 2f - 40f, 6f, 80f, 80f), role.Icon);
            GUI.color = accent;
            Widgets.Label(new Rect(0f, 88f, inRect.width, 40f), role.LabelCap.ToString().ToUpper());
            GUI.color = Color.white;
            Text.Font = GameFont.Small;
            Widgets.Label(new Rect(20f, 128f, inRect.width - 40f, 44f), role.description);
            Text.Anchor = TextAnchor.UpperLeft;

            float y = 180f;
            foreach (var ability in role.abilities)
            {
                GUI.color = accent;
                Widgets.Label(new Rect(30f, y, inRect.width - 60f, 24f), ability.label);
                GUI.color = Color.white;
                float h = Text.CalcHeight(ability.description, inRect.width - 60f);
                Widgets.Label(new Rect(30f, y + 24f, inRect.width - 60f, h), ability.description);
                y += 24f + h + 10f;
            }

            if (!role.tip.NullOrEmpty())
            {
                Text.Anchor = TextAnchor.MiddleCenter;
                GUI.color = Color.gray;
                Widgets.Label(new Rect(20f, inRect.height - 100f, inRect.width - 40f, 40f), role.tip);
                GUI.color = Color.white;
                Text.Anchor = TextAnchor.UpperLeft;
            }

            if (Widgets.ButtonText(new Rect(inRect.width / 2f - 100f, inRect.height - 50f, 200f, 40f),
                    "BS_Understood".Translate()))
                Close();
        }
    }
}
