using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TTEngine.Editor.Models
{
    public class TileChangeCommand
    {
        public int Index { get; }
        public int OldValue { get; }
        public int NewValue { get; }

        public TileChangeCommand(int index, int oldValue, int newValue)
        {
            Index = index;
            OldValue = oldValue;
            NewValue = newValue;
        }

        public void Undo(int[] Tiles)
        {
            Tiles[Index] = OldValue;
        }

        public void Redo(int[] Tiles) 
        {
            Tiles[Index] = NewValue;
        }
    }
}
