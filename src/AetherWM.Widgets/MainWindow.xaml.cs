using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using AetherWM.Platform.Win32.Services;

namespace AetherWM.Widgets;

public partial class MainWindow : Window
{
    private readonly HardwareMonitorService _hardwareMonitor = new();
    private readonly DispatcherTimer _timer = new();

    public MainWindow()
    {
        InitializeComponent();

        // Initial Position: Top-Right of working area
        double screenWidth = SystemParameters.WorkArea.Width;
        Left = screenWidth - Width - 30;
        Top = 40;

        Loaded += MainWindow_Loaded;
        SetupTimer();
    }

    private void MainWindow_Loaded(object sender, RoutedEventArgs e)
    {
        DesktopWidgetHelper.PinToDesktop(this);
        UpdateMetrics();
    }

    private void SetupTimer()
    {
        _timer.Interval = TimeSpan.FromSeconds(1);
        _timer.Tick += (s, e) => UpdateMetrics();
        _timer.Start();
    }

    private void UpdateMetrics()
    {
        var now = DateTime.Now;
        ClockText.Text = now.ToString("HH:mm:ss");
        DateText.Text = now.ToString("dddd, dd 'DE' MMMM", System.Globalization.CultureInfo.GetCultureInfo("es-ES")).ToUpperInvariant();

        double cpu = _hardwareMonitor.GetCpuUsagePercent();
        CpuBar.Value = cpu;
        CpuText.Text = $"{cpu:F0}%";

        var (usedGb, totalGb, ramPercent) = _hardwareMonitor.GetRamUsage();
        RamBar.Value = ramPercent;
        RamText.Text = $"{usedGb:F1} / {totalGb:F1} GB ({ramPercent:F0}%)";
    }

    private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (e.ButtonState == MouseButtonState.Pressed)
        {
            DragMove();
        }
    }
}
