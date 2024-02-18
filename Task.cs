using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RaceTo21
{
    /// <summary>
    /// Tracks game state. 
    /// Replaces less reliable string-based tracking in original.
    /// </summary>
    public enum Task
    {
        GetNumberOfPlayers,
        GetNames,
        //I added this in task, setgoal, in this stage, player can set a goal to win the game
        SetGoal,
        IntroducePlayers,
        PlayerTurn,
        CheckForEnd,
        GameOver
    }
}
