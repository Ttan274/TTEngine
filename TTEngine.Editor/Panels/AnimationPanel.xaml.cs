using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using TTEngine.Editor.Models.Animation;
using TTEngine.Editor.Models.Project;
using TTEngine.Editor.Services.IO;

namespace TTEngine.Editor.Panels
{
    /// <summary>
    /// Interaction logic for AnimationPanel.xaml
    /// </summary>
    public partial class AnimationPanel : UserControl
    {
        private readonly ProjectSession _project;
        private AnimationDefinition _model;
        private string _filePath;

        private int _currentFrame = 0;
        private DispatcherTimer _timer;

        public AnimationPanel(ProjectSession project)
        {
            InitializeComponent();
            _project = project;
        }

        public void LoadFile(string filePath)
        {
            _filePath = filePath;
            _model = JsonFileService.Load<AnimationDefinition>(_filePath);

            DataContext = _model;

            BuildTimeline();
            UpdatePreview();
        }

        #region Event Frames

        private void BuildTimeline()
        {
            TimelineItemsControl.Items.Clear();

            for(int i = 0; i < _model.FrameCount; i++)
            {
                var border = new Border
                {
                    Width = 20,
                    Height = 20,
                    Margin = new Thickness(2),
                    Background = _model.EventFrames.Contains(i)
                        ? Brushes.Red : Brushes.Gray,
                    Tag = i
                };

                border.MouseDown += TimelineFrameClicked;

                TimelineItemsControl.Items.Add(border);
                HighlightCurrentFrame();
            }
        }

        private void TimelineFrameClicked(object sender, MouseButtonEventArgs e)
        {
            if (sender is not Border b)
                return;

            int frame = (int)b.Tag;

            if (_model.EventFrames.Contains(frame))
                _model.EventFrames.Remove(frame);
            else
                _model.EventFrames.Add(frame);

            BuildTimeline();
        }
        
        private void HighlightCurrentFrame()
        {
            foreach (var item in TimelineItemsControl.Items)
            {
                if(item is Border b && b.Tag is int frameIndex)
                {
                    if(frameIndex == _currentFrame)
                    {
                        b.BorderBrush = Brushes.LightBlue;
                        b.BorderThickness = new Thickness(2);
                    }
                    else
                    {
                        b.BorderBrush = Brushes.Transparent;
                        b.BorderThickness = new Thickness(0);
                    }
                }
            }
        }

        #endregion
       
        private void UpdatePreview()
        {
            if (string.IsNullOrWhiteSpace(_model.SpriteSheetPath) || _model == null)
                return;

            string absolutePath = Path.Combine(_project.TexturesPath, _model.SpriteSheetPath);

            if (!File.Exists(absolutePath))
                return;

            var bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.CacheOption = BitmapCacheOption.OnLoad;
            bitmap.UriSource = new Uri(absolutePath, UriKind.Absolute);
            bitmap.EndInit();

            var rect = new Int32Rect(
                _currentFrame * _model.FrameWidth,
                0,
                _model.FrameWidth,
                _model.FrameHeight);

            var cropped = new CroppedBitmap(bitmap, rect);

            PreviewImage.Source = cropped;
        }

        #region UI Clicks

        private void BrowseSpriteSheet(object sender, RoutedEventArgs e)
        {
            //şimdilik kalsın
        }

        private void PlayClick(object sender, RoutedEventArgs e)
        {
            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromSeconds(_model.FrameTime);
            _timer.Tick += (s, args) =>
            {
                _currentFrame++;
                if(_currentFrame >= _model.FrameCount)
                    _currentFrame = _model.Loop ? 0 : _model.FrameCount - 1;

                UpdatePreview();
            };

            _timer.Start();
        }

        private void StopClick(object sender, RoutedEventArgs e)
        {
            _timer?.Stop();
        }

        private void PrevFrameClick(object sender, RoutedEventArgs e)
        {
            if (_model == null || _model.FrameCount <= 0)
                return;

            _currentFrame = Math.Max(0, _currentFrame - 1);
            UpdatePreview();
            HighlightCurrentFrame();
        }

        private void NextFrameClick(object sender, RoutedEventArgs e)
        {
            if (_model == null || _model.FrameCount <= 0)
                return;

            _currentFrame = Math.Min(_model.FrameCount - 1, _currentFrame + 1);
            UpdatePreview();
            HighlightCurrentFrame();
        }

        private void SaveClicked(object sender, RoutedEventArgs e)
        {
            JsonFileService.Save(_filePath, _model);
        }

        #endregion
    }
}
