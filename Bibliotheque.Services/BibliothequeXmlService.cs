using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using Bibliotheque.Model;

namespace Bibliotheque.Services
{
    public class BibliothequeXmlService : IBibliothequeXmlService
    {
        private readonly string _cheminFichier;

        public BibliothequeXmlService(string cheminFichierBibliotheque)
        {
            _cheminFichier = cheminFichierBibliotheque;
        }

        // ================== FONCTION UTILITAIRE ==================

        /// <summary>
        /// Charge le XmlDocument de bibliotheque.xml.
        /// Si le fichier n'existe pas, crée une structure de base.
        /// </summary>
        private Task<XmlDocument> ChargerDocumentAsync()
        {
            return Task.Run(() =>
            {
                var doc = new XmlDocument();

                var dir = Path.GetDirectoryName(_cheminFichier);
                if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
                    Directory.CreateDirectory(dir);

                if (!File.Exists(_cheminFichier))
                {
                    // Création d’un document de base :
                    // <Bibliotheque>
                    //   <Livres />
                    //   <Comptes />
                    //   <Evaluations />
                    // </Bibliotheque>
                    var declaration = doc.CreateXmlDeclaration("1.0", "utf-8", null);
                    doc.AppendChild(declaration);

                    var racine = doc.CreateElement("Bibliotheque");
                    doc.AppendChild(racine);

                    racine.AppendChild(doc.CreateElement("Livres"));
                    racine.AppendChild(doc.CreateElement("Comptes"));
                    racine.AppendChild(doc.CreateElement("Evaluations"));

                    doc.Save(_cheminFichier);
                    return doc;
                }

                doc.Load(_cheminFichier);

                // On s’assure que la structure de base existe
                var root = doc.DocumentElement;
                if (root == null || root.Name != "Bibliotheque")
                {
                    doc.RemoveAll();

                    var declaration = doc.CreateXmlDeclaration("1.0", "utf-8", null);
                    doc.AppendChild(declaration);

                    root = doc.CreateElement("Bibliotheque");
                    doc.AppendChild(root);
                }

                if (root.SelectSingleNode("Livres") == null)
                    root.AppendChild(doc.CreateElement("Livres"));

                if (root.SelectSingleNode("Comptes") == null)
                    root.AppendChild(doc.CreateElement("Comptes"));

                if (root.SelectSingleNode("Evaluations") == null)
                    root.AppendChild(doc.CreateElement("Evaluations"));

                return doc;
            });
        }

        // ================== COMPTES ==================

        public async Task<IList<Compte>> ChargerComptesAsync()
        {
            var doc = await ChargerDocumentAsync();
            var result = new List<Compte>();

            var comptesNodes = doc.SelectNodes("/Bibliotheque/Comptes/Compte");
            if (comptesNodes == null)
                return result;

            foreach (XmlNode node in comptesNodes)
            {
                if (node is not XmlElement elem)
                    continue;

                var compte = new Compte
                {
                    Email = elem["Email"]?.InnerText ?? string.Empty,
                    MotDePasse = elem["MotDePasse"]?.InnerText ?? string.Empty,
                    Nom = elem["Nom"]?.InnerText ?? string.Empty,
                    Prenom = elem["Prenom"]?.InnerText ?? string.Empty
                    // Role fixé dans le ViewModel (RoleCompte.Client)
                };

                result.Add(compte);
            }

            return result;
        }

        // ================== LIVRES ==================

        public async Task<IList<Livre>> ChargerLivresAsync()
        {
            var doc = await ChargerDocumentAsync();
            var livres = new List<Livre>();

            var livresNodes = doc.SelectNodes("/Bibliotheque/Livres/Livre");
            if (livresNodes == null)
                return livres;

            foreach (XmlNode node in livresNodes)
            {
                if (node is not XmlElement elem)
                    continue;

                string titre = elem["Titre"]?.InnerText ?? string.Empty;
                string auteur = elem["Auteur"]?.InnerText ?? string.Empty;
                string isbn = elem["ISBN"]?.InnerText ?? string.Empty;
                string maisonEdition = elem["MaisonEdition"]?.InnerText ?? string.Empty;
                string description = elem["Description"]?.InnerText ?? string.Empty;

                DateTime datePub;
                if (!DateTime.TryParse(elem["DatePublication"]?.InnerText, out datePub))
                {
                    datePub = DateTime.MinValue;
                }

                double moyenne;
                if (!double.TryParse(elem["MoyenneEvaluation"]?.InnerText,
                                     NumberStyles.Any,
                                     CultureInfo.InvariantCulture,
                                     out moyenne))
                {
                    moyenne = 0.0;
                }

                int nbEvaluations;
                if (!int.TryParse(elem["NombreEvaluations"]?.InnerText, out nbEvaluations))
                {
                    nbEvaluations = 0;
                }

                livres.Add(new Livre
                {
                    Titre = titre,
                    Auteur = auteur,
                    ISBN = isbn,
                    MaisonEdition = maisonEdition,
                    DatePublication = datePub,
                    Description = description,
                    MoyenneEvaluation = moyenne,
                    NombreEvaluations = nbEvaluations
                });
            }

            return livres;
        }

