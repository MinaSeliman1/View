using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using System.Xml;
using Bibliotheque.Services;
using NUnit.Framework;

namespace Bibliotheque.Tests
{
    [TestFixture]
    public class BibliothequeXmlServiceTests
    {
        private string _cheminTemp;

        [SetUp]
        public void SetUp()
        {
            _cheminTemp = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName() + ".xml");

            // Création du XML avec XmlDocument (plus de XDocument)
            var doc = new XmlDocument();

            var declaration = doc.CreateXmlDeclaration("1.0", "utf-8", null);
            doc.AppendChild(declaration);

            var racine = doc.CreateElement("Bibliotheque");
            doc.AppendChild(racine);

            var comptesElem = doc.CreateElement("Comptes");
            racine.AppendChild(comptesElem);

            var livresElem = doc.CreateElement("Livres");
            racine.AppendChild(livresElem);

            var livreElem = doc.CreateElement("Livre");
            livresElem.AppendChild(livreElem);

            void AddChild(XmlElement parent, string nom, string valeur)
            {
                var child = doc.CreateElement(nom);
                child.InnerText = valeur;
                parent.AppendChild(child);
            }

            AddChild(livreElem, "Titre", "Livre de test");
            AddChild(livreElem, "Auteur", "Auteur");
            AddChild(livreElem, "ISBN", "TEST-ISBN");
            AddChild(livreElem, "MaisonEdition", "Maison");
            AddChild(livreElem, "DatePublication", "2000-01-01");
            AddChild(livreElem, "Description", "Description de test");
            AddChild(livreElem, "MoyenneEvaluation", "0.0");
            AddChild(livreElem, "NombreEvaluations", "0");

            var evalsElem = doc.CreateElement("Evaluations");
            racine.AppendChild(evalsElem);

            doc.Save(_cheminTemp);
        }

        [TearDown]
        public void TearDown()
        {
            if (File.Exists(_cheminTemp))
                File.Delete(_cheminTemp);
        }

        [Test]
        public async Task MettreAJourEvaluationAsync_PremiereEvaluation_MetAJourMoyenneEtCompteur()
        {
            // Arrange
            var service = new BibliothequeXmlService(_cheminTemp);

            // Act : première note = 4
            await service.MettreAJourEvaluationAsync("TEST-ISBN", "client@test.com", 4);

            // Assert : on relit avec XmlDocument
            var doc = new XmlDocument();
            doc.Load(_cheminTemp);

            var livreElem = (XmlElement?)doc.SelectSingleNode("/Bibliotheque/Livres/Livre[ISBN='TEST-ISBN']");
            Assert.IsNotNull(livreElem, "Le livre TEST-ISBN devrait exister dans le XML.");

            var moyenneText = livreElem["MoyenneEvaluation"]?.InnerText ?? "0";
            var nbText = livreElem["NombreEvaluations"]?.InnerText ?? "0";

            double moyenne = double.Parse(moyenneText, CultureInfo.InvariantCulture);
            int nb = int.Parse(nbText);

            Assert.That(nb, Is.EqualTo(1));
            Assert.That(moyenne, Is.EqualTo(4.0).Within(0.0001));
        }

        [Test]
        public async Task MettreAJourEvaluationAsync_ModifieNoteExistante_NeChangePasCompteur_MetAJourMoyenne()
        {
            // Arrange
            var service = new BibliothequeXmlService(_cheminTemp);

            // Act : même utilisateur, même livre, 2 notes de suite
            await service.MettreAJourEvaluationAsync("TEST-ISBN", "client@test.com", 4);
            await service.MettreAJourEvaluationAsync("TEST-ISBN", "client@test.com", 5);

            // Assert
            var doc = new XmlDocument();
            doc.Load(_cheminTemp);

            var livreElem = (XmlElement?)doc.SelectSingleNode("/Bibliotheque/Livres/Livre[ISBN='TEST-ISBN']");
            Assert.IsNotNull(livreElem, "Le livre TEST-ISBN devrait exister dans le XML.");

            var moyenneText = livreElem["MoyenneEvaluation"]?.InnerText ?? "0";
            var nbText = livreElem["NombreEvaluations"]?.InnerText ?? "0";

            double moyenne = double.Parse(moyenneText, CultureInfo.InvariantCulture);
            int nb = int.Parse(nbText);

            // Une seule évaluation pour ce couple (email, isbn)
            Assert.That(nb, Is.EqualTo(1));
            // Comme il n'y a qu'un évaluateur, la moyenne = dernière note
            Assert.That(moyenne, Is.EqualTo(5.0).Within(0.0001));
        }
    }
}
