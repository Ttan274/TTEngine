using System.Collections.ObjectModel;

namespace TTEngine.Editor.Models.Editor
{
    public class EditorConsole
    {
        public ObservableCollection<string> Messages { get; }
            = new ObservableCollection<string>();

        public void Clear()
        {
            Messages.Clear(); 
        }

        public void Log(string msg)
        {
            Messages.Add(msg);
        }
    }
}
