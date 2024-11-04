using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using UserRegistrationDatabase;
using User_model;
using CurrentUser_model;
using File_model;

namespace UserRegistration
{
    public partial class AdminWindow : Window
    {
        private readonly string userEmail;

        private readonly string userPassword;


        public AdminWindow(string userEmail, string userPassword)
        {
            InitializeComponent();

            this.userEmail = userEmail;
            this.userPassword = userPassword;
            this.Loaded += AcceptFilesWindow_Loaded;
            RefreshUserData();
        }
        private void AcceptFilesWindow_Loaded(object sender, RoutedEventArgs e)
        {
            userDataGrid.Columns[0].Header = "Name";
            userDataGrid.Columns[1].Header = "Last Name";
            userDataGrid.Columns[2].Header = "Email";
            userDataGrid.Columns[3].Header = "IsAdmin";
        }
        private void CheckAdminStatus(string userEmail)
        {
            bool isAdmin = DatabaseHelper.IsCurrentUserAdmin(userEmail);
            if (!isAdmin)
            {
                MessageBox.Show("You do not have permission to access this window.");
                this.Close();
            }
        }

        private void RefreshUserData()
        {
            userDataGrid.ItemsSource = DatabaseHelper.GetAllUsers();
        }

        private void DeleteUser_Click(object sender, RoutedEventArgs e)
        {
            User_model.User selectedUser = userDataGrid.SelectedItem as User_model.User;
            if (selectedUser != null)
            {
                DatabaseHelper.DeleteUser(selectedUser.Id, selectedUser.Email);
                RefreshUserData();
            }
        }

        private void BlockUser_Click(object sender, RoutedEventArgs e)
        {
            User_model.User selectedUser = userDataGrid.SelectedItem as User_model.User;
            if (selectedUser != null)
            {
                if (dpBlockDate.SelectedDate.HasValue && TimeSpan.TryParse(txtBlockTime.Text, out TimeSpan blockTime))
                {
                    DateTime blockUntil = dpBlockDate.SelectedDate.Value.Add(blockTime);
                    if (blockUntil <= DateTime.Now)
                    {
                        MessageBox.Show("The block time must be in the future.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    TimeSpan existingBlockDuration = DatabaseHelper.GetBlockedDuration(selectedUser.Email);
                    if (existingBlockDuration == TimeSpan.Zero)
                    {
                        DatabaseHelper.BlockUser(selectedUser.Id, blockUntil - DateTime.Now);
                        RefreshUserData();
                        MessageBox.Show($"User successfully blocked until {blockUntil}.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        var result = MessageBox.Show($"User already blocked. Would you like to change the block duration?", "User Already Blocked", MessageBoxButton.YesNo, MessageBoxImage.Question);
                        if (result == MessageBoxResult.Yes)
                        {
                            DatabaseHelper.BlockUser(selectedUser.Id, blockUntil - DateTime.Now);
                            RefreshUserData();
                            MessageBox.Show($"User successfully blocked until {blockUntil}.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Invalid date or time format. Please enter a valid future date and time in correct format: hh:mm:ss.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }


        private void UnblockUser_Click(object sender, RoutedEventArgs e)
        {
            User_model.User selectedUser = userDataGrid.SelectedItem as User_model.User;
            if (selectedUser != null)
            {
                DatabaseHelper.UnblockUser(selectedUser.Id);
                RefreshUserData();
                MessageBox.Show($"User {selectedUser.Email} successfully unblocked.");
            }
        }

        private void RemoveAdminUser_Click(object sender, RoutedEventArgs e)
        {
            User_model.User selectedUser = userDataGrid.SelectedItem as User_model.User;
            if (selectedUser != null)
            {
                DatabaseHelper.SetAdminStatus(selectedUser.Id, false);
                RefreshUserData();
            }
        }


        private void AddAdminUser_Click(object sender, RoutedEventArgs e)
        {
            User_model.User selectedUser = userDataGrid.SelectedItem as User_model.User;
            if (selectedUser != null)
            {
                DatabaseHelper.SetAdminStatus(selectedUser.Id, true);
                RefreshUserData();
            }
        }

        private void OpenUserPage_Click(object sender, RoutedEventArgs e)
        {
            UserWindow userWindow = new UserWindow(userEmail, userPassword);
            userWindow.Show();
            this.Close();
        }



        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            LoginWindow loginWindow = new LoginWindow();
            loginWindow.Show();
            this.Close();
        }
    }
}
