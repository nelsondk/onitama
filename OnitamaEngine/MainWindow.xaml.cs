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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace OnitamaEngine
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        // List of cards
        // TODO: Replace with card objects
        Card[] cards =
        {
            new Card("Tiger", new List<Point> {new Point(0,2), new Point(0,-1)}, PlayerColor.BLUE),
            new Card("Crab", new List<Point> {new Point(-2,0), new Point(0,1), new Point (2,0)}, PlayerColor.BLUE),
            new Card("Monkey", new List<Point> {new Point(-1,1),new Point(-1,-1),new Point(1,1),new Point(1,-1)}, PlayerColor.BLUE),
            new Card("Crane", new List<Point> {new Point(-1,-1),new Point(0,1),new Point(1,-1)}, PlayerColor.BLUE),
            new Card("Dragon", new List<Point> {new Point(-2,1),new Point(-1,-1),new Point(2,1),new Point(1,-1)}, PlayerColor.RED),
            new Card("Elephant", new List<Point> {new Point(-1,1),new Point(-1,0),new Point(1,1),new Point(1,0)}, PlayerColor.RED),
            new Card("Mantis", new List<Point> {new Point(-1,1),new Point(0,-1),new Point(1,1)}, PlayerColor.RED),
            new Card("Boar", new List<Point> {new Point(-1,0),new Point(0,1),new Point(1,0)}, PlayerColor.RED),
            new Card("Frog", new List<Point> {new Point(-2,0),new Point(-1,1),new Point(1,-1)}, PlayerColor.BLUE),
            new Card("Goose", new List<Point> {new Point(-1,1),new Point(-1,0),new Point(1,0),new Point(1,-1)}, PlayerColor.BLUE),
            new Card("Horse", new List<Point> {new Point(-1,0),new Point(0,1),new Point(0,-1)}, PlayerColor.BLUE),
            new Card("Eel", new List<Point> {new Point(-1,1),new Point(-1,-1),new Point(1,0)}, PlayerColor.BLUE),
            new Card("Rabbit", new List<Point> {new Point(-1,-1),new Point(1,1),new Point(2,0)}, PlayerColor.RED),
            new Card("Rooster", new List<Point> {new Point(-1,-1),new Point(-1,0),new Point(1,1),new Point(1,0)}, PlayerColor.RED),
            new Card("Ox", new List<Point> {new Point(0,1),new Point(0,-1),new Point(1,0)}, PlayerColor.RED),
            new Card("Cobra", new List<Point> {new Point(-1,0),new Point(1,1),new Point(1,-1)}, PlayerColor.RED),
        };

        // Track the stage we are.  This is terrible, but quick and easy.
        State setupState = 0;
        enum State
        {
            RED_PLAYER_SELECTION = 1,
            BLUE_PLAYER_SELECTION = 2,
            PASS_CARD_SELECTION = 3,
            START_GAME_STATE = 4
        };

        public MainWindow()
        {
            InitializeComponent();
            TestGame();
            //SetupGame();
        }

        public void TestGame()
        {
            Card tiger = new Card("Tiger", new List<Point> { new Point ( 0, 2 ), new Point ( 0, -1 ) }, PlayerColor.BLUE);
            Card crab = new Card("Crab", new List<Point> { new Point ( -2, 0 ), new Point ( 0, 1 ), new Point ( 2, 0 ) }, PlayerColor.BLUE);
            Card monkey = new Card("Monkey", new List<Point> { new Point ( -1, 1 ), new Point ( -1, -1 ), new Point ( 1, 1 ), new Point ( 1, -1 ) }, PlayerColor.BLUE);
            Card crane = new Card("Crane", new List<Point> { new Point ( -1, -1 ), new Point ( 0, 1 ), new Point ( 1, -1 ) }, PlayerColor.BLUE);
            Card dragon = new Card("Dragon", new List<Point> { new Point ( -2, 1 ), new Point ( -1, -1 ), new Point ( 2, 1 ), new Point ( 1, -1 ) }, PlayerColor.RED);

            BoardWindow board = new BoardWindow(new List<Card> { tiger, crab }, new List<Card> { monkey, crane }, dragon);
            board.Show();
        }



        public void SetupGame()
        {
            // Add selectable cards
            foreach (Card card in cards)
            {
                cardListBox.Items.Add(card);
            }

            // Select red player's cards
            statusLabel.Content = "Select Red Player's Cards";
            statusLabel.Foreground = Brushes.Red;
            setupState = State.RED_PLAYER_SELECTION;
        }

        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            startButton.IsEnabled = false;
        }

        private void cardListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int selectionCount = cardListBox.SelectedItems.Count;
            if (setupState == State.PASS_CARD_SELECTION && selectionCount == 1)
            {
                // PASS CARD only allows a single value
                submitButton.IsEnabled = true;
                return;
            }
            else if (setupState == State.START_GAME_STATE)
            {
                // Don't enable once all cards have been chosen
                submitButton.IsEnabled = false;
                return;
            }
            else if (cardListBox.SelectedItems.Count == 2)
            {
                // Red/Blue selection requires two cards
                submitButton.IsEnabled = true;
                return;
            }
            submitButton.IsEnabled = false;
        }

        private void submitButton_Click(object sender, RoutedEventArgs e)
        {
            IList newCards = new ArrayList();
            foreach (Card card in cardListBox.SelectedItems)
            {
                newCards.Add(card);
            }

            if (setupState == State.RED_PLAYER_SELECTION)
            {
                foreach (Card card in newCards)
                {
                    redCardListBox.Items.Add(card);
                    cardListBox.Items.Remove(card);
                }
                statusLabel.Content = "Select Blue Player's Cards";
                statusLabel.Foreground = Brushes.Blue;
                setupState = State.BLUE_PLAYER_SELECTION;
                submitButton.IsEnabled = false;
            }
            else if (setupState == State.BLUE_PLAYER_SELECTION)
            {
                foreach (Card card in newCards)
                {
                    blueCardListBox.Items.Add(card);
                    cardListBox.Items.Remove(card);
                }
                statusLabel.Content = "Select Pass Card";
                statusLabel.Foreground = Brushes.Green;
                setupState = State.PASS_CARD_SELECTION;
                submitButton.IsEnabled = false;
            }
            else
            {
                statusLabel.Content = "LET THE GAME BEGIN!";
                statusLabel.Foreground = Brushes.Green;
                passCardListBox.Items.Add(newCards[0]);
                cardListBox.Items.Remove(newCards[0]);
                setupState = State.START_GAME_STATE;
                submitButton.IsEnabled = false;
                startButton.IsEnabled = true;
            }
        }

        private void startButton_Click(object sender, RoutedEventArgs e)
        {
            BoardWindow boardWindow = new BoardWindow(redCardListBox.Items, blueCardListBox.Items, passCardListBox.Items.GetItemAt(0));
            boardWindow.Show();
        }
    }
}
