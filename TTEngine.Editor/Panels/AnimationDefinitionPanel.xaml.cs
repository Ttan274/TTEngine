using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using TTEngine.Editor.Models.Animation;
using TTEngine.Editor.Services;

namespace TTEngine.Editor.Panels
{
    /// <summary>
    /// Interaction logic for AnimationDefinitionPanel.xaml
    /// </summary>
    public partial class AnimationDefinitionPanel : UserControl, INotifyPropertyChanged
    {
        private int _currentFrame = 0;
        private BitmapImage _spriteSheet;
        private DispatcherTimer _timer;

        public ObservableCollection<AnimationDefinition> Animations { get; }
        public ObservableCollection<int> TimelineFrames { get; } = new();

        private AnimationDefinition _selectedAnimation;
        public AnimationDefinition SelectedAnimation 
        {
            get => _selectedAnimation; 
            set
            {
                _selectedAnimation = value;
                OnPropertyChanged(nameof(SelectedAnimation));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string name)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        public AnimationDefinitionPanel()
        {
            InitializeComponent();

            
            Animations = new ObservableCollection<AnimationDefinition>(
                    AnimationDefinitionService.All
                );

            SelectedAnimation = AnimationDefinitionService.All.FirstOrDefault()
                              ?? AnimationDefinitionService.Create();

            DataContext = this;

            SetupTimer();
            ReloadAnimation();
        }

        #region SpriteSheet

        private void LoadSpriteSheet()
        {
            if (string.IsNullOrWhiteSpace(SelectedAnimation.SpriteSheetPath))
            {
                _spriteSheet = null;
                return;
            }

            try
            {
                _spriteSheet = new BitmapImage();
                _spriteSheet.BeginInit();
                _spriteSheet.UriSource = new Uri(SelectedAnimation.SpriteSheetPath, UriKind.Absolute);
                _spriteSheet.CacheOption = BitmapCacheOption.OnLoad;
                _spriteSheet.EndInit();
            }
            catch
            {
                _spriteSheet = null;
            }
        }

        private void BrowseSpriteSheet(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog
            {
                Filter = "PNG Files (*.png)|*.png",
                InitialDirectory = EditorPaths.GetAnimationFolder()
            };

            if (dialog.ShowDialog() != true)
                return;

            SelectedAnimation.SpriteSheetPath = dialog.FileName;
            LoadSpriteSheet();
            _currentFrame = 0;
            UpdatePreviewFrame();
        }

        private void OnAnimationChanged(object sender, SelectionChangedEventArgs e)
        {
            ReloadAnimation();
        }

        #endregion

        #region Timer/Preview/Timeline

        private void SetupTimer()
        {
            _timer = new DispatcherTimer();
            _timer.Tick += OnTick;
        }

        private void OnTick(object sender, EventArgs e)
        {
            if (_spriteSheet == null || SelectedAnimation == null)
                return;

            _currentFrame++;

            if(_currentFrame >= SelectedAnimation.FrameCount)
            {
                if (SelectedAnimation.Loop)
                {
                    _currentFrame = 0;
                }
                else
                {
                    _currentFrame = SelectedAnimation.FrameCount - 1;
                    _timer.Stop();
                }
            }

            UpdatePreviewFrame();
            UpdateTimelineVisuals();
        }

        private void UpdatePreviewFrame()
        {
            if (_spriteSheet == null || SelectedAnimation == null)
                return;

            int fw = SelectedAnimation.FrameWidth;
            int fh = SelectedAnimation.FrameHeight;

            if (fw <= 0 || fh <= 0)
                return;

            int x = _currentFrame * fw;

            var cropped = new CroppedBitmap(
                             _spriteSheet,
                             new Int32Rect(x, 0, fw, fh)
                          );

            PreviewImage.Source = cropped;
        }

        private void ReloadAnimation()
        {
            _timer?.Stop();
            _currentFrame = 0;

            LoadSpriteSheet();
            RebuildTimeline();
            UpdatePreviewFrame();

            Dispatcher.BeginInvoke(DispatcherPriority.Loaded,
                                   new Action(UpdateTimelineVisuals));
        }

        private void RebuildTimeline()
        {
            TimelineFrames.Clear();

            if (SelectedAnimation == null)
                return;

            for (int i = 0; i < SelectedAnimation.FrameCount; i++)
                TimelineFrames.Add(i);
        }

        private void UpdateTimelineVisuals()
        {
            foreach (var item in TimelineItemsControl.Items)
            {
                var container = TimelineItemsControl
                               .ItemContainerGenerator
                               .ContainerFromItem(item) as ContentPresenter;

                if (container == null || VisualTreeHelper.GetChildrenCount(container) == 0) continue;

                var border = VisualTreeHelper.GetChild(container, 0) as Border;
                if (border == null) continue;

                int frameIndex = (int)item;

                if(frameIndex == _currentFrame)
                {
                    border.Background = Brushes.DeepSkyBlue;
                    border.BorderBrush = Brushes.Transparent;
                    border.BorderThickness = new Thickness(2);
                }
                else if (SelectedAnimation.EventFrames.Contains(frameIndex))
                {
                    border.Background = Brushes.OrangeRed;
                    border.BorderBrush = Brushes.DarkRed;
                    border.BorderThickness = new Thickness(1);
                }
                else
                {
                    border.Background = new SolidColorBrush(Color.FromArgb(255, 60, 60, 60));
                    border.BorderBrush = Brushes.Black;
                    border.BorderThickness = new Thickness(1);
                }
            }
        }

        #endregion

        #region Controls

        private void PlayClick(object sender, RoutedEventArgs e)
        {
            if (SelectedAnimation == null)
                return;

            _timer.Interval = TimeSpan.FromSeconds(Math.Max(0.01f, SelectedAnimation.FrameTime));
            _timer.Start();
        }

        private void StopClick(object sender, RoutedEventArgs e)
        {
            _timer.Stop();
            _currentFrame = 0;
            UpdatePreviewFrame();
        }

        private void PrevFrameClick(object sender, RoutedEventArgs e)
        {
            _currentFrame = Math.Max(0, _currentFrame - 1);
            UpdatePreviewFrame();
            UpdateTimelineVisuals();
        }

        private void NextFrameClick(object sender, RoutedEventArgs e)
        {
            _currentFrame = Math.Min(SelectedAnimation.FrameCount - 1, _currentFrame + 1);
            UpdatePreviewFrame();
            UpdateTimelineVisuals();
        }

        private void SaveClicked(object sender, RoutedEventArgs e)
        {
            AnimationDefinitionService.Save(SelectedAnimation);
        }
        
        private void OnTimelineFrameClicked(object sender, MouseButtonEventArgs e)
        {
            if(sender is Border b && b.Tag is int frame)
            {
                if(SelectedAnimation.EventFrames.Contains(frame))
                    SelectedAnimation.EventFrames.Remove(frame);
                else
                    SelectedAnimation.EventFrames.Add(frame);

                UpdateTimelineVisuals();
            }
        }

        #endregion

        #region Add/Delete Anim

        private void AddAnimationClick(object sender, RoutedEventArgs e)
        {
            var anim = AnimationDefinitionService.Create();

            _spriteSheet = null;
            Animations.Add(anim);
            SelectedAnimation = anim;
            ReloadAnimation();
        }

        private void DeleteAnimationClick(object sender, RoutedEventArgs e)
        {
            if (SelectedAnimation == null)
                return;

            AnimationDefinitionService.Delete(SelectedAnimation.Id);
            Animations.Remove(SelectedAnimation);

            SelectedAnimation = AnimationDefinitionService.All.FirstOrDefault();
            ReloadAnimation();
        }

        #endregion
    }
}
