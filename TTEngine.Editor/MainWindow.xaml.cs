using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using TTEngine.Editor.EditorServices.EngineLauncher;
using TTEngine.Editor.EditorServices.Interaction;
using TTEngine.Editor.EditorServices.Rendering;
using TTEngine.Editor.Models.Editor;
using TTEngine.Editor.Models.GameObject;
using TTEngine.Editor.Models.Selection;
using TTEngine.Editor.Models.Tile;
using TTEngine.Editor.Models.Validation;
using TTEngine.Editor.Panels;
using TTEngine.Editor.Services;
using TTEngine.Editor.Services.IO;

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
        private readonly AnimationPanel _animationPanel;
        private bool IsAnimTabOpened = false;

        //Editor State
        public EditorState editorState { get; }

        //Engine Launcher
        private EngineLauncher _engineLauncher = new EngineLauncher();

        public MainWindow(EditorState editor, AssetPanel assetPanel, AnimationPanel animationPanel)
        {
            InitializeComponent();
            editorState = editor;
            _assetPanel = assetPanel;
            _animationPanel = animationPanel;
            DataContext = editorState;

            _assetPanel.AssetCreated += OpenAsset;
            _assetPanel.AssetOpened += OpenAsset;
            AddTab("Assets", assetPanel);

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

            EnsureScene();

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
            TileTools.DataContext = editorState;
            ConsoleEditor.DataContext = editorState;
            Inspector.DataContext = editorState;
            ScenePanel.Bind(editorState);
        }

        private void ChangeEventBindings()
        {
            editorState.SceneSession.PropertyChanged += (_, e) =>
            {
                if (e.PropertyName == nameof(editorState.SceneSession.ActiveScene))
                {
                    _renderer.InitializeGrid();
                    _renderer.DrawStatic();
                }
            };

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
            Point pos = e.GetPosition(MapCanvas);

            //Selection override
            if (Keyboard.IsKeyDown(Key.LeftAlt))
            {
                if (TryGetTilePosition(pos, out int sx, out int sy))
                    _selection.HandleSelection(sx, sy);
                return;
            }

            Mouse.Capture(MapCanvas);

             //Selection
            if (TryGetTilePosition(pos, out int x, out int y))
                _selection.HandleSelection(x, y);

             //Interaction
            _interaction.OnMouseDown(pos, e);
        }

        private void Canvas_MouseMove(object sender, MouseEventArgs e)
        {
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
            if (_engineLauncher.IsRunning)
                StopGame();
            else
                StartEngine();
        }

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

            if (editorState.SceneSession.ActiveScene == null)
                return false;

            var map = editorState.SceneSession.ActiveScene.Map;

            x = (int)(pos.X / map.TileSize);
            y = (int)(pos.Y / map.TileSize);

            if (x < 0 || y < 0 || x >= map.Width || y >= map.Height)
                return false;

            return true;
        }

        private EditorValidationResult ValidateMap()
        {
            editorState.Console.Clear();

            var result = EditorValidator.ValidateMap(editorState.SceneSession.ActiveScene);

            foreach (var error in result.Errors)
                editorState.Console.Log(error);

            return result;
        }

        private void UpdateRunBtn() => TileTools.SetStartButtonTxt(_engineLauncher.IsRunning ? "Stop" : "Start");

        //Might be changed
        private void OpenAsset(string path)
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
            if (path.Contains("Tiles"))
                return JsonFileService.Load<TileDefinition>(path);

            if(path.Contains("Animations"))
            {
                _animationPanel.LoadFile(path);

                if(!IsAnimTabOpened)
                {
                    AddTab("Animation", _animationPanel);
                    IsAnimTabOpened = true;
                }

                return null;
            }

            if (path.Contains("GameObjects"))
                return JsonFileService.Load<GameObject>(path);

            return null;
        }

        private object CreateAssetSelection(object model, string path)
        {
            if (model is TileDefinition t)
                return new TileAssetSelectionViewModel(t, path);

            if (model is GameObject g)
                return new GameObjectAssetSelectionViewModel(g, path);

            return null;
        }   
        
        private void EnsureScene()
        {
            var scenes = editorState.SceneSession.GetAllScenes();

            if(scenes.Count == 0)
            {
                editorState.SceneSession.Create("DefaultScene");
            }
            else
            {
                editorState.SceneSession.Load(scenes.First());
            }
        }

        private void AddTab(string header, object panel)
        {
            var tab = new TabItem
            {
                Header = header,
                Content = panel
            };

            DocumentTabs.Items.Add(tab);
            DocumentTabs.SelectedItem = tab;
        }
    }
}