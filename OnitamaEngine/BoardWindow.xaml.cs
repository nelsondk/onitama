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
        List<Pawn> redPawns, bluePawns;
        List<Button> highLightedButtons;

        // How deep does the rabbit hole go?
        int depth = 0;

        /// <summary>
        /// A mapping that details which button has an occupying pawn.
        /// 
        /// TODO: Allows faster querying of if a button is occupied then checking each pawns location, but requires updating location 
        /// both on a button level and for each pawn.  Should be a less brittle way of doing this.
        /// </summary>
        Dictionary<Button, Pawn> occupiedBtnDict = new Dictionary<Button, Pawn>();

        /// <summary>
        /// Mapping that allows getting the expected button by passing in it's Cartesian coordinates
        /// </summary>
        Dictionary<Point, Button> pointButtonDict = new Dictionary<Point, Button>();

        Pawn ActivePawn { get; set; }
        Card ActiveCard { get; set; }

        PlayerColor currentTurn = 0;

        /*************************************** INITIALIZATION METHODS ***************************************/
        public BoardWindow(IList redCards, IList blueCards, object passCard)
        {
            InitializeComponent();

            // Distribute the cards
            // TODO: Use something besides ListBoxes for these UI widgets
            RedCard1.Items.Add(redCards[0]);
            RedCard2.Items.Add(redCards[1]);
            BlueCard1.Items.Add(blueCards[0]);
            BlueCard2.Items.Add(blueCards[1]);
            PassCard.Items.Add(passCard);

            // Create a point-to-button dict for easy reference
            CreatePointButtonDict();

            // Set up the pieces in their star positions
            InitializeBoard();

            // Determine first player
            currentTurn = ((Card)PassCard.Items[0]).StartingColor;

            // Begin the actual game
            StartTurn();
        }

        /// <summary>
        /// Creates Point to Button mapping, allowing me to get the expected button by passing in it's Cartesian coordinates
        /// </summary>
        private void CreatePointButtonDict()
        {
            foreach (object child in BoardGrid.Children)
            {
                if (!(child is Button))
                    continue;

                Button button = (Button) child;
                pointButtonDict.Add(ConvertButtonToPoint((Button)child), button);
            }
        }

        /// <summary>
        /// Create a 4 pawns and 1 master for each team and assigns them a button as a location
        /// Update the occupiedBtnDict to include a key/value reference of occupiedBtn/Pawn
        /// Update the associated buttons content to reflect the occupying pawn type.
        /// </summary>
        private void InitializeBoard()
        {
            // Create red pawns
            redPawns = new List<Pawn>();
            for (int i = 1; i < 6; i++)
            {
                Button button = pointButtonDict[new Point(i, 1)];
                Pawn pawn = null;
                if (i == 3)
                {
                    pawn = new Pawn(true, button, PlayerColor.RED);
                    redPawns.Add(pawn);
                }
                else
                {
                    pawn = new Pawn(button, PlayerColor.RED);
                    redPawns.Add(pawn);
                }
                UpdatePawnLocation(button, pawn);
            }

            // Create blue pawns
            bluePawns = new List<Pawn>();
            for (int i = 1; i < 6; i++)
            {
                Button button = pointButtonDict[new Point(i, 5)];
                Pawn pawn = null;
                if (i == 3)
                {
                    pawn = new Pawn(true, button, PlayerColor.BLUE);
                    bluePawns.Add(pawn);
                }
                else
                {
                    pawn = new Pawn(button, PlayerColor.BLUE);
                    bluePawns.Add(pawn);
                }
                UpdatePawnLocation(button, pawn);
            }
        }


        /*************************************** GAME PLAY METHODS ***************************************/
        /// <summary>
        /// Begins the game, lets the correct playes know to select a piece
        /// </summary>
        private void StartTurn()
        {
            // Let use know to select a piece
            selPieceCheck.IsChecked = true;

            // Update the status message to show who's turn it is
            StatusPanel.Text = "Current turn: " + GetCurrentPlayerText() + " - Pick a pawn";
        }

        /// <summary>
        /// Depending on stage:
        ///   If "select piece" is checked, then marks the active pawn and moves to card selection phase
        ///   If "select destination" is checked, then updates the pawn location, capturing pieces as necessary
        ///   Otherwise no effect
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HandleButtonClick(object sender, RoutedEventArgs e)
        {
            // TODO: should i have to cast to bool here?
            if ((bool)selPieceCheck.IsChecked)
            {
                Button btn = (Button)sender;

                // Make sure pawn selection is for the current team
                if (!occupiedBtnDict.ContainsKey(btn) || currentTurn != occupiedBtnDict[btn].Team)
                {
                    return;
                }

                ActivePawn = occupiedBtnDict[btn];
                string coordName = btn.Name.Substring(1);
                coordName = coordName[0] + "," + coordName[1];

                // Update the status message to pick a card
                StatusPanel.Text = GetCurrentPlayerText() + " selected " + coordName + " - Pick a card";
                selCardCheck.IsChecked = true;
            }

            if ((bool)selDestinationCheck.IsChecked)
            {
                Button btn = (Button)sender;

                // ignore buttons that are not possible moves
                if (!highLightedButtons.Contains(btn))
                    return;

                bool victoryAchieved = UpdatePawnLocation(btn, ActivePawn);

                // TODO: Victory dialog and end game
                if (victoryAchieved)
                    StatusPanel.Text = GetCurrentPlayerText() + " has triumphed!";
                else
                    NextTurn();
            }
        }

        private void CardSelected(object sender, RoutedEventArgs e)
        {
            if ((bool)selCardCheck.IsChecked)
            {
                // TODO: validate so that only the correct players cards can be selected
                ActiveCard = (Card)((ListBox)sender).Items[0];
                if (currentTurn == PlayerColor.RED)
                {
                    if (ActiveCard != RedCard1.Items[0] && ActiveCard != RedCard2.Items[0])
                        return;
                }
                else
                {
                    if (ActiveCard != BlueCard1.Items[0] && ActiveCard != BlueCard2.Items[0])
                        return;
                }

                // Update the status message to pick a target destination
                StatusPanel.Text = GetCurrentPlayerText() + ": pick a target destination";
                selDestinationCheck.IsChecked = true;

                // Highlight legal moves for the active pawn/card combo
                highLightedButtons = GetPossibleMoves(ActivePawn, ActiveCard);
                foreach (Button button in highLightedButtons)
                    button.Background = Brushes.LightGreen;

                // TODO: Handle changing your mind, if you pick a different card then you need to clear all previous highlights
            }
        }

        private void NextTurn()
        {
            // Clear all highlighted buttons
            foreach (Button button in highLightedButtons)
            {
                button.Background = new SolidColorBrush(Color.FromRgb(221, 221, 221));
            }

            // Rotate cards
            Card tmpCard = (Card)PassCard.Items[0];
            PassCard.Items[0] = ActiveCard;
            if (RedCard1.Items[0] == ActiveCard)
            {
                RedCard1.Items[0] = tmpCard;
            }
            else if (RedCard2.Items[0] == ActiveCard)
            {
                RedCard2.Items[0] = tmpCard;
            }
            else if (BlueCard1.Items[0] == ActiveCard)
            {
                BlueCard1.Items[0] = tmpCard;
            }
            else if (BlueCard2.Items[0] == ActiveCard)
            {
                BlueCard2.Items[0] = tmpCard;
            }

            // Clear all global fields
            ActivePawn = null;
            ActiveCard = null;

            // Update currentTurn field
            currentTurn = currentTurn == PlayerColor.RED ? PlayerColor.BLUE : PlayerColor.RED;

            // Start pawn selection phase
            StartTurn();
        }


        /*************************************** UTILITY METHODS ***************************************/
        /// <summary>
        /// Update the Location property of a pawn to the passed in Button
        /// </summary>
        /// <param name="button"></param>
        /// <param name="pawn"></param>
        /// <returns>victoryAchieved</returns>
        private bool UpdatePawnLocation(Button button, Pawn pawn)
        {
            bool victoryAchieved = false;
            
            // Check for captured pawns
            // TODO: candidate for breakout method
            if (occupiedBtnDict.ContainsKey(button))
            {
                // Captured a pawn!
                Pawn capturedPawn = occupiedBtnDict[button];

                // Remove pawn from the occupied button map, as it is now dead
                occupiedBtnDict.Remove(capturedPawn.Location);

                // Remove the pawn from the appropriate teams available pawn set
                if (capturedPawn.Team == PlayerColor.RED)
                    redPawns.Remove(capturedPawn);
                else
                    bluePawns.Remove(capturedPawn);

                // Check if the Master was killed and if the game is now over
                if (capturedPawn.IsMaster)
                    victoryAchieved = true;
            }

            // TODO: Victory: Player Master takes Opponent Throne?

            // Update location of the moving pawn
            // TODO: candidate for breakout method
            // * Remove old location from occupied button map and clear old buttons content
            // * Update Pawns Location property
            // * Add new location to the occupied button map
            occupiedBtnDict.Remove(pawn.Location);
            pawn.Location.Content = "";
            pawn.Location = button;
            occupiedBtnDict.Add(button, pawn);

            // Update new button's foreground color & content
            button.Foreground = (pawn.Team == PlayerColor.RED) ? Brushes.Red : Brushes.Blue;
            if (pawn.IsMaster)
                button.Content = "M";
            else
                button.Content = "o";

            return victoryAchieved;
        }

        /// <summary>
        /// For the provided pawn and card, return all legal destination buttons that can be moved to
        /// </summary>
        /// <param name="pawn">The pawn that is being moved</param>
        /// <param name="card">That card that is providing the possible moves</param>
        /// <param name="team">The owner of the pawn</param>
        /// <returns></returns>
        private List<Button> GetPossibleMoves(Pawn pawn, Card card)
        {
            List<Point> possibleMoves = new List<Point>();
            foreach (Point moveLoc in card.Moves)
            {
                // Invert move locations for Blue player
                int mod = pawn.Team == PlayerColor.RED ? 1 : -1;

                Point activePawnLoc = ConvertButtonToPoint(pawn.Location);
                int X = (int)(activePawnLoc.X + (mod * moveLoc.X));
                int Y = (int)(activePawnLoc.Y + (mod * moveLoc.Y));

                // Ignore moves that go outside the board
                if (X <= 0 || X > 5 || Y <= 0 || Y > 5)
                    continue;

                possibleMoves.Add(new Point(X, Y));
            }

            List<Button> possibleTargetButtons = new List<Button>();
            foreach (Point point in possibleMoves)
            {
                Button button = pointButtonDict[point];
                if (occupiedBtnDict.ContainsKey(button))
                {
                    Pawn targetPawn = occupiedBtnDict[button];
                    if (targetPawn.Team == pawn.Team)
                    {
                        // Can't target your own team
                        continue;
                    }
                }
                possibleTargetButtons.Add(button);
            }
            return possibleTargetButtons;
        }

        private string GetCurrentPlayerText()
        {
            return currentTurn == PlayerColor.RED ? "RED" : "BLUE";
        }

        private Point ConvertButtonToPoint(Button button)
        {
            return new Point(char.GetNumericValue(button.Name[1]), char.GetNumericValue(button.Name[2]));
        }

        /*************************************** ENGINE METHODS ***************************************/
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            /*
            Dictionary<Button, Pawn> dictCopy = new Dictionary<Button, Pawn>();
            foreach (KeyValuePair<Button, Pawn> entry in occupiedBtnDict)
            {
                dictCopy.Add(entry.Key, entry.Value);
            }
            */
            List<Pawn> redPawnCopy = new List<Pawn>();
            foreach (Pawn pawn in redPawns)
            {
                redPawnCopy.Add(pawn);
            }
            List<Pawn> bluePawnCopy = new List<Pawn>();
            foreach (Pawn pawn in bluePawns)
            {
                bluePawnCopy.Add(pawn);
            }

            GetBestMove(currentTurn);

            depth = 0;

            foreach (Pawn pawn in redPawnCopy)
                UpdatePawnLocation(pawn.Location, pawn);

            foreach (Pawn pawn in bluePawnCopy)
                UpdatePawnLocation(pawn.Location, pawn);

            redPawns = redPawnCopy;
            bluePawns = bluePawnCopy;

        }

        int MAX_DEPTH = 1;

        private int GetBestMove(PlayerColor team)
        {
            //Console.WriteLine("In GetBestMove");


            // TODO: randomize start order in pawn Arrays to make it so the first moves are not predictable 
            List<Pawn> pawnList = null;
            List<Card> cards = new List<Card>();
            if (team == PlayerColor.RED)
            {
                pawnList = redPawns;
                cards.Add((Card)RedCard1.Items[0]);
                cards.Add((Card)RedCard2.Items[0]);
            }
            else
            {
                pawnList = bluePawns;
                cards.Add((Card)BlueCard1.Items[0]);
                cards.Add((Card)BlueCard2.Items[0]);
            }

            // int=moveValue, button1=origLocation, card=Card, button2=targetLocation
            Tuple<int, Button, Card, Button> bestMove = new Tuple<int, Button, Card, Button>(-9999, null, null, null);
            foreach (Pawn pawn in pawnList)
            {
                foreach (Card card in cards) {
                    // Console.WriteLine("Calc for pawn on " + pawn.Location.Name + " using " + card);
                    // Try with first card
                    List<Button> moves = GetPossibleMoves(pawn, card);
                    foreach (Button move in moves)
                    {
                        int moveValue = CalculateMoveValue(team, move);

                        // Avoid infinite recursion
                        if (!(depth >= MAX_DEPTH))
                        {
                            depth++;

                            // Store original location
                            Button oldLocation = pawn.Location;

                            // Temporarily move pawn so it can be used in further calculations
                            UpdatePawnLocation(move, pawn);

                            // Check the next players best move
                            int opponentMoveValue = GetBestMove(team == PlayerColor.RED ? PlayerColor.BLUE : PlayerColor.RED);
                            //Console.WriteLine("Op val: " + opponentMoveValue);
                            moveValue -= opponentMoveValue;

                            // Reset to old location
                            UpdatePawnLocation(oldLocation, pawn);
                        }

                        // TODO: 
                        // * temporarily make the move without updating the UI
                        // * for each pawn in blue
                        //   * get all possible moves
                        //   * calculate the value of each move and return the hights value
                        //   * subtract blues best move value from this moves value
                        //   * store THAT number in bestMove

                        // get all values for other player and check the difference

                        // roll back the move

                        if (moveValue >= bestMove.Item1)
                        {
                            bestMove = new Tuple<int, Button, Card, Button>(moveValue, pawn.Location, card, move);
                        }
                        // Console.WriteLine(" - Move: " + move.Name);
                        // Console.WriteLine(" - Value: " + moveValue);
                    }
                }
            }
            

            if (depth == 0)
                StatusPanel.Text = "Best Move: (" + bestMove.Item1 + ") Move from " + bestMove.Item2.Name + " to " + bestMove.Item4.Name + " using " + bestMove.Item3.Name;
            depth--;
            return bestMove.Item1;

            // Calculate all available moves for all available pawns, and all available opponent moves for each move

            // Pick the move with the highest combined point value of playerMove - opponentsBestMove
        }

        private int CalculateMoveValue(PlayerColor team, Button moveLocation)
        {
            int moveValue = 0;

            // First get value based on location
            int X = (int)char.GetNumericValue(moveLocation.Name[1]);
            switch(X)
            {
                case 1:
                case 5:
                    moveValue += 5;
                    break;
                case 2:
                case 4:
                    moveValue += 15;
                    break;
                case 3:
                    moveValue += 25;
                    break;
            }

            // invert Y values for bluePlayer
            int[] yVals = team == PlayerColor.RED ? new int[]{ 0, 5, 10, 25, 15 } : new int[]{ 15, 25, 10, 5, 0 };
            int Y = (int)char.GetNumericValue(moveLocation.Name[2]);
            switch(Y)
            {
                case 1:
                    moveValue += yVals[0];
                    break;
                case 2:
                    moveValue += yVals[1];
                    break;
                case 3:
                    moveValue += yVals[2];
                    break;
                case 4:
                    moveValue += yVals[3];
                    break;
                case 5:
                    moveValue += yVals[4];
                    break;
            }

            // Next get value based on captured pawns
            if (occupiedBtnDict.ContainsKey(moveLocation))
            {
                Pawn targetPawn = occupiedBtnDict[moveLocation];
                if (targetPawn.IsMaster)
                    moveValue += 1000;
                else
                    moveValue += 200;
            }

            return moveValue;
        }
    }
}
