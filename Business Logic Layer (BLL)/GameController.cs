/// ---------------------------
/// Author: Szilveszter Dezsi
/// Created: 2018-09-31
/// Modified: n/a
/// ---------------------------

using System;
using System.Linq;
using System.ComponentModel;
using DAL;

namespace BLL
{
    /// <summary>
    /// Controller class of the Black Jack game that serves the presentation layer.
    /// 
    /// 
    /// </summary>
    public class GameController
    {
        public delegate void HitPlayerEventHandler(int playerID, string playerName, Card card);
        public delegate void ResultEventHandler(string[] results);
        public delegate bool CardsLowEventHandler();
        public event HitPlayerEventHandler HitPlayerEvent;
        public event CardsLowEventHandler CardsLowEvent;
        public event EventHandler PlayerBlackJackEvent;
        public event EventHandler ResetPlayersEvent;
        public event EventHandler PlayerBustEvent;
        public event EventHandler OutOfCardsEvent;
        public event ResultEventHandler ResultEvent;
        public BindingList<Player> playerList = new BindingList<Player>();
        public Player gameDealer = new Player(-1, "Dealer");
        public Deck gameDeck = new Deck(1);
        public int currentRound;
        public int currentPlayer;
        public bool sessionSaved = true;
        public bool neverSaved = true;
        string defaultFilePath = "BlackJack.sav";

        /// <summary>
        /// Empty constructor.
        /// </summary>
        public GameController()
        {

        }

        /// <summary>
        /// Enables edit of player name.
        /// </summary>
        /// <param name="playerID"></param>
        public void EditPlayerName(int playerID)
        {
            playerList[playerID].SaveName = "Visible";
        }

        /// <summary>
        /// Saves edit of player name.
        /// </summary>
        /// <param name="playerID"></param>
        public void SavePlayerName(int playerID)
        {
            playerList[playerID].EditName = "Visible";
        }

        /// <summary>
        /// Initiates game with number of players and decks.
        /// </summary>
        /// <param name="players">Number of players.</param>
        /// <param name="decks">Number of decks.</param>
        public void InitiateGame(int players, int decks)
        {
            gameDealer.Reset(this, EventArgs.Empty);
            UnsubcribeAllDelegates();
            InitiateGameDeck(decks);
            InitiateGameDealer();
            InitiatePlayers(players);
            currentRound = 1;
            neverSaved = true;
            sessionSaved = false;
        }

        /// <summary>
        /// Starts game by dealing initial cards.
        /// </summary>
        public void StartGame()
        {
            DealInitialCards();
        }

        /// <summary>
        /// Unsubscribes delegates from deck, dealer and controller events.
        /// </summary>
        private void UnsubcribeAllDelegates()
        {
            gameDeck.UnsubscribeDelegates();
            gameDealer.UnsubscribeDelegates();
            HitPlayerEvent = null;
            CardsLowEvent = null;
            OutOfCardsEvent = null;
            PlayerBustEvent = null;
            PlayerBlackJackEvent = null;
            ResetPlayersEvent = null;
            ResultEvent = null;
        }

        /// <summary>
        /// Fires 'ResetPlayersEvent' to reset all player hand card(s), values and status.
        /// </summary>
        private void ResetAllPlayers()
        {
            ResetPlayersEvent(this, EventArgs.Empty);
        }

        /// <summary>
        /// Initiates deck with multiplier and subscribes to related events.
        /// </summary>
        /// <param name="multiplier">Deck multiplier.</param>
        private void InitiateGameDeck(int multiplier)
        {
            gameDeck.FillDeck(multiplier);
            gameDeck.CardsRunningLowEvent += () => { return CardsLowEvent(); };
            gameDeck.OutOfCardsEvent += (sender, args) => { OutOfCardsEvent(sender, args); };
        }

        /// <summary>
        /// Initiates dealer and scubscribes to related events.
        /// </summary>
        private void InitiateGameDealer()
        {
            HitPlayerEvent += new HitPlayerEventHandler(gameDealer.ReceiveCard);
            gameDealer.PlayerBustEvent += (sender, args) => { PlayerBustEvent(sender, args); };
            gameDealer.PlayerBlackJackEvent += (sender, args) => { PlayerBlackJackEvent(sender, args); };
            ResetPlayersEvent += new EventHandler(gameDealer.Reset);
        }

        /// <summary>
        /// Initiates player list and scubscribes to related events.
        /// </summary>
        /// <param name="players">Number of players.</param>
        private void InitiatePlayers(int players)
        {
            playerList.Clear();
            for (int i = 0; i < players; i++)
            {
                Player player = new Player(i, "Player " + (i+1));
                HitPlayerEvent += new HitPlayerEventHandler(player.ReceiveCard);
                player.PlayerBustEvent += (sender, args) => { PlayerBustEvent(sender, args); };
                player.PlayerBlackJackEvent += (sender, args) => { PlayerBlackJackEvent(sender, args); };
                ResetPlayersEvent += new EventHandler(player.Reset);
                playerList.Add(player);
            }
        }

