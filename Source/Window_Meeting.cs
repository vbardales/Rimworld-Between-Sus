using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;

namespace BetweenSus
{
    /// <summary>
    /// The meeting: the player votes for a pawn (with an optional argument) or skips, the others vote on their own.
    /// </summary>
    public class Window_Meeting : Window
    {
        private readonly string title;
        private readonly List<Pawn> candidates;
        private readonly Pawn main;
        private Pawn pending;
        private string argument = "";

        public override Vector2 InitialSize => new Vector2(900f, 560f);

        public Window_Meeting(string title, List<Pawn> candidates, Pawn main)
        {
            this.title = title;
            this.candidates = candidates;
            this.main = main;
            forcePause = true;
            absorbInputAroundWindow = true;
            closeOnClickedOutside = false;
            closeOnAccept = false;
            closeOnCancel = false;
            doCloseX = false;
        }

        public override void DoWindowContents(Rect inRect)
        {
            bool mainVotes = main != null && !main.Dead && candidates.Contains(main);

            Text.Font = GameFont.Medium;
            Text.Anchor = TextAnchor.MiddleCenter;
            Widgets.Label(new Rect(0f, 10f, inRect.width, 50f), title);
            Text.Font = GameFont.Small;

            if (pending != null)
            {
                DrawArgument(inRect);
                Text.Anchor = TextAnchor.UpperLeft;
                return;
            }

            Widgets.Label(new Rect(0f, 60f, inRect.width, 30f),
                (mainVotes ? "BS_VotePrompt" : "BS_VoteSpectator").Translate().ToString());
            Text.Anchor = TextAnchor.UpperLeft;

            const float cell = 100f;
            int perRow = Mathf.Max(1, (int)(inRect.width / cell));
            float rowStart = (inRect.width - Mathf.Min(candidates.Count, perRow) * cell) / 2f;
            for (int i = 0; i < candidates.Count; i++)
            {
                var r = new Rect(rowStart + (i % perRow) * cell, 110f + (i / perRow) * (cell + 50f), cell, cell);
                Widgets.ThingIcon(new Rect(r.x + 10f, r.y, 80f, 80f), candidates[i]);
                Text.Anchor = TextAnchor.UpperCenter;
                Widgets.Label(new Rect(r.x, r.y + 80f, cell, 22f), candidates[i].LabelShort);
                Text.Anchor = TextAnchor.UpperLeft;
                if (mainVotes && candidates[i] != main
                    && Widgets.ButtonText(new Rect(r.x + 10f, r.y + 102f, 80f, 24f), "BS_Vote".Translate()))
                    pending = candidates[i];
            }

            if (Widgets.ButtonText(new Rect(inRect.width / 2f - 100f, inRect.height - 50f, 200f, 40f),
                    (mainVotes ? "BS_Skip" : "BS_Continue").Translate()))
                Finish(null, null);
        }

        private void DrawArgument(Rect inRect)
        {
            Widgets.Label(new Rect(0f, 70f, inRect.width, 30f), "BS_ArgumentPrompt".Translate(pending.LabelShort).ToString());
            Text.Anchor = TextAnchor.UpperLeft;
            Widgets.ThingIcon(new Rect(inRect.width / 2f - 40f, 110f, 80f, 80f), pending);
            argument = Widgets.TextField(new Rect(80f, 220f, inRect.width - 160f, 32f), argument, 140);

            if (Widgets.ButtonText(new Rect(inRect.width / 2f - 210f, inRect.height - 50f, 200f, 40f), "BS_Back".Translate()))
                pending = null;
            if (Widgets.ButtonText(new Rect(inRect.width / 2f + 10f, inRect.height - 50f, 200f, 40f), "BS_ConfirmVote".Translate()))
                Finish(pending, argument.Trim());
        }

        private void Finish(Pawn choice, string text)
        {
            Close();
            GameComponent_Ship.Instance?.ResolveMeeting(choice, text);
        }
    }
}
