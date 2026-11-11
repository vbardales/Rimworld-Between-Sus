using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;

namespace BetweenSus
{
    /// <summary>The meeting: the player votes for a pawn or skips, the others vote on their own.</summary>
    public class Window_Meeting : Window
    {
        private readonly string title;
        private readonly List<Pawn> candidates;
        private readonly Pawn main;

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
            bool mainAlive = main != null && !main.Dead && candidates.Contains(main);

            Text.Font = GameFont.Medium;
            Text.Anchor = TextAnchor.MiddleCenter;
            Widgets.Label(new Rect(0f, 10f, inRect.width, 50f), title);
            Text.Font = GameFont.Small;
            Widgets.Label(new Rect(0f, 60f, inRect.width, 30f),
                (mainAlive ? "BS_VotePrompt" : "BS_VoteSpectator").Translate().ToString());
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
                if (mainAlive && candidates[i] != main && Widgets.ButtonText(new Rect(r.x + 10f, r.y + 102f, 80f, 24f), "BS_Vote".Translate()))
                    Finish(candidates[i]);
            }

            var skip = new Rect(inRect.width / 2f - 100f, inRect.height - 50f, 200f, 40f);
            if (Widgets.ButtonText(skip, (mainAlive ? "BS_Skip" : "BS_Continue").Translate()))
                Finish(null);
        }

        private void Finish(Pawn choice)
        {
            Close();
            GameComponent_Ship.Instance?.ResolveMeeting(choice);
        }
    }
}
