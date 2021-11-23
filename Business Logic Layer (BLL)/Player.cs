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
    /// Class for handling Black Jack players.
    /// Implements the 'INotifyPropertyChanged' interface to notify GUI components to update.
    /// </summary>
    [Serializable]
    public class Player : INotifyPropertyChanged
    {
        [field: NonSerialized]
        public event PropertyChangedEventHandler PropertyChanged;
        [field: NonSerialized]
        public event EventHandler PlayerBustEvent;
        [field: NonSerialized]
        public event EventHandler PlayerBlackJackEvent;
        private int id;
        private string name;
        private BindingList<Card> hand = new BindingList<Card>();
        private int handValue;
        private int wins = 0;
        private int losses = 0;
        private bool isEnabled = false;
        private bool isBust = false;
        private bool blackJack = false;
        private string saveName = "Collapsed";
        private string editName = "Visible";

        /// <summary>
        /// Empty constrctor.
        /// </summary>
        public Player()
        {

        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="id">Player ID.</param>
        /// <param name="name">Player name.</param>
        public Player(int id, string name)
        {
            this.id = id;
            this.name = name;
        }

        /// <summary>
        /// Gets and sets the player ID.
        /// </summary>
        public int ID
        {
            get { return id; }
            set { id = value; }
        }

        /// <summary>
        /// Gets and sets the player name.
        /// Used to visualize player name in GUI.
        /// 'NotifyPropertyChanged' event fires when value is set to update GUI components.
        /// </summary>
        public string Name
        {
            get { return name; }
            set
            {
                if (name != value)
                {
                    name = value;
                    NotifyPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets and sets the player hand of cards.
        /// Used to visualize player hand of cards in GUI.
        /// 'NotifyPropertyChanged' event fires when value is set to update GUI components.
        /// </summary>
        public BindingList<Card> Hand
        {
            get { return hand; }
            set
            {
                if (hand != value)
                {
                    hand = value;
                    NotifyPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets and sets the player hand of cards value.
        /// Used to visualize player hand of cards value in GUI.
        /// 'NotifyPropertyChanged' event fires when value is set to update GUI components.
        /// </summary>
        public int HandValue
        {
            get { return handValue; }
            set
            {
                if (handValue != value)
                {
                    handValue = value;
                    NotifyPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets and sets the player wins.
        /// Used to visualize player wins in GUI.
        /// 'NotifyPropertyChanged' event fires when value is set to update GUI components.
        /// </summary>
        public int Wins
        {
            get { return wins; }
            set
            {
                if (wins != value)
                {
                    wins = value;
                    NotifyPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets and sets the player losses.
        /// Used to visualize player losses in GUI.
        /// 'NotifyPropertyChanged' event fires when value is set to update GUI components.
        /// </summary>
        public int Losses
        {
            get { return losses; }
            set
            {
                if (losses != value)
                {
                    losses = value;
                    NotifyPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets and sets the player enabled boolean.
        /// Used to visualize player enabled in GUI.
        /// 'NotifyPropertyChanged' event fires when value is set to update GUI components.
        /// </summary>
        public bool IsEnabled
        {
            get { return isEnabled; }
            set
            {
                if (isEnabled != value)
                {
                    isEnabled = value;
                    NotifyPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets and sets the player busted boolean.
        /// Used to visualize player busted status in GUI.
        /// 'NotifyPropertyChanged' event fires when value is set to update GUI components.
        /// </summary>
        public bool IsBust
        {
            get { return isBust; }
            set
            {
                if (isBust != value)
                {
                    isBust = value;
                    NotifyPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets and sets the player black jack boolean.
        /// Used to visualize player black jack status in GUI.
        /// 'NotifyPropertyChanged' event fires when value is set to update GUI components.
        /// </summary>
        public bool BlackJack
        {
            get { return blackJack; }
            set
            {
                if (blackJack != value)
                {
                    blackJack = value;
                    NotifyPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets and sets the player save name boolean.
        /// Used to enable/diable player save name mode in GUI.
        /// 'NotifyPropertyChanged' event fires when value is set to update GUI components.
        /// </summary>
        public string SaveName
        {
            get { return saveName; }
            set
            {
                if (saveName != value)
                {
                    EditName = "Collapsed";
                    saveName = value;
                    NotifyPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets and sets the player edit name boolean.
        /// Used to enable/diable player edit name mode in GUI.
        /// 'NotifyPropertyChanged' event fires when value is set to update GUI components.
        /// </summary>
        public string EditName
        {
            get { return editName; }
            set
            {
                if (editName != value)
                {
                    SaveName = "Collapsed";
                    editName = value;
                    NotifyPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Adds a card to player hand of cards.
        /// Subcribed to event that fires to all players (incl. dealer) and card is only added if ID's match.
        /// </summary>
        /// <param name="playerID">Played ID.</param>
        /// <param name="playerName">Player name.</param>
        /// <param name="card">Playing card.</param>
        public void ReceiveCard(int playerID, string playerName, Card card)
        {
            if (playerID == ID)
            {
                Hand.Add(card);
            }
        }

        /// <summary>
        /// Calculates hand value based on current cards in Hand-list.
        /// Cards 2-10 are valued at face value. Jack, Queen and King are valued at 10.
        /// Aces are valued at 11 unless hand total exceeds 21, then revalued at 1.
        /// If hand total is 21 (Black Jack) 'PlayerBlackJackEvent' is fired, boolean 'BlackJack'
        /// is set to true and 'IsEnabled' is to to false to enable automatic switch to next player;
        /// If hand total exceeds 21 (Bust) 'PlayerBustEvent' is fired, boolean 'IsBust'
        /// is set to true, 'Losses' is incremented and 'IsEnabled' is to to false to enable
        /// automatic switch to next player;
        /// </summary>
        public void EvaluateHand()
        {
            int score = 0;
            foreach (Card c in hand) {
                if ((int)c.Rank == 1)
                    score += 11;
                else if ((int)c.Rank > 10)
                    score += 10;
                else
                    score += (int)c.Rank;
            }
            if (score > 21)
                foreach (Card c in hand)
                    if ((int)c.Rank == 1)
                        score -= 10;
            HandValue = score;
            if (score > 21)
            {
                IsBust = true;
                PlayerBustEvent(this, EventArgs.Empty);
                Losses++;
                IsEnabled = false;
            }
            else if (score == 21)
            {
                BlackJack = true;
                PlayerBlackJackEvent(this, EventArgs.Empty);
                IsEnabled = false;
            }
            else { }
        }

        /// <summary>
        /// Resets player hand card(s), value and status
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Event arguments.</param>
        public void Reset(object sender, EventArgs e)
        {
            Hand.Clear();
            HandValue = 0;
            IsEnabled = false;
            BlackJack = false;
            IsBust = false;
        }

        /// <summary>
        /// Unsubscribes all delegates from 'PlayerBustEvent' and 'PlayerBlackJackEvent'.
        /// </summary>
        public void UnsubscribeDelegates()
        {
            if (PlayerBustEvent != null)
            {
                foreach (Delegate d in PlayerBustEvent.GetInvocationList())
                {
                    PlayerBustEvent -= (EventHandler)d;
                }
            }
            if (PlayerBlackJackEvent != null)
            {
                foreach (Delegate d in PlayerBlackJackEvent.GetInvocationList())
                {
                    PlayerBlackJackEvent -= (EventHandler)d;
                }
            }
        }

        /// <summary>
        /// Method that fires event to notify GUI components to update.
        /// </summary>
        /// <param name="propertyName">Empty string.</param>
        public void NotifyPropertyChanged(String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
