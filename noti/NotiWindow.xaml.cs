using Microsoft.UI.Xaml;
using Microsoft.UI.Windowing;
using WinRT;
using WinRT.Interop;
using Windows.Graphics;
using noti.Services;
using Microsoft.UI;
using System;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Animation;

namespace noti
{
    public sealed partial class NotiWindow : Window
    {
        private readonly RandomMessageService _service = new();
        private DispatcherTimer _timer;
        private bool _isVisible = false;
        private AppWindow _appWindow;
        private Storyboard _slideDownStoryboard;
        private Storyboard _slideUpStoryboard;
        private TranslateTransform _spiderTranslate;

        public NotiWindow()
        {
            this.InitializeComponent();
            SetupWidgetWindow();
            SetupAnimation();
            StartAutoNotifier();
            _appWindow.Hide();

            this.Closed += (s, e) =>
            {
                _timer.Stop();
                _timer.Tick -= OnTimerTick;
            };
        }

        private void SetupWidgetWindow()
        {
            var hWnd = WindowNative.GetWindowHandle(this);
            var windowId = Win32Interop.GetWindowIdFromWindow(hWnd);
            _appWindow = AppWindow.GetFromWindowId(windowId);

            var widgetWidth = 350;
            var widgetHeight = 150;

            _appWindow.Resize(new SizeInt32(widgetWidth, widgetHeight));

            var displayArea = DisplayArea.GetFromWindowId(windowId, DisplayAreaFallback.Primary);
            var workArea = displayArea.WorkArea;
            var x = workArea.X + workArea.Width - widgetWidth - 20;
            var y = workArea.Y;
            _appWindow.Move(new PointInt32(x, y));

            if (_appWindow.Presenter is OverlappedPresenter presenter)
            {
                presenter.SetBorderAndTitleBar(false, false);
                presenter.IsResizable = false;
                presenter.IsAlwaysOnTop = true;
            }

            this.ExtendsContentIntoTitleBar = true;
            this.SetTitleBar(null);

        }

        private void SetupAnimation()
        { 
            _spiderTranslate = (TranslateTransform)SpiderPanel.RenderTransform;

            var animation = new DoubleAnimation
            {
                From = -350,
                To = 0,
                Duration = new Duration(TimeSpan.FromMilliseconds(800)),
                EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut }
            };

            Storyboard.SetTarget(animation, _spiderTranslate);
            Storyboard.SetTargetProperty(animation, "Y");

            _slideDownStoryboard = new Storyboard();
            _slideDownStoryboard.Children.Add(animation);

            // Slide up animation (0 -> -350)
            var slideUpAnimation = new DoubleAnimation
            {
                From = 0,
                To = -350,
                Duration = new Duration(TimeSpan.FromMilliseconds(600)),
                EasingFunction = new CubicEase { EasingMode = EasingMode.EaseIn }
            };

            Storyboard.SetTarget(slideUpAnimation, _spiderTranslate);
            Storyboard.SetTargetProperty(slideUpAnimation, "Y");

            _slideUpStoryboard = new Storyboard();
            _slideUpStoryboard.Children.Add(slideUpAnimation);
            _slideUpStoryboard.Completed += OnSlideUpCompleted;
        }

        private void OnSlideUpCompleted(object sender, object e)
        {
            _appWindow.Hide();
            _isVisible = false;
        }

        private void StartAutoNotifier()
        {
            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromMinutes(Random.Shared.Next(15, 31));
            _timer.Tick += OnTimerTick;
            _timer.Start();
        }

        private void OnTimerTick(object sender, object e)
        {
            MessageText.Text = _service.GetRandomMessage();

            if (!_isVisible)
            {
                _spiderTranslate.Y = -350;
                _appWindow.Show();
                _slideDownStoryboard.Begin();
                _isVisible = true;
            }

            _timer.Interval = TimeSpan.FromMinutes(Random.Shared.Next(15, 31));
        }

        private void OnWidgetClicked(object sender, TappedRoutedEventArgs e)
        {
            if (!_isVisible) return;
            _slideDownStoryboard.Stop();
            _slideUpStoryboard.Begin();
        }

    }
}
