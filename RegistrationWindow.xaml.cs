using System;
using System.Collections.Generic;
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
using System.IO;
using System.Security.Cryptography;
using System.Data.SqlClient;
using System.Xml.Linq;
using UserRegistrationDatabase;


namespace UserRegistration
{
    public partial class RegistrationWindow : Window

    {
        public RegistrationWindow()
        {

            InitializeComponent();

            try
            {
                DatabaseHelper.CreateDatabase();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error creating database: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public bool ValidateName(string name)
        {
            return !string.IsNullOrEmpty(name) && char.IsUpper(name[0]);
        }

        public bool ValidateLastName(string lastName)
        {
            return !string.IsNullOrEmpty(lastName) && char.IsUpper(lastName[0]);
        }

        public bool ValidateEmail(string email)
        {
            return !string.IsNullOrEmpty(email) && email.Contains("@");
        }
        private void TxtName_TextChanged(object sender, TextChangedEventArgs e)
        {
            lblError.Content = ValidateName(txtName.Text.Trim()) ? "" : "Name must start with a capital letter.";
        }

        private void TxtLastName_TextChanged(object sender, TextChangedEventArgs e)
        {
            lblError.Content = ValidateLastName(txtLastName.Text.Trim()) ? "" : "Last name must start with a capital letter.";
        }

        private void TxtEmail_TextChanged(object sender, TextChangedEventArgs e)
        {
            lblError.Content = ValidateEmail(txtEmail.Text.Trim()) ? "" : "Email is invalid.";
        }

        private void TxtPassword_PasswordChanged(object sender, RoutedEventArgs e)
        {
            string password = txtPassword.Password;
            string confirmPassword = txtConfirmPassword.Password;

            if (string.IsNullOrEmpty(password) || string.IsNullOrEmpty(confirmPassword))
            {
                lblError.Content = "Password fields are required.";
                return;
            }

            if (password != confirmPassword)
            {
                lblError.Content = "Passwords do not match.";
                return;
            }

            if (password.Length < 12 ||
                !password.Any(char.IsDigit) ||
                !password.Any(char.IsLetter) ||
                !password.Any(char.IsUpper) ||
                !password.Any(char.IsLower) ||
                password.All(c => !char.IsLetterOrDigit(c) && !char.IsSymbol(c)))
            {
                lblError.Content = "Password must be at least 12 characters long and contain digits, letters, symbols, uppercase and lowercase characters.";
                return;
            }

            lblError.Content = "";
        }

        private void TxtConfirmPassword_PasswordChanged(object sender, RoutedEventArgs e)
        {
            TxtPassword_PasswordChanged(sender, e);
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

        private void BtnShowConfirmPassword_Click(object sender, RoutedEventArgs e)
        {
            if (btnShowConfirmPassword.IsChecked == true)
            {
                txtVisibleConfirmPassword.Text = txtConfirmPassword.Password;
                txtConfirmPassword.Visibility = Visibility.Collapsed;
                txtVisibleConfirmPassword.Visibility = Visibility.Visible;
            }
            else
            {
                txtConfirmPassword.Visibility = Visibility.Visible;
                txtVisibleConfirmPassword.Visibility = Visibility.Collapsed;
            }
        }

        public void Login_Click(object sender, RoutedEventArgs e)
        {
            LoginWindow loginWindow = new LoginWindow();
            loginWindow.Show();
            this.Close();
        }

        public void Register_Click(object sender, RoutedEventArgs e)
        {
            string name = txtName.Text.Trim();
            string lastName = txtLastName.Text.Trim();
            string email = txtEmail.Text.Trim();
            string password = txtPassword.Password;
            string confirmPassword = txtConfirmPassword.Password;

            if (!ValidateName(name))
            {
                lblError.Content = "Name must start with a capital letter.";
                return;
            }

            if (!ValidateLastName(lastName))
            {
                lblError.Content = "Last name must start with a capital letter.";
                return;
            }

            if (!ValidateEmail(email))
            {
                lblError.Content = "Email is invalid.";
                return;
            }

            if (string.IsNullOrEmpty(password) || string.IsNullOrEmpty(confirmPassword))
            {
                lblError.Content = "Password fields are required.";
                return;
            }

            if (password != confirmPassword)
            {
                lblError.Content = "Passwords do not match.";
                return;
            }

            if (password.Length < 12 ||
                !password.Any(char.IsDigit) ||
                !password.Any(char.IsLetter) ||
                !password.Any(char.IsUpper) ||
                !password.Any(char.IsLower) ||
                password.All(c => !char.IsLetterOrDigit(c) && !char.IsSymbol(c)))
            {
                lblError.Content = "Password must be at least 12 characters long and contain digits, letters, symbols, uppercase and lowercase characters.";
                return;
            }

            try
            {
                if (DatabaseHelper.UserExists(email))
                {
                    lblError.Content = "User with this email already exists.";
                    return;
                }

                string hashedPassword = HashPassword(password);
                DatabaseHelper.CreateUser(name, lastName, email, hashedPassword);

                MessageBox.Show("User registered successfully!");
                ClearFields();

                UserWindow userWindow = new UserWindow(email, password);
                userWindow.Show();
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error registering user: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ClearFields()
        {
            txtName.Text = "";
            txtLastName.Text = "";
            txtEmail.Text = "";
            txtPassword.Password = "";
            txtConfirmPassword.Password = "";
            lblError.Content = "";
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
