using WhereISSit.Services;

namespace WhereISSit;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();

        string savedThemeChoice = ThemeService.GetSavedTheme();

        if (savedThemeChoice == "Dark")
        {
            ThemeService.ApplyDark();
        }
        else
        {
            ThemeService.ApplyLight();
        }

        MainPage = new AppShell();
    }
}