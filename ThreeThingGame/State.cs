using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThreeThingGame
{
    internal class State
    {
        // Variables
        public enum GameState
        {
            Menu_Main,
            Intro_Load,
            Intro_Main,
            Day_Load,
            Day_Main,
            Game_Load,
            Game_Main,
            DayEnd_Load,
            DayEnd_Main
        }
    }
}
