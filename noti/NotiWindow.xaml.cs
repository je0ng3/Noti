using Microsoft.UI.Xaml;
using Microsoft.UI.Windowing;
using WinRT.Interop;
using Windows.Graphics;
using noti.Services;
using Microsoft.UI;
using System;

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

        public NotiWindow()
        {
            this.InitializeComponent();
            SetupWidgetWindow();
            StartAutoNotifier();
        }

        private void SetupWidgetWindow()
        {
            var hWnd = WindowNative.GetWindowHandle(this);
            var windowId = Win32Interop.GetWindowIdFromWindow(hWnd);
            var appWindow = AppWindow.GetFromWindowId(windowId);

            appWindow.Resize(new SizeInt32(260, 120));
            appWindow.Move(new PointInt32(1600, 40));
            
            if (appWindow.Presenter is OverlappedPresenter presenter)
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
            _timer.Interval = TimeSpan.FromMinutes(Random.Shared.Next(15, 31));
            _timer.Tick += OnTimerTick;
            _timer.Start();
        }

        private void OnTimerTick(object sender, object e)
        {
            MessageText.Text = _service.GetRandomMessage();

            _timer.Interval = TimeSpan.FromMinutes(Random.Shared.Next(15, 31));
        }
    }
}
