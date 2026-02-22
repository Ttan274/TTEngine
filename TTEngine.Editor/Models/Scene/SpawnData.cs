using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TTEngine.Editor.Models.Scene
{
    public class  SpawnDef
    {
        public int X { get; set; }
        public int Y { get; set; }
        public string DefinitionId { get; set; }
    }

    public class SpawnData
    {
        public SpawnDef Player { get; set; }
        public List<SpawnDef> Enemies { get; set; } = new();
        public List<SpawnDef> Interactables { get; set; } = new();
        public List<SpawnDef> Traps { get; set; } = new();
    }
}
