using TTEngine.Editor.Models.Definitions;
using TTEngine.Editor.Models.Interactable;
using TTEngine.Editor.Models.Tile;
using TTEngine.Editor.Models.Trap;
using TTEngine.Editor.Services.IO;
using TTEngine.Editor.ViewModels.Panel;

namespace TTEngine.Editor.Models.Selection
{
    public abstract class AssetSelectionViewModel
    {
        public string FilePath { get; }

        protected AssetSelectionViewModel(string filePath)
        {
            FilePath = filePath;
        }

        public abstract object GetModel();

        public void Save()
        {
            JsonFileService.Save(FilePath, GetModel());
        }
    }

    public class EntityAssetSelectionViewModel : AssetSelectionViewModel
    {
        public EntityDefinition Model { get; }
        public GenericInspectorViewModel Inspector { get; }

        public EntityAssetSelectionViewModel(EntityDefinition model, string filePath) 
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

    public class TileAssetSelectionViewModel : AssetSelectionViewModel
    {
        public TileDefinition Model { get; }
        public GenericInspectorViewModel Inspector { get; }

        public TileAssetSelectionViewModel(TileDefinition model, string filePath)
            : base(filePath)
        {
            Model = model;
            Inspector = new GenericInspectorViewModel(model); ;
        }

        public override object GetModel()
        {
            return Model;
        }
    }

    public class InteractableAssetSelectionViewModel : AssetSelectionViewModel
    {
        public InteractableDefinition Model { get; }
        public GenericInspectorViewModel Inspector { get; }

        public InteractableAssetSelectionViewModel(InteractableDefinition model, string filePath)
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

    public class TrapAssetSelectionViewModel : AssetSelectionViewModel
    {
        public TrapDefinition Model { get; }
        public GenericInspectorViewModel Inspector { get; }

        public TrapAssetSelectionViewModel(TrapDefinition model, string filePath) 
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
}
