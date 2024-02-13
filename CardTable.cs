using System;
using System.Collections.Generic;

namespace RaceTo21
{
    public class CardTable
    {
        public CardTable()
        {
            Console.WriteLine("Setting Up Table...");
        }

        /* Shows the name of each player and introduces them by table position.
         * Is called by Game object.
         * Game object provides list of players.
         * Calls Introduce method on each player object.
         */
        public void ShowPlayers(List<Player> players)
        {
            for (int i = 0; i < players.Count; i++)
            {
                players[i].Introduce(i+1); // List is 0-indexed but user-friendly player positions would start with 1...
            }
        }

        /* Gets the user input for number of players.
         * Is called by Game object.
         * Returns number of players to Game object.
         */
        public int GetNumberOfPlayers()
        {
            Console.Write("How many players? ");
            string response = Console.ReadLine();
            int numberOfPlayers;
            while (int.TryParse(response, out numberOfPlayers) == false
                || numberOfPlayers < 2 || numberOfPlayers > 8)
            {
                Console.WriteLine("Invalid number of players.");
                Console.Write("How many players?");
                response = Console.ReadLine();
            }
            return numberOfPlayers;
        }

        /* Gets the name of a player
         * Is called by Game object
         * Game object provides player number
         * Returns name of a player to Game object
         */
        public string GetPlayerName(int playerNum)
        {
            Console.Write("What is the name of player# " + playerNum + "? ");
            string response = Console.ReadLine();
            while (response.Length < 1)
            {
                Console.WriteLine("Invalid name.");
                Console.Write("What is the name of player# " + playerNum + "? ");
                response = Console.ReadLine();
            }
            return response;
        }

        public int OfferACard(Player player)
        {
            while (true)
            {
                Console.Write(player.name + ", do you want a card? (Y/N)");
                string response = Console.ReadLine();
                if (response.ToUpper().StartsWith("Y"))
                {
                    while (response.ToUpper().StartsWith("Y"))
                    {
                        Console.Write(player.name + ", how many cards do you want(Up to 3)?");
                        string cardNumber = Console.ReadLine();
                        if(int.TryParse(cardNumber, out int number))
                        {
                            if (number <= 3 && number > 0)
                            {
                                return number;
                            }
                            else Console.WriteLine("Only up to 3 cards and no Negative numbers !");
                        }
                        else
                        {
                            Console.WriteLine("Please input only number!");
                        }
                    }

                }
                else if (response.ToUpper().StartsWith("N"))
                {
                    return 0;
                }
                else
                {
                    Console.WriteLine("Please answer Y(es) or N(o)!");
                }
            }
        }

        public void ShowHand(Player player)
        {
            if (player.cards.Count > 0)
            {
                Console.Write(player.name + " has: ");
                foreach (Card card in player.cards)
                {
                    /*Using list.IndexOf(T) method to get current card's index, if it isn't the last card, then add ","
                      else only out put itself */
                    if(player.cards.IndexOf(card) != player.cards.Count - 1)
                    {
                        Console.Write(card.name + ", ");
                    }
                    else
                    {
                        Console.Write(card.name);
                    }
                    
                }
                Console.Write("=" + player.score + "/21 ");
                if (player.status != PlayerStatus.active)
                {
                    Console.Write("(" + player.status.ToString().ToUpper() + ")");
                }
                Console.WriteLine();
            }
        }

        public void ShowHands(List<Player> players)
        {
            foreach (Player player in players)
            {
                ShowHand(player);
            }
        }

        public void ShowEarnedPoints(List<Player> players)
        {
            foreach(Player player in players)
            {
                Console.WriteLine(player.name + " has " + player.earnedPoints + " Points");
            }
        }


        public void AnnounceWinner(Player player, List<Player> players)
        {
            if (player != null)
            {
                Console.WriteLine(player.name + " wins!");
                //After announced the winner, printing current earned points of each player
                foreach (Player player1 in players)
                {
                    Console.WriteLine(player1.name + " has " + player1.earnedPoints + " Points");
                }
            }
            else
            {
                Console.WriteLine("Everyone busted!");
            }
            Console.Write("Press <Enter> to exit... ");
            while (Console.ReadKey().Key != ConsoleKey.Enter) { }
        }
    }
}