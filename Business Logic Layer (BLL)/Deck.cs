/// ---------------------------
/// Author: Szilveszter Dezsi
/// Created: 2018-10-31
/// Modified: n/a
/// ---------------------------

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace BLL
{
    /// <summary>
    /// Class for handling multiple decks of cards.
    /// Implements the 'INotifyPropertyChanged' interface to notify GUI components to update.
    /// </summary>
    [Serializable]
    public class Deck : INotifyPropertyChanged
    {
        [field: NonSerialized]
        public event PropertyChangedEventHandler PropertyChanged;
        [field: NonSerialized]
        public event GameController.CardsLowEventHandler CardsRunningLowEvent;
        [field: NonSerialized]
        public event EventHandler OutOfCardsEvent;
        private int count;
        private int max;
        private int multiplier;
        private bool shuffleRequest;
        private bool cardsRunningLow;
        private BindingList<Card> cards = new BindingList<Card>();

        /// <summary>
        /// Empty constrctor.
        /// </summary>
        public Deck()
        {
            
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="multiplier">Deck multiplier.</param>
        public Deck(int multiplier)
        {
            FillDeck(multiplier);
        }

        /// <summary>
        /// Fills the deck with multiple decks of 52 cards (seperately shuffled).
        /// </summary>
        /// <param name="multiplier">Deck multiplier.</param>
        public void FillDeck(int multiplier)
        {
            Multiplier = multiplier;
            Cards.Clear();
            List<Card> freshDeck = new List<Card>();
            foreach (Suit suit in Enum.GetValues(typeof(Suit)))
            {
                foreach (Rank rank in Enum.GetValues(typeof(Rank)))
                {
                    freshDeck.Add(new Card(suit, rank));
                }
            }
            Random rnd = new Random();
            for (int i = 0; i < multiplier; i++)
            {
                freshDeck = new List<Card>(freshDeck.OrderBy(x => rnd.Next()));
                freshDeck.ForEach(x => Cards.Add(x));
            }
            Max = multiplier * 52;
            Count = Cards.Count();
            ShuffleRequest = false;
            CardsRunningLow = false;
        }

        /// <summary>
        /// Fills the deck with current deck multiplier (seperately shuffled).
        /// </summary>
        public void Shuffle() {
            FillDeck(multiplier);
        }

        /// <summary>
        /// "Draws a card" from the deck by removing the first card and returning it.
        /// If less than 25% of deck maximum is reached 'CardsRunningLowEvent' will fire, event returns boolean 
        /// 'ShuffleRequest' that indicates if refill will occur after current round is over.
        /// After last card is removed 'OutOfCardsEvent' will fire and deck will automatically refill.
        /// </summary>
        /// <returns>First playing card in the deck.</returns>
        public Card DrawCard() {
            if (!ShuffleRequest && Count < (Max / 4))
            {
                CardsRunningLow = true;
                ShuffleRequest = CardsRunningLowEvent();
            }
            Count--;
            Card card = Cards.First();
            Cards.RemoveAt(0);
            if (Count == 0)
            {
                OutOfCardsEvent(this, EventArgs.Empty);
                Shuffle();
            }  
            return card;
        }

        /// <summary>
        /// Gets and sets the deck playing card count.
        /// Used to visualize deck status in GUI.
        /// 'NotifyPropertyChanged' event fires when value is set to update GUI components.
        /// </summary>
        public int Count
        {
            get { return Cards.Count(); }
            set
            {
                if (count != value)
                {
                    count = value;
                    NotifyPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets and sets the deck multiplier.
        /// Used to visualize deck status in GUI.
        /// 'NotifyPropertyChanged' event fires when value is set to update GUI components.
        /// </summary>
        public int Multiplier
        {
            get { return multiplier; }
            set
            {
                if (multiplier != value)
                {
                    multiplier = value;
                    NotifyPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets and sets the deck max playing card count.
        /// Used to visualize deck status in GUI.
        /// 'NotifyPropertyChanged' event fires when value is set to notify GUI components to update.
        /// </summary>
        public int Max
        {
            get { return max; }
            set
            {
                if (max != value)
                {
                    max = value;
                    NotifyPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets and sets boolean that indicates that card count is low.
        /// Used to visualize deck status in GUI.
        /// 'NotifyPropertyChanged' event fires when value is set to notify GUI components to update.
        /// </summary>
        public bool CardsRunningLow
        {
            get { return cardsRunningLow; }
            set
            {
                if (cardsRunningLow != value)
                {
                    cardsRunningLow = value;
                    NotifyPropertyChanged();
                }
            }
        }
        /// <summary>
        /// Gets and sets boolean that indicates that shuffle (refill) has been requested.
        /// </summary>
        public bool ShuffleRequest
        {
            get { return shuffleRequest; }
            set { shuffleRequest = value; }
        }

        /// <summary>
        /// Gets and sets list of all cards in the deck.
        /// Used to load saved games and visualize deck status in GUI.
        /// 'NotifyPropertyChanged' event fires when value is set to notify GUI components to update.
        /// </summary>
        public BindingList<Card> Cards
        {
            get { return cards; }
            set
            {
                if (cards != value)
                {
                    cards = value;
                    NotifyPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Unsubscribes all delegates from 'CardsRunningLowEvent' and 'OutOfCardsEvent'.
        /// </summary>
        public void UnsubscribeDelegates()
        {
            if (CardsRunningLowEvent != null)
            {
                foreach (Delegate d in CardsRunningLowEvent.GetInvocationList())
                {
                    CardsRunningLowEvent -= (GameController.CardsLowEventHandler)d;
                }
            }
            if (OutOfCardsEvent != null)
            {
                foreach (Delegate d in OutOfCardsEvent.GetInvocationList())
                {
                    OutOfCardsEvent -= (EventHandler)d;
                }
            }
        }

        /// <summary>
        /// Method that fires event to notify GUI components to update.
        /// </summary>
        /// <param name="propertyName">Empty string.</param>
        public void NotifyPropertyChanged(string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
