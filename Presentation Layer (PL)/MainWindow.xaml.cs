/// ---------------------------
/// Author: Szilveszter Dezsi
/// Created: 2018-10-31
/// Modified: n/a
/// ---------------------------

using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using BLL;

namespace PL
{
    /// <summary>
    /// Partial presentation class that initializes GUI components and opens a new game dialog.
    /// </summary>
    public partial class MainWindow : Window
    {
        private GameController gameController;
        private NewGameDialog newGameDialog;
        public BindingList<Player> Players { get { return gameController.playerList; } }
        public Player Dealer { get { return gameController.gameDealer; } }
        public Deck Deck { get { return gameController.gameDeck; } }
        public BindingList<string> Updates { get; } = new BindingList<string>();

        /// <summary>
        /// Constructor that initializes GUI componenets and controller.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
            gameController = new GameController();
            NewGameDialog();
        }

        /// <summary>
        /// Opens a new game dialog and starts a new game if dialog returns results on dialog close.
        /// </summary>
        private void NewGameDialog()
        {
            newGameDialog = new NewGameDialog();
            newGameDialog.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            newGameDialog.Topmost = true;
            newGameDialog.ShowDialog();
            if (newGameDialog.valuesSet)
            {
                gameController.InitiateGame(newGameDialog.players, newGameDialog.decks);
                Updates.Clear();
                Updates.Insert(0, "New game, start of round " + gameController.currentRound + ".");
                SubscribeToGameUpdates();
                spDealer.Visibility = Visibility.Visible;
                spDeck.Visibility = Visibility.Visible;
                gameController.StartGame();
            }
            AutoResizeColumns();
        }

        /// <summary>
        /// Forces ListView to automatically resize column widths to fit updated content.
        /// </summary>
        private void AutoResizeColumns()
        {
            GridView gridView = lvPlayers.View as GridView;
            if (gridView != null)
            {
                foreach (GridViewColumn gridViewColumn in gridView.Columns)
                {
                    if (double.IsNaN(gridViewColumn.Width))
                    {
                        gridViewColumn.Width = gridViewColumn.ActualWidth;
                    }
                    gridViewColumn.Width = double.NaN;
                }
            }
        }
    }
}
