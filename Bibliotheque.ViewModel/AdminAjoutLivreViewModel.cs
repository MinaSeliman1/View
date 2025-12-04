using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Bibliotheque.Model;
using Bibliotheque.Services;
using Microsoft.Maui.Controls;

namespace Bibliotheque.ViewModel
{
    public class AdminAjoutLivreViewModel : BaseViewModel
    {
        private readonly IBibliothequeXmlService _service;

        private string _titre = string.Empty;
        private string _auteur = string.Empty;
        private string _isbn = string.Empty;
        private string _maisonEdition = string.Empty;
        private string _datePublicationTexte = string.Empty;
        private string _description = string.Empty;
        private string _message = string.Empty;

        public AdminAjoutLivreViewModel(IBibliothequeXmlService service)
        {
            _service = service;
            AjouterLivreCommand = new Command(async () => await AjouterLivreAsync(), () => !EstEnChargement);
        }

        public string Titre
        {
            get => _titre;
            set => SetProperty(ref _titre, value);
        }

        public string Auteur
        {
            get => _auteur;
            set => SetProperty(ref _auteur, value);
        }

        public string Isbn
        {
            get => _isbn;
            set => SetProperty(ref _isbn, value);
        }

        public string MaisonEdition
        {
            get => _maisonEdition;
            set => SetProperty(ref _maisonEdition, value);
        }

        public string DatePublicationTexte
        {
            get => _datePublicationTexte;
            set => SetProperty(ref _datePublicationTexte, value);
        }

        public string Description
        {
            get => _description;
            set => SetProperty(ref _description, value);
        }

        public string Message
        {
            get => _message;
            set => SetProperty(ref _message, value);
        }

        public ICommand AjouterLivreCommand { get; }

        private async Task AjouterLivreAsync()
        {
            if (EstEnChargement) return;

            try
            {
                EstEnChargement = true;
                Message = string.Empty;

                if (string.IsNullOrWhiteSpace(Titre) ||
                    string.IsNullOrWhiteSpace(Auteur) ||
                    string.IsNullOrWhiteSpace(Isbn))
                {
                    Message = "Titre, auteur et ISBN sont obligatoires.";
                    return;
                }

                DateTime datePub = DateTime.MinValue;
                if (!string.IsNullOrWhiteSpace(DatePublicationTexte))
                {
                    if (!DateTime.TryParse(DatePublicationTexte, out datePub))
                    {
                        Message = "La date de publication n'est pas valide (format AAAA-MM-JJ).";
                        return;
                    }
                }

                var livre = new Livre
                {
                    Titre = Titre,
                    Auteur = Auteur,
                    ISBN = Isbn,
                    MaisonEdition = MaisonEdition,
                    DatePublication = datePub == DateTime.MinValue ? DateTime.Today : datePub,
                    Description = Description,
                    MoyenneEvaluation = 0,
                    NombreEvaluations = 0
                };

                await _service.AjouterLivreAsync(livre);

                Message = "Livre ajouté avec succès.";

                // Reset des champs
                Titre = string.Empty;
                Auteur = string.Empty;
                Isbn = string.Empty;
                MaisonEdition = string.Empty;
                DatePublicationTexte = string.Empty;
                Description = string.Empty;
            }
            catch (Exception ex)
            {
                Message = $"Erreur lors de l'ajout : {ex.Message}";
            }
            finally
            {
                EstEnChargement = false;
                if (AjouterLivreCommand is Command cmd)
                    cmd.ChangeCanExecute();
            }
        }
    }
}
