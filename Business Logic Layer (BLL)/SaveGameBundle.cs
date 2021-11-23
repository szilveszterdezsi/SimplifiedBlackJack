/// ---------------------------
/// Author: Szilveszter Dezsi
/// Created: 2018-10-31
/// Modified: n/a
/// ---------------------------

using System;
using System.ComponentModel;

namespace BLL
{
    /// <summary>
    /// Class for bundling game objects to be saved/loaded to/from file (serialization).
    /// </summary>
    [Serializable]
    public class SaveGameBundle
    {
        private int currentRound;
        private int currentPlayer;
        private Player dealer;
        private Deck deck;
        private BindingList<Player> players;
        private BindingList<string> updates;

        /// <summary>
        /// Empty constrctor.
        /// </summary>
        public SaveGameBundle()
        {

        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="currentRound">Current round to be saved.</param>
        /// <param name="currentPlayer">Current player ID to be saved.</param>
        /// <param name="deck">Deck to be saved.</param>
        /// <param name="dealer">Dealer to be saved.</param>
        /// <param name="players">List of players to be saved.</param>
        /// <param name="updates">List if updates to be saved.</param>
        public SaveGameBundle(int currentRound, int currentPlayer, Deck deck, Player dealer, BindingList<Player> players, BindingList<string> updates)
        {
            this.currentRound = currentRound;
            this.currentPlayer = currentPlayer;
            this.deck = deck;
            this.dealer = dealer;
            this.players = players;
            this.updates = updates;
        }

        /// <summary>
        /// Gets and sets the current round to be saved.
        /// </summary>
        public int CurrentRound { get => currentRound; set => currentRound = value; }

        /// <summary>
        /// Gets and sets the current player ID to be saved.
        /// </summary>
        public int CurrentPlayer { get => currentPlayer; set => currentPlayer = value; }

        /// <summary>
        /// Gets and sets the dealer to be saved.
        /// </summary>
        public Player Dealer { get => dealer; set => dealer = value; }

        /// <summary>
        /// Gets and sets the deck to be saved.
        /// </summary>
        public Deck Deck { get => deck; set => deck = value; }

        /// <summary>
        /// Gets and sets the list of players to be saved.
        /// </summary>
        public BindingList<Player> Players { get => players; set => players = value; }

        /// <summary>
        /// Gets and sets the list of updates to be saved.
        /// </summary>
        public BindingList<string> Updates { get => updates; set => updates = value; }
    }
}
