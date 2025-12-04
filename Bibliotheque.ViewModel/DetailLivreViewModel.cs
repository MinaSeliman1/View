using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Bibliotheque.Model;
using Bibliotheque.Services;
using Microsoft.Maui.Controls;

namespace Bibliotheque.ViewModel
{
    public class DetailLivreViewModel : BaseViewModel
    {
        private readonly IBibliothequeXmlService _service;
        private Livre? _livre;
        private double _noteEnCours = 3; // valeur par défaut
        private string _message = string.Empty;

        public DetailLivreViewModel(IBibliothequeXmlService service)
        {
            _service = service;
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

                // On recharge le livre pour rafraîchir la moyenne
                var livres = await _service.ChargerLivresAsync();
                Livre = livres.FirstOrDefault(l => l.ISBN == Livre.ISBN);

                Message = "Votre évaluation a été enregistrée.";
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
