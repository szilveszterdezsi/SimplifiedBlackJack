/// ---------------------------
/// Author: Szilveszter Dezsi
/// Created: 2018-10-31
/// Modified: n/a
/// ---------------------------

using System;
using System.ComponentModel;
using System.Windows;
using Microsoft.Win32;

namespace PL
{
    /// <summary>
    /// Partial presentation class that handles File-menu events and I/O interaction with the user.
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Checks if current game session is unsaved lets user choose "Yes, No or Cancel".
        /// If user chooses "Yes" the method 'SaveCommand_Executed' is fired as if user clicked "Save" in menu before exiting.
        /// If user chooses "No" game exits without saving.
        /// If user chooses "Canel" game exit is aborted.
        /// </summary>
        /// <returns>True to exit or false to about exit.</returns>
        private bool SaveCheck()
        {
            if (!gameController.sessionSaved)
            {
                MessageBoxResult result = MessageBox.Show("Would you like to save current game first?", "Save", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    SaveCommand_Executed(null, null);
                    return true;
                }
                else if (result == MessageBoxResult.No)
                    return true;
                else if (result == MessageBoxResult.Cancel)
                    return false;
                else
                    return true;
            }
            return true;
        }

        /// <summary>
        /// Detects when "New Game" is clicked in the File-menu and performs a 'SaveCheck'.
        /// If 'SaveCheck' returns true new game dialog will open.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">RoutedEventArgs.</param>
        private void NewCommand_Executed(object sender, RoutedEventArgs e)
        {
            if (SaveCheck())
            {
                NewGameDialog();
                miSaveAs.IsEnabled = false;
            }
        }

        /// <summary>
        /// Detects when "Load Game..." is clicked in the File-menu and performs a 'SaveCheck'.
        /// If 'SaveCheck' returns true a dialog to select file opens and attempt to load data from selected file is made.
        /// If attempt fails an error info message is displayed.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">RoutedEventArgs.</param>
        private void OpenCommand_Executed(object sender, RoutedEventArgs e)
        {
            if (SaveCheck())
            {
                OpenFileDialog op = new OpenFileDialog { Title = "Load Game", Filter = "Save files (*.sav)|*.sav" };
                if (op.ShowDialog() == true)
                {
                    try
                    {
                        BindingList<string> gameUpdates = gameController.LoadGame(op.FileName);
                        Updates.Clear();
                        foreach (string s in gameUpdates)
                        {
                            Updates.Add(s);
                        }
                        SubscribeToGameUpdates();
                        AutoResizeColumns();
                        miSaveAs.IsEnabled = true;
                        MessageBox.Show("Game successfully loaded from file!", "Load Game", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message + "\n" + ex.StackTrace, ex.GetType().Name, MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }

        /// <summary>
        /// Detects when "Save Game" is clicked in the File-menu and checks if game has never been saved.
        /// If 'neverSaved' returns false the method 'SaveAsCommand_Executed' is fired as if user clicked "Save Game as..." in menu.
        /// If 'neverSaved' returns true attempt to save current game session to default save file is made.
        /// If attempt fails an error info message is displayed.
        /// </summary>
        /// <param name="sender">Component clicked.</param>
        /// <param name="e">Routed event.</param>
        private void SaveCommand_Executed(object sender, RoutedEventArgs e)
        {
            if (gameController.neverSaved)
                SaveAsCommand_Executed(sender, e);
            else
            {
                try
                {
                    gameController.SaveGame(Updates);
                    MessageBox.Show("Game successfully saved to file!", "Save", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message + "\n" + ex.StackTrace, ex.GetType().Name, MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        /// <summary>
        /// Detects when "Save Game as..." is clicked in the File-menu.
        /// A dialog to select file opens and  attempt to save current session to selected save file is made.
        /// If attempt fails an error info message is displayed.
        /// </summary>
        /// <param name="sender">Component clicked.</param>
        /// <param name="e">Routed event.</param>
        private void SaveAsCommand_Executed(object sender, RoutedEventArgs e)
        {
            SaveFileDialog op = new SaveFileDialog { Title = "Save Game...", Filter = "Save files (*.sav)|*.sav" };
            if (op.ShowDialog() == true)
            {
                try
                {
                    gameController.SaveGameAs(op.FileName, Updates);
                    miSaveAs.IsEnabled = true;
                    MessageBox.Show("Game successfully saved!", "Save Game", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message + "\n" + ex.StackTrace, ex.GetType().Name, MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        /// <summary>
        /// Detects when "Exit" is clicked in the File-menu and exits.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">RoutedEventArgs.</param>
        private void ExitCommand_Executed(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        /// <summary>
        /// Overrides and detects when any shutdown event is triggered and performs a 'SaveCheck'.
        /// If SaveCheck returns true game exits.
        /// If SaveCheck returns false exit is aborted.
        /// </summary>
        /// <param name="e">CancelEventArgs.</param>
        protected override void OnClosing(CancelEventArgs e)
        {
            if (!SaveCheck())
                e.Cancel = true;
        }
    }
}
