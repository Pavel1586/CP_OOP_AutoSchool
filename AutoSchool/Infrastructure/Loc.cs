using System.Globalization;
using System.Windows;

namespace AutoSchool.Infrastructure
{
    public static class Loc
    {
        public static string T(string key)
            => Application.Current?.TryFindResource(key) as string ?? key;

        public static string F(string key, params object[] args)
            => string.Format(CultureInfo.CurrentUICulture, T(key), args);
    }
}