using Microsoft.UI.Xaml;
using Microsoft.UI.Windowing;
using Windows.Graphics;
using noti.Services;
using System;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Animation;
using WinUIEx;

namespace noti
{
    public sealed partial class NotiWindow : WindowEx
    {
        private readonly RandomMessageService _service = new();
        private DispatcherTimer _timer;
        private bool _isVisible = false;
        private Storyboard _slideDownStoryboard;
        private Storyboard _slideUpStoryboard;
        private TranslateTransform _spiderTranslate;

        public NotiWindow()
        {
            this.InitializeComponent();
            SetupWidgetWindow();
            SetupAnimation();
            StartAutoNotifier();
            this.Hide();

            this.Closed += (s, e) =>
            {
                _timer.Stop();
                _timer.Tick -= OnTimerTick;
            };
        }

        private void SetupWidgetWindow()
        {
            var displayArea = DisplayArea.GetFromWindowId(this.AppWindow.Id, DisplayAreaFallback.Primary);
            var workArea = displayArea.WorkArea;
            var x = workArea.X + workArea.Width - 350 - 20;
            var y = workArea.Y;
            this.MoveAndResize(x, y, 350, 150);
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
            this.Hide();
            _isVisible = false;
        }

        private void StartAutoNotifier()
        {
            _timer = new DispatcherTimer();
            //_timer.Interval = TimeSpan.FromMinutes(Random.Shared.Next(15, 31));
            _timer.Interval = TimeSpan.FromSeconds(10); // For testing purposes
            _timer.Tick += OnTimerTick;
            _timer.Start();
        }

        private void OnTimerTick(object sender, object e)
        {
            MessageText.Text = _service.GetRandomMessage();

            if (!_isVisible)
            {
                _spiderTranslate.Y = -350;
                this.Show();
                _slideDownStoryboard.Begin();
                _isVisible = true;
            }

            //_timer.Interval = TimeSpan.FromMinutes(Random.Shared.Next(15, 31));
            _timer.Interval = TimeSpan.FromSeconds(10); // For testing purposes
        }

        private void OnWidgetClicked(object sender, TappedRoutedEventArgs e)
        {
            if (!_isVisible) return;
            _slideDownStoryboard.Stop();
            _slideUpStoryboard.Begin();
        }

    }
}
