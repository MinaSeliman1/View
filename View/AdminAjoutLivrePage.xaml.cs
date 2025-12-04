using System;
using System.IO;
using Bibliotheque.Services;
using Bibliotheque.ViewModel;
using Microsoft.Maui.Controls;

namespace View;

public partial class AdminAjoutLivrePage : ContentPage
{
    public AdminAjoutLivrePage()
    {
        InitializeComponent();

        var cheminXml = Path.Combine(AppContext.BaseDirectory, "bibliotheque.xml");
        var service = new BibliothequeXmlService(cheminXml);

        BindingContext = new AdminAjoutLivreViewModel(service);
    }

    private async void OnRetourClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(AdminHomePage));

    }
}
