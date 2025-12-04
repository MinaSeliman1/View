using System.IO;
using Bibliotheque.Services;
using Bibliotheque.ViewModel;
using Microsoft.Maui.Controls;

namespace View;

public partial class LoginPage : ContentPage
{
    public LoginPage()
    {
        InitializeComponent();

        // On crée le service XML et on le donne au ViewModel
        var cheminXml = Path.Combine(AppContext.BaseDirectory, "bibliotheque.xml");
        var service = new BibliothequeXmlService(cheminXml);

        BindingContext = new LoginViewModel(service);
    }
}
