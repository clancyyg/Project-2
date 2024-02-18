using System;
using System.Collections.Generic;

namespace RaceTo21
{

    public class Game
    {
        int bustCount = 0;// Counting how many player are busted in one game
        int numberOfPlayers; // number of players in current game
        List<Player> players = new List<Player>(); // list of objects containing player data
        CardTable cardTable; // object in charge of displaying game information
        Deck deck = new Deck(); // deck of cards
        int currentPlayer = 0; // current player on list
        public Task nextTask; // keeps track of game state
        int winngPoint = 0;// This is the goal that players need to reach to win the whole game


        // Game Manager constructor.      
        public Game(CardTable c)
        {
            cardTable = c;
            deck.Shuffle();
            deck.ShowAllCards();
            nextTask = Task.GetNumberOfPlayers;
        }

        // Adding a player to the current game.
        public void AddPlayer(string n)
        {
            players.Add(new Player(n));
        }

        //Doing task in a game
        public void DoNextTask()
        {
            //This meaning there are still more than 1 player's status isn't bust, game continue
            if (bustCount != players.Count - 1)
            {
                if (nextTask == Task.GetNumberOfPlayers)
                {
                    //Get number of players
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
                    nextTask = Task.SetGoal;// Switching to next task
                }
                else if (nextTask == Task.SetGoal)
                {
                    while (true)
                    {
                        //Reading input for player
                        Console.Write("Please set the goal for this game(Only numbers and no negative or 0: ");
                        string temp = Console.ReadLine();
                        if (int.TryParse(temp, out winngPoint) && winngPoint > 0)
                        {
                            //IsAgreed is a method to jude that whether there are half or more than half players agree with the goal
                            if (IsAgreed())
                            {
                                Console.WriteLine("Half or more that half players agreed with this goal, let's play ! ");
                                Console.WriteLine("================================");
                                nextTask = Task.IntroducePlayers;// Switching to next task
                                break;
                            }
                            else
                            {
                                Console.WriteLine("Most of plyers disagreed with this goal, try another one");
                                Console.WriteLine("================================");
                            }
                        }
                        else
                        {
                            Console.WriteLine("Please Only numbers and no negative or 0! ");
                            Console.WriteLine("================================");
                        }
                    }
                }
                else if (nextTask == Task.IntroducePlayers)
                {
                    cardTable.ShowPlayers(players);//showing all players
                    nextTask = Task.PlayerTurn;// Switching to next task
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
                            for (int i = 0; i < temp; i++)
                            {
                                //Dealing a card and calculating score
                                Card card = deck.DealTopCard();
                                player.AddCard(card);
                                player.SetScore(ScoreHand(player));
                            }

                            if (player.GetScore() > 21)//if score > 21, player's status goes to bust
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
                                cardTable.AnnounceWinner(player, players);
                                player.SetEarnedPoints(player.GetScore());
                                /*Juding whether current winner reach the goal, if so, annouce the winner and end the game,
                                  if not, game coutinues*/
                                if (player.GetEarnedPoints() >= winngPoint)
                                {
                                    Console.WriteLine(player.GetName() + " reached the goal " + player.GetName() + " wins!");
                                    nextTask = Task.GameOver;
                                    return;
                                }
                                else
                                {
                                    PunishForBust();//This method is used to minus 21 points from players who go bust
                                    cardTable.ShowEarnedPoints(players);
                                    Restart();//This method is renew the round
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
                        cardTable.AnnounceWinner(winner, players);
                        /*Juding whether current winner reach the goal, if so, annouce the winner and end the game,
                        if not, game coutinues*/
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
                cardTable.AnnounceWinner(winner, players);
                /*Juding whether current winner reach the goal, if so, annouce the winner and end the game,
                if not, game coutinues*/
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
        /// ask for a value and set player score to it.
        /// </summary>
        /// <param name="player">Instance representing one player</param>
        /// <returns></returns>
        public int ScoreHand(Player player)
        {
            int score = 0;
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
            return score;
        }

        // This function is to find busted players and their earnedPoint will be minus 21
        public void PunishForBust()
        {

            foreach (var player in players)
            {
                //Iterating the player list and find players whose status is bust
                if (player.status == PlayerStatus.bust)
                {
                    Console.WriteLine(player.GetName() + " will lose 21 points");
                    int temp = -21;
                    player.SetEarnedPoints(temp);
                }
            }
        }

        //This function is checking whether there are any player's status is active
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

        /*        
       This funtion is to jude whether half or more than half players agree with the current goal
        if half or more than half players agreed, return true, else, return false
        */
        public bool IsAgreed()
        {
            int agreedCount = 0;// This variable is to count how many palyer agreed
            foreach (Player player in players)//Iterating player list
            {
                Console.Write(player.GetName() + " do you agree with this goal?(Y/N) :");
                string response = Console.ReadLine();
                if (response.ToUpper().StartsWith("Y"))
                {
                    agreedCount++;//if agreed, count++
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
            //If half or more than half players agreed, return true, else, return false
            if (agreedCount >= (players.Count + 1) / 2) return true;
            else return false;
        }


        // This funtion is to resets some attributes of player and game to start a new round 
        public void Restart()
        {
            foreach (Player player in players)
            {
                player.SetScore(0);//Setting player score to 0
                player.ClearHand();//Clearing up player's card
                player.status = PlayerStatus.active;// Seting status to active, which is original status
            }
            currentPlayer = 0;//change current player to the frist one
            bustCount = 0;//Since all status go back to active, so bustcount goes to 0
            Deck deck = new Deck();//Creating a new deck for a new round and reshuffle the card
            deck.Shuffle();
            deck.ShowAllCards();
        }

        //Calculating player's score
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
                // If all players chose to not to draw a card at the first drawing stage, then randomly pick a player as winner
                Random rnd = new Random();
                int randomWinner = rnd.Next(0, players.Count);
                return players[randomWinner];
            }

        }
    }
}
