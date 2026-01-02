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

        public void Undo(int[] tiles)
        {
            for (int i = _commands.Count - 1; i >= 0; i--)
                _commands[i].Undo(tiles);
        }

        public void Redo(int[] tiles)
        {
            foreach (var cmd in _commands)
                cmd.Redo(tiles);
        }
    }
}
