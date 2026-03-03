using TTEngine.Editor.Models.Definitions;
using TTEngine.Editor.Models.Scene;
using TTEngine.Editor.Models.Tile;

namespace TTEngine.Editor.Services.Editor
{
    public static class EditorValidator
    {
        //public static bool CanPlaceObject(Scene scene, int x, int y)
        //{
        //    var map = scene.Map;

        //    if (!IsInside(map, x, y))
        //        return false;

        //    if (IsSolid(map, x, y))
        //        return false;

        //    if (IsOccupied(scene, x, y))
        //        return false;

        //    return true;
        //}

        ////Helpers
        //private static bool IsInside(MapData map, int x, int y)
        //    => x >= 0 && y >= 0 && x < map.Width && y < map.Height;

        //private static bool IsSolid(MapData map, int x, int y)
        //{
        //    if (!IsInside(map, x, y))
        //        return true;

        //    int tileId = map.CollisionTiles[y][x];
        //    return tileId != 0;
        //}

        //private static bool IsOccupied(Scene scene, int x, int y)
        //{
        //    if (scene.Spawns.Player != null &&
        //        scene.Spawns.Player.X == x && scene.Spawns.Player.Y == y)
        //        return true;

        //    if (scene.Spawns.Enemies.Any(e => e.X == x && e.Y == y))
        //        return true;

        //    if (scene.Spawns.Interactables.Any(i => i.X == x && i.Y == y))
        //        return true;

        //    if (scene.Spawns.Traps.Any(t => t.X == x && t.Y == y))
        //        return true;

        //    return false;
        //}

        ////Full Map Validation
        //public static EditorValidationResult ValidateMap(Scene scene)
        //{
        //    var result = new EditorValidationResult();
        //    var map = scene.Map;

        //    if(scene.Spawns.Player == null)
        //        result.Errors.Add("Player is not spawned");

        //    if(scene.Spawns.Player != null)
        //    {
        //        if(IsSolid(map, scene.Spawns.Player.X, scene.Spawns.Player.Y))
        //            result.Errors.Add("Player on solid tile");
        //    }

        //    if (SolidTileCounter(map, scene.Spawns.Enemies, "Enemy", out string e))
        //        result.Errors.Add(e);
        //    if (SolidTileCounter(map, scene.Spawns.Interactables, "Interactable", out string i))
        //        result.Errors.Add(i);
        //    if (SolidTileCounter(map, scene.Spawns.Traps, "Trap", out string t))
        //        result.Errors.Add(t);

        //    return result;
        //}

        //private static bool SolidTileCounter(MapData map, List<SpawnDef> Spawns, string type, out string finalVersion)
        //{
        //    int counter = 0;
        //    foreach (var s in Spawns)
        //    {
        //        if (IsSolid(map, s.X, s.Y))
        //            counter++;
        //    }

        //    finalVersion = (counter != 0) ? $"{counter} type of {type} object on solid tile" : string.Empty;
        //    return counter > 0 && finalVersion != string.Empty;
        //}
    }
}
