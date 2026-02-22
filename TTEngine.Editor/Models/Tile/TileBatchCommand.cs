using TTEngine.Editor.Models.Scene;

namespace TTEngine.Editor.Models.Tile
{
    public class TileBatchCommand
    {
        private readonly List<TileChangeCommand> _commands = new();

        public void Add(TileChangeCommand command)
        {
            _commands.Add(command);
        }

        public bool IsEmpty() => _commands.Count == 0;

        public void Undo(MapData map)
        {
            for (int i = _commands.Count - 1; i >= 0; i--)
                _commands[i].Undo(map);
        }

        public void Redo(MapData map)
        {
            foreach (var cmd in _commands)
                cmd.Redo(map);
        }
    }
}
