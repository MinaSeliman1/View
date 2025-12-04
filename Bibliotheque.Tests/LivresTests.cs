using System;
using Bibliotheque.Model;
using NUnit.Framework;

namespace Bibliotheque.Tests
{
    [TestFixture]
    public class LivreTests
    {
        [Test]
        public void AjouterEvaluation_PremiereNote_InitialiseMoyenneEtCompteur()
        {
            // Arrange
            var livre = new Livre();

            // Act
            livre.AjouterEvaluation(4);

            // Assert
            Assert.That(livre.MoyenneEvaluation, Is.EqualTo(4.0).Within(0.0001));
            Assert.That(livre.NombreEvaluations, Is.EqualTo(1));
        }

        [Test]
        public void AjouterEvaluation_PlusieursNotes_CalculeBonneMoyenne()
        {
            // Arrange
            var livre = new Livre();

            // Act : notes 5, 3, 4 -> moyenne = 4
            livre.AjouterEvaluation(5);
            livre.AjouterEvaluation(3);
            livre.AjouterEvaluation(4);

            // Assert
            Assert.That(livre.NombreEvaluations, Is.EqualTo(3));
            Assert.That(livre.MoyenneEvaluation, Is.EqualTo(4.0).Within(0.0001));
        }

        [Test]
        public void ModifierEvaluation_ModifieMoyenne_SansChangerNombreEvaluations()
        {
            // Arrange : 2 notes -> (4 + 2) / 2 = 3
            var livre = new Livre();
            livre.AjouterEvaluation(4);
            livre.AjouterEvaluation(2);

            // Act : on remplace la note 2 par 5
            livre.ModifierEvaluation(ancienneNote: 2, nouvelleNote: 5);

            // Nouvelle moyenne attendue :
            // total initial = 3 * 2 = 6
            // nouveau total = 6 - 2 + 5 = 9
            // moyenne = 9 / 2 = 4.5
            Assert.That(livre.NombreEvaluations, Is.EqualTo(2));
            Assert.That(livre.MoyenneEvaluation, Is.EqualTo(4.5).Within(0.0001));
        }

        [Test]
        public void AjouterEvaluation_NoteInvalide_DeclencheArgumentOutOfRangeException()
        {
            var livre = new Livre();

            // Note < 0
            Assert.Throws<ArgumentOutOfRangeException>(() => livre.AjouterEvaluation(-1));

            // Note > 5
            Assert.Throws<ArgumentOutOfRangeException>(() => livre.AjouterEvaluation(6));
        }

        [Test]
        public void ModifierEvaluation_SansEvaluation_DeclencheInvalidOperationException()
        {
            var livre = new Livre();

            Assert.Throws<InvalidOperationException>(
                () => livre.ModifierEvaluation(ancienneNote: 3, nouvelleNote: 4));
        }
    }
}
