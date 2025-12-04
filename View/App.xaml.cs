using Microsoft.Maui.Controls;

namespace View;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();

        // On utilise la navigation avec Shell
        MainPage = new AppShell();
    }
}