        /// <summary>
        /// Deals all players and dealer two (2) cards each and evaluates thier hands.
        /// Current player is set to first player ID (0) and current player is enabled.
        /// </summary>
        private void DealInitialCards()
        {
            foreach (Player player in playerList)
            {
                for (int i = 0; i < 2; i++)
                {
                    HitPlayerEvent(player.ID, player.Name, gameDeck.DrawCard());
                    playerList[player.ID].EvaluateHand();
                }

            }
            for (int i = 0; i < 2; i++)
            {
                HitPlayerEvent(gameDealer.ID, gameDealer.Name, gameDeck.DrawCard());
                gameDealer.EvaluateHand();
            }
            currentPlayer = 0;
            CurrentPlayer();
        }

        /// <summary>
        /// Enables current player, if player has Black Jack or is busted turn moves to next player.
        /// </summary>
        private void CurrentPlayer()
        {
            if (playerList[currentPlayer].IsBust || playerList[currentPlayer].BlackJack)
            {
                NextPlayer(currentPlayer);
            }
            else
            {
                playerList[currentPlayer].IsEnabled = true;
            }
        }

        /// <summary>
        /// "Hits" or deals the target player with a card drawn from the deck by firing 'HitPlayerEvent' 
        /// and the evaluates thier hand. If player has Black Jack or is busted turn moves to next player.
        /// </summary>
        /// <param name="playerID">Player ID.</param>
        /// <param name="playerName">Player name.</param>
        public void HitPlayer(int playerID, string playerName)
        {
            sessionSaved = false;
            HitPlayerEvent(playerID, playerName, gameDeck.DrawCard());
            playerList[playerID].EvaluateHand();
            if (playerList[playerID].IsBust || playerList[playerID].BlackJack)
            {
                NextPlayer(playerID);
            }
        }

        /// <summary>
        /// Enables next player after 'playerID' that doesn't already have Black Jack or is busted.
        /// After last player turn moves to dealer.
        /// </summary>
        /// <param name="playerID"></param>
        public void NextPlayer(int playerID)
        {
            sessionSaved = false;
            currentPlayer = playerID;
            playerList[currentPlayer].IsEnabled = false;
            currentPlayer++;
            if (currentPlayer <= playerList.Count()-1)
            {
                bool allBust = false;
                for (int i = currentPlayer; i < playerList.Count(); i++)
                {
                    if (playerList[i].IsBust || playerList[i].BlackJack)
                    {
                        allBust = true;
                    }
                    else
                    {
                        allBust = false;
                        playerList[i].IsEnabled = true;
                        break;
                    }
                }
                if (allBust)
                {
                    DealersTurn();
                }
            }
            else
            {
                DealersTurn();
            }
        }

        /// <summary>
        /// Dealer turn logic.
        /// If dealer has Black Jack or all players are bust it will do nothing.
        /// While dealer's hand value is 17 or less it continously draw cards.
        /// If dealer's hand value is between 17 and 20 there's a 20% chance it will play out it's hand.
        /// After that round is over.
        /// </summary>
        private void DealersTurn()
        {
            if (gameDealer.BlackJack || playerList.Where(x => !x.IsBust).Count() == 0)
            {
                // Do nothing!
            }
            else
            {
                gameDealer.IsEnabled = true;
                while (gameDealer.HandValue < 17)
                {
                    HitPlayerEvent(gameDealer.ID, gameDealer.Name, gameDeck.DrawCard());
                    gameDealer.EvaluateHand();
                }
                Random r = new Random();
                if (r.Next(0, 5) == 0 && gameDealer.HandValue > 17 && gameDealer.HandValue < 21)
                {
                    while (playerList.Where(x => !x.IsBust && x.HandValue > gameDealer.HandValue).Count() != 0)
                    {
                        HitPlayerEvent(gameDealer.ID, gameDealer.Name, gameDeck.DrawCard());
                        gameDealer.EvaluateHand();
                    }
                }
            }
            RoundResults();
        }

