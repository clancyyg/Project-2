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
        int winningPoint;
        int bustCount = 0;// Counting how many player are busted in one game
        int numberOfPlayers; // number of players in current game
        List<Player> players = new List<Player>(); // list of objects containing player data
        CardTable cardTable; // object in charge of displaying game information
        Deck deck = new Deck(); // deck of cards
        int currentPlayer = 0; // current player on list
        public Task nextTask; // keeps track of game state
        private bool cheating = false; // lets you cheat for testing purposes if true

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
                    nextTask = Task.SetWinningPoint;
                }
                else if (nextTask == Task.SetWinningPoint)
                {
                    while (true)
                    {
                        Console.Write("Please set the winning point(Only numbers and no negative or 0) : ");
                        string temp = Console.ReadLine();
                        if(!int.TryParse(temp, out winningPoint))
                        {
                            Console.WriteLine("Please enter Only numbers and no negative");
                        }
                        else
                        {
                            
                            if(winningPoint <= 0)
                            {
                                Console.WriteLine("No negative or 0!");
                            }
                            else
                            {
                                if (IsAgreed())
                                {
                                    Console.WriteLine("half or more than half playes agree with this number, let's play!");
                                    nextTask = Task.IntroducePlayers;
                                    break;
                                }
                                else
                                {
                                    Console.WriteLine("Less than half playes agree with this number, try another one");
                                }
                            }
                        }
                        Console.WriteLine("================================");
                    }

                }
                else if (nextTask == Task.IntroducePlayers)
                {
                    cardTable.ShowPlayers(players);
                    Console.WriteLine("The winning point is " + winningPoint);
                    cardTable.ShowEarnedPoints(players);
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
                                player.cards.Add(card);
                                player.score = ScoreHand(player);
                            }

                            if (player.score > 21)
                            {
                                player.status = PlayerStatus.bust;
                                //counting how many players are busted
                                bustCount++;
                            }
                            else if (player.score == 21)
                            {
                                //If there is a player first reached 21, the game end, and return the function
                                player.status = PlayerStatus.win;
                                //Winner will earn points that equals to their current hand
                                player.earnedPoints += player.score;
                                cardTable.ShowHand(player);
                                cardTable.AnnounceWinner(player,players);
                                MinusBustedPlayersPoint();
                                foreach (Player player1 in players)
                                {
                                    if(player1.status == PlayerStatus.bust)
                                    {
                                        if (player1.earnedPoints - player1.score < 0) player1.earnedPoints = 0;
                                        else player1.earnedPoints -= player1.score;
                                    }
                                }
                                if(player.earnedPoints >= winningPoint)
                                {
                                    Console.WriteLine(player.name + " has reached the winning point and win the whole game !");
                                    nextTask = Task.GameOver;
                                    return;
                                }
                                else
                                {
                                    RestartGame();
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
                        winner.earnedPoints += winner.score;
                        cardTable.AnnounceWinner(winner,players);
                        MinusBustedPlayersPoint();
                        if (winner.earnedPoints >= winningPoint)
                        {
                            Console.WriteLine(winner.name + " has reached the winning point and win the whole game !");
                            nextTask = Task.GameOver;
                            return;
                        }
                        else
                        {
                            RestartGame();
                            return;
                        }
                        
                        //nextTask = Task.GameOver;
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
                winner.earnedPoints += winner.score;
                cardTable.AnnounceWinner(winner,players);
                MinusBustedPlayersPoint();
                if (winner.earnedPoints >= winningPoint)
                {
                    Console.WriteLine(winner.name + " has reached the winning point and win the whole game !");
                    nextTask = Task.GameOver;
                    return;
                }
                else
                {
                    RestartGame();
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
                    Console.Write("OK, what should player " + player.name + "'s score be?");
                    response = Console.ReadLine();
                }
                return score;
            }
            else
            {
                foreach (Card card in player.cards)
                {
                    string faceValue = card.ID.Remove(card.ID.Length - 1);
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

        public void MinusBustedPlayersPoint()
        {
            foreach (Player player in players)
            {
                if (player.status == PlayerStatus.bust)
                {
                    if (player.earnedPoints - 21 < 0) player.earnedPoints = 0;
                    else player.earnedPoints -= 21;
                }
            }
        }

        public bool IsAgreed()
        {
            int totalPlayer = players.Count;
            int agreedPlayer = 0;
            while (true)
            {
                foreach(Player player in players)
                {
                    Console.Write(player.name + ", do you agree with this winning point? (Y/N)");
                    string response = Console.ReadLine();
                    if (response.ToUpper().StartsWith("Y"))
                    {
                        agreedPlayer++;

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

                if (agreedPlayer > totalPlayer % 2) return true;
                else return false;
                
            }
        }

        public void RestartGame()
        {
            foreach (Player player in players)
            {
                player.cards.Clear();
                player.status = PlayerStatus.active;
                player.score = 0;
            }
            Deck deck = new Deck();
            deck.Shuffle();
            deck.ShowAllCards();
            currentPlayer = 0;
            bustCount = 0;
            nextTask = Task.IntroducePlayers;
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
                    if (player.score > highScore)
                    {
                        highScore = player.score;
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
                return players.Find(player => player.score == highScore);
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
