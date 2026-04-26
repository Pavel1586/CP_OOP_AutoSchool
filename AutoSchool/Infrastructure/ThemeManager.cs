using System;
using System.Windows;

namespace AutoSchool.Infrastructure
{
    public enum AppTheme { Light, Dark }

    public static class ThemeManager
    {
        public static event EventHandler? ThemeChanged;

        private static readonly Uri LightUri = new("Themes/LightTheme.xaml", UriKind.Relative);
        private static readonly Uri DarkUri = new("Themes/DarkTheme.xaml", UriKind.Relative);

        public static AppTheme CurrentTheme { get; private set; } = AppTheme.Light;

        public static void Apply(AppTheme theme)
        {
            var app = Application.Current;
            if (app == null) return;

            var dicts = app.Resources.MergedDictionaries;

            RemoveByUri(dicts, LightUri);
            RemoveByUri(dicts, DarkUri);

            dicts.Add(new ResourceDictionary
            {
                Source = theme == AppTheme.Dark ? DarkUri : LightUri
            });

            CurrentTheme = theme;
            ThemeChanged?.Invoke(null, EventArgs.Empty);
        }

        private static void RemoveByUri(System.Collections.Generic.IList<ResourceDictionary> dicts, Uri uri)
        {
            for (int i = dicts.Count - 1; i >= 0; i--)
            {
                var src = dicts[i].Source;
                if (src == null) continue;

                if (string.Equals(src.OriginalString, uri.OriginalString, StringComparison.OrdinalIgnoreCase))
                    dicts.RemoveAt(i);
            }
        }
    }
}