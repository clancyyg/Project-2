using System;
using System.Diagnostics;
using System.Numerics;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace RaceTo21
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 

    public partial class MainWindow : Window
    {

        public int bustCount = 0;// Counting how many player are busted in one game
        public int numberOfPlayers; // number of players in current game
        public List<Player> players = new List<Player>(); // list of objects containing player data
        public CardTable cardTable; // object in charge of displaying game information
        public Deck deck = new Deck(); // deck of cards
        public int currentPlayer = 0; // current player on list
        public int winngPoint = 100;// This is the goal that players need to reach to win the whole game
        public CardTable c;

        private List<TextBox> textBoxes = new List<TextBox>();


        public MainWindow()
        {
            //Initialzing start up components
            InitializeComponent();
            cardTable = new CardTable();//Creating card table
            deck.Shuffle();//shuffle the card deck
        }

        //This function will create textboxes to get input for Player's name
        private void GenerateTextBoxes_Click(object sender, RoutedEventArgs e)
        {
            textBoxContainer.Children.Clear();
            textBoxes.Clear();

            //If player number is vaild, then create boxes to get input
            if (int.TryParse(inputTextBox.Text, out numberOfPlayers) && numberOfPlayers >= 2 && numberOfPlayers < 10)
            {
                for (int i = 0; i < numberOfPlayers; i++)
                {
                    TextBox textBox = new TextBox();
                    textBox.Name = "textBox_" + (i + 1);
                    textBox.Text = "please enter player " + (i + 1) + " 's name";
                    textBox.Margin = new Thickness(5);
                    textBoxes.Add(textBox);
                    textBoxContainer.Children.Add(textBox);
                }
                NumberInput.Visibility = Visibility.Collapsed;
                inputTextBox.Visibility = Visibility.Hidden;
                SaveName.Visibility = Visibility.Visible;
            }
            else
            {
                
                MessageBox.Show("Please enter a valid number.Input must be number and greater than 1 but less than 10");
            }

        }

        //Definding progress that will happen after click the savename button, just like get into the next task
        private void SaveNames_Click(object sender, RoutedEventArgs e)
        {
            namesListBox.Items.Clear();
            for (int i = 0; i < textBoxes.Count; i++)
            {
                AddPlayer(textBoxes[i].Text);
            }

            //Priting players information
            foreach (Player player in players)
            {
                namesListBox.Items.Add("Player " + (players.IndexOf(player) + 1) + ": " + player.GetName().ToString() + " Score: " + player.GetScore().ToString() + " Status:" + player.status.ToString() + " Earned points:" + player.GetEarnedPoints());
            }

            ///Summary: Follwing steps are hiding or calling some UI element of current or next task
            textBoxContainer.Children.Clear();
            textBoxContainer.Visibility = Visibility.Hidden;
            SaveName.Visibility = Visibility.Hidden;
            Target.Visibility = Visibility.Visible;
            PlayerTurn.Visibility = Visibility.Visible;

            AskPlayerForCards();//Get into the next task, player's turn
        }

        //Asking players whether they cards and then do the further process 
        private void AskPlayerForCards()
        {
            while (true)
            {
                //If there are more than 1 player stay in the table, game continues
                if (bustCount != players.Count - 1)
                {
                    if (currentPlayer <= players.Count - 1)
                    {
                        if (players[currentPlayer].status == PlayerStatus.active)
                        {
                            //Aksing player whether this player needs darw cards 
                            MessageBoxResult result = MessageBox.Show("Player " + players[currentPlayer].GetName().ToString() + ", do you want to draw cards?", "Draw Cards", MessageBoxButton.YesNo);
                            if (result == MessageBoxResult.Yes)
                            {
                                int numCardsToDraw = AskNumCardsToDraw();
                                if (numCardsToDraw > 0) DrawCardsForPlayer(numCardsToDraw);
                                PlayerName.Text = players[currentPlayer].GetName().ToString() + "'s Cards";
                                
                                //show the card's images on UI
                                CardsListView.ItemsSource = players[currentPlayer].GetCard();

                                //if score > 21, player's status goes to bust
                                if (players[currentPlayer].GetScore() > 21)
                                {
                                    players[currentPlayer].status = PlayerStatus.bust;
                                    //counting how many players are busted
                                    bustCount++;
                                }
                                else if (players[currentPlayer].GetScore() == 21)
                                {
                                    //If there is a player first reached 21, the game end, and return the function
                                    players[currentPlayer].status = PlayerStatus.win;
                                    //Winner will earn points that equals to their current hand
                                    cardTable.AnnounceWinner(players[currentPlayer], players);
                                    players[currentPlayer].SetEarnedPoints(players[currentPlayer].GetScore());
                                    PunishForBust();
                                    //If the winner reached the winning point, this winner will win the game
                                    if (players[currentPlayer].GetEarnedPoints() >= winngPoint)
                                    {
                                        MessageBoxResult end = MessageBox.Show(players[currentPlayer].GetName().ToString() + " Win the whole game, Game over!");
                                        break;
                                    }
                                    else
                                    {
                                        //Restart a turn and punish those players who busted, minus 21 points
                                        PunishForBust();
                                        Restart();//Reshuffle the deck
                                        //Update information
                                        for (int i = 0; i < players.Count; i++)
                                        {
                                            namesListBox.Items[i] = "Player " + (players.IndexOf(players[i]) + 1) + ": " + players[i].GetName().ToString() + "   Score: " + players[i].GetScore().ToString() + " Status:" + players[i].status.ToString() + " Earned points:" + players[i].GetEarnedPoints();
                                        }
                                        AskPlayerForCards();//start a new turn
                                        break;
                                    }
                                }
                            }
                            else if (result == MessageBoxResult.No)//if the result is no then change the status to stay
                            {
                                players[currentPlayer].status = PlayerStatus.stay;
                                //Checking where all players chose to stay, if so, do final scoring and annouce the winner 
                                if (!CheckActivePlayers())
                                {
                                    Player winner = DoFinalScoring();
                                    winner.SetEarnedPoints(winner.GetScore());
                                    cardTable.AnnounceWinner(winner, players);
                                    /*Juding whether current winner reach the goal, if so, annouce the winner and end the game,
                                    if not, game coutinues*/
                                    if (winner.GetEarnedPoints() >= winngPoint)
                                    {
                                        MessageBoxResult end = MessageBox.Show(winner.GetName().ToString() + " Win the whole game, Game over!");
                                        break;
                                    }
                                    else
                                    {
                                        //Restart a turn and punish those players who busted, minus 21 points
                                        PunishForBust();
                                        Restart();
                                        for (int i = 0; i < players.Count; i++)
                                        {
                                            namesListBox.Items[i] = "Player " + (players.IndexOf(players[i]) + 1) + ": " + players[i].GetName().ToString() + "   Score: " + players[i].GetScore().ToString() + " Status:" + players[i].status.ToString() + " Earned points:" + players[i].GetEarnedPoints();
                                        }
                                        AskPlayerForCards();//start a new turn
                                        break;
                                    }
                                }
                            }
                            //Updating current player' infromation
                            namesListBox.Items[currentPlayer] = "Player " + (players.IndexOf(players[currentPlayer]) + 1) + ": " + players[currentPlayer].GetName().ToString() + "   Score: " + players[currentPlayer].GetScore().ToString() + " Status:" + players[currentPlayer].status.ToString() + " Earned points:" + players[currentPlayer].GetEarnedPoints();
                            currentPlayer++;
                        }
                        else if (players[currentPlayer].status == PlayerStatus.stay)//if stay, print out the message and then go for the next player
                        {
                            MessageBoxResult result = MessageBox.Show(players[currentPlayer].GetName().ToString() + " chose to stay");
                            currentPlayer++;
                        }
                        else
                        {
                            MessageBoxResult result = MessageBox.Show(players[currentPlayer].GetName().ToString() + " is busted");//if busted, print out the message and then go for the next player
                            currentPlayer++;
                        }
                    }
                    else
                    {
                        if (currentPlayer >= players.Count - 1)
                        {
                            currentPlayer = 0; // back to the first player...
                        }
                    }
                }
                else
                {
                    // if there are all but one player busted, the remaining player will be the winner
                    int index = 0;
                    foreach (Player player in players)
                    {
                        //Finding the player whose status is active or stay
                        if (player.status != PlayerStatus.bust) index = players.IndexOf(player);
                    }
                    players[index].status = PlayerStatus.win;
                    cardTable.AnnounceWinner(players[index], players);
                    players[index].SetEarnedPoints(players[index].GetScore());
                    PunishForBust();// players who busted will lose 21 points
                    for (int i = 0; i < players.Count; i++)//Updating UI
                    {
                        namesListBox.Items[i] = "Player " + (players.IndexOf(players[i]) + 1) + ": " + players[i].GetName().ToString() + "   Score: " + players[i].GetScore().ToString() + " Status:" + players[i].status.ToString() + " Earned points:" + players[i].GetEarnedPoints();
                    }

                    if (players[index].GetEarnedPoints() >= winngPoint)
                    {
                        //There has a player win the game, game over
                        MessageBoxResult end = MessageBox.Show(players[index].GetName().ToString() + " Win the whole game, Game over!");
                        break;
                    }
                    else
                    {
                        //If not, reshuffle the deck start a new turn
                        Restart();
                        for (int i = 0; i < players.Count; i++)
                        {
                            namesListBox.Items[i] = "Player " + (players.IndexOf(players[i]) + 1) + ": " + players[i].GetName().ToString() + "   Score: " + players[i].GetScore().ToString() + " Status:" + players[i].status.ToString() + " Earned points:" + players[i].GetEarnedPoints();
                        }
                        AskPlayerForCards();
                        break;
                    }
                }

            }

        }

        //This function is to get input about how many card the player needs and then do the further process based on the result
        private int AskNumCardsToDraw()
        {
            int number = 0;
            
            //Asking player how many cards to draw
            string input = Microsoft.VisualBasic.Interaction.InputBox("Enter number of cards to draw (up to 3):", "Number of Cards", "1");
            if (int.TryParse(input, out number))
            {
                if (number < 1 || number > 3)
                {
                    MessageBox.Show("Please enter a number between 1 and 3.");
                    number = AskNumCardsToDraw(); // Recursively call until a valid number is input.
                }
            }
            else
            {
                MessageBox.Show("Please enter a valid number.");
                number = AskNumCardsToDraw(); // Recursively call until a valid number is input.
            }

            return number;
        }

        //Calculating how many points a player has based the the cards in player's hand
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

        //Assign a card to player and show the corrspondent image on the UI
        private void DrawCardsForPlayer(int numCardsToDraw)
        {
            // Show message indicating how many cards have been given to the player.
            MessageBox.Show("Drawing " + numCardsToDraw + " cards for Player " + players[currentPlayer].GetName().ToString());
            for (int i = 0; i < numCardsToDraw; i++)
            {
                //Dealing a card and calculating score
                Card card = deck.DealTopCard();
                players[currentPlayer].AddCard(card);
                players[currentPlayer].SetScore(ScoreHand(players[currentPlayer]));
            }
        }

        //Adding player into the players List
        public void AddPlayer(string n)
        {
            players.Add(new Player(n));
        }

        //This function is to reset attributes and restart a new turn
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
            CardsListView.ItemsSource = null;// Cleaning the cardlist view so there will have no images
            PlayerName.Text = string.Empty;//Cleaning the header of cardlist
        }

        //This function is to minus 21 points to players whoes status are busted
        public void PunishForBust()
        {
            foreach (var player in players)
            {
                //Iterating the player list and find players whose status is bust
                if (player.status == PlayerStatus.bust)
                {
                    MessageBox.Show(player.GetName() + " will lose 21 points because of busted");
                    int temp = -21;
                    player.SetEarnedPoints(temp);
                }
            }
        }

        //Check if there are still active players
        public bool CheckActivePlayers()
        {
            foreach (var player in players)
            {
                if (player.status == PlayerStatus.active)
                {
                    return true; // at least one player is still going!
                }
            }
            return false; // everyone has stayed or busted, or someone won!
        }

        // Calculate the player's score and determine the winner.
        public Player DoFinalScoring()
        {
            int highScore = 0;
            foreach (var player in players)
            {

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

        //This function is to call or hiding some UI elements for next steps
        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            inputTextBox.Visibility = Visibility.Visible;
            NumberInput.Visibility = Visibility.Visible;
            Start.Visibility = Visibility.Hidden;
            Title.Visibility = Visibility.Hidden;
            LeftTitle.Visibility = Visibility.Hidden;
            RightTitle.Visibility = Visibility.Hidden;
        }

    }
}