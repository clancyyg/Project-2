using System;
using System.Collections.Generic;

namespace RaceTo21
{	

	public class Player
	{
		private string name;
		private List<Card> cards = new List<Card>();
		public PlayerStatus status = PlayerStatus.active;
		private int score;
		private int earnedPoints = 0;//This shows that how many points that a player earned
		
		public void ClearHand()
        {
			cards.Clear();
        }

		public void AddCard(Card card)
        {
			cards.Add(card);
        }

		public List<Card> GetCard()
        {
			return cards;
        }
		public int GetScore()
        {
			return score;
        }

		public void SetScore(int score)
        {
			this.score = score;
        }

		public int GetEarnedPoints()
        {
			return earnedPoints;
        }

		public void SetEarnedPoints(int point)
        {
			if(point < 0)
            {
				if (earnedPoints + point < 0) earnedPoints = 0;
				else earnedPoints = earnedPoints + point;
            }
            else
            {
				earnedPoints += point;
            }
        }

		public void SetName(string name)
        {
			this.name = name;
        }

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

