using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bibliotheque.Model
{
   // stock les info de l'utilisateur connecter 
    public static class SessionUtilisateur
    {
        public static string? CourrielConnecte { get; set; }
        public static bool EstAdmin { get; set; }
    }
}
