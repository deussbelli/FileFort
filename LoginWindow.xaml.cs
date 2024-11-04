using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Windows;
using UserRegistrationDatabase;
using System.Security.Principal;

namespace UserRegistration
{
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
        }

        private void Login_Click(object sender, RoutedEventArgs e)
        {
            string email = txtEmail.Text.Trim();
            string password = txtPassword.Password;

            string hashedPassword = HashPassword(password);

            try
            {
                string currentUser = WindowsIdentity.GetCurrent().Name;

                //  MessageBox.Show($"Login successful! Current user: {currentUser}");

                if (!DatabaseHelper.UserExists(email))
                {
                    lblError.Content = "User with this email does not exist.";
                    return;
                }

                string storedHashedPassword = DatabaseHelper.GetHashedPassword(email);

                if (hashedPassword != storedHashedPassword)
                {
                    lblError.Content = "Incorrect password.";
                    return;
                }

                TimeSpan blockDuration = DatabaseHelper.GetBlockedDuration(email);
                if (blockDuration.TotalMilliseconds > 0)
                {
                    MessageBox.Show($"You are blocked for {blockDuration.TotalMinutes} minutes.", "Blocked", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    return;
                }

                bool isAdmin = DatabaseHelper.IsCurrentUserAdmin(email);
                if (isAdmin)
                {
                    MessageBox.Show("Login successful!");
                    AdminWindow adminWindow = new AdminWindow(email, password);
                    adminWindow.Show();
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Login successful!");
                    UserWindow userWindow = new UserWindow(email, password);
                    userWindow.Show();
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error authenticating user: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private void BtnShowPassword_Click(object sender, RoutedEventArgs e)
        {
            if (btnShowPassword.IsChecked == true)
            {
                txtVisiblePassword.Text = txtPassword.Password;
                txtPassword.Visibility = Visibility.Collapsed;
                txtVisiblePassword.Visibility = Visibility.Visible;
            }
            else
            {
                txtPassword.Visibility = Visibility.Visible;
                txtVisiblePassword.Visibility = Visibility.Collapsed;
            }
        }

        private void Register_Click(object sender, RoutedEventArgs e)
        {
            RegistrationWindow registrationWindow = new RegistrationWindow();
            registrationWindow.Show();
            this.Close();
        }

        public string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                StringBuilder builder = new StringBuilder();
                foreach (byte b in bytes)
                {
                    builder.Append(b.ToString("x2"));
                }
                return builder.ToString();
            }
        }
    }
}
