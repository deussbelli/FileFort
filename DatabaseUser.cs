using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using System.IO;
using static UserRegistration.AdminWindow;
using System.Xml.Linq;
using User_model;
using FileTransferRequest_model;
using CurrentUser_model;
using System.Security.AccessControl;
using System.Security.Principal;
using Microsoft.VisualBasic.ApplicationServices;
using System.IO.Pipes;

namespace UserRegistrationDatabase
{
    public class DatabaseHelper
    {
        private const string DbConnectionString = "Data Source=UserDatabase.db;Version=3;";
        private const string ActiveUsersFolderPath = @"E:\C#\Secure file manager\ACTIVE USERS";

        private static List<FileTransferRequest> fileTransferRequests = new List<FileTransferRequest>();
        private static int nextRequestId = 1;


        public static void CreateFileTransferRequest(string senderEmail, string recipientEmail, string filePath)
        {
            string commonAppDataDirectory = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);

            string recipientTempDirectory = Path.Combine(commonAppDataDirectory, "FileTransfer", recipientEmail);
            Directory.CreateDirectory(recipientTempDirectory);

            string destinationPath = Path.Combine(recipientTempDirectory, Path.GetFileName(filePath));
            File.Copy(filePath, destinationPath, true);

            DirectoryInfo directoryInfo = new DirectoryInfo(recipientTempDirectory);
            directoryInfo.Attributes |= FileAttributes.System;

            using (SQLiteConnection connection = new SQLiteConnection(DbConnectionString))
            {
                connection.Open();
                using (SQLiteCommand cmd = connection.CreateCommand())
                {
                    cmd.CommandText = "INSERT INTO FileTransferRequests (SenderEmail, RecipientEmail, FilePath) VALUES (@SenderEmail, @RecipientEmail, @FilePath)";
                    cmd.Parameters.AddWithValue("@SenderEmail", senderEmail);
                    cmd.Parameters.AddWithValue("@RecipientEmail", recipientEmail);
                    cmd.Parameters.AddWithValue("@FilePath", destinationPath);
                    cmd.ExecuteNonQuery();
                }
            }
        }


        public static void UpdateFileTransferRequest(FileTransferRequest request)
        {
            using (SQLiteConnection connection = new SQLiteConnection(DbConnectionString))
            {
                connection.Open();
                using (SQLiteCommand cmd = connection.CreateCommand())
                {
                    cmd.CommandText = "UPDATE FileTransferRequests SET IsAccepted = @IsAccepted WHERE Id = @Id";
                    cmd.Parameters.AddWithValue("@IsAccepted", request.IsAccepted);
                    cmd.Parameters.AddWithValue("@Id", request.Id);
                    cmd.ExecuteNonQuery();
                }
            }
        }


