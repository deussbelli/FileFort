using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using User_model;
using UserRegistrationDatabase;
using System.Security.Cryptography;
using File_model;
using UserRegistration;

namespace UserRegistration
{
    public partial class LobbyWindow : Window
    {
        private readonly byte[] key;
        private readonly UserWindow userWindow;
        private readonly string filePath;


        public LobbyWindow(string filePath, byte[] key, UserWindow userWindow)
        {
            InitializeComponent();
            this.filePath = filePath;
            this.key = key;
            this.userWindow = userWindow;


            LoadFileContent();
        }
        private void LoadFileContent()
        {
            try
            {
                if (File.Exists(filePath))
                {
                    if (Path.GetFileName(filePath).Contains("_UNRELIABLE"))
                    {
                        byte[] contentBytes = File.ReadAllBytes(filePath);
                        string content = Encoding.UTF8.GetString(contentBytes);
                        fileContentTextBox.Text = content;
                    }
                    else
                    {
                        byte[] encryptedBytes = File.ReadAllBytes(filePath);
                        if (encryptedBytes.Length > 0)
                        {
                            string decryptedContent = userWindow.DecryptString(encryptedBytes);
                            fileContentTextBox.Text = decryptedContent;
                        }
                        else
                        {
                            MessageBox.Show("The file is empty.");
                        }
                    }
                }
                else
                {
                    MessageBox.Show("The file does not exist.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading file content: {ex.Message}");
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string contentToSave = fileContentTextBox.Text;

                if (Path.GetFileName(filePath).Contains("_UNRELIABLE"))
                {
                    byte[] contentBytes = Encoding.UTF8.GetBytes(contentToSave);
                    File.WriteAllBytes(filePath, contentBytes);
                }
                else
                {
                    byte[] encryptedBytes = userWindow.EncryptString(contentToSave);
                    File.WriteAllBytes(filePath, encryptedBytes);
                }

                MessageBox.Show("File saved successfully!");
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving file content: {ex.Message}");
            }
        }


    }
}
