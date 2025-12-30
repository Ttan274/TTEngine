using System.Windows;
using System.Windows.Controls;

namespace TTEngine.Editor.Panels
{
    /// <summary>
    /// Interaction logic for PlayerSpawnInspector.xaml
    /// </summary>
    public partial class PlayerSpawnInspector : UserControl
    {
        public PlayerSpawnInspector(Point spawnPos)
        {
            InitializeComponent();

            PosText.Text = $"Position: X={spawnPos.X}, Y={spawnPos.Y}";
        }
    }
}
