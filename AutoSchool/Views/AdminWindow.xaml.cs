using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using AutoSchool.Infrastructure;
using AutoSchool.Services.Navigation;
using AutoSchool.ViewModels.Admin;

namespace AutoSchool.Views;

public partial class AdminWindow : Window
{
    public AdminWindow()
    {
        InitializeComponent();

        ApplyAdminBackground();
        ThemeManager.ThemeChanged += ThemeManager_ThemeChanged;
        Closed += (_, __) => ThemeManager.ThemeChanged -= ThemeManager_ThemeChanged;

        var nav = new FramePageNavigationService(MainFrame);
        DataContext = new AdminShellViewModel(nav);

        // стартуем с билетов
        ((AdminShellViewModel)DataContext).OpenTicketsCommand.Execute(null);
    }

    private void ThemeManager_ThemeChanged(object? sender, EventArgs e)
    {
        ApplyAdminBackground();
    }

    private void ApplyAdminBackground()
    {
        // Эти файлы должны существовать и быть Resource:
        // Assets/admin_light.png и Assets/admin_dark.png
        string uri = ThemeManager.CurrentTheme == AppTheme.Dark
            ? "pack://application:,,,/Assets/background2.png"
            : "pack://application:,,,/Assets/background.png";

        string fallback = "pack://application:,,,/Assets/background.png";

        Background = TryCreateBackgroundBrush(uri)
                     ?? TryCreateBackgroundBrush(fallback)
                     ?? Brushes.Black;
    }

    private static Brush? TryCreateBackgroundBrush(string absolutePackUri)
    {
        try
        {
            var bmp = new BitmapImage();
            bmp.BeginInit();
            bmp.UriSource = new Uri(absolutePackUri, UriKind.Absolute);
            bmp.CacheOption = BitmapCacheOption.OnLoad;
            bmp.EndInit();
            bmp.Freeze();

            var brush = new ImageBrush(bmp)
            {
                Stretch = Stretch.Fill,
                AlignmentX = AlignmentX.Center,
                AlignmentY = AlignmentY.Center
            };
            brush.Freeze();

            return brush; // <- возвращаем как Brush
        }
        catch
        {
            return null;
        }
    }
}