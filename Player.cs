using System;
using System.Collections.Generic;

namespace RaceTo21
{	

	public class Player
	{
		public string name;
		public List<Card> cards = new List<Card>();
		public PlayerStatus status = PlayerStatus.active;
		public int score;
		public int earnedPoints = 0;//This shows that how many points that a player earned

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