        public static List<FileTransferRequest> GetFileTransferRequestsForUser(string email)
        {
            List<FileTransferRequest> requests = new List<FileTransferRequest>();
            using (SQLiteConnection connection = new SQLiteConnection(DbConnectionString))
            {
                connection.Open();
                using (SQLiteCommand cmd = connection.CreateCommand())
                {
                    cmd.CommandText = "SELECT * FROM FileTransferRequests WHERE RecipientEmail = @Email";
                    cmd.Parameters.AddWithValue("@Email", email);
                    using (SQLiteDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            requests.Add(new FileTransferRequest
                            {
                                Id = Convert.ToInt32(reader["Id"]),
                                SenderEmail = reader["SenderEmail"].ToString(),
                                RecipientEmail = reader["RecipientEmail"].ToString(),
                                FilePath = reader["FilePath"].ToString()
                            });
                        }
                    }
                }
            }
            return requests;
        }

        public static List<FileTransferRequest> GetReceivedFileTransferRequests(string email)
        {
            List<FileTransferRequest> requests = new List<FileTransferRequest>();
            using (SQLiteConnection connection = new SQLiteConnection(DbConnectionString))
            {
                connection.Open();
                using (SQLiteCommand cmd = connection.CreateCommand())
                {
                    cmd.CommandText = "SELECT * FROM FileTransferRequests WHERE RecipientEmail = @Email";
                    cmd.Parameters.AddWithValue("@Email", email);
                    using (SQLiteDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            requests.Add(new FileTransferRequest
                            {
                                Id = Convert.ToInt32(reader["Id"]),
                                SenderEmail = reader["SenderEmail"].ToString(),
                                RecipientEmail = reader["RecipientEmail"].ToString(),
                                FilePath = reader["FilePath"].ToString()
                            });
                        }
                    }
                }
            }
            return requests;
        }

        public static List<FileTransferRequest> GetSentFileTransferRequests(string email)
        {
            List<FileTransferRequest> requests = new List<FileTransferRequest>();
            using (SQLiteConnection connection = new SQLiteConnection(DbConnectionString))
            {
                connection.Open();
                using (SQLiteCommand cmd = connection.CreateCommand())
                {
                    cmd.CommandText = "SELECT * FROM FileTransferRequests WHERE SenderEmail = @Email";
                    cmd.Parameters.AddWithValue("@Email", email);
                    using (SQLiteDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            requests.Add(new FileTransferRequest
                            {
                                Id = Convert.ToInt32(reader["Id"]),
                                SenderEmail = reader["SenderEmail"].ToString(),
                                RecipientEmail = reader["RecipientEmail"].ToString(),
                                FilePath = reader["FilePath"].ToString()
                            });
                        }
                    }
                }
            }
            return requests;
        }

        public static void DeleteFileTransferRequest(int requestId)
        {
            using (SQLiteConnection connection = new SQLiteConnection(DbConnectionString))
            {
                connection.Open();
                using (SQLiteCommand cmd = connection.CreateCommand())
                {
                    cmd.CommandText = "DELETE FROM FileTransferRequests WHERE Id = @Id";
                    cmd.Parameters.AddWithValue("@Id", requestId);
                    cmd.ExecuteNonQuery();
                }
            }
        }



        public static void CreateUser(string name, string lastName, string email, string hashedPassword, bool isAdmin = false)
        {
            using (SQLiteConnection connection = new SQLiteConnection(DbConnectionString))
            {
                connection.Open();
                using (SQLiteCommand cmd = connection.CreateCommand())
                {
                    cmd.CommandText = "INSERT INTO Users (Name, LastName, Email, Password, IsAdmin) VALUES (@Name, @LastName, @Email, @Password, @IsAdmin)";
                    cmd.Parameters.AddWithValue("@Name", name);
                    cmd.Parameters.AddWithValue("@LastName", lastName);
                    cmd.Parameters.AddWithValue("@Email", email);
                    cmd.Parameters.AddWithValue("@Password", hashedPassword);
                    cmd.Parameters.AddWithValue("@IsAdmin", isAdmin ? 1 : 0);
                    cmd.ExecuteNonQuery();
                }
            }
            string userDirectory = Path.Combine(ActiveUsersFolderPath, email);
            Directory.CreateDirectory(userDirectory);

            SetUserDirectoryPermissions(userDirectory);
            SetAnastasiiaPermissions(userDirectory); ;
            //HideOtherUserDirectories(userDirectory);
        }


        public static bool UserExists(string email)
        {
            using (SQLiteConnection connection = new SQLiteConnection(DbConnectionString))
            {
                connection.Open();
                using (SQLiteCommand cmd = connection.CreateCommand())
                {
                    cmd.CommandText = "SELECT COUNT(*) FROM Users WHERE Email = @Email";
                    cmd.Parameters.AddWithValue("@Email", email);
                    int count = Convert.ToInt32(cmd.ExecuteScalar());
                    return count > 0;
                }
            }
        }

        public static string GetHashedPassword(string email)
        {
            using (SQLiteConnection connection = new SQLiteConnection(DbConnectionString))
            {
                connection.Open();
                using (SQLiteCommand cmd = connection.CreateCommand())
                {
                    cmd.CommandText = "SELECT Password FROM Users WHERE Email = @Email";
                    cmd.Parameters.AddWithValue("@Email", email);
                    string hashedPassword = (string)cmd.ExecuteScalar();
                    return hashedPassword;
                }
            }
        }

        public static void CreateDatabase()
        {
            if (!File.Exists("UserDatabase.db"))
            {
                SQLiteConnection.CreateFile("UserDatabase.db");
                using (SQLiteConnection connection = new SQLiteConnection(DbConnectionString))
                {
                    connection.Open();
                    using (SQLiteCommand cmd = connection.CreateCommand())
                    {
                        cmd.CommandText = @"CREATE TABLE Users (
                                                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                                                Name TEXT NOT NULL,
                                                LastName TEXT NOT NULL,
                                                Email TEXT UNIQUE NOT NULL,
                                                Password TEXT NOT NULL,
                                                IsAdmin INTEGER NOT NULL DEFAULT 0)";
                        cmd.ExecuteNonQuery();

                        cmd.CommandText = @"CREATE TABLE BlockedUsers (
                                                UserId INTEGER PRIMARY KEY,
                                                BlockTime DATETIME)";
                        cmd.ExecuteNonQuery();

                        cmd.CommandText = @"CREATE TABLE FileTransferRequests (
                                                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                                                SenderEmail TEXT NOT NULL,
                                                RecipientEmail TEXT NOT NULL,
                                                FilePath TEXT NOT NULL)";
                        cmd.ExecuteNonQuery();
                    }
                }
            }

            CreateActiveUsersFolder();
        }


        private static void CreateActiveUsersFolder()
        {

            if (!Directory.Exists(ActiveUsersFolderPath))
            {
                Directory.CreateDirectory(ActiveUsersFolderPath);

                DirectoryInfo directoryInfo = new DirectoryInfo(ActiveUsersFolderPath);
                directoryInfo.Attributes |= FileAttributes.Hidden;

                string applicationUserId = GetCurrentUserId();

                SetDirectoryPermissions(ActiveUsersFolderPath, applicationUserId);
            }
        }


        public static void DeleteUser(int userId, string userEmail)
        {
            string filesDatabaseName = $"{userEmail}_files.db";
            if (File.Exists(filesDatabaseName))
            {
                File.Delete(filesDatabaseName);
            }

            using (SQLiteConnection connection = new SQLiteConnection(DbConnectionString))
            {
                connection.Open();
                using (SQLiteCommand cmd = connection.CreateCommand())
                {
                    cmd.CommandText = "DELETE FROM Users WHERE Id = @UserId";
                    cmd.Parameters.AddWithValue("@UserId", userId);
                    cmd.ExecuteNonQuery();
                }
            }

            string userDirectory = Path.Combine(ActiveUsersFolderPath, userEmail);
            if (Directory.Exists(userDirectory))
            {
                Directory.Delete(userDirectory, true);
            }
        }



        public static void SetAdminStatus(int userId, bool isAdmin)
        {
            using (SQLiteConnection connection = new SQLiteConnection(DbConnectionString))
            {
                connection.Open();
                using (SQLiteCommand cmd = connection.CreateCommand())
                {
                    cmd.CommandText = "UPDATE Users SET IsAdmin = @IsAdmin WHERE Id = @UserId";
                    cmd.Parameters.AddWithValue("@IsAdmin", isAdmin ? 1 : 0);
                    cmd.Parameters.AddWithValue("@UserId", userId);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public static TimeSpan GetBlockedDuration(string email)
        {
            TimeSpan blockDuration = TimeSpan.Zero;

            using (SQLiteConnection connection = new SQLiteConnection(DbConnectionString))
            {
                connection.Open();

                using (SQLiteCommand cmd = connection.CreateCommand())
                {
                    cmd.CommandText = @"
                SELECT BlockTime 
                FROM BlockedUsers 
                WHERE UserId = (SELECT Id FROM Users WHERE Email = @Email)";
                    cmd.Parameters.AddWithValue("@Email", email);

                    object result = cmd.ExecuteScalar();

                    if (result != null && result != DBNull.Value)
                    {
                        DateTime blockTime = Convert.ToDateTime(result);
                        blockDuration = blockTime - DateTime.Now;

                        if (blockDuration < TimeSpan.Zero)
                        {
                            blockDuration = TimeSpan.Zero;
                        }
                    }
                }
            }

            return blockDuration;
        }


        public static void UnblockUser(int userId)
        {
            using (SQLiteConnection connection = new SQLiteConnection(DbConnectionString))
            {
                connection.Open();

                using (SQLiteCommand cmd = connection.CreateCommand())
                {
                    cmd.CommandText = "DELETE FROM BlockedUsers WHERE UserId = @UserId";
                    cmd.Parameters.AddWithValue("@UserId", userId);
                    cmd.ExecuteNonQuery();
                }
            }
        }


        public static void BlockUser(int userId, TimeSpan blockTime)
        {
            using (SQLiteConnection connection = new SQLiteConnection(DbConnectionString))
            {
                connection.Open();
                using (SQLiteCommand cmd = connection.CreateCommand())
                {
                    cmd.CommandText = @"INSERT OR REPLACE INTO BlockedUsers (UserId, BlockTime) 
                                VALUES (@UserId, @BlockTime)";
                    cmd.Parameters.AddWithValue("@UserId", userId);
                    cmd.Parameters.AddWithValue("@BlockTime", DateTime.Now.Add(blockTime));
                    cmd.ExecuteNonQuery();
                }
            }
        }



        public static bool IsCurrentUserAdmin(string userEmail)
        {
            User_model.User currentUser = GetUserByEmail(userEmail);
            return currentUser != null && currentUser.IsAdmin;
        }

        public static User_model.User GetUserByEmail(string email)
        {
            using (SQLiteConnection connection = new SQLiteConnection(DbConnectionString))
            {
                connection.Open();
                using (SQLiteCommand cmd = connection.CreateCommand())
                {
                    cmd.CommandText = "SELECT * FROM Users WHERE Email = @Email";
                    cmd.Parameters.AddWithValue("@Email", email);
                    using (SQLiteDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            User_model.User user = new User_model.User
                            {
                                Id = reader.GetInt32(0),
                                Name = reader.GetString(1),
                                LastName = reader.GetString(2),
                                Email = reader.GetString(3),
                                IsAdmin = reader.GetBoolean(5)
                            };
                            return user;
                        }
                    }
                }
            }
            return null;
        }

        private static CurrentUser GetCurrentLoggedInUser()
        {
            return new CurrentUser { IsAdmin = true };
        }

        public static List<User_model.User> GetAllUsers()
        {
            List<User_model.User> users = new List<User_model.User>();
            using (SQLiteConnection connection = new SQLiteConnection(DbConnectionString))
            {
                connection.Open();
                using (SQLiteCommand cmd = connection.CreateCommand())
                {
                    cmd.CommandText = "SELECT * FROM Users";
                    using (SQLiteDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            User_model.User user = new User_model.User
                            {
                                Id = reader.GetInt32(0),
                                Name = reader.GetString(1),
                                LastName = reader.GetString(2),
                                Email = reader.GetString(3),
                                IsAdmin = reader.GetBoolean(5)
                            };
                            users.Add(user);
                        }
                    }
                }
            }
            return users;
        }

        public static void CreateFilesDatabase(string userEmail, string userDirectory)
        {
            string filesDatabaseName = $"{userEmail}_files.db";
            SQLiteConnection.CreateFile(filesDatabaseName);

            using (SQLiteConnection connection = new SQLiteConnection($"Data Source={filesDatabaseName};Version=3;"))
            {
                connection.Open();
                using (SQLiteCommand cmd = connection.CreateCommand())
                {
                    cmd.CommandText = @"CREATE TABLE Files (
                   Id INTEGER PRIMARY KEY AUTOINCREMENT,
                   FileName TEXT NOT NULL,
                   FileType TEXT NOT NULL,
                   FilePath TEXT NOT NULL,
                   UserId INTEGER NOT NULL)";
                    cmd.ExecuteNonQuery();
                }
            }

            FileInfo databaseFileInfo = new FileInfo(filesDatabaseName);
            databaseFileInfo.Attributes |= FileAttributes.Hidden;

            int userId = GetUserIdByEmail(userEmail);

            SetFilePermissions(filesDatabaseName, userId);
            SetDirectoryPermissions(userDirectory);
        }

        //private static void HideOtherUserDirectories(string currentUserDirectory)
        //{
        //    string[] userDirectories = Directory.GetDirectories(ActiveUsersFolderPath);
        //    foreach (string directory in userDirectories)
        //    {
        //        if (directory != currentUserDirectory)
        //        {
        //            DirectoryInfo dirInfo = new DirectoryInfo(directory);
        //            dirInfo.Attributes |= FileAttributes.Hidden;
        //        }
        //    }
        //}

        private static void SetUserDirectoryPermissions(string directoryPath)
        {
            DirectorySecurity directorySecurity = new DirectorySecurity();
            directorySecurity.AddAccessRule(new FileSystemAccessRule(
                new SecurityIdentifier(WindowsIdentity.GetCurrent().User.Value),
                FileSystemRights.FullControl,
                InheritanceFlags.ContainerInherit | InheritanceFlags.ObjectInherit,
                PropagationFlags.None,
                AccessControlType.Allow));

            Directory.SetAccessControl(directoryPath, directorySecurity);
        }
        private static void SetAnastasiiaPermissions(string directoryPath)
        {
            DirectorySecurity directorySecurity = Directory.GetAccessControl(directoryPath);
            directorySecurity.SetAccessRuleProtection(true, false);

            FileSystemAccessRule allowRule = new FileSystemAccessRule(
                new NTAccount("DESKTOP-2HPB52A", "Anastasiia"),
                FileSystemRights.FullControl,
                InheritanceFlags.ContainerInherit | InheritanceFlags.ObjectInherit,
                PropagationFlags.None,
                AccessControlType.Allow);
            directorySecurity.AddAccessRule(allowRule);

            Directory.SetAccessControl(directoryPath, directorySecurity);
        }


        private static void SetDirectoryPermissions(string directoryPath, string userId = null)
        {
            DirectorySecurity directorySecurity = Directory.GetAccessControl(directoryPath);
            directorySecurity.SetAccessRuleProtection(true, false);

            FileSystemAccessRule allowAdminRule = new FileSystemAccessRule(
                new SecurityIdentifier(WellKnownSidType.BuiltinAdministratorsSid, null),
                FileSystemRights.FullControl,
                InheritanceFlags.ContainerInherit | InheritanceFlags.ObjectInherit,
                PropagationFlags.None,
                AccessControlType.Allow);
            directorySecurity.AddAccessRule(allowAdminRule);

            if (userId != null)
            {
                FileSystemAccessRule allowOwnerRule = new FileSystemAccessRule(
                    new SecurityIdentifier(userId),
                    FileSystemRights.FullControl,
                    InheritanceFlags.ContainerInherit | InheritanceFlags.ObjectInherit,
                    PropagationFlags.None,
                    AccessControlType.Allow);
                directorySecurity.AddAccessRule(allowOwnerRule);
            }

            FileSystemAccessRule allowUsersRule = new FileSystemAccessRule(
                new SecurityIdentifier(WellKnownSidType.BuiltinUsersSid, null),
                FileSystemRights.Modify,
                InheritanceFlags.ContainerInherit | InheritanceFlags.ObjectInherit,
                PropagationFlags.None,
                AccessControlType.Allow);
            directorySecurity.AddAccessRule(allowUsersRule);

            Directory.SetAccessControl(directoryPath, directorySecurity);
        }


        private static void SetFilePermissions(string filePath, int userId)
        {
            FileSecurity fileSecurity = File.GetAccessControl(filePath);
            fileSecurity.SetAccessRuleProtection(true, false);

            SecurityIdentifier userSid = new SecurityIdentifier(WellKnownSidType.BuiltinUsersSid, null);

            FileSystemAccessRule allowRule = new FileSystemAccessRule(
                userSid,
                FileSystemRights.FullControl,
                AccessControlType.Allow);

            fileSecurity.AddAccessRule(allowRule);

            File.SetAccessControl(filePath, fileSecurity);
        }


        public static int GetUserIdByEmail(string email)
        {
            using (SQLiteConnection connection = new SQLiteConnection(DbConnectionString))
            {
                connection.Open();
                using (SQLiteCommand cmd = connection.CreateCommand())
                {
                    cmd.CommandText = "SELECT Id FROM Users WHERE Email = @Email";
                    cmd.Parameters.AddWithValue("@Email", email);
                    return Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
        }

        private static string GetCurrentUserId()
        {
            return WindowsIdentity.GetCurrent().User.Value;
        }
    }
}

