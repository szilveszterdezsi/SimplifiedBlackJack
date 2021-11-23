/// ---------------------------
/// Author: Szilveszter Dezsi
/// Created: 2018-10-31
/// Modified: n/a
/// ---------------------------

using System;

namespace BLL
{
    /// <summary>
    /// Class for handling a playing card.
    /// </summary>
    [Serializable]
    public class Card
    {
        private Suit suit;
        private Rank rank;
        private string image;

        /// <summary>
        /// Empty constrctor.
        /// </summary>
        public Card()
        {

        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="suit">Suit type of the card.</param>
        /// <param name="rank">Rank type of the card.</param>
        public Card (Suit suit, Rank rank)
        {
            this.suit = suit;
            this.rank = rank;
            this.image = "pack://application:,,,/Resources/PlayingCards/" + rank.ToString() + suit.ToString() + ".png";
        }

        /// <summary>
        /// Gets and sets the suit type of the card.
        /// </summary>
        public Suit Suit
        {
            get { return suit; }
            set { suit = value; }
        }

        /// <summary>
        /// Gets and sets the rank type of the card.
        /// </summary>
        public Rank Rank
        {
            get { return rank; }
            set { rank = value; }
        }

        /// <summary>
        /// Gets and sets the card face image source.
        /// </summary>
        public string Image
        {
            get { return image; }
            set { image = value; }
        }

        /// <summary>
        /// Presentation
        /// </summary>
        /// <returns>The playing card formated for presentation.</returns>
        public override string ToString()
        {
            return rank.ToString() + " of " + suit.ToString();
        }
    }

    /// <summary>
    /// Suit types.
    /// </summary>
    public enum Suit
    {
        Hearts,
        Diamonds,
        Spades,
        Clubs
    }

    /// <summary>
    /// Rank types.
    /// </summary>
    public enum Rank
    {
        Ace = 1,
        Deuce,
        Three,
        Four,
        Five,
        Six,
        Seven,
        Eight,
        Nine,
        Ten,
        Jack,
        Queen,
        King
    }
}
