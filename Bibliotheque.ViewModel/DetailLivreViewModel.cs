using Bibliotheque.Model;
using Bibliotheque.Services;
using System.Windows.Input;

namespace Bibliotheque.ViewModel
{
    public class DetailLivreViewModel : BaseViewModel
    {
        private readonly IBibliothequeXmlService _service;
        private readonly IFavorisService _favorisService; // Nouvelle dépendance pour les favoris
        private Livre? _livre;
        private double _noteEnCours = 3; // valeur par défaut
        private string _message = string.Empty;

        // Mise à jour du constructeur pour accepter le service des favoris
        public DetailLivreViewModel(IBibliothequeXmlService service, IFavorisService favorisService)
        {
            _service = service;
            _favorisService = favorisService; // Initialisation du service favoris
            EnregistrerNoteCommand = new Command(
                async () => await EnregistrerNoteAsync(),
                () => !EstEnChargement
                & Livre != null);
        }

        public Livre? Livre
        {
            get => _livre;
            set => SetProperty(ref _livre, value);
        }

        /// <summary>
        /// Note sélectionnée par l'utilisateur (0 à 5).
        /// </summary>
        public double NoteEnCours
        {
            get => _noteEnCours;
            set => SetProperty(ref _noteEnCours, value);
        }

        public string Message
        {
            get => _message;
            set => SetProperty(ref _message, value);
        }

        public ICommand EnregistrerNoteCommand { get; }

        public async Task ChargerLivreParIsbnAsync(string isbn)
        {
            var livres = await _service.ChargerLivresAsync();
            Livre = livres.FirstOrDefault(l => l.ISBN == isbn);

            // on remet une note par défaut
            NoteEnCours = 3;
            Message = string.Empty;
        }

        private async Task EnregistrerNoteAsync()
        {
            if (Livre == null)
                return;

            var email = SessionUtilisateur.CourrielConnecte;
            if (string.IsNullOrWhiteSpace(email))
            {
                Message = "Vous devez être connecté pour évaluer un livre.";
                return;
            }

            int note = (int)Math.Round(NoteEnCours);
            if (note < 0) note = 0;
            if (note > 5) note = 5;

            try
            {
                EstEnChargement = true;
                Message = string.Empty;

                await _service.MettreAJourEvaluationAsync(Livre.ISBN, email, note);

                // On recharge le livre pour rafraîchir la moyenne (obligatoire pour la logique Favoris)
                var livres = await _service.ChargerLivresAsync();
                Livre = livres.FirstOrDefault(l => l.ISBN == Livre.ISBN);

                // === NOUVELLE LOGIQUE POUR LES FAVORIS (Note >= 4.0) ===
                if (Livre != null)
                {
                    // L'appel à MettreAJourFavoriAsync gère l'ajout si >= 4.0 et la suppression si < 4.0
                    await _favorisService.MettreAJourFavoriAsync(email, Livre);

                    if (Livre.MoyenneEvaluation >= 4.0 && Livre.NombreEvaluations > 0)
                    {
                        Message = "Votre évaluation a été enregistrée. Le livre est désormais dans vos favoris.";
                    }
                    else
                    {
                        Message = "Votre évaluation a été enregistrée.";
                    }
                }
                else
                {
                    Message = "Votre évaluation a été enregistrée, mais les détails du livre n'ont pas pu être rechargés.";
                }
            }
            catch (Exception ex)
            {
                Message = $"Erreur lors de l'enregistrement de l'évaluation : {ex.Message}";
            }
            finally
            {
                EstEnChargement = false;
                if (EnregistrerNoteCommand is Command cmd)
                    cmd.ChangeCanExecute();
            }
        }
    }
}