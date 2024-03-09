using System;
namespace RaceTo21
{

    /*Player's current participation in game.
	 "active" allows player to continue taking cards.
     All others skip this player until game end. */


    public enum PlayerStatus
    {
        active,
        stay,
        bust,
        win
    }
}

