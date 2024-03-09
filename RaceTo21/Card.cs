﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RaceTo21
{
 
    //Represents an individual card in deck two ways: 
    //ID string is two-character short name and name is full card name written out

    public class Card
    {
        public string ID { get; set; }// The ID must be public for the converter to find it and match it with images.
        private string name; // Option 1: just store the name for each card alongside the ID

        //Setter method for ID
        public void SetID(string cardName , string cardSuit)
        {
            ID = cardName + cardSuit;
        }

        //Getter method for ID
        public string GetID()
        {
            return ID.ToString();
        }


        // <returns>String containing full name of card, calculated from the ID</returns>
        public string GetName()
        {
            string cardLongName;
            string cardVal = ID.Remove(ID.Length - 1);
            switch (cardVal)
                    {
                        case "A":
                            cardLongName = "Ace of ";
                            break;
                        case "J":
                            cardLongName = "Jack of ";
                            break;
                        case "Q":
                            cardLongName = "Queen of ";
                            break;
                        case "K":
                            cardLongName = "King of ";
                            break;
                        default:
                            cardLongName = cardVal.ToString() + " of ";
                            break;
                    }

            string cardSuit = ID.Remove(0,1);
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

            return cardLongName;
        }
    }
}