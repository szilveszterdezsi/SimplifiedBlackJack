/// ---------------------------
/// Author: Szilveszter Dezsi
/// Created: 2018-10-31
/// Modified: n/a
/// ---------------------------
/// 
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;

namespace PL
{
    /// <summary>
    /// Presentation dialog class used for starting a new game and I/O interaction with the user.
    /// Primarily used to pass new game variables from user to MainWindow.
    /// </summary>
    public partial class NewGameDialog : Window
    {
        public int players;
        public int decks;
        public bool valuesSet = false;

        /// <summary>
        /// Constructor.
        /// </summary>
        public NewGameDialog()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Detects when the "Start Game" button is clicked.
        /// Sets variables for number of players and decks and closes itself.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">EventArgs.</param>
        private void btnStartGame_Click(object sender, RoutedEventArgs e)
        {
            if (InputCheck())
            {
                valuesSet = true;
                players = int.Parse(tbPlayers.Text);
                decks = int.Parse(tbDecks.Text);
                Close();
            }
        }

        /// <summary>
        /// Detects when text is entered in inputs.
        /// Rejects any charachters that are not digits (0-9).
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">TextCompositionEventArgs.</param>
        private void NumberCheckTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        /// <summary>
        /// Checks all inputs and displays a messages if any inputs are null, empty, not digits or zero (0).
        /// Also warns to increase numbers of decks if ((players+dealer)*8.6) are greater than (decks*52).
        /// </summary>
        /// <returns>True if input is correct, otherwise false.</returns>
        private bool InputCheck()
        {
            string title = "Incorrect Input";
            MessageBoxButton button = MessageBoxButton.OK;
            MessageBoxImage image = MessageBoxImage.Warning;
            if (string.IsNullOrEmpty(tbPlayers.Text) || !tbPlayers.Text.All(char.IsDigit) || tbPlayers.Text.Equals("0"))
                MessageBox.Show("Please enter number of players!", title, button, image);
            else if (string.IsNullOrEmpty(tbDecks.Text) || !tbDecks.Text.All(char.IsDigit) || tbDecks.Text.Equals("0"))
                MessageBox.Show("Please enter number of decks!", title, button, image);
            else if ((double.Parse(tbPlayers.Text)+1)*8.6 > int.Parse(tbDecks.Text)*52)
                MessageBox.Show("Please increase number of decks for " + tbPlayers.Text + " players to minimize risk of running out of cards mid round. Dealer in this version of the game still needs more training.", title, button, image);
            else
                return true;
            return false;
        }
    }
}
