using System.Collections.Generic;
using UnityEngine;

namespace BetweenSus
{
    /// <summary>One suit colour per pawn, no duplicate.</summary>
    public static class Palette
    {
        public static readonly List<Color> SuitColors = new List<Color>
        {
            new Color32(197, 17, 17, 255),    // red
            new Color32(19, 46, 209, 255),    // blue
            new Color32(17, 127, 45, 255),    // green
            new Color32(237, 84, 186, 255),   // pink
            new Color32(239, 125, 13, 255),   // orange
            new Color32(245, 245, 87, 255),   // yellow
            new Color32(63, 71, 78, 255),     // black
            new Color32(214, 224, 240, 255),  // white
            new Color32(107, 47, 188, 255),   // purple
            new Color32(113, 73, 30, 255),    // brown
            new Color32(56, 254, 220, 255),   // cyan
            new Color32(80, 239, 57, 255),    // lime
        };
    }
}