        public async Task<Livre?> ObtenirLivreParIsbnAsync(string isbn)
        {
            if (string.IsNullOrWhiteSpace(isbn))
                return null;

            var livres = await ChargerLivresAsync();

            return livres.FirstOrDefault(l =>
                string.Equals(l.ISBN, isbn, StringComparison.OrdinalIgnoreCase));
        }

        public async Task AjouterLivreAsync(Livre livre)
        {
            if (livre == null)
                throw new ArgumentNullException(nameof(livre));

            var doc = await ChargerDocumentAsync();
            var racine = doc.DocumentElement ?? throw new InvalidOperationException("XML invalide.");

            var livresElem = racine.SelectSingleNode("Livres") as XmlElement;
            if (livresElem == null)
            {
                livresElem = doc.CreateElement("Livres");
                racine.AppendChild(livresElem);
            }

            // Vérifier ISBN dupliqué
            XmlElement? existant = null;
            foreach (XmlNode n in livresElem.ChildNodes)
            {
                if (n is not XmlElement e || e.Name != "Livre")
                    continue;

                var isbnNode = e["ISBN"];
                if (isbnNode != null &&
                    string.Equals(isbnNode.InnerText, livre.ISBN,
                                  StringComparison.OrdinalIgnoreCase))
                {
                    existant = e;
                    break;
                }
            }

            if (existant != null)
                throw new InvalidOperationException("Un livre avec cet ISBN existe déjà.");

            var nouvelElement = doc.CreateElement("Livre");

            void AddChild(string nom, string valeur)
            {
                var child = doc.CreateElement(nom);
                child.InnerText = valeur;
                nouvelElement.AppendChild(child);
            }

            AddChild("Titre", livre.Titre);
            AddChild("Auteur", livre.Auteur);
            AddChild("ISBN", livre.ISBN);
            AddChild("MaisonEdition", livre.MaisonEdition);
            AddChild("DatePublication", livre.DatePublication.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture));
            AddChild("Description", livre.Description);
            AddChild("MoyenneEvaluation", livre.MoyenneEvaluation.ToString(CultureInfo.InvariantCulture));
            AddChild("NombreEvaluations", livre.NombreEvaluations.ToString());

            livresElem.AppendChild(nouvelElement);

            doc.Save(_cheminFichier);
        }

        public async Task SupprimerLivreParIsbnAsync(string isbn)
        {
            if (string.IsNullOrWhiteSpace(isbn))
                return; // reoutne vrai si null ou vide

            var doc = await ChargerDocumentAsync();
            var racine = doc.DocumentElement;
            if (racine == null)
                return;

            var livresElem = racine.SelectSingleNode("Livres") as XmlElement;
            if (livresElem != null)
            {
                XmlElement? cible = null;
                foreach (XmlNode n in livresElem.ChildNodes)
                {
                    if (n is not XmlElement e || e.Name != "Livre")
                        continue;

                    var isbnNode = e["ISBN"];
                    if (isbnNode != null &&
                        string.Equals(isbnNode.InnerText, isbn,
                                      StringComparison.OrdinalIgnoreCase))
                    {
                        cible = e;
                        break;
                    }
                }

                if (cible != null)
                    livresElem.RemoveChild(cible);
            }

            // Si tu veux, tu pourrais ici aussi supprimer les <Evaluation> associées à ce livre

            doc.Save(_cheminFichier);
        }

        // ================== EVALUATIONS ==================

        public async Task MettreAJourEvaluationAsync(string isbnLivre, string emailClient, int nouvelleNote)
        {
            var doc = await ChargerDocumentAsync();

            var racine = doc.DocumentElement
                         ?? throw new InvalidOperationException("XML invalide : pas de racine <Bibliotheque>.");

            // ====== SECTION <Evaluations> ======
            var evaluationsElem = racine.SelectSingleNode("Evaluations") as XmlElement;
            if (evaluationsElem == null)
            {
                evaluationsElem = doc.CreateElement("Evaluations");
                racine.AppendChild(evaluationsElem);
            }

            // On cherche une évaluation existante pour (email, isbn)
            XmlElement? evalExistante = null;
            foreach (XmlNode n in evaluationsElem.ChildNodes)
            {
                if (n is not XmlElement e || e.Name != "Evaluation")
                    continue;

                var isbnNode = e["ISBN"];
                var emailNode = e["Email"];

                if (isbnNode == null || emailNode == null)
                    continue;

                if (isbnNode.InnerText == isbnLivre &&
                    string.Equals(emailNode.InnerText, emailClient,
                                  StringComparison.OrdinalIgnoreCase))
                {
                    evalExistante = e;
                    break;
                }
            }

            bool estNouvelleEvaluation = evalExistante == null;
            int ancienneNote = 0;

            if (estNouvelleEvaluation)
            {
                var eval = doc.CreateElement("Evaluation");

                var isbnElem = doc.CreateElement("ISBN");
                isbnElem.InnerText = isbnLivre;
                eval.AppendChild(isbnElem);

                var emailElem = doc.CreateElement("Email");
                emailElem.InnerText = emailClient;
                eval.AppendChild(emailElem);

                var noteElem = doc.CreateElement("Note");
                noteElem.InnerText = nouvelleNote.ToString(CultureInfo.InvariantCulture);
                eval.AppendChild(noteElem);

                evaluationsElem.AppendChild(eval);
            }
            else
            {
                var noteNode = evalExistante["Note"];
                if (noteNode != null)
                {
                    int.TryParse(noteNode.InnerText, out ancienneNote);
                    noteNode.InnerText = nouvelleNote.ToString(CultureInfo.InvariantCulture);
                }
            }

            // ====== MISE À JOUR DU LIVRE (moyenne + nb) ======
            var livresElem = racine.SelectSingleNode("Livres") as XmlElement;
            if (livresElem != null)
            {
                XmlElement? livreElem = null;
                foreach (XmlNode n in livresElem.ChildNodes)
                {
                    if (n is not XmlElement e || e.Name != "Livre")
                        continue;

                    var isbnNode = e["ISBN"];
                    if (isbnNode != null && isbnNode.InnerText == isbnLivre)
                    {
                        livreElem = e;
                        break;
                    }
                }

                if (livreElem != null)
                {
                    double moyenneActuelle = 0.0;
                    int nbDeNote = 0;

                    var moyenneNode = livreElem["MoyenneEvaluation"];
                    if (moyenneNode != null)
                    {
                        double.TryParse(moyenneNode.InnerText,
                            NumberStyles.Any,
                            CultureInfo.InvariantCulture,
                            out moyenneActuelle);
                    }

                    var nbNode = livreElem["NombreEvaluations"];
                    if (nbNode != null)
                    {
                        int.TryParse(nbNode.InnerText, out nbDeNote);
                    }

                    double nouvelleMoyenne;
                    int nouveauNbDeNote;

                    if (estNouvelleEvaluation || nbDeNote == 0)
                    {
                        double total = moyenneActuelle * nbDeNote + nouvelleNote;
                        nouveauNbDeNote = nbDeNote + 1;
                        nouvelleMoyenne = total / nouveauNbDeNote;
                    }
                    else
                    {
                        double total = moyenneActuelle * nbDeNote;
                        total = total - ancienneNote + nouvelleNote;
                        nouveauNbDeNote = nbDeNote;
                        nouvelleMoyenne = total / nouveauNbDeNote;
                    }

                    if (moyenneNode == null)
                    {
                        moyenneNode = doc.CreateElement("MoyenneEvaluation");
                        livreElem.AppendChild(moyenneNode);
                    }
                    moyenneNode.InnerText = nouvelleMoyenne.ToString("0.0", CultureInfo.InvariantCulture);

                    if (nbNode == null)
                    {
                        nbNode = doc.CreateElement("NombreEvaluations");
                        livreElem.AppendChild(nbNode);
                    }
                    nbNode.InnerText = nouveauNbDeNote.ToString();
                }
            }

            doc.Save(_cheminFichier);
        }
    }
}
