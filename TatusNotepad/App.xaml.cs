using System.Globalization;

namespace TatusNotepad
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            var culture = new CultureInfo("pl-pl");
            Thread.CurrentThread.CurrentCulture = culture;
            Thread.CurrentThread.CurrentUICulture = culture;
            MainPage = new AppShell();
        }
    }
}
