/// ---------------------------
/// Author: Szilveszter Dezsi
/// Created: 2018-10-31
/// Modified: n/a
/// ---------------------------

using System.Windows;
using System.Windows.Controls;
using BLL;

namespace PL
{
    /// <summary>
    /// Partial presentation class that handles button events and I/O interaction with the user.
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Detects when player name is edited and automatically resizes column width to fit updated name.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">TextChangedEventArgs.</param>
        private void txtChangedEventHandler(object sender, TextChangedEventArgs e)
        {
            AutoResizeColumns();
        }

        /// <summary>
        /// Detects when "HIT"-button is clicked and forwards sender player to controller.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">EventArgs.</param>
        private void btnHitPlayer_Click(object sender, RoutedEventArgs e)
        {
            Player player = (Player)((FrameworkElement)sender).DataContext;
            gameController.HitPlayer(player.ID, player.Name);
        }

        /// <summary>
        /// Detects when "STAND"-button is clicked and forwards sender player to controller.
        /// Also posts update about which player chose to stand.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">EventArgs.</param>
        private void btnStandPlayer_Click(object sender, RoutedEventArgs e)
        {
            Player player = (Player)((FrameworkElement)sender).DataContext;
            Updates.Insert(0, player.Name + " stands.");
            gameController.NextPlayer(player.ID);
        }

        /// <summary>
        /// Detects when name "SAVE"-button is clicked and forwards sender player to controller.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">EventArgs.</param>
        private void btnSavePlayerName_Click(object sender, RoutedEventArgs e)
        {
            gameController.SavePlayerName(((Player)((FrameworkElement)sender).DataContext).ID);
        }

        /// <summary>
        /// Detects when name "EDIT"-button is clicked and forwards sender player to controller.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">EventArgs.</param>
        private void btnEditPlayerName_Click(object sender, RoutedEventArgs e)
        {
            gameController.EditPlayerName(((Player)((FrameworkElement)sender).DataContext).ID);
        }

        /// <summary>
        /// Detects when name "Request Shuffle"-button is clicked and forwards to deck.
        /// Also posts and update that deck will be shuffles before next round.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">EventArgs.</param>
        private void btnRequestDeckShuffle_Click(object sender, RoutedEventArgs e)
        {
            Updates.Insert(0, "Deck will be shuffled before next round!");
            Deck.ShuffleRequest = true;
        }
    }
}
