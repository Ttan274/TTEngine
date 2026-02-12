using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using TTEngine.Editor.EditorServices.Interaction;
using TTEngine.Editor.EditorServices.Rendering;
using TTEngine.Editor.Models;
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
        private int _brushSize = 1;
        public const string DEFAULT_MAP_ID = "Map_Default";

        //Selection + Entity Def
        private SelectionModel _currentSelection = new();

        //Layer
        public EditorState editorState { get; } = new EditorState();

        public MainWindow()
        {
            InitializeComponent();
            WindowSetup();
        }

        #region Setup

        //Setup
        private void WindowSetup()
        {
            //Context setup
            AnimationDefinitionService.LoadAll();
            LayerEditor.DataContext = editorState;
            TileTools.DataContext = editorState;
            ConsoleEditor.DataContext = editorState;
            ToolHost.BindEditor(editorState);

            editorState.PropertyChanged += (_, e) =>
            {
                if (e.PropertyName == nameof(editorState.ActiveMap))
                    _renderer.DrawStatic();
            };

            foreach (var layer in editorState.Layers)
                layer.VisibilityChanged += OnLayerVisibilityChanged;

            _renderer = new MapRenderer(MapCanvas, editorState);
            _interaction = new MapInteractionController(editorState, () => _renderer.DrawStatic());
           
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
            _interaction.OnMouseDown(e.GetPosition(MapCanvas), e);
        }

        private void Canvas_MouseMove(object sender, MouseEventArgs e)
        {
            _renderer.UpdateHover(e.GetPosition(MapCanvas), _brushSize);
            _interaction.OnMouseMove(e.GetPosition(MapCanvas), e);
        }

        private void Canvas_MouseUp(object sender, MouseButtonEventArgs e)
        {
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
            var validation = ValidateMap();

            if (!validation.IsValid)
                return;

            StartGame(this, new RoutedEventArgs());
        }

        private void OnLayerVisibilityChanged(EditorLayer layer)
            => _renderer.DrawStatic();

        private void StartGame(object sender, RoutedEventArgs e) => Process.Start(EditorPaths.GetEngineExe());

        #endregion

        
        //Selection hariç her şeyi taşıdık !!

        //Mouse Event Helpers
        //private void HandleSelection(int x, int y)
        //{
        //    if(editorState.ActiveLayer.LayerType == MapLayerType.Interactable)
        //    {
        //        foreach (var interactable in ActiveMap.Interactables)
        //        {
        //            if (interactable.X == x && interactable.Y == y)
        //            {
        //                _currentSelection = new SelectionModel
        //                {
        //                    Type = SelectionType.Interactable,
        //                    InteractableModel = interactable
        //                };
        //            }
        //            ShowInspector();
        //            return;
        //        }
        //    }

        //    if (ActiveMap.PlayerSpawn != null && ActiveMap.PlayerSpawn.Position.X == x && ActiveMap.PlayerSpawn.Position.Y == y)
        //    {
        //        _currentSelection = new SelectionModel
        //        {
        //            Type = SelectionType.Player,
        //            PlayerSpawnModel = new PlayerSpawnModel
        //            {
        //                Position = new Point(x, y)
        //            }
        //        };

        //        ShowInspector();
        //        return;
        //    }

        //    foreach (var spawn in ActiveMap.EnemySpawns)
        //    {
        //        if (spawn.Position.X == x && spawn.Position.Y == y)
        //        {
        //            _currentSelection = new SelectionModel
        //            {
        //                Type = SelectionType.Enemy,
        //                EnemySpawnModel = new EnemySpawnModel
        //                {
        //                    Position = spawn.Position,
        //                    DefinitionId = spawn.DefinitionId
        //                }
        //            };

        //            ShowInspector();
        //            return;
        //        }
        //    }


        //    _currentSelection = new SelectionModel
        //    {
        //        Type = SelectionType.Tile,
        //        TileX = x,
        //        TileY = y
        //    };

        //    ShowInspector();
        //}

        //private void ShowInspector()
        //{
        //    switch (_currentSelection.Type)
        //    {
        //        case SelectionType.Tile:
        //            int index = ActiveMap.GetIndex(_currentSelection.TileX, _currentSelection.TileY);
        //            Inspector.SetContent(new TileSpawnInspector(_currentSelection.TileX, _currentSelection.TileY, ActiveTiles[index]));
        //            break;
        //        case SelectionType.Player:
        //            Inspector.SetContent(new PlayerSpawnInspector(_currentSelection.PlayerSpawnModel, editorState.EntityDefinitions.ToList()));
        //            break;
        //        case SelectionType.Enemy:
        //            Inspector.SetContent(new EnemySpawnInspector(_currentSelection.EnemySpawnModel, editorState.EntityDefinitions.ToList()));
        //            break;
        //        case SelectionType.Interactable:
        //            Inspector.SetContent(new InteractableInspector(_currentSelection.InteractableModel, editorState.InteractableDefinitions));
        //            break;
        //        default:
        //            Inspector.Clear();
        //            break;
        //    }
        //}

        #region Load Default Map

        private void EnsureDefaultMap()
        {
            if(!MapFileService.Exists(DEFAULT_MAP_ID))
            {
                var model = new TileMapModel();
                model.Init();

                MapFileService.Save(DEFAULT_MAP_ID, MapFileService.ToDto(model));
            }

            var data = MapFileService.Load(DEFAULT_MAP_ID);
            var mapModel = MapFileService.FromDto(data);

            editorState.ActiveMapId = DEFAULT_MAP_ID;
            editorState.ActiveMap = mapModel;
            editorState.Console.Log($"{DEFAULT_MAP_ID} is loaded.");
        }

        #endregion

        #region Validation

        private EditorValidationResult ValidateMap()
        {
            editorState.Console.Clear();

            var result = EditorValidator.ValidateMap(editorState.ActiveMap);

            foreach (var error in result.Errors)
                editorState.Console.Log(error);

            return result;
        }

        #endregion
    }
}