using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace OnitamaEngine
{
    public class Pawn
    {
        public bool IsMaster { get; }
        public Button Location { get; set; }
        public PlayerColor Team { get; }
        public Pawn(Button location, PlayerColor team) : this(false, location, team) { }
        public Pawn(bool isMaster, Button location, PlayerColor team)
        {
            IsMaster = isMaster;
            Location = location;
            Team = team;
        }
    }
}
