using System.Collections.Generic;
using Verse;

namespace BetweenSus
{
    /// <summary>
    /// Ship layout in layout coordinates (origin at the bottom left, 95 x 47 cells).
    /// Interior rectangles are separated by one cell of wall. Doors sit on those wall cells.
    /// </summary>
    public static class ShipLayout
    {
        public const int Width = 95;
        public const int Height = 47;

        public struct Room
        {
            public string Name;
            public CellRect Rect;
            public string Label => ("BS_Room_" + Name).Translate().ToString();
            public Room(string name, int x0, int z0, int x1, int z1)
            {
                Name = name;
                Rect = CellRect.FromLimits(x0, z0, x1, z1);
            }
        }

        public static readonly List<Room> Rooms = new List<Room>
        {
            new Room("ReactorRear", 0, 17, 9, 34),
            new Room("UpperEngine", 11, 37, 26, 46),
            new Room("Security", 11, 25, 18, 35),
            new Room("Electrical", 20, 25, 26, 35),
            new Room("Medbay", 11, 15, 26, 23),
            new Room("LowerEngine", 11, 5, 26, 13),
            new Room("CorridorLeft", 28, 5, 29, 46),
            new Room("Cafeteria", 31, 32, 52, 46),
            new Room("Admin", 31, 19, 52, 30),
            new Room("Storage", 31, 6, 44, 17),
            new Room("Communications", 46, 6, 52, 17),
            new Room("CorridorRight", 54, 5, 55, 46),
            new Room("Weapons", 57, 36, 72, 46),
            new Room("O2", 57, 24, 72, 34),
            new Room("Shields", 57, 11, 72, 22),
            new Room("Navigation", 74, 24, 83, 34),
            new Room("ReactorFront", 85, 17, 94, 34),
        };

        public static readonly List<IntVec2> Doors = new List<IntVec2>
        {
            new IntVec2(10, 28), new IntVec2(19, 30), new IntVec2(14, 24),
            new IntVec2(27, 41), new IntVec2(27, 30), new IntVec2(27, 19), new IntVec2(27, 9),
            new IntVec2(30, 40), new IntVec2(30, 24), new IntVec2(30, 10), new IntVec2(45, 11),
            new IntVec2(53, 40), new IntVec2(53, 24), new IntVec2(53, 10),
            new IntVec2(56, 41), new IntVec2(56, 29), new IntVec2(56, 16),
            new IntVec2(60, 35), new IntVec2(60, 23), new IntVec2(73, 29), new IntVec2(84, 28),
        };
    }
}
