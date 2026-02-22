using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TTEngine.Editor.Models.Scene;

namespace TTEngine.Editor.Models.Tile
{
    public class TileChangeCommand
    {
        public int X { get; }
        public int Y { get; }
        public int OldValue { get; }
        public int NewValue { get; }

        public TileChangeCommand(int x, int y, int oldValue, int newValue)
        {
            X = x;
            Y = y;
            OldValue = oldValue;
            NewValue = newValue;
        }

        public void Undo(MapData map)
            => map.CollisionTiles[Y][X] = OldValue;

        public void Redo(MapData map) 
            => map.CollisionTiles[Y][X] = NewValue;
    }
}
