using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bibliotheque.Model
{

    // client a pas les meme access que admin
    public enum RoleCompte
    {
        Client,
        Administrateur
    }
}
