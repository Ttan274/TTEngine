using System.Windows;
using System.Windows.Controls;
using TTEngine.Editor.Models.Component;
using TTEngine.Editor.Models.Component.ComponentRegistry;
using TTEngine.Editor.Models.Selection;
using TTEngine.Editor.ViewModels.Panel;

namespace TTEngine.Editor.Panels.InspectorViews
{
    /// <summary>
    /// Interaction logic for GameObjectInspectorView.xaml
    /// </summary>
    public partial class GameObjectInspectorView : UserControl
    {
        public GameObjectInspectorView()
        {
            InitializeComponent();
            ComponentDropdown.ItemsSource = ComponentRegistry.Types;
        }

        private void RemoveComponent_Click(object sender, RoutedEventArgs e)
        {
            if(sender is Button btn && btn.DataContext is ComponentViewModel comp)
            {
                var vm = (GameObjectAssetSelectionViewModel)DataContext;
                vm.RemoveComponent(comp);
            }
        }

        private void ComponentDropdown_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(ComponentDropdown.SelectedItem is ComponentTypeInfo typeInfo)
            {
                var vm = (GameObjectAssetSelectionViewModel)DataContext;
                vm.AddComponent(typeInfo.TypeName);

                ComponentDropdown.SelectedItem = null;
            }
        }
    }
}
