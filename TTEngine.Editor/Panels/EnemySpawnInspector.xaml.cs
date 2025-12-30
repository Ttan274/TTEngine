using System.Windows;
using System.Windows.Controls;

namespace TTEngine.Editor.Panels
{
    /// <summary>
    /// Interaction logic for EnemySpawnInspector.xaml
    /// </summary>
    public partial class EnemySpawnInspector : UserControl
    {
        public EnemySpawnInspector(Point pos)
        {
            InitializeComponent();

            PosText.Text = $"Position: X={pos.X}, Y={pos.Y}";
        }
    }
}
