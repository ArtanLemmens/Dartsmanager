using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using Dartsmanager.Models;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using BC = BCrypt.Net.BCrypt;

namespace Dartsmanager.Views.Windows
{
    /// <summary>
    /// Interaction logic for PasswordWindow.xaml
    /// </summary>
    public partial class PasswordWindow : Window
    {
        private User? _gebruiker = null;
        public bool _CorrectWachtwoord = false;
        public PasswordWindow(User gebruiker)
        {
            InitializeComponent();
            _gebruiker = gebruiker;
            TB_Wachtwoord.Focus();
        }

        private void BT_OK_Click(object sender, RoutedEventArgs e)
        {
            if (_gebruiker != null)
            {
                string password = TB_Wachtwoord.Password;
                if (BC.Verify(password, _gebruiker.WachtwoordHash))
                {
                    _CorrectWachtwoord = true;
                    Close();
                }
                else
                {
                    MessageBox.Show("Ongeldig wachtwoord!"); return;
                }
            }
            
        }

        private void BT_Cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
