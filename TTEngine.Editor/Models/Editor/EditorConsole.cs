using System.Collections.ObjectModel;
using System.Windows;

namespace TTEngine.Editor.Models.Editor
{
    public class EditorConsole
    {
        private const int maxLogs = 100;

        public ObservableCollection<string> Messages { get; }
            = new ObservableCollection<string>();

        public void Clear()
        {
            Messages.Clear(); 
        }

        public void Log(string msg)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                Messages.Add(msg);

                if(Messages.Count > maxLogs)
                    Messages.RemoveAt(0);
            });
        }
    }
}
