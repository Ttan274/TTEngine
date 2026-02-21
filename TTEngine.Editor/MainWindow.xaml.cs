using System.IO;
using System.Text.Json;
using System.Windows;
using System.Windows.Input;
using TTEngine.Editor.EditorServices.EngineLauncher;
using TTEngine.Editor.EditorServices.Interaction;
using TTEngine.Editor.EditorServices.Rendering;
using TTEngine.Editor.Models.Editor;
using TTEngine.Editor.Models.Entity;
using TTEngine.Editor.Models.Interactable;
using TTEngine.Editor.Models.Selection;
using TTEngine.Editor.Models.Tile;
using TTEngine.Editor.Models.Trap;
using TTEngine.Editor.Models.Validation;
using TTEngine.Editor.Panels;
using TTEngine.Editor.Services;

namespace TTEngine.Editor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //Map
        private MapRenderer _renderer;
        private MapInteractionController _interaction;
        private SelectionController _selection;
        private int _brushSize = 1;
        public const string DEFAULT_MAP_ID = "Map_Default";

        //Assets
        private readonly AssetPanel _assetPanel;

        //Editor State
        public EditorState editorState { get; }
        
        //Engine Launcher
        private EngineLauncher _engineLauncher = new EngineLauncher();

        public MainWindow(EditorState editor, AssetPanel assetPanel)
        {
            InitializeComponent();
            editorState = editor;
            DataContext = editorState;
            _assetPanel = assetPanel;
            _assetPanel.AssetCreated += OnAssetCreated;
            AssetPanelHost.Content = _assetPanel;
            ContextSetup();
            WindowSetup();
            ChangeEventBindings();

            this.Closed += (_, _) =>
            {
                _engineLauncher.Stop();
            };

            _engineLauncher.EngineExited += () =>
            {
                Dispatcher.Invoke(() =>
                {
                    editorState.Console.Log("Engine Stopped");
                    UpdateRunBtn();
                });
            };
        }

        #region Setup

        private void WindowSetup()
        {
            _renderer = new MapRenderer(MapCanvas, editorState);
            _interaction = new MapInteractionController(editorState, () => _renderer.DrawStatic());
            _selection = new SelectionController(editorState);
           
            EnsureDefaultMap();
            _renderer.InitializeGrid();
            _renderer.DrawStatic();

            CommandBindings.Add(new CommandBinding(
              ApplicationCommands.Undo,
              (_, _) => _interaction.Undo()
            ));

            CommandBindings.Add(new CommandBinding(
                ApplicationCommands.Redo,
                (_, _) => _interaction.Redo()
            ));

            CommandBindings.Add(new CommandBinding(
                ApplicationCommands.Save,
                (_, _) => editorState.SaveActiveMap()
            ));
        }

        private void ContextSetup()
        {
            LayerEditor.DataContext = editorState;
            TileTools.DataContext = editorState;
            ConsoleEditor.DataContext = editorState;
            //ToolHost.BindEditor(editorState);
            Inspector.DataContext = editorState;
        }

        private void ChangeEventBindings()
        {
            editorState.MapSession.PropertyChanged += (_, e) =>
            {
                if (e.PropertyName == nameof(editorState.MapSession.ActiveMap))
                    _renderer.DrawStatic();
            };

            foreach (var layer in editorState.Layer.Layers)
                layer.VisibilityChanged += OnLayerVisibilityChanged;

            //Tile Tool Panel Events
            TileTools.ToolModeChanged += mode => editorState.Tool.CurrentToolMode = mode;
            TileTools.BrushSizechanged += size =>
            {
                _brushSize = size;
                _interaction.SetBrushSize(_brushSize);
            };
            TileTools.StartGameClicked += OnStartRequested;
        }

        #endregion

        #region Mouse Events

        private void Canvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (editorState.MapSession.IsDefaultMap)
                return;

            if (editorState.Layer.IsActiveLayerLocked)
                return;

            Point pos = e.GetPosition(MapCanvas);

            //Selection override
            if (Keyboard.IsKeyDown(Key.LeftAlt))
            {
                if (TryGetTilePosition(pos, out int sx, out int sy))
                    _selection.HandleSelection(sx, sy);
                return;
            }

            Mouse.Capture(MapCanvas);
           
            // Selection
            if (TryGetTilePosition(pos, out int x, out int y))
                _selection.HandleSelection(x, y);

            // Interaction
            _interaction.OnMouseDown(pos, e);
        }

        private void Canvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (editorState.MapSession.IsDefaultMap)
                return;

            _renderer.UpdateHover(e.GetPosition(MapCanvas), _brushSize);
            _interaction.OnMouseMove(e.GetPosition(MapCanvas), e);
        }

        private void Canvas_MouseUp(object sender, MouseButtonEventArgs e)
        {
            Mouse.Capture(null);
            _interaction.OnMouseUp();
        }

        private void Canvas_MouseLeave(object sender, MouseEventArgs e)
        {
            _renderer.MakeHoverUnvisible();
        }

        #endregion

        #region Button Events

        private void OnStartRequested()
        {
            if(_engineLauncher.IsRunning)
                StopGame();
            else
                StartEngine();
        }

        private void OnLayerVisibilityChanged(EditorLayer layer)
            => _renderer.DrawStatic();

        private void StartEngine()
        {
            var validation = ValidateMap();
            if (!validation.IsValid)
                return;

            var exePath = EditorPaths.GetEngineExe();

            editorState.Console.Log("Engine Starting...");

            _engineLauncher.Start(exePath, line =>
            {
                Dispatcher.Invoke(() =>
                {
                    editorState.Console.Log(line);
                });
            });

            editorState.Console.Log("Engine Launched");
            UpdateRunBtn();
        }

        private void StopGame()
        {
            editorState.Console.Log("Engine Stopped");
            _engineLauncher.Stop();
            UpdateRunBtn();
        }
        #endregion

        private bool TryGetTilePosition(Point pos, out int x, out int y)
        {
            x = 0;
            y = 0;

            if (editorState.MapSession.ActiveMap == null)
                return false;

            var map = editorState.MapSession.ActiveMap;

            x = (int)(pos.X / map.TileSize);
            y = (int)(pos.Y / map.TileSize);

            if (x < 0 || y < 0 || x >= map.Width || y >= map.Height)
                return false;

            return true;
        }

        private void EnsureDefaultMap()
        {
            return;

            if(!editorState.MapSession.MapService.Exists(DEFAULT_MAP_ID))
            {
                var model = new TileMapModel();
                model.Init();

                editorState.MapSession.MapService.Save(DEFAULT_MAP_ID, model);
            }

            var mapModel = editorState.MapSession.MapService.Load(DEFAULT_MAP_ID);

            editorState.MapSession.ActiveMapId = DEFAULT_MAP_ID;
            editorState.MapSession.ActiveMap = mapModel;
            editorState.Console.Log($"{DEFAULT_MAP_ID} is loaded.");
        }

        private EditorValidationResult ValidateMap()
        {
            editorState.Console.Clear();

            var result = EditorValidator.ValidateMap(editorState.MapSession.ActiveMap);

            foreach (var error in result.Errors)
                editorState.Console.Log(error);

            return result;
        }

        private void UpdateRunBtn() => TileTools.SetStartButtonTxt(_engineLauncher.IsRunning ? "Stop" : "Start");

        //Migh be changed
        private void OnAssetCreated(string path)
        {
            if (!File.Exists(path))
                return;

            object model = DeserializeByPath(path);

            if (model == null)
                return;

            object selection = CreateAssetSelection(model, path);

            editorState.CurrentSelection = selection;
        }

        private object DeserializeByPath(string path)
        {
            string json = File.ReadAllText(path);

            if (path.Contains("Entities"))
                return JsonSerializer.Deserialize<EntityDefinitionModel>(json);

            if (path.Contains("Tiles"))
                return JsonSerializer.Deserialize<TileDefinition>(json);

            if (path.Contains("Traps"))
                return JsonSerializer.Deserialize<TrapDefinition>(json);

            if (path.Contains("Interactables"))
                return JsonSerializer.Deserialize<InteractableDefinition>(json);

            return null;
        }

        private object CreateAssetSelection(object model, string path)
        {
            if (model is EntityDefinitionModel e)
                return new EntityAssetSelectionViewModel(e, path);

            if (model is TileDefinition t)
                return new TileAssetSelectionViewModel(t, path);

            if (model is InteractableDefinition i)
                return new InteractableAssetSelectionViewModel(i, path);

            if(model is TrapDefinition trap)
                return new TrapAssetSelectionViewModel(trap, path);

            return null;
        }
    }
}