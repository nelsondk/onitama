using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace OnitamaEngine
{
    public class Card
    {
        public string Name { get; }
        /// <summary>
        /// A list of int[2] - index 0 is x-axis, index 1 is y-axis, and origin is the player location
        /// </summary>
        public List<Point> Moves { get; }
        public Color StartingColor { get; }

        public enum Color
        {
            BLUE = 0,
            RED = 1
        }

        public Card(string name, List<Point> moves, Color startingColor)
        {
            this.Name = name;
            this.Moves = moves;
            this.StartingColor = startingColor;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
