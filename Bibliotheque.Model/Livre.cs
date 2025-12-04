using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;
using Bibliotheque.Model;


namespace Bibliotheque.Model
{
    // tout les propriétés d'un livre
    public class Livre
    {
        public string Titre { get; set; } = string.Empty;
        public string Auteur { get; set; } = string.Empty;
        public string ISBN { get; set; } = string.Empty;
        public string MaisonEdition { get; set; } = string.Empty;
        public DateTime DatePublication { get; set; }
        public string Description { get; set; } = string.Empty;

        public double MoyenneEvaluation { get; set; }
        public int NombreEvaluations { get; set; }

        public void AjouterEvaluation(int note)
        {
            if (note < 0 || note > 5)
                throw new ArgumentOutOfRangeException(nameof(note), "La note doit être entre 0 et 5."); // si note en bas de 0 et en haut de 5

           // calcluer nouvelle moyenne
            
            double total = MoyenneEvaluation * NombreEvaluations + note;
            NombreEvaluations++;
            MoyenneEvaluation = total / NombreEvaluations;

        }
        
        public void ModifierEvaluation(int ancienneNote, int nouvelleNote)
        {
            if (NombreEvaluations <= 0)
                throw new InvalidOperationException("Aucune évaluation à modifier.");

            if (nouvelleNote < 0 || nouvelleNote > 5)
                throw new ArgumentOutOfRangeException(nameof(nouvelleNote), "La note doit être entre 0 et 5.");

            // modification d'une evaluation
            double total = MoyenneEvaluation * NombreEvaluations;
            total = total - ancienneNote + nouvelleNote;
            MoyenneEvaluation = total / NombreEvaluations;
        }
    }
}
