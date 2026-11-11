using System.Linq;
using RimWorld;
using Verse;

namespace BetweenSus
{
    /// <summary>
    /// Builds the ship: floors, walls, doors and roof from ShipLayout, then sets the player start in the cafeteria.
    /// </summary>
    public class GenStep_ShipLayout : GenStep
    {
        public override int SeedPart => 83412907;

        public override void Generate(Map map, GenStepParams parms)
        {
            var origin = new IntVec3(
                map.Size.x / 2 - ShipLayout.Width / 2,
                0,
                map.Size.z / 2 - ShipLayout.Height / 2);
            if (origin.x < 1 || origin.z < 1)
            {
                Log.Error("[Between Sus] Map too small for the ship layout (needs at least " +
                          (ShipLayout.Width + 2) + " x " + (ShipLayout.Height + 2) + ").");
                return;
            }

            var interior = new bool[map.Size.x, map.Size.z];
            foreach (var room in ShipLayout.Rooms)
                foreach (var c in room.Rect.Cells)
                    interior[origin.x + c.x, origin.z + c.z] = true;

            var wall = ThingDefOf.Wall;
            var door = ThingDefOf.Door;
            var steel = ThingDefOf.Steel;
            var doorCells = ShipLayout.Doors
                .Select(d => new IntVec3(origin.x + d.x, 0, origin.z + d.z))
                .ToHashSet();

            for (int x = origin.x - 1; x <= origin.x + ShipLayout.Width; x++)
            {
                for (int z = origin.z - 1; z <= origin.z + ShipLayout.Height; z++)
                {
                    var cell = new IntVec3(x, 0, z);
                    bool isInterior = interior[x, z];
                    if (!isInterior && !TouchesInterior(interior, x, z, map.Size)) continue;

                    foreach (var thing in cell.GetThingList(map).ToList())
                        if (!(thing is Pawn)) thing.Destroy();

                    map.terrainGrid.SetTerrain(cell, TerrainDefOf.MetalTile);
                    map.roofGrid.SetRoof(cell, RoofDefOf.RoofConstructed);

                    if (isInterior) continue;

                    var def = doorCells.Contains(cell) ? door : wall;
                    var thing2 = ThingMaker.MakeThing(def, steel);
                    thing2.SetFaction(Faction.OfPlayer);
                    GenSpawn.Spawn(thing2, cell, map);
                }
            }

            var cafeteria = ShipLayout.Rooms.First(r => r.Name == "Cafeteria").Rect.CenterCell;
            MapGenerator.PlayerStartSpot = new IntVec3(origin.x + cafeteria.x, 0, origin.z + cafeteria.z);
        }

        private static bool TouchesInterior(bool[,] interior, int x, int z, IntVec3 size)
        {
            for (int dx = -1; dx <= 1; dx++)
                for (int dz = -1; dz <= 1; dz++)
                {
                    int nx = x + dx, nz = z + dz;
                    if (nx >= 0 && nz >= 0 && nx < size.x && nz < size.z && interior[nx, nz]) return true;
                }
            return false;
        }
    }
}
