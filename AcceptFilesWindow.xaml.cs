using FileTransferRequest_model;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using UserRegistrationDatabase;
using System.Windows.Controls;
using DataTables;
using System.Security.AccessControl;
using System.Security.Principal;

namespace UserRegistration
{
    public partial class AcceptFilesWindow : Window
    {
        private readonly string userEmail;
        private readonly string userPassword;
        public AcceptFilesWindow(string userEmail, string userPassword)
        {
            InitializeComponent();
            this.userEmail = userEmail;
            this.userPassword = userPassword;
            LoadFilesToAccept();
            LoadOutgoingRequests();
        }

        private void LoadFilesToAccept()
        {
            List<FileTransferRequest> filesToAccept = DatabaseHelper.GetReceivedFileTransferRequests(userEmail);
            filesToAcceptListView.ItemsSource = filesToAccept.Select(request => new
            {
                request.Id,
                request.SenderEmail,
                FileName = Path.GetFileName(request.FilePath),
                request.FilePath,
                Size = new FileInfo(request.FilePath).Length,
                IsSafe = IsFileSafe(request.FilePath),
                DateSent = File.GetLastWriteTime(request.FilePath)
            }).ToList();
        }

        private void LoadOutgoingRequests()
        {
            List<FileTransferRequest> outgoingRequests = DatabaseHelper.GetSentFileTransferRequests(userEmail);
            outgoingRequestsListView.ItemsSource = outgoingRequests.Select(request => new
            {
                request.Id,
                request.RecipientEmail,
                FileName = Path.GetFileName(request.FilePath),
                request.FilePath,
                Size = new FileInfo(request.FilePath).Length,
                DateSent = File.GetLastWriteTime(request.FilePath),
                Status = GetFileTransferStatus(request),
                IsInProcess = request.IsAccepted == null,
                IsRejected = request.IsAccepted == false
            }).ToList();
        }

        private bool IsFileSafe(string filePath)
        {
            return true;
        }

        private string GetFileTransferStatus(FileTransferRequest request)
        {
            if (request.IsAccepted == null) return "In Process";
            return request.IsAccepted == true ? "Accepted" : "Rejected";
        }

        private void AcceptFile_Click(object sender, RoutedEventArgs e)
        {
            var selectedItem = (dynamic)((Button)sender).DataContext;

            if (selectedItem != null)
            {
                string fileName = selectedItem.FileName;
                string destinationPath = Path.Combine(@"E:\C#\Secure file manager\ACTIVE USERS", userEmail, fileName);
                string sourcePath = selectedItem.FilePath;

                try
                {
                    File.Copy(sourcePath, destinationPath, overwrite: true);

                    FileSecurity fileSecurity = File.GetAccessControl(destinationPath);
                    SecurityIdentifier sid = WindowsIdentity.GetCurrent().User;
                    fileSecurity.SetOwner(sid);
                    File.SetAccessControl(destinationPath, fileSecurity);

                    MessageBox.Show("File received successfully!");
                    DatabaseHelper.DeleteFileTransferRequest(selectedItem.Id);
                    LoadFilesToAccept();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error receiving file: {ex.Message}");
                }
            }
            else
            {
                MessageBox.Show("Please select a file to accept.");
            }
        }

        private void RejectFile_Click(object sender, RoutedEventArgs e)
        {
            var selectedItem = (dynamic)((Button)sender).DataContext;

            if (selectedItem != null)
            {
                string fileName = selectedItem.FileName;
                string senderEmail = selectedItem.SenderEmail;

                DatabaseHelper.DeleteFileTransferRequest(selectedItem.Id);
                MessageBox.Show($"File '{fileName}' rejected from {senderEmail}.");
                LoadFilesToAccept();
            }
            else
            {
                MessageBox.Show("Please select a file to reject.");
            }
        }

        private void CancelFile_Click(object sender, RoutedEventArgs e)
        {
            var selectedItem = (dynamic)((Button)sender).DataContext;

            if (selectedItem != null)
            {
                DatabaseHelper.DeleteFileTransferRequest(selectedItem.Id);
                MessageBox.Show($"File transfer request to '{selectedItem.RecipientEmail}' cancelled.");
                LoadOutgoingRequests();
            }
            else
            {
                MessageBox.Show("Please select a file to cancel.");
            }
        }

        private void ResendFile_Click(object sender, RoutedEventArgs e)
        {
            var selectedItem = (dynamic)((Button)sender).DataContext;

            if (selectedItem != null)
            {
                DatabaseHelper.CreateFileTransferRequest(userEmail, selectedItem.RecipientEmail, selectedItem.FilePath);
                MessageBox.Show($"File transfer request to '{selectedItem.RecipientEmail}' resent.");
                LoadOutgoingRequests();
            }
            else
            {
                MessageBox.Show("Please select a file to resend.");
            }
        }
    }
}