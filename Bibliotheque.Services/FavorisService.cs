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
    public class FavorisService : IFavorisService
    {
        private readonly string _cheminFichier;

        public FavorisService(string cheminFichierFavoris)
        {
            _cheminFichier = cheminFichierFavoris;
        }

        // ================== FONCTIONS UTILITAIRES POUR XML ==================

        /// <summary>
        /// Charge le XmlDocument de favoris.xml. Crée une structure de base si le fichier n'existe pas.
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
                    var declaration = doc.CreateXmlDeclaration("1.0", "utf-8", null);
                    doc.AppendChild(declaration);
                    var racine = doc.CreateElement("Favoris");
                    doc.AppendChild(racine);
                    doc.Save(_cheminFichier);
                    return doc;
                }

                doc.Load(_cheminFichier);
                return doc;
            });
        }

        private XmlElement? ObtenirElementClient(XmlDocument doc, string emailClient)
        {
            // Utilise un XPath sécurisé pour trouver l'élément Client
            return (XmlElement?)doc.SelectSingleNode($"/Favoris/Client[@Email='{emailClient}']");
        }

        private XmlElement CreerElementClient(XmlDocument doc, string emailClient)
        {
            var element = doc.CreateElement("Client");
            var emailAttr = doc.CreateAttribute("Email");
            emailAttr.Value = emailClient;
            element.Attributes.Append(emailAttr);
            return element;
        }

        private Livre XmlElementToLivre(XmlElement elem)
        {
            DateTime.TryParse(elem["DatePublication"]?.InnerText, out var datePub);

            return new Livre
            {
                Titre = elem["Titre"]?.InnerText ?? string.Empty,
                Auteur = elem["Auteur"]?.InnerText ?? string.Empty,
                ISBN = elem["ISBN"]?.InnerText ?? string.Empty,
                MaisonEdition = elem["MaisonEdition"]?.InnerText ?? string.Empty,
                Description = elem["Description"]?.InnerText ?? string.Empty,
                MoyenneEvaluation = double.TryParse(elem["MoyenneEvaluation"]?.InnerText, NumberStyles.Any, CultureInfo.InvariantCulture, out var m) ? m : 0.0,
                NombreEvaluations = int.TryParse(elem["NombreEvaluations"]?.InnerText, out var n) ? n : 0,
                DatePublication = datePub,
            };
        }

        private XmlElement LivreToXmlElement(XmlDocument doc, Livre livre)
        {
            var element = doc.CreateElement("Livre");
            void AddChild(string nom, string valeur)
            {
                var child = doc.CreateElement(nom);
                child.InnerText = valeur;
                element.AppendChild(child);
            }

            AddChild("Titre", livre.Titre);
            AddChild("Auteur", livre.Auteur);
            AddChild("ISBN", livre.ISBN);
            AddChild("MaisonEdition", livre.MaisonEdition);
            AddChild("DatePublication", livre.DatePublication.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture));
            AddChild("Description", livre.Description);
            AddChild("MoyenneEvaluation", livre.MoyenneEvaluation.ToString(CultureInfo.InvariantCulture));
            AddChild("NombreEvaluations", livre.NombreEvaluations.ToString());

            return element;
        }

        // ================== IMPLEMENTATION ==================

        public async Task<IList<Livre>> ChargerFavorisAsync(string emailClient)
        {
            if (string.IsNullOrWhiteSpace(emailClient))
                return new List<Livre>();

            var doc = await ChargerDocumentAsync();
            var result = new List<Livre>();
            emailClient = emailClient.Trim();

            // Note: XPath pour attribut Email='...'
            var clientNode = (XmlElement?)doc.SelectSingleNode($"/Favoris/Client[@Email='{emailClient}']");
            if (clientNode == null)
                return result;

            var livresNodes = clientNode.SelectNodes("Livre");
            if (livresNodes == null)
                return result;

            foreach (XmlNode node in livresNodes)
            {
                if (node is XmlElement elem)
                {
                    result.Add(XmlElementToLivre(elem));
                }
            }

            return result;
        }

        public async Task MettreAJourFavoriAsync(string emailClient, Livre livre)
        {
            if (livre == null || string.IsNullOrWhiteSpace(emailClient) || string.IsNullOrWhiteSpace(livre.ISBN))
                return;

            var doc = await ChargerDocumentAsync();
            var racine = doc.DocumentElement ?? throw new InvalidOperationException("XML Favoris invalide.");
            emailClient = emailClient.Trim();

            var clientNode = (XmlElement?)doc.SelectSingleNode($"/Favoris/Client[@Email='{emailClient}']");
            if (clientNode == null)
            {
                clientNode = CreerElementClient(doc, emailClient);
                racine.AppendChild(clientNode);
            }

            // Chercher si le livre existe déjà
            XmlElement? livreExistant = null;
            foreach (XmlNode n in clientNode.ChildNodes)
            {
                if (n is XmlElement e && e.Name == "Livre")
                {
                    var isbnNode = e["ISBN"];
                    if (isbnNode != null && string.Equals(isbnNode.InnerText, livre.ISBN, StringComparison.OrdinalIgnoreCase))
                    {
                        livreExistant = e;
                        break;
                    }
                }
            }

            // Si le livre a une note < 4.0, on le supprime des favoris
            if (livre.MoyenneEvaluation < 4.0)
            {
                if (livreExistant != null)
                {
                    clientNode.RemoveChild(livreExistant);
                    doc.Save(_cheminFichier);
                }
                return;
            }

            // Si le livre est >= 4.0, on l'ajoute ou on le met à jour
            if (livreExistant != null)
            {
                // Mise à jour: on retire l'ancien et on insère le nouveau avec les notes à jour
                var nouvelElement = LivreToXmlElement(doc, livre);
                clientNode.ReplaceChild(nouvelElement, livreExistant);
            }
            else
            {
                // Ajout: c'est un nouveau favori
                var nouvelElement = LivreToXmlElement(doc, livre);
                clientNode.AppendChild(nouvelElement);
            }

            doc.Save(_cheminFichier);
        }
    }
}