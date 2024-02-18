using System;
using System.Collections.Generic;

namespace RaceTo21
{	

	public class Player
	{
		private string name;
		private List<Card> cards = new List<Card>();
		public PlayerStatus status = PlayerStatus.active;// Keep this public because in real game, player can know other player's status
		private int score;
		private int earnedPoints = 0;//This shows that how many points that a player earned
		
		//Clearing cards in player's hand
		public void ClearHand()
        {
			cards.Clear();
        }

		//Adding new card into plaer's hand
		public void AddCard(Card card)  
        {
			cards.Add(card);
        }

		//Getter mothod for cards
		public List<Card> GetCard()
        {
			return cards;
        }

		//Getter mothod for score
		public int GetScore()
        {
			return score;
        }

		//Setter mothod for score
		public void SetScore(int score)
        {
			this.score = score;
        }

		//Getter mothod for earnedPoints
		public int GetEarnedPoints()
        {
			return earnedPoints;
        }

		//Setter mothod for earnedPoints
		public void SetEarnedPoints(int point)
        {
			if(point < 0)
            {
				//Since point is a nagetive number, so using plus not minus
				if (earnedPoints + point < 0) earnedPoints = 0;//If current result lower than 0,  earnedPoints will be 0
				else earnedPoints = earnedPoints + point;
            }
            else
            {
				earnedPoints += point;
            }
        }

		//Setter mothod for name
		public void SetName(string name)
        {
			this.name = name;
		}

		//Getter mothod for name
		public string GetName()
        {
			return name.ToString();
        }

		public Player(string n)
		{
			name = n;
        }


		public void Introduce(int playerNum)
		{
			Console.WriteLine("Hello, my name is " + name + " and I am player #" + playerNum);
		}
	}
}

