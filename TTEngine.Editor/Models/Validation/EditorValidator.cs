using TTEngine.Editor.Enums;
using TTEngine.Editor.Models.Tile;

namespace TTEngine.Editor.Models.Validation
{
    public static class EditorValidator
    {
        //Map Placement
        public static bool CanPlacePlayer(TileMapModel map, int x, int y) => CanPlace(map, x, y);

        public static bool CanPlaceObject(TileMapModel map, int x, int y)
        {
            if (map.PlayerSpawn != null && map.PlayerSpawn.Position.X == x && map.PlayerSpawn.Position.Y == y)
                return false;

            return CanPlace(map, x, y);
        }
        
        //Helper
        private static bool CanPlace(TileMapModel map, int x, int y)
        {
            if (IsSolid(map, x, y))
                return false;

            if (map.EnemySpawns.Any(e => e.Position.X == x && e.Position.Y == y))
                return false;

            if (map.Interactables.Any(i => i.X == x && i.Y == y))
                return false;

            if (map.Traps.Any(t => t.X == x && t.Y == y))
                return false;

            return true;
        }

        private static bool IsSolid(TileMapModel map, int x, int y)
        {
            int index = map.GetIndex(x, y);
            int tileId = map.Layers[MapLayerType.Collision][index];

            return tileId == 1 || tileId == 2;
        }

        //Full Map Validation
        public static EditorValidationResult ValidateMap(TileMapModel map)
        {
            var result = new EditorValidationResult();

            if(map.PlayerSpawn == null)
            {
                result.Errors.Add("Player is not spawned");
                return result;
            }

            if (IsSolid(map, (int)map.PlayerSpawn.Position.X, (int)map.PlayerSpawn.Position.Y))
                result.Errors.Add("Player on solid tile");

            foreach (var enemy in map.EnemySpawns)
            {
                if (IsSolid(map, (int)enemy.Position.X, (int)enemy.Position.Y))
                    result.Errors.Add("Enemy on solid tile");
            }

            //Same process for trap and interactables

            return result;
        }

    }
}
