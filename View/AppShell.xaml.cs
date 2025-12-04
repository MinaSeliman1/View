using Microsoft.Maui.Controls;

namespace View;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        // Routes pour la navigation avec Shell
        Routing.RegisterRoute(nameof(LoginPage), typeof(LoginPage));
        Routing.RegisterRoute(nameof(ListeLivresPage), typeof(ListeLivresPage));
        Routing.RegisterRoute(nameof(DetailLivrePage), typeof(DetailLivrePage));
        Routing.RegisterRoute(nameof(EvaluationPage), typeof(EvaluationPage));
        Routing.RegisterRoute(nameof(FavorisPage), typeof(FavorisPage));
        Routing.RegisterRoute(nameof(AdminHomePage), typeof(AdminHomePage));
        Routing.RegisterRoute(nameof(AdminAjoutLivrePage), typeof(AdminAjoutLivrePage));
        Routing.RegisterRoute(nameof(AdminSuppressionPage), typeof(AdminSuppressionPage));
        Routing.RegisterRoute(nameof(AdminUtilisateursPage), typeof(AdminUtilisateursPage));
    }
}
