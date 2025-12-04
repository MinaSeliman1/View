using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bibliotheque.Model
{
    
    // Représente un compte utilisateur (client ou admin).
   
    public class Compte
    {
        public string Email { get; set; } = string.Empty;
        public string MotDePasse { get; set; } = string.Empty;
        public string Nom { get; set; } = string.Empty;
        public string Prenom { get; set; } = string.Empty;

       
        // Rôle du compte (Client ou Administrateur).
       
        public RoleCompte Role { get; set; } = RoleCompte.Client;

        public string NomComplet => $"{Prenom} {Nom}".Trim();
    }
}

