using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace OnitamaEngine
{
    /// <summary>
    /// Interaction logic for BoardWindow.xaml
    /// </summary>
    public partial class BoardWindow : Window
    {
        List<Pawn> redPawns;
        List<Pawn> bluePawns;
        Dictionary<Button, Pawn> occupiedButtons = new Dictionary<Button, Pawn>();
        Pawn ActivePawn { get; set; }

        CurrentTurn currentTurn = 0;

        enum CurrentTurn
        {
            BLUE = 0,
            RED = 1
        };

        public BoardWindow(IList redCards, IList blueCards, object passCard)
        {
            InitializeComponent();

            // Distribute the cards
            RedCard1.Items.Add(redCards[0]);
            RedCard2.Items.Add(redCards[1]);
            BlueCard1.Items.Add(blueCards[0]);
            BlueCard2.Items.Add(blueCards[1]);
            PassCard.Items.Add(passCard);

            // Build pawn sets
            redPawns = new List<Pawn> { new Pawn(false, new Point(1,1)), new Pawn(false, new Point(1, 2)), new Pawn(true, new Point(1, 3)),
                new Pawn(false, new Point(1, 4)), new Pawn(false, new Point(1, 5)) };
            bluePawns = new List<Pawn> { new Pawn(false, new Point(5,1)), new Pawn(false, new Point(5, 2)), new Pawn(true, new Point(5, 3)),
                new Pawn(false, new Point(5, 4)), new Pawn(false, new Point(5, 5)) };

            CreateButtonMap();
            foreach (var pair in occupiedButtons)
            {
                UpdatePawnLocation(pair);
            }

                // Determine first player
                // TODO: I'm sure there is a much cleaner way to do this
                if (((Card)PassCard.Items[0]).StartingColor == Card.Color.RED)
            {
                currentTurn = CurrentTurn.RED;
            }
            else
            {
                currentTurn = CurrentTurn.BLUE;
            }

            StartGame();
        }

        /// <summary>
        /// Creates a map that details which button each pawn occupies
        /// </summary>
        private void CreateButtonMap()
        {
            // Add red pawns
            occupiedButtons.Add(b11, redPawns[0]);
            occupiedButtons.Add(b12, redPawns[1]);
            occupiedButtons.Add(b13, redPawns[2]);
            occupiedButtons.Add(b14, redPawns[3]);
            occupiedButtons.Add(b15, redPawns[4]);

            // Add blue pawns
            occupiedButtons.Add(b51, bluePawns[0]);
            occupiedButtons.Add(b52, bluePawns[1]);
            occupiedButtons.Add(b53, bluePawns[2]);
            occupiedButtons.Add(b54, bluePawns[3]);
            occupiedButtons.Add(b55, bluePawns[4]);
        }

        private void UpdatePawnLocation(KeyValuePair<Button, Pawn> pair)
        {
            Pawn pawn = pair.Value;
            Button btn = pair.Key;
            if (pawn.IsMaster)
                btn.Content = "M";
            else
                btn.Content = "o";
        }


        private void StartGame()
        {
            Console.WriteLine("In here");
            // Let use know to select a piece
            selPieceCheck.IsChecked = true;

            // Update the status message to show who's turn it is
            StatusPanel.Text = "Current turn: " + GetCurrentPlayer();
        }

        class Pawn
        {
            public bool IsMaster {get; }
            public Point Location { get; set; }
            public Pawn(bool isMaster, Point location)
            {
                IsMaster = isMaster;
                Location = location;
            }
        }

        private void HandleButtonClick(object sender, RoutedEventArgs e)
        {
            if ((bool)selPieceCheck.IsChecked)
            {
                Button btn = (Button)sender;
                ActivePawn = occupiedButtons[btn];
                string coordName = btn.Name.Substring(1);
                coordName = coordName[0] + "," + coordName[1];

                // Update the status message to pick a card
                StatusPanel.Text = GetCurrentPlayer() + " selected " + coordName + " - Pick a card";
                selCardCheck.IsChecked = true;
            }
        }

        private string GetCurrentPlayer()
        {
            return currentTurn == CurrentTurn.BLUE? "BLUE" : "RED";
        }

        private void CardSelected(object sender, RoutedEventArgs e)
        {
            if ((bool)selCardCheck.IsChecked)
            {
                // TODO: validate so that only the correct players cards can be selected

                // Update the status message to pick a target destination
                StatusPanel.Text = GetCurrentPlayer() + ": pick a target destination";
                selDestinationCheck.IsChecked = true;

                // TODO: Highlight possible destinations
                Point playerLoc = ActivePawn.Location;
                Card card = (Card)((ListBox)sender).Items[0];
                foreach (Point moveLoc in card.Moves)
                {
                    Point possibleLoc = new Point(playerLoc.X + moveLoc.Y, playerLoc.Y + moveLoc.X);
                    Console.WriteLine(playerLoc);
                    Console.WriteLine(moveLoc);
                    Console.WriteLine(possibleLoc);
                }
                
                // Should be able to get the card from the send, which can then use the selected piece (which I probably 
                // need to persist from the previous action) to figure out possible coords for the piece to move

                // TODO: Handle changing your mind, if you pick a different card then you need to clear all previous highlights
            }
        }
    }
}
