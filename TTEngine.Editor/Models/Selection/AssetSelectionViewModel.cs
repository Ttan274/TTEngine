using System.Collections.ObjectModel;
using System.Windows;
using TTEngine.Editor.Models.Component;
using TTEngine.Editor.Models.Component.ComponentRegistry;
using TTEngine.Editor.Models.Editor;
using TTEngine.Editor.Models.Tile;
using TTEngine.Editor.Services.IO;
using TTEngine.Editor.ViewModels.Panel;

namespace TTEngine.Editor.Models.Selection
{
    public abstract class AssetSelectionViewModel : ObservableObject
    {
        public string FilePath { get; }

        private bool _isDirty;
        public bool IsDirty
        {
            get => _isDirty;
            set => SetProperty(ref _isDirty, value);
        }

        protected AssetSelectionViewModel(string filePath)
        {
            FilePath = filePath;
        }

        public abstract object GetModel();

        public void Save()
        {
            JsonFileService.Save(FilePath, GetModel());
            IsDirty = false;
        }
    }

    public class TileAssetSelectionViewModel : AssetSelectionViewModel
    {
        public TileDefinition Model { get; }
        public GenericInspectorViewModel Inspector { get; }

        public TileAssetSelectionViewModel(TileDefinition model, string filePath)
            : base(filePath)
        {
            Model = model;
            Inspector = new GenericInspectorViewModel(model); 
        }

        public override object GetModel()
        {
            return Model;
        }
    }

    public class GameObjectAssetSelectionViewModel : AssetSelectionViewModel
    {
        public GameObject.GameObject Model { get; }
        public GenericInspectorViewModel RootInspector { get; }
        public ObservableCollection<ComponentViewModel> Components { get; }
            = new ObservableCollection<ComponentViewModel>();

        public GameObjectAssetSelectionViewModel(GameObject.GameObject model, string filePath) 
            : base(filePath)
        {
            Model = model;
            RootInspector = new GenericInspectorViewModel(model);
            RootInspector.OnValueChanged = () => IsDirty = true;

            foreach (var comp in Model.Components)
                CompVmCreator(comp);
        }

        public void AddComponent(string typeName)
        {
            var typeInfo = ComponentRegistry.Get(typeName);
            if (typeInfo == null)
                return;

            //Duplication control
            if(!typeInfo.AllowMultiple)
            {
                bool alreadyExists = Model.Components.Any(c => c.Type == typeName);

                if (alreadyExists)
                {
                    MessageBox.Show($"{typeName} already exists");
                    return;
                }
            }

            var instance = (ComponentBase)Activator.CreateInstance(typeInfo.DefType);

            Model.Components.Add(instance);
            CompVmCreator(instance);
            IsDirty = true;
        }

        public void RemoveComponent(ComponentViewModel comp)
        {
            if (!comp.IsRemovable)
                return;

            Model.Components.Remove(comp.Model);
            Components.Remove(comp);

            IsDirty = true;
        }

        public override object GetModel()
        {
            return Model;
        }

        //Helper 
        private void CompVmCreator(ComponentBase instance)
        {
            var compVm = new ComponentViewModel(instance);
            compVm.Inspector.OnValueChanged = () => IsDirty = true;
            Components.Add(compVm);
        }
    }
}
