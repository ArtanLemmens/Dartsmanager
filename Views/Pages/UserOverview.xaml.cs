using Dartsmanager.Models;
using Dartsmanager.Services;
using Dartsmanager.Views.Windows;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Dartsmanager.Views.Pages
{
    /// <summary>
    /// Interaction logic for UserOverview.xaml
    /// </summary>
    public partial class UserOverview : Page
    {
        private User? _actieve_gebruiker = null;
        private Frame _frame;

        public UserOverview(User? actieve_gebruiker, Frame frame)
        {
            InitializeComponent();
            _actieve_gebruiker = actieve_gebruiker;
            _frame = frame;
            GetUsers();
        }

        private void GetUsers()
        {
            var users = UserService.GetAll();
            if (users.Count > 0)
            {
                LB_Users.ItemsSource = users;
            }
        }
        private void FilterUsers()
        {
            if (LB_Users == null)
            {
                return;
            }
            string filter = TB_Gebruiker.Text;
            // Bij een lege waarde of "zoek gerbuiker" mogen al de spelers getoond worden
            if (string.IsNullOrWhiteSpace(filter) || filter == "Zoek gebruiker...")
            {
                var users = UserService.GetAll();
                if (users.Count > 0)
                {
                    LB_Users.ItemsSource = users;
                }
                return;
            }
            // filteren op de gefilterde waarde
            var gefilterde_gebruikers = UserService.GetUsersFromNameFilter(filter);
            LB_Users.ItemsSource = gefilterde_gebruikers;
            LB_Users.Items.Refresh();
        }

        private void TB_Gebruiker_GotFocus(object sender, RoutedEventArgs e)
        {
            if (TB_Gebruiker.Text == "Zoek gebruiker...")
            {
                TB_Gebruiker.Text = "";
            }
        }

        private void TB_Gebruiker_LostFocus(object sender, RoutedEventArgs e)
        {
            if (TB_Gebruiker.Text == "")
            {
                TB_Gebruiker.Text = "Zoek gebruiker...";
            }
        }

        private void TB_Gebruiker_TextChanged(object sender, TextChangedEventArgs e)
        {
            FilterUsers();
        }

        private void BT_Create_User_Click(object sender, RoutedEventArgs e)
        {
            var RegistreerScherm = new UserWindow();
            RegistreerScherm.ShowDialog();
            FilterUsers();
        }

        private void BT_View_User_Click(object sender, RoutedEventArgs e)
        {
            if (LB_Users.SelectedItem is User geselcteerde_gebruiker)
            {
                _frame.Navigate(new UserPage(_actieve_gebruiker, geselcteerde_gebruiker, _frame));
            }
            else
            {
                MessageBox.Show("Gelieve eerst een gebruiker te selecteren");
            }
        }

        private void BT_Remove_User_Click(object sender, RoutedEventArgs e)
        {
            if (LB_Users.SelectedItem is User gebruiker)
            {
                if (_actieve_gebruiker == null)
                {
                    MessageBox.Show("Login in om de gebruiker te kunnen verwijderen");
                }
                // Enkel verwijderen toelaten als de gebruiker bij de actieve gebruiker hoort of als hij een admin is
                else if (_actieve_gebruiker.IsAdmin == true || _actieve_gebruiker.Id == gebruiker.Id )
                {
                    MessageBoxResult result = MessageBox.Show($"Bent u zeker dat u deze gebruiker wenst te verwijderen:\n{gebruiker.Username}?",
                                                  "Bevestig verwijdering",
                                                  MessageBoxButton.YesNo,
                                                  MessageBoxImage.Question);
                    if (result == MessageBoxResult.Yes)
                    {
                        try
                        {
                            UserService.Remove(gebruiker);
                            FilterUsers();
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message, "Fout", MessageBoxButton.OK, MessageBoxImage.Warning);
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show("Gelieve eerst een gebruiker te selecteren");
            }
        }
    }
}
