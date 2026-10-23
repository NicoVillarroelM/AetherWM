using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using AetherWM.Core.Config;
using AetherWM.Platform.Win32.Config;

namespace AetherWM.Settings;

public partial class MainWindow : Window
{
    private readonly FileSystemConfigService _configService;
    private AetherConfig _config;

    public MainWindow()
    {
        InitializeComponent();

        _configService = new FileSystemConfigService();
        _config = _configService.LoadOrCreateDefault();

        LoadConfigToUi();
    }

    private void LoadConfigToUi()
    {
        SliderInnerGap.Value = _config.InnerGap;
        LblInnerGap.Text = $"{_config.InnerGap} px";

        SliderOuterGap.Value = _config.OuterGap;
        LblOuterGap.Text = $"{_config.OuterGap} px";

        ChkAutoTile.IsChecked = _config.AutoTileOnLaunch;

        ChkEnableBorders.IsChecked = _config.EnableBorderStyling;
        TxtActiveColor.Text = _config.ActiveBorderColor;
        TxtInactiveColor.Text = _config.InactiveBorderColor;

        bool isSquare = _config.CornerPreference.Equals("Square", StringComparison.OrdinalIgnoreCase);
        RadioSquare.IsChecked = isSquare;
        RadioRounded.IsChecked = !isSquare;

        UpdateColorPreview(TxtActiveColor.Text, ColorPreviewBox);
        UpdateColorPreview(TxtInactiveColor.Text, InactiveColorPreviewBox);
    }

    private void Tab_Checked(object sender, RoutedEventArgs e)
    {
        if (PanelMosaico == null || PanelEstilo == null || PanelAtajos == null || PanelWidgets == null)
            return;

        PanelMosaico.Visibility = Visibility.Collapsed;
        PanelEstilo.Visibility = Visibility.Collapsed;
        PanelAtajos.Visibility = Visibility.Collapsed;
        PanelWidgets.Visibility = Visibility.Collapsed;

        if (TabMosaico.IsChecked == true) PanelMosaico.Visibility = Visibility.Visible;
        else if (TabEstilo.IsChecked == true) PanelEstilo.Visibility = Visibility.Visible;
        else if (TabAtajos.IsChecked == true) PanelAtajos.Visibility = Visibility.Visible;
        else if (TabWidgets.IsChecked == true) PanelWidgets.Visibility = Visibility.Visible;
    }

    private void SliderInnerGap_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
        if (LblInnerGap != null)
        {
            LblInnerGap.Text = $"{(int)e.NewValue} px";
        }
    }

    private void SliderOuterGap_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
        if (LblOuterGap != null)
        {
            LblOuterGap.Text = $"{(int)e.NewValue} px";
        }
    }

    private void PresetColor_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button btn && btn.Tag is string hex)
        {
            TxtActiveColor.Text = hex;
        }
    }

    private void TxtActiveColor_TextChanged(object sender, TextChangedEventArgs e)
    {
        UpdateColorPreview(TxtActiveColor.Text, ColorPreviewBox);
    }

    private void TxtInactiveColor_TextChanged(object sender, TextChangedEventArgs e)
    {
        UpdateColorPreview(TxtInactiveColor.Text, InactiveColorPreviewBox);
    }

    private static void UpdateColorPreview(string hex, Border? box)
    {
        if (box == null) return;
        try
        {
            var color = (Color)ColorConverter.ConvertFromString(hex);
            box.Background = new SolidColorBrush(color);
        }
        catch
        {
            // Invalid hex while typing, ignore
        }
    }

    private void BtnSave_Click(object sender, RoutedEventArgs e)
    {
        _config.InnerGap = (int)SliderInnerGap.Value;
        _config.OuterGap = (int)SliderOuterGap.Value;
        _config.AutoTileOnLaunch = ChkAutoTile.IsChecked ?? true;

        _config.EnableBorderStyling = ChkEnableBorders.IsChecked ?? true;
        _config.ActiveBorderColor = TxtActiveColor.Text.Trim();
        _config.InactiveBorderColor = TxtInactiveColor.Text.Trim();
        _config.CornerPreference = RadioSquare.IsChecked == true ? "Square" : "Rounded";

        _configService.SaveConfig(_config);

        LblStatusMessage.Text = "✅ ¡Configuración guardada! Se aplicó en vivo en tu sistema.";

        // Clear status after 3 seconds
        var timer = new System.Windows.Threading.DispatcherTimer { Interval = TimeSpan.FromSeconds(3) };
        timer.Tick += (s, ev) =>
        {
            LblStatusMessage.Text = "";
            timer.Stop();
        };
        timer.Start();
    }

    private void BtnLaunchWidget_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            string appDir = AppDomain.CurrentDomain.BaseDirectory;
            // Launch widgets
            Process.Start(new ProcessStartInfo("dotnet", "run --project src/AetherWM.Widgets")
            {
                UseShellExecute = true,
                CreateNoWindow = true
            });
            LblStatusMessage.Text = "🚀 Widget de escritorio iniciado.";
        }
        catch (Exception ex)
        {
            LblStatusMessage.Text = $"Error al iniciar widget: {ex.Message}";
        }
    }

    private void BtnKillWidget_Click(object sender, RoutedEventArgs e)
    {
        foreach (var proc in Process.GetProcessesByName("AetherWM.Widgets"))
        {
            try { proc.Kill(); } catch { }
        }
        LblStatusMessage.Text = "⏹️ Widget de escritorio cerrado.";
    }
}
