using System.Windows.Controls;
using TTEngine.Editor.Models.Editor;

namespace TTEngine.Editor.Panels
{
    /// <summary>
    /// Interaction logic for EditorConsolePanel.xaml
    /// </summary>
    public partial class EditorConsolePanel : UserControl
    {
        public EditorConsolePanel()
        {
            InitializeComponent();

            Loaded += (_, _) =>
            {
                if(DataContext is EditorState state)
                {
                    state.Console.Messages.CollectionChanged += (_, _) =>
                    {
                        if (LogList.Items.Count > 0)
                            LogList.ScrollIntoView(LogList.Items[LogList.Items.Count - 1]);
                    };
                }
            };
        }
    }
}
