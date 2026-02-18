using System.Windows;
using System.Windows.Input;
using TTEngine.Editor.EditorServices.EngineLauncher;
using TTEngine.Editor.EditorServices.Interaction;
using TTEngine.Editor.EditorServices.Rendering;
using TTEngine.Editor.Models.Editor;
using TTEngine.Editor.Models.Tile;
using TTEngine.Editor.Models.Validation;
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

        //Editor State
        public EditorState editorState { get; } = new EditorState();
        
        //Engine Launcher
        private EngineLauncher _engineLauncher = new EngineLauncher();

        public MainWindow()
        {
            InitializeComponent();
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
            ToolHost.BindEditor(editorState);
            Inspector.DataContext = editorState;
        }

        private void ChangeEventBindings()
        {
            editorState.PropertyChanged += (_, e) =>
            {
                if (e.PropertyName == nameof(editorState.ActiveMap))
                    _renderer.DrawStatic();
            };

            foreach (var layer in editorState.Layers)
                layer.VisibilityChanged += OnLayerVisibilityChanged;

            //Tile Tool Panel Events
            TileTools.ToolModeChanged += mode => editorState.CurrentToolMode = mode;
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
            if (editorState.IsDefaultMap)
                return;

            if (editorState.IsActiveLayerLocked)
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
            if (editorState.IsDefaultMap)
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

            if (editorState.ActiveMap == null)
                return false;

            var map = editorState.ActiveMap;

            x = (int)(pos.X / map.TileSize);
            y = (int)(pos.Y / map.TileSize);

            if (x < 0 || y < 0 || x >= map.Width || y >= map.Height)
                return false;

            return true;
        }

        private void EnsureDefaultMap()
        {
            if(!editorState.MapService.Exists(DEFAULT_MAP_ID))
            {
                var model = new TileMapModel();
                model.Init();

                editorState.MapService.Save(DEFAULT_MAP_ID, model);
            }

            var mapModel = editorState.MapService.Load(DEFAULT_MAP_ID);

            editorState.ActiveMapId = DEFAULT_MAP_ID;
            editorState.ActiveMap = mapModel;
            editorState.Console.Log($"{DEFAULT_MAP_ID} is loaded.");
        }

        private EditorValidationResult ValidateMap()
        {
            editorState.Console.Clear();

            var result = EditorValidator.ValidateMap(editorState.ActiveMap);

            foreach (var error in result.Errors)
                editorState.Console.Log(error);

            return result;
        }

        private void UpdateRunBtn() => TileTools.SetStartButtonTxt(_engineLauncher.IsRunning ? "Stop" : "Start");
    }
}