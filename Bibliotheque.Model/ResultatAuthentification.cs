using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bibliotheque.Model
{
    
    public class ResultatAuthentification
    {
        public bool EstValide { get; set; }
        public string Message { get; set; } = string.Empty;
        public Compte? Compte { get; set; }
    }
}