        /// <summary>
        /// End of round result evaluation for remaining unbusted players.
        /// Builds s string array and increments wins/losses according the the following rules:
        /// 1. If dealer is busted, all remaining players get a win increment.
        /// 2. If player and dealer both have Black Jack, it's a draw (push).
        /// 3. If dealer has Black Jack and player has 20 or less, player get a loss increment.
        /// 4. If player has Black Jack and dealer has 20 or less, player get a win increment.
        /// 5. If player and dealer both have the same hand value, it's a draw (push).
        /// 6. If player has a greater hand value than dealer, player gets a win increment.
        /// 7. If player has a lower hand value than dealer, player gets a loss increment.
        /// 8. Unforseen result exception, never experienced but it's there just in case. :)
        /// After that 'ResultEvent' is fired and results are sent to be presented in GUI.
        /// Lastly next round is initiated.
        /// </summary>
        private void RoundResults()
        {
            string[] results = new string[playerList.Where(x => !x.IsBust).Count() + 1];
            results[0] = "End of round " + currentRound + ".";
            int index = 1;
            foreach (Player player in playerList.Where(x => !x.IsBust))
            {
                if (gameDealer.IsBust)
                {
                    results[index] = player.Name + " wins with " + player.HandValue + " as dealer busted with " + gameDealer.HandValue + ".";
                    player.Wins++;
                }
                else if (gameDealer.BlackJack && player.BlackJack)
                {
                    results[index] = player.Name + " and dealer both have Black Jack, push.";
                }
                else if (gameDealer.BlackJack && !player.BlackJack)
                {
                    results[index] = player.Name + " loses with " + player.HandValue + " agains dealer's Black Jack.";
                    player.Losses++;
                }
                else if (!gameDealer.BlackJack && player.BlackJack)
                {
                    results[index] = player.Name + " wins with Black Jack agains dealer's " + gameDealer.HandValue + ".";
                    player.Wins++;
                }
                else if (gameDealer.HandValue == player.HandValue)
                {
                    results[index] = player.Name + " and dealer both have " + gameDealer.HandValue + ", push.";
                }
                else if (gameDealer.HandValue < player.HandValue)
                {
                    results[index] = player.Name + " wins with " + player.HandValue + " against dealer's " + gameDealer.HandValue + ".";
                    player.Wins++;
                }
                else if (gameDealer.HandValue > player.HandValue)
                {
                    results[index] = player.Name + " loses with " + player.HandValue + " against dealer's " + gameDealer.HandValue + ".";
                    player.Losses++;
                }
                else
                {
                    results[index] = "Unforseen result exception for " + player.Name + ".";
                }
                index++;
            }
            ResultEvent(results);
            NextRound();
        }

        /// <summary>
        /// Initites next round.
        /// Deck will be refilled if there is a 'ShuffleRequest' pending. 
        /// </summary>
        private void NextRound()
        {
            if (gameDeck.ShuffleRequest)
                gameDeck.Shuffle();
            currentRound++;
            ResetAllPlayers();
            DealInitialCards();
            CurrentPlayer();
        }

        /// <summary>
        /// Loads a save game file and deserializes a 'SaveGameBundle' which is "unpacked" and current game session
        /// objects are reinitialized with unpacked objects.
        /// </summary>
        /// <param name="filePath">Path to the save game file.</param>
        /// <returns>Updates list.</returns>
        public BindingList<string> LoadGame(string filePath)
        {
            neverSaved = false;
            sessionSaved = true;
            defaultFilePath = filePath;
            SaveGameBundle saveGame = Serialization.BinaryDeserializeFromFile<SaveGameBundle>(filePath);
            UnsubcribeAllDelegates();
            currentRound = saveGame.CurrentRound;
            currentPlayer = saveGame.CurrentPlayer;
            gameDeck.Cards = saveGame.Deck.Cards;
            gameDeck.Multiplier = saveGame.Deck.Multiplier;
            gameDeck.Max = saveGame.Deck.Max;
            gameDeck.CardsRunningLowEvent += () => { return CardsLowEvent(); };
            gameDeck.OutOfCardsEvent += (sender, args) => { OutOfCardsEvent(sender, args); };
            gameDealer.Hand = saveGame.Dealer.Hand;
            gameDealer.EvaluateHand();
            InitiateGameDealer();
            playerList.Clear();
            foreach (Player player in saveGame.Players)
            {
                HitPlayerEvent += new HitPlayerEventHandler(player.ReceiveCard);
                player.PlayerBustEvent += (sender, args) => { PlayerBustEvent(sender, args); };
                player.PlayerBlackJackEvent += (sender, args) => { PlayerBlackJackEvent(sender, args); };
                ResetPlayersEvent += new EventHandler(player.Reset);
                playerList.Add(player);
            }
            BindingList<string> updates = new BindingList<string>(saveGame.Updates);
            CurrentPlayer();
            return updates;
        }

        /// <summary>
        /// Saves current game to current default save path by forward to 'SaveGameAs' method.
        /// </summary>
        /// <param name="gameUpdates">Updates list.</param>
        public void SaveGame(BindingList<string> gameUpdates)
        {
            SaveGameAs(defaultFilePath, gameUpdates);
        }

        /// <summary>
        /// Saves current game by "packing" a 'SaveGameBundle' with current game session objects and serializing to file.
        /// </summary>
        /// <param name="filePath">Path to the save game file.</param>
        /// <param name="gameUpdates">Updates list.</param>
        public void SaveGameAs(string filePath, BindingList<string> gameUpdates)
        {
            neverSaved = false;
            sessionSaved = true;
            defaultFilePath = filePath;
            SaveGameBundle saveGame = new SaveGameBundle(currentRound, currentPlayer, gameDeck, gameDealer, playerList, gameUpdates);
            Serialization.BinarySerializeToFile(saveGame, filePath);
        }
    }
}
