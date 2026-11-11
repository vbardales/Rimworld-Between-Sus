using System;
using System.Collections.Generic;
using RimWorld;
using Verse;

namespace BetweenSus
{
    /// <summary>
    /// Furnishes the ship, room by room. Offsets are relative to the bottom left interior cell of each room.
    /// A prop that does not fit (overlap, outside the room) is skipped.
    /// </summary>
    public static class ShipFurnisher
    {
        private sealed class Context
        {
            public Map Map;
            public IntVec3 Min;
            public CellRect Room;

            public bool Place(string defName, int dx, int dz, Rot4? rot = null)
            {
                var def = DefDatabase<ThingDef>.GetNamedSilentFail(defName);
                if (def == null) return false;
                var r = rot ?? Rot4.North;
                int w = r.IsHorizontal ? def.size.z : def.size.x;
                int h = r.IsHorizontal ? def.size.x : def.size.z;
                int minX = Min.x + dx, minZ = Min.z + dz;
                var rect = CellRect.FromLimits(minX, minZ, minX + w - 1, minZ + h - 1);
                if (!Room.Contains(new IntVec3(rect.minX, 0, rect.minZ)) || !Room.Contains(new IntVec3(rect.maxX, 0, rect.maxZ)))
                    return false;
                foreach (var c in rect)
                    if (c.GetEdifice(Map) != null) return false;

                var center = new IntVec3(minX + (w - 1) / 2, 0, minZ + (h - 1) / 2);
                var stuff = def.MadeFromStuff ? GenStuff.DefaultStuffFor(def) : null;
                var thing = ThingMaker.MakeThing(def, stuff);
                thing.SetFaction(Faction.OfPlayer);
                GenSpawn.Spawn(thing, center, Map, r);
                return true;
            }

            public void TableWithChairs(string tableDef, int dx, int dz)
            {
                var def = DefDatabase<ThingDef>.GetNamedSilentFail(tableDef);
                if (def == null || !Place(tableDef, dx, dz)) return;
                for (int x = 0; x < def.size.x; x++)
                {
                    Place("DiningChair", dx + x, dz - 1);
                    Place("DiningChair", dx + x, dz + def.size.z);
                }
            }

            public void Row(string defName, int dx, int dz, int count, int step)
            {
                for (int i = 0; i < count; i++) Place(defName, dx + i * step, dz);
            }
        }

        public static void Furnish(Map map, IntVec3 origin)
        {
            foreach (var room in ShipLayout.Rooms)
            {
                var rect = new CellRect(origin.x + room.Rect.minX, origin.z + room.Rect.minZ,
                    room.Rect.Width, room.Rect.Height);
                var ctx = new Context { Map = map, Min = new IntVec3(rect.minX, 0, rect.minZ), Room = rect };

                // Lighting first, so props never block it.
                for (int dx = Math.Min(3, rect.Width / 2); dx < rect.Width; dx += 6)
                    for (int dz = Math.Min(3, rect.Height / 2); dz < rect.Height; dz += 6)
                        ctx.Place("BS_CeilingLight", dx, dz);

                switch (room.Name)
                {
                    case "Cafeteria":
                        ctx.TableWithChairs("Table2x4c", 3, 10);
                        ctx.TableWithChairs("Table2x4c", 14, 10);
                        ctx.TableWithChairs("Table2x4c", 9, 6);
                        ctx.TableWithChairs("Table2x4c", 3, 2);
                        ctx.TableWithChairs("Table2x4c", 14, 2);
                        ctx.Place("BS_EmergencyButton", 10, 9);
                        ctx.Place("BS_TaskStation", 1, 12);
                        ctx.Place("BS_TaskStation", 19, 1);
                        break;
                    case "Admin":
                        ctx.TableWithChairs("Table3x3c", 9, 4);
                        ctx.Row("Shelf", 1, 10, 2, 4);
                        ctx.Place("BS_AdminConsole", 15, 7);
                        ctx.Place("BS_FixO2Station", 19, 1);
                        ctx.Place("BS_TaskStation", 1, 1);
                        break;
                    case "Storage":
                        ctx.Row("Shelf", 1, 9, 3, 4);
                        ctx.Row("Shelf", 1, 2, 3, 4);
                        ctx.Place("BS_TaskStation", 12, 10);
                        ctx.Place("BS_TaskStation", 12, 1);
                        break;
                    case "Weapons":
                        ctx.Row("CommsConsole", 2, 9, 3, 4);
                        ctx.Place("Shelf", 12, 2);
                        ctx.Place("BS_TaskStation", 14, 9);
                        ctx.Place("BS_TaskStation", 8, 2);
                        break;
                    case "Navigation":
                        ctx.Place("CommsConsole", 4, 9);
                        ctx.Place("CommsConsole", 7, 9);
                        ctx.Place("DiningChair", 4, 8);
                        ctx.Place("DiningChair", 7, 8);
                        ctx.Place("BS_TaskStation", 8, 2);
                        break;
                    case "O2":
                        ctx.Row("PlantPot", 2, 8, 6, 2);
                        ctx.Row("Battery", 2, 2, 3, 2);
                        ctx.Place("BS_FixO2Station", 13, 2);
                        ctx.Place("BS_TaskStation", 13, 8);
                        break;
                    case "Shields":
                        ctx.Row("Battery", 3, 3, 5, 2);
                        ctx.Place("Heater", 3, 8);
                        ctx.Place("Heater", 8, 8);
                        ctx.Place("BS_TaskStation", 13, 3);
                        ctx.Place("BS_TaskStation", 13, 9);
                        break;
                    case "Communications":
                        ctx.Place("CommsConsole", 2, 9);
                        ctx.Place("DiningChair", 2, 8);
                        ctx.Place("Shelf", 4, 2);
                        ctx.Place("BS_TaskStation", 5, 9);
                        break;
                    case "Electrical":
                        ctx.Row("Battery", 1, 8, 3, 2);
                        ctx.Row("Battery", 1, 3, 3, 2);
                        ctx.Place("BS_TaskStation", 5, 5);
                        ctx.Place("BS_TaskStation", 1, 1);
                        break;
                    case "Security":
                        ctx.Row("TubeTelevision", 1, 8, 3, 2);
                        ctx.Place("BS_SecurityConsole", 3, 3);
                        ctx.Place("BS_TaskStation", 6, 3);
                        ctx.Place("DiningChair", 3, 2);
                        break;
                    case "Medbay":
                        ctx.Row("HospitalBed", 1, 6, 4, 3);
                        ctx.Place("Shelf", 13, 6);
                        ctx.Place("BS_TaskStation", 13, 1);
                        break;
                    case "UpperEngine":
                    case "LowerEngine":
                        ctx.Row("Heater", 2, 2, 4, 3);
                        ctx.Row("Battery", 2, 6, 2, 3);
                        ctx.Place("BS_TaskStation", 13, 4);
                        break;
                    case "ReactorRear":
                    case "ReactorFront":
                        ctx.Place("Heater", 3, 8);
                        ctx.Place("Heater", 5, 8);
                        ctx.Place("Heater", 3, 11);
                        ctx.Place("Heater", 5, 11);
                        ctx.Place("BS_FixReactorStation", 1, 14);
                        ctx.Place("BS_TaskStation", 7, 3);
                        break;
                }
            }
        }
    }
}
