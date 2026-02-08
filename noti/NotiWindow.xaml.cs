using Microsoft.UI.Xaml;
using Microsoft.UI.Windowing;
using WinRT.Interop;
using Windows.Graphics;
using noti.Services;
using Microsoft.UI;
using System;
using Microsoft.UI.Xaml.Input;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace noti
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class NotiWindow : Window
    {
        private readonly RandomMessageService _service = new();
        private DispatcherTimer _timer;
        private bool _isVisible = false;
        private AppWindow _appWindow;

        public NotiWindow()
        {
            this.InitializeComponent();
            SetupWidgetWindow();
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

            var widgetWidth = 260;
            var widgetHeight = 120;
            var margin = 40;

            _appWindow.Resize(new SizeInt32(widgetWidth, widgetHeight));

            var displayArea = DisplayArea.GetFromWindowId(windowId, DisplayAreaFallback.Primary);
            var workArea = displayArea.WorkArea;
            var x = workArea.X + workArea.Width - widgetWidth - margin;
            var y = workArea.Y + margin;
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
                _appWindow.Show();
                this.Activate();
                _isVisible = true;
            }

            _timer.Interval = TimeSpan.FromMinutes(Random.Shared.Next(15, 31));
        }

        private void OnWidgetClicked(object sender, TappedRoutedEventArgs e)
        {
            _appWindow.Hide();
            _isVisible = false;
        }
    }
}
