using System;
using Microsoft.Maui.Controls;

namespace View
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        // Handler demandé par le XAML
        private void OnCounterClicked(object sender, EventArgs e)
        {
            // Tu peux laisser vide si tu n'utilises pas cette page
            // ou mettre un Debug.WriteLine, etc.
        }
    }
}