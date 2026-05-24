using System;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Windows;

namespace AutoSchool.Infrastructure
{
    public static class LocalizationManager
    {
        public static event EventHandler? LanguageChanged;

        private static readonly Uri BaseRuUri = new("Localization/Strings.xaml", UriKind.Relative);
        private static readonly Uri EnUri = new("Localization/Strings.en-US.xaml", UriKind.Relative);

        public static string CurrentLanguage { get; private set; } = "ru-RU";

        public static void Apply(string cultureName)
        {
            var app = Application.Current;
            if (app == null) return;

            var culture = new CultureInfo(cultureName);
            Thread.CurrentThread.CurrentCulture = culture;
            Thread.CurrentThread.CurrentUICulture = culture;

            var dicts = app.Resources.MergedDictionaries;

            // RU base always loaded
            if (!dicts.Any(d => string.Equals(d.Source?.OriginalString, BaseRuUri.OriginalString, StringComparison.OrdinalIgnoreCase)))
                dicts.Insert(0, new ResourceDictionary { Source = BaseRuUri });

            // remove EN overlay if exists
            for (int i = dicts.Count - 1; i >= 0; i--)
            {
                var src = dicts[i].Source?.OriginalString ?? "";
                if (string.Equals(src, EnUri.OriginalString, StringComparison.OrdinalIgnoreCase))
                    dicts.RemoveAt(i);
            }

            // add EN overlay if needed
            if (cultureName.StartsWith("en", StringComparison.OrdinalIgnoreCase))
                dicts.Add(new ResourceDictionary { Source = EnUri });

            CurrentLanguage = cultureName;
            LanguageChanged?.Invoke(null, EventArgs.Empty);
        }
    }
}