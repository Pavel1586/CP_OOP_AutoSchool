using System;
using System.Windows;
using System.Windows.Input;
using AutoSchool.Infrastructure;

namespace AutoSchool.ViewModels
{
    public class SettingsViewModel : BaseViewModel
    {
        public ICommand CloseCommand { get; }

        public SettingsViewModel()
        {
            CloseCommand = new RelayCommand(w => (w as Window)?.Close());

            LocalizationManager.LanguageChanged += (_, __) =>
            {
                OnPropertyChanged(nameof(IsRussian));
                OnPropertyChanged(nameof(IsEnglish));
            };
        }

        // ===== THEME =====
        public bool IsDarkTheme
        {
            get => ThemeManager.CurrentTheme == AppTheme.Dark;
            set
            {
                if (value) ThemeManager.Apply(AppTheme.Dark);
                else ThemeManager.Apply(AppTheme.Light);

                OnPropertyChanged();
                OnPropertyChanged(nameof(IsLightTheme));
            }
        }

        public bool IsLightTheme
        {
            get => ThemeManager.CurrentTheme == AppTheme.Light;
            set
            {
                if (value) ThemeManager.Apply(AppTheme.Light);
                else ThemeManager.Apply(AppTheme.Dark);

                OnPropertyChanged();
                OnPropertyChanged(nameof(IsDarkTheme));
            }
        }

        // ===== LANGUAGE =====
        public bool IsRussian
        {
            get => LocalizationManager.CurrentLanguage.StartsWith("ru", StringComparison.OrdinalIgnoreCase);
            set
            {
                if (value) LocalizationManager.Apply("ru-RU");
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsEnglish));
            }
        }

        public bool IsEnglish
        {
            get => LocalizationManager.CurrentLanguage.StartsWith("en", StringComparison.OrdinalIgnoreCase);
            set
            {
                if (value) LocalizationManager.Apply("en-US");
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsRussian));
            }
        }
    }
}