using System;
using System.Collections.Generic;

namespace RaceTo21
{
    /// <summary>
    /// Game Manager class.
    /// Handles game flow and scoring.
    /// </summary>
    public class Game
    {
        int bustCount = 0;// Counting how many player are busted in one game
        int numberOfPlayers; // number of players in current game
        List<Player> players = new List<Player>(); // list of objects containing player data
        CardTable cardTable; // object in charge of displaying game information
        Deck deck = new Deck(); // deck of cards
        int currentPlayer = 0; // current player on list
        public Task nextTask; // keeps track of game state
        private bool cheating = false; // lets you cheat for testing purposes if true
        int winngPoint = 0;
        

        /// <summary>
        /// Game Manager constructor.
        /// </summary>
        /// <param name="c">A CardTable instance, which manages player input & output</param>
        public Game(CardTable c)
        {
            cardTable = c;
            deck.Shuffle();
            deck.ShowAllCards();
            nextTask = Task.GetNumberOfPlayers;
        }

        /// <summary>
        /// Adds a player to the current game.
        /// Called by DoNextTask() method.
        /// </summary>
        /// <param name="n">Player name</param>
        public void AddPlayer(string n)
        {
            players.Add(new Player(n));
        }


        public void DoNextTask()
        {

            //This meaning there are still more than 1 player's status isn't bust, game continue
            if (bustCount != players.Count - 1)
            {
                if (nextTask == Task.GetNumberOfPlayers)
                {
                    numberOfPlayers = cardTable.GetNumberOfPlayers();
                    nextTask = Task.GetNames;
                    Console.WriteLine("================================"); 
                }
                else if (nextTask == Task.GetNames)
                {
                    for (var count = 1; count <= numberOfPlayers; count++)
                    {
                        var name = cardTable.GetPlayerName(count);
                        AddPlayer(name); // NOTE: player list will start from 0 index even though we use 1 for our count here to make the player numbering more human-friendly
                    }
                    Console.WriteLine("================================"); 
                    nextTask = Task.SetGoal;
                }
                else if (nextTask == Task.SetGoal)
                {
                    while (true)
                    {
                        Console.Write("Please set the goal for this game(Only numbers and no negative or 0: ");
                        string temp = Console.ReadLine();
                        if(int.TryParse(temp, out winngPoint) && winngPoint >= 0)
                        {
                            if (IsAgreed())
                            {
                                Console.WriteLine("Half or more that half players agreed with this goal, let's play ! ");
                                Console.WriteLine("================================");
                                nextTask = Task.IntroducePlayers;
                                break;
                            }
                            else
                            {
                                Console.WriteLine("Most of plyers disagreed with this goal, try another one");
                            }
                        }
                        else
                        {
                            Console.WriteLine("Please Only numbers and no negative or 0! ");
                        }
                    }
                }
                else if (nextTask == Task.IntroducePlayers)
                {
                    cardTable.ShowPlayers(players);
                    
                    nextTask = Task.PlayerTurn;
                    Console.WriteLine("================================"); 
                }
                else if (nextTask == Task.PlayerTurn)
                {
                    Player player = players[currentPlayer];
                    if (player.status == PlayerStatus.active)
                    {
                        int temp = cardTable.OfferACard(player);
                        if (temp != 0)
                        {
                            for(int i = 0; i < temp; i++)
                            {
                                Card card = deck.DealTopCard();
                                player.AddCard(card);
                                player.SetScore(ScoreHand(player));
                            }

                            if (player.GetScore() > 21)
                            {
                                player.status = PlayerStatus.bust;
                                //counting how many players are busted
                                bustCount++;
                            }
                            else if (player.GetScore() == 21)
                            {
                                //If there is a player first reached 21, the game end, and return the function
                                player.status = PlayerStatus.win;
                                //Winner will earn points that equals to their current hand
                                cardTable.ShowHand(player);
                                cardTable.AnnounceWinner(player,players);
                                player.SetEarnedPoints(player.GetScore());
                                if(player.GetEarnedPoints() >= winngPoint)
                                {
                                    Console.WriteLine(player.GetName() + " reached the goal " + player.GetName() + " wins!");
                                    nextTask = Task.GameOver;
                                    return;
                                }
                                else
                                {
                                    PunishForBust();
                                    cardTable.ShowEarnedPoints(players);
                                    Restart();
                                    nextTask = Task.PlayerTurn;
                                    return;
                                }

                            }
                            Console.WriteLine("================================");
                        }
                        else
                        {
                            player.status = PlayerStatus.stay;
                            Console.WriteLine("================================");
                        }
                    }
                    cardTable.ShowHand(player);
                    Console.WriteLine("================================");
                    cardTable.ShowHands(players);
                    nextTask = Task.CheckForEnd; 
                }
                else if (nextTask == Task.CheckForEnd)
                {
                    if (!CheckActivePlayers())
                    {
                        Player winner = DoFinalScoring();
                        //Winner will earn points that equals to their current hand
                        winner.SetEarnedPoints(winner.GetScore());
                        cardTable.AnnounceWinner(winner,players);
                        if (winner.GetEarnedPoints() >= winngPoint)
                        {
                            Console.WriteLine(winner.GetName() + " reached the goal " + winner.GetName() + " wins!");
                            nextTask = Task.GameOver;
                            return;
                        }
                        else
                        {
                            PunishForBust();
                            cardTable.ShowEarnedPoints(players);
                            Restart();
                            nextTask = Task.PlayerTurn;
                            return;
                        }
                    }
                    else
                    {
                        currentPlayer++;
                        if (currentPlayer > players.Count - 1)
                        {
                            currentPlayer = 0; // back to the first player...
                        }
                        nextTask = Task.PlayerTurn;
                    }
                }
            }
            else
            {
                //Right now there are all but one player "bust" 
                int index = 0;
                foreach (Player player in players)
                {
                    if (player.status != PlayerStatus.bust) index = players.IndexOf(player);
                }
                //Printing current player's inforamtion and announce winner.
                foreach (var player in players)
                {
                    cardTable.ShowHand(player);
                }
                Player winner = players[index];
                //Winner will earn points that equals to their current hand
                winner.SetEarnedPoints(winner.GetScore());
                cardTable.AnnounceWinner(winner,players);
                if (winner.GetEarnedPoints() >= winngPoint)
                {
                    Console.WriteLine(winner.GetName() + " reached the goal " + winner.GetName() + " wins!");
                    nextTask = Task.GameOver;
                    return;
                }
                else
                {
                    PunishForBust();
                    cardTable.ShowEarnedPoints(players);
                    Restart();
                    nextTask = Task.PlayerTurn;
                    return;
                }
            }


        }

        /// <summary>
        /// Score the cards in the player's list of cards OR (if cheating)
        /// ask for a value and set player score to it.
        /// </summary>
        /// <param name="player">Instance representing one player</param>
        /// <returns></returns>
        public int ScoreHand(Player player)
        {
            int score = 0;
            if (cheating == true && player.status == PlayerStatus.active)
            {
                string response = null;
                while (int.TryParse(response, out score) == false)
                {
                    Console.Write("OK, what should player " + player.GetName() + "'s score be?");
                    response = Console.ReadLine();
                }
                return score;
            }
            else
            {
                foreach (Card card in player.GetCard())
                {
                    string faceValue = card.GetID().Remove(card.GetID().Length - 1);
                    switch (faceValue)
                    {
                        case "K":
                        case "Q":
                        case "J":
                            score = score + 10;
                            break;
                        case "A":
                            score = score + 1;
                            break;
                        default:
                            score = score + int.Parse(faceValue);
                            break;
                    }
                }
               
            }
            return score;
        }

        // This function is to find busted player and their earnedPoint will minus 21
        public void PunishForBust()
        {

            foreach (var player in players)
            {
                if(player.status == PlayerStatus.bust)
                {
                    Console.WriteLine(player.GetName() + " will lose 21 points");
                    int temp = -21;
                    player.SetEarnedPoints(temp);
                }
            }
        }

        public bool CheckActivePlayers()
        {
            /* Reminder that var is perfectly OK in C# unlike in JavaScript; it is handy for temporary variables! */
            foreach (var player in players)
            {
                if (player.status == PlayerStatus.active)
                {
                    return true; // at least one player is still going!
                }
            }
            return false; // everyone has stayed or busted, or someone won!
        }

        public bool IsAgreed()
        {
            int agreedCount = 0;
            foreach(Player player in players)
            {
                Console.Write(player.GetName() + " do you agree with this goal?(Y/N) :");
                string response = Console.ReadLine();
                if (response.ToUpper().StartsWith("Y"))
                {
                    agreedCount++;
                }
                else if (response.ToUpper().StartsWith("N"))
                {
                    continue;
                }
                else
                {
                    Console.WriteLine("Please answer Y(es) or N(o)!");
                }
            }

            if (agreedCount >= (players.Count+1) / 2) return true;
            else return false;
        }



        public void Restart()
        {
            foreach(Player player in players)
            {
                player.SetScore(0);
                player.ClearHand();
                player.status = PlayerStatus.active;
            }
            currentPlayer = 0;
            bustCount = 0;
            Deck deck = new Deck();
            deck.Shuffle();
            deck.ShowAllCards();
        }

        public Player DoFinalScoring()
        {
            int highScore = 0;
            foreach (var player in players)
            {
                cardTable.ShowHand(player);
                if (player.status == PlayerStatus.win) // someone hit 21
                {
                    return player;
                }
                if (player.status == PlayerStatus.stay) // still could win...
                {
                    if (player.GetScore() > highScore)
                    {
                        highScore = player.GetScore();
                    }
                }
                // if busted don't bother checking!
                if (player.status == PlayerStatus.bust)
                {
                    continue;
                }
            }
            if (highScore > 0) 
            {
                // find the FIRST player in list who meets win condition
                return players.Find(player => player.GetScore() == highScore);
            }
            else
            {
                // If all players chose to not to draw a card, then randomly pick a player as winner
                Random rnd = new Random();
                int randomWinner = rnd.Next(0, players.Count);
                return players[randomWinner];
            }
            
        }
    }
}
