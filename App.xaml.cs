using System.Windows;

namespace FaturaQueryBuilder
{
    public partial class App : Application
    {
        public static void SwitchTheme(bool isDark)
        {
            var dict = new ResourceDictionary();
            dict.Source = new System.Uri(
                isDark ? "Themes/DarkTheme.xaml" : "Themes/LightTheme.xaml",
                System.UriKind.Relative);

            Current.Resources.MergedDictionaries[0] = dict;
        }
    }
}
