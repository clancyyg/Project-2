using System;
using System.Collections.Generic;
using System.Linq; // currently only needed if we use alternate shuffle method

namespace RaceTo21
{
    /// <summary>
    /// Manages a standard deck of 52 cards, represented as a list of Card instances.
    /// </summary>
    public class Deck
    {
        List<Card> cards = new List<Card>();

        /// <summary>
        /// Constructor for an ordered deck of cards.
        /// </summary>
        public Deck()
        {
            string[] suits = { "S", "H", "C", "D" };

            for (int cardVal = 1; cardVal <= 13; cardVal++)
            {
                foreach (string cardSuit in suits)
                {
                    string cardName;
                    string cardLongName;

                    switch (cardVal)
                    {
                        case 1:
                            cardName = "A";
                            cardLongName = "Ace of ";
                            break;
                        case 11:
                            cardName = "J";
                            cardLongName = "Jack of ";
                            break;
                        case 12:
                            cardName = "Q";
                            cardLongName = "Queen of ";
                            break;
                        case 13:
                            cardName = "K";
                            cardLongName = "King of ";
                            break;
                        default:
                            cardName = cardVal.ToString();
                            cardLongName = cardVal.ToString() + " of ";
                            break;
                    }

                    switch (cardSuit)
                    {
                        case "S":
                            cardLongName += "Spades";
                            break;
                        case "H":
                            cardLongName += "Hearts";
                            break;
                        case "C":
                            cardLongName += "Clubs";
                            break;
                        case "D":
                            cardLongName += "Diamonds";
                            break;
                    }
                    //card a card set its ID and then put it into the cards array
                    Card card = new Card();
                    card.SetID(cardName, cardSuit);
                    cards.Add(card);
                }
            }
        }

        // Randomly swap cards to shuffle the deck.
        public void Shuffle()
        {
            Console.WriteLine("Shuffling Cards...");

            Random rng = new Random(); // rng is short for "Random Number Generator"

            // multi-line approach that uses Array notation on a list!
            // (this should be easier to understand):
            for (int i=0; i<cards.Count; i++)
            {
                Card tmp = cards[i];
                int swapindex = rng.Next(cards.Count);
                cards[i] = cards[swapindex];
                cards[swapindex] = tmp;
            }
        }

        // Remove top card (defined here as last card in the list), an instance of Card
        //returnsthe removed instance of Card, representing one of the 52 cards in the deck
        public Card DealTopCard()
        {
            Card card = cards[cards.Count - 1];
            cards.RemoveAt(cards.Count - 1);
            return card;
        }
    }
}

