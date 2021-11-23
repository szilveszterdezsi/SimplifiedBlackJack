/// ---------------------------
/// Author: Szilveszter Dezsi
/// Created: 2018-10-31
/// Modified: n/a
/// ---------------------------

using System;
using System.Windows;
using BLL;

namespace PL
{
    /// <summary>
    /// Partial presentation class that handles game events and I/O interaction with the user.
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Subcribes to all game related events.
        /// </summary>
        private void SubscribeToGameUpdates()
        {
            gameController.HitPlayerEvent += new GameController.HitPlayerEventHandler(HitPlayerEvent);
            gameController.ResetPlayersEvent += new EventHandler(ResetPlayersEvent);
            gameController.PlayerBustEvent += new EventHandler(PlayerBustEvent);
            gameController.PlayerBlackJackEvent += new EventHandler(BlackJackEvent);
            gameController.CardsLowEvent += new GameController.CardsLowEventHandler(CardsLowEvent);
            gameController.OutOfCardsEvent += new EventHandler(OutOfCardsEvent);
            gameController.ResultEvent += new GameController.ResultEventHandler(ResultEvent);
        }

        /// <summary>
        /// Detects when end of round occurs and post updates.
        /// </summary>
        /// <param name="results">Array with end of round result strings.</param>
        private void ResultEvent(string[] results)
        {
            string resultStr = "";
            foreach (string s in results)
            {
                Updates.Insert(0, s);
                resultStr += s + "\n";
            }
            MessageBox.Show(resultStr, results[0], MessageBoxButton.OK, MessageBoxImage.Information);
        }

        /// <summary>
        /// Detect when deck is running low on cards and lets user choose "Yes of No" to refill deck.
        /// </summary>
        /// <returns>True if yes, false if no.</returns>
        private bool CardsLowEvent()
        {
            MessageBoxResult result = MessageBox.Show("Cards are running low, shuffle deck?", "Shuffle", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                Updates.Insert(0, "Deck will be shuffled before next round!");
                return true;
            }
            else if (result == MessageBoxResult.No)
            {
                Updates.Insert(0, "Deck will automatically shuffle after last card!");
                return false;
            }
            else
                return false;
        }

        /// <summary>
        /// Detects when deck runs out of cards and posts update.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">EventArgs.</param>
        private void OutOfCardsEvent(object sender, EventArgs e)
        {
            Updates.Insert(0, "Out of cards, deck was shuffled automatically!");
        }

        /// <summary>
        /// Detects when player (or dealer) is dealt a card and posts update.
        /// </summary>
        /// <param name="playerID">Player ID.</param>
        /// <param name="playerName">Player name.</param>
        /// <param name="card">Card.</param>
        private void HitPlayerEvent(int playerID, string playerName, Card card)
        {
            Updates.Insert(0, playerName + " drew " + card.Rank + " of " + card.Suit + ".");
            AutoResizeColumns();
        }

        /// <summary>
        /// Detects when player or dealer is busted and posts update.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">EventArgs.</param>
        private void PlayerBustEvent(object sender, EventArgs e)
        {
            Updates.Insert(0, ((Player)sender).Name + " busted with a hand of " + ((Player)sender).HandValue + ".");
        }

        /// <summary>
        /// Detects when player or dealer has Black Jack and posts update.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">EventArgs.</param>
        private void BlackJackEvent(object sender, EventArgs e)
        {
            Updates.Insert(0, ((Player)sender).Name + " has Black Jack!");
        }

        /// <summary>
        /// Detects when new round starts and post an update.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">EventArgs.</param>
        private void ResetPlayersEvent(object sender, EventArgs e)
        {
            Updates.Insert(0, "Start of round " + gameController.currentRound + ".");
            AutoResizeColumns();
        }
    }
}
