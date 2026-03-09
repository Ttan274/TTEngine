using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using TTEngine.Editor.Models.Definitions;
using TTEngine.Editor.Models.Editor;
using TTEngine.Editor.Models.GameObject;
using TTEngine.Editor.Models.Selection;
using TTEngine.Editor.Models.Tile;
using TTEngine.Editor.Panels;
using TTEngine.Editor.Services;
using TTEngine.Editor.Services.IO;
using TTEngine.Editor.Services.Map;

namespace TTEngine.Editor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //Map Controllers
        private MapRenderer _renderer;
        private MapInteractionController _interaction;
        private SelectionController _selection;
        private MapNavigationController _navigation;
        private MapInputController _input;
        private int _brushSize = 1;

        //Panels
        private readonly AssetPanel _assetPanel;
        private readonly AnimationPanel _animationPanel;
        private readonly AnimatorPanel _animatorPanel;
        private readonly HierarchyPanel _hierarchyPanel;
        private bool IsAnimTabOpened = false;
        private bool IsAnimatorTabOpened = false;

        //Editor State
        public EditorState editorState { get; }

        //Engine Launcher
        private EngineLauncher _engineLauncher = new EngineLauncher();

        public MainWindow(EditorState editor, 
            AssetPanel assetPanel, 
            AnimationPanel animationPanel,
            AnimatorPanel animatorPanel,
            HierarchyPanel hierarchyPanel)
        {
            InitializeComponent();
            //Binding
            editorState = editor;
            _assetPanel = assetPanel;
            _animationPanel = animationPanel;
            _animatorPanel = animatorPanel;
            _hierarchyPanel = hierarchyPanel;
            DataContext = editorState;

            //Event Bindings
            _assetPanel.AssetCreated += OpenAsset;
            _assetPanel.AssetOpened += OpenAsset;
            _hierarchyPanel.RequestMapRedraw += RedrawMap;
           

            AddTab("Assets", assetPanel);
            AddTab("Hierarchy", hierarchyPanel);

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
            
            //Key eventler için yaptık
            this.Focusable = true;
            this.Focus();
        }

        #region Setup

        private void WindowSetup()
        {
            _renderer = new MapRenderer(MapCanvas, editorState);
            _interaction = new MapInteractionController(editorState, _renderer, () => _renderer.DrawStatic());
            _selection = new SelectionController(editorState);
            _navigation = new MapNavigationController(
                _renderer,
                () =>
                {
                    _renderer.InitializeGrid();
                    _renderer.DrawStatic();
                    _renderer.UpdateSelection();
                });
            _input = new MapInputController(editorState, _renderer, _interaction,
                _selection, _navigation, () => _brushSize);

            MapCanvas.MouseEnter += (_, _) => _renderer.OnMouseEnter();
            MapCanvas.MouseLeave += (_, _) => _renderer.OnMouseLeave();
            EnsureScene();

            _renderer.InitializeGrid();
            _renderer.DrawStatic();
            SetupCommands();
        }

        private void SetupCommands()
        {
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
                (_, _) => editorState.SaveHelper()
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

                if(e.PropertyName == nameof(editorState.SceneSession.SelectedObject))
                {
                    _renderer.UpdateSelection();
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

        #region Window Events

        private void Canvas_MouseDown(object sender, MouseButtonEventArgs e)
            => _input.MouseDown(MapCanvas, e);

        private void Canvas_MouseMove(object sender, MouseEventArgs e)
            => _input.MouseMove(MapCanvas, e);

        private void Canvas_MouseUp(object sender, MouseButtonEventArgs e)
            => _input.MouseUp();

        private void Canvas_MouseWheel(object sender, MouseWheelEventArgs e)
            => _input.MouseWheel(e.Delta);

        private void Window_KeyDown(object sender, KeyEventArgs e)
           => _input.KeyDown(MapCanvas, e.Key);

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

        private EditorValidationResult ValidateMap()
        {
            editorState.Console.Clear();

            var result = new EditorValidationResult();
            //var result = EditorValidator.ValidateMap(editorState.SceneSession.ActiveScene);

            //foreach (var error in result.Errors)
            //    editorState.Console.Log(error);

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

            if(path.Contains("Animators"))
            {
                _animatorPanel.LoadFile(path);

                if(!IsAnimatorTabOpened)
                {
                    AddTab("Animator", _animatorPanel);
                    IsAnimatorTabOpened = true;
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

        private void RedrawMap()
        {
            _renderer.DrawStatic();
        }
    }
}