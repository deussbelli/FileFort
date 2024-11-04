using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using User_model;
using UserRegistrationDatabase;
using System.Security.Cryptography;
using System.Text;
using File_model;
using UserRegistration;
using System.Windows.Input;
using FileTransferRequest_model;


namespace UserRegistration
{
    public partial class UserWindow : Window
    {
        private readonly string userEmail;
        private List<FileItem> userFilesAndFolders;
        private readonly byte[] key;
        private readonly byte[] userPasswordHash;
        private readonly string userPassword;

        public UserWindow(string userEmail, string userPassword)
        {
            InitializeComponent();
            this.userEmail = userEmail;
            this.userPassword = userPassword;
            using (SHA256 sha256 = SHA256.Create())
            {
                userPasswordHash = sha256.ComputeHash(Encoding.UTF8.GetBytes(userPassword));
            }

            key = userPasswordHash.Take(32).ToArray();

            User currentUser = DatabaseHelper.GetUserByEmail(userEmail);
            if (currentUser != null && currentUser.IsAdmin)
            {
                adminPageButton.Visibility = Visibility.Visible;
            }
            else
            {
                adminPageButton.Visibility = Visibility.Collapsed;
            }

            LoadUserFilesAndFolders();

            RefreshCounts();
        }

        private void AdminPageButton_Click(object sender, RoutedEventArgs e)
        {
            AdminWindow adminWindow = new AdminWindow(userEmail, userPassword);
            adminWindow.Show();
            this.Close();
        }


        private string GetFileIcon(string fileName)
        {
            string extension = Path.GetExtension(fileName).ToLower();
            string iconFileName = "";

            if (!Path.GetExtension(fileName).ToLower().Contains("."))
            {
                return iconFileName = @"E:\C#\Secure file manager\ICONS\icons8-папка-48.png";
            }
            else
            {


                switch (extension)
                {
                    case ".txt":
                        iconFileName = "icons8-документ-txt-48.png";
                        break;
                    case ".pdf":
                        iconFileName = "icons8-pdf-48.png";
                        break;
                    case ".doc":
                    case ".docx":
                        iconFileName = "icons8-документ-word-48.png";
                        break;
                    case ".zip":
                        iconFileName = "icons8-архив-48.png";
                        break;
                    case ".csv":
                        iconFileName = "icons8-csv-48.png";
                        break;
                    case ".db":
                        iconFileName = "icons8-db-2-48.png";
                        break;
                    case ".exe":
                        iconFileName = "icons8-exe-48.png";
                        break;
                    case ".gif":
                        iconFileName = "icons8-gif-48.png";
                        break;
                    case ".jpg":
                        iconFileName = "icons8-jpg-48.png";
                        break;
                    case ".mpg":
                        iconFileName = "icons8-mpg-48.png";
                        break;
                    case ".png":
                        iconFileName = "icons8-png-48.png";
                        break;
                    case ".psd":
                        iconFileName = "icons8-psd-48.png";
                        break;
                    case ".py":
                        iconFileName = "icons8-py-48.png";
                        break;
                    case ".wsh":
                        iconFileName = "icons8-wsh-48.png";
                        break;
                    case ".xaml":
                        iconFileName = "icons8-xaml-48.png";
                        break;
                    case ".xsl":
                        iconFileName = "icons8-xsl-48.png";
                        break;
                    default:
                        iconFileName = "uncnown_file.png";
                        break;
                }

                string iconPath = Path.Combine(@"E:\C#\Secure file manager\ICONS", iconFileName);
                return iconPath;
            }
        }

        private void LoadUserFilesAndFolders()
        {
            User currentUser = DatabaseHelper.GetUserByEmail(userEmail);
            string userDirectory = Path.Combine(@"E:\C#\Secure file manager\ACTIVE USERS", currentUser.Email);

            if (!Directory.Exists(userDirectory))
            {
                Directory.CreateDirectory(userDirectory);
            }

            userFilesAndFolders = new List<FileItem>();
            DirectoryInfo directoryInfo = new DirectoryInfo(userDirectory);
            foreach (var item in directoryInfo.GetFileSystemInfos())
            {
                string type = (item.Attributes & FileAttributes.Directory) == FileAttributes.Directory ? "Folder" : "File";
                string icon = type == "File" ? GetFileIcon(item.FullName) : @"E:\C#\Secure file manager\ICONS\icons8-папка-48.png";
                long size = type == "File" ? new FileInfo(item.FullName).Length : 0;
                userFilesAndFolders.Add(new FileItem { Name = item.Name, Type = type, Icon = icon, Size = size });
            }

            fileListView.ItemsSource = userFilesAndFolders;
        }


        private bool AskForEncryption()
        {
            var result = MessageBox.Show("Would you like to encrypt this file?", "Encryption Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);
            return result == MessageBoxResult.Yes;
        }
        private void CreateFile_Click(object sender, RoutedEventArgs e)
        {
            string fileName = Microsoft.VisualBasic.Interaction.InputBox("Enter the file name:", "Create File", "");

            if (!string.IsNullOrWhiteSpace(fileName))
            {
                if (Path.GetExtension(fileName) == string.Empty)
                {
                    MessageBox.Show("Please enter a file name with an extension.");
                    return;
                }

                try
                {
                    string filePath = Path.Combine(@"E:\C#\Secure file manager\ACTIVE USERS", userEmail, fileName);

                    if (fileName.Contains("_UNRELIABLE"))
                    {
                        File.WriteAllText(filePath, "");
                    }
                    else
                    {
                        if (AskForEncryption())
                        {
                            string encryptedFileName = Path.GetFileNameWithoutExtension(fileName) + "_ENCRYPTED" + Path.GetExtension(fileName);
                            filePath = Path.Combine(Path.GetDirectoryName(filePath), encryptedFileName);

                            File.WriteAllText(filePath, "");
                        }
                        else
                        {
                            string unencryptedFileName = Path.GetFileNameWithoutExtension(fileName) + "_UNRELIABLE" + Path.GetExtension(fileName);
                            filePath = Path.Combine(Path.GetDirectoryName(filePath), unencryptedFileName);

                            File.WriteAllText(filePath, "");
                        }
                    }

                    MessageBox.Show("File created successfully!");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error creating file: {ex.Message}");
                }
            }
            RefreshListView();
        }

        private void CreateFolder_Click(object sender, RoutedEventArgs e)
        {
            string folderName = Microsoft.VisualBasic.Interaction.InputBox("Enter the folder name:", "Create Folder", "");

            if (!string.IsNullOrWhiteSpace(folderName))
            {
                try
                {
                    string folderPath = Path.Combine(@"E:\C#\Secure file manager\ACTIVE USERS", userEmail, folderName);
                    Directory.CreateDirectory(folderPath);

                    DirectoryInfo directoryInfo = new DirectoryInfo(folderPath);

                    MessageBox.Show("Folder created successfully!");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error creating folder: {ex.Message}");
                }
            }
            RefreshListView();
        }

        private void Edit_Click(object sender, RoutedEventArgs e)
        {

            FileItem selectedItem = (FileItem)fileListView.SelectedItem;
            if (selectedItem != null && selectedItem.Type == "File")
            {
                string filePath = Path.Combine(@"E:\C#\Secure file manager\ACTIVE USERS", userEmail, selectedItem.Name);

                if (File.Exists(filePath))
                {
                    try
                    {
                        string extension = Path.GetExtension(selectedItem.Name).ToLower();

                        if (extension == ".txt")
                        {
                            LobbyWindow lobbyWindow = new LobbyWindow(filePath, key, this);

                            lobbyWindow.ShowDialog();
                        }
                        else
                        {
                            switch (extension)
                            {
                                case ".pdf":
                                    System.Diagnostics.Process.Start("WINWORD.exe", filePath);
                                    break;
                                case ".doc":
                                case ".docx":
                                    System.Diagnostics.Process.Start("WINWORD.EXE", filePath);
                                    break;
                                case ".ppt":
                                case ".pptx":
                                    System.Diagnostics.Process.Start("POWERPNT.EXE", filePath);
                                    break;
                                case ".xls":
                                case ".xlsx":
                                    System.Diagnostics.Process.Start("EXCEL.EXE", filePath);
                                    break;
                                case ".jpg":
                                case ".png":
                                case ".gif":
                                    System.Diagnostics.Process.Start("D:\\уроки\\рабочий стол\\sai\\Paint-Tool-SAI 2.0\\Paint Tool SAI 2.0 (64bit)\\sai2.exe", filePath);
                                    break;
                                default:
                                    System.Diagnostics.Process.Start(filePath);
                                    break;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error editing file: {ex.Message}");
                    }
                }
                else
                {
                    MessageBox.Show("The selected file does not exist.");
                }
            }
            else
            {
                MessageBox.Show("Please select a file to edit.");
            }
            RefreshListView();
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            FileItem selectedItem = (FileItem)fileListView.SelectedItem;
            if (selectedItem != null)
            {
                try
                {
                    string path = Path.Combine(@"E:\C#\Secure file manager\ACTIVE USERS", userEmail, selectedItem.Name);

                    if (selectedItem.Type == "File")
                    {
                        File.Delete(path);
                    }
                    else
                    {
                        Directory.Delete(path, true);
                    }

                    MessageBox.Show("Item deleted successfully!");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error deleting item: {ex.Message}");
                }

                LoadUserFilesAndFolders();
            }
            else
            {
                MessageBox.Show("Please select an item to delete.");
            }
            RefreshListView();
        }

        private void Move_Click(object sender, RoutedEventArgs e)
        {
            FileItem selectedItem = (FileItem)fileListView.SelectedItem;
            if (selectedItem != null)
            {
                try
                {
                    string newPath = Microsoft.VisualBasic.Interaction.InputBox("Enter the new path:", "Move Item", "");

                    if (string.IsNullOrWhiteSpace(newPath))
                    {
                        MessageBox.Show("Please enter a valid path.");
                        return;
                    }

                    if (!Directory.Exists(newPath))
                    {
                        MessageBox.Show("The specified path does not exist.");
                        return;
                    }

                    string currentItemPath = Path.Combine(@"E:\C#\Secure file manager\ACTIVE USERS", userEmail, selectedItem.Name);

                    if (selectedItem.Type == "File")
                    {
                        File.Move(currentItemPath, Path.Combine(newPath, selectedItem.Name));
                    }
                    else
                    {
                        Directory.Move(currentItemPath, Path.Combine(newPath, selectedItem.Name));
                    }

                    MessageBox.Show("Item moved successfully!");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error moving item: {ex.Message}");
                }

                RefreshListView();
            }
            else
            {
                MessageBox.Show("Please select an item to move.");
            }
        }

        private void Open_Click(object sender, RoutedEventArgs e)
        {
            RefreshListView();
        }

        private void OpenFolder_Click(object sender, RoutedEventArgs e)
        {
            FileItem selectedItem = (FileItem)fileListView.SelectedItem;
            if (selectedItem != null && selectedItem.Type == "Folder")
            {
                string folderPath = Path.Combine(@"E:\C#\Secure file manager\ACTIVE USERS", userEmail, selectedItem.Name);

                if (Directory.Exists(folderPath))
                {
                    try
                    {
                        System.Diagnostics.Process.Start("explorer.exe", folderPath);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error opening folder: {ex.Message}");
                    }
                }
                else
                {
                    MessageBox.Show("The selected folder does not exist.");
                }
            }
            else
            {
                MessageBox.Show("Please select a folder to open.");
            }
        }

        private void RefreshListView()
        {
            userFilesAndFolders.Clear();
            LoadUserFilesAndFolders();
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            LoginWindow loginWindow = new LoginWindow();
            loginWindow.Show();
            this.Close();
        }
        public byte[] EncryptString(string plainText)
        {
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = key;
                aesAlg.GenerateIV();

                byte[] salt = key.Take(16).ToArray();

                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(key, salt, 10000);
                aesAlg.Key = pdb.GetBytes(32);
                aesAlg.IV = pdb.GetBytes(16);

                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    msEncrypt.Write(BitConverter.GetBytes(aesAlg.IV.Length), 0, sizeof(int));
                    msEncrypt.Write(aesAlg.IV, 0, aesAlg.IV.Length);
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                    {
                        swEncrypt.Write(plainText);
                    }
                    return msEncrypt.ToArray();
                }
            }
        }

        public string DecryptString(byte[] cipherBytes)
        {
            using (Aes aesAlg = Aes.Create())
            {
                int ivLength = BitConverter.ToInt32(cipherBytes, 0);
                aesAlg.IV = cipherBytes.Skip(sizeof(int)).Take(ivLength).ToArray();

                byte[] salt = key.Take(16).ToArray();

                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(key, salt, 10000);
                aesAlg.Key = pdb.GetBytes(32);
                aesAlg.IV = pdb.GetBytes(16);

                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                using (MemoryStream msDecrypt = new MemoryStream(cipherBytes, sizeof(int) + ivLength, cipherBytes.Length - (sizeof(int) + ivLength)))
                using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                {
                    return srDecrypt.ReadToEnd();
                }
            }
        }

        private void SendFile_Click(object sender, RoutedEventArgs e)
        {
            FileItem selectedItem = (FileItem)fileListView.SelectedItem;
            if (selectedItem != null && selectedItem.Type == "File")
            {
                try
                {
                    string filePath = Path.Combine(@"E:\C#\Secure file manager\ACTIVE USERS", userEmail, selectedItem.Name);

                    if (filePath.Contains("_ENCRYPTED"))
                    {
                        MessageBox.Show("Encrypted files cannot be sent. Please select an UNRELIABLE file.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                    if (!filePath.Contains("_UNRELIABLE"))
                    {
                        MessageBox.Show("Only UNRELIABLE files can be sent. Please select a valid file.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                    string recipientEmail = Microsoft.VisualBasic.Interaction.InputBox("Enter recipient email:", "Send File", "");

                    User recipient = DatabaseHelper.GetUserByEmail(recipientEmail);
                    if (recipient == null)
                    {
                        MessageBox.Show("Recipient not found in the system.");
                        return;
                    }

                    DatabaseHelper.CreateFileTransferRequest(userEmail, recipientEmail, filePath);
                    MessageBox.Show("File transfer request sent successfully.");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error sending file: {ex.Message}");
                }
                RefreshCounts();
            }
            else
            {
                MessageBox.Show("Please select a file to send.");
            }
        }



        private void FilesToAccept_Click(object sender, RoutedEventArgs e)
        {
            AcceptFilesWindow acceptFilesWindow = new AcceptFilesWindow(userEmail, userPassword);


            acceptFilesWindow.ShowDialog();

            RefreshListView();
            RefreshCounts();
        }

        private void RefreshCounts()
        {
            int incomingCount = DatabaseHelper.GetReceivedFileTransferRequests(userEmail).Count;
            incomingFilesCount.Text = incomingCount.ToString();

            int outgoingCount = DatabaseHelper.GetSentFileTransferRequests(userEmail).Count;
            outgoingFilesCount.Text = outgoingCount.ToString();
        }

        private void Refresh_page_Click(object sender, RoutedEventArgs e)
        {
            RefreshListView();
            RefreshCounts();
            MessageBox.Show("The page has been successfully updated!");
        }
    }

}