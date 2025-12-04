using Bibliotheque.ViewModel;
using Microsoft.Maui.Controls;

namespace View
{
    public partial class AdminHomePage : ContentPage
    {
        public AdminHomePage()
        {
            InitializeComponent();                    // on garde l’appel
            BindingContext = new AdminHomeViewModel();
        }
    }
}
