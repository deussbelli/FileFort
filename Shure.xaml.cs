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
using System.Diagnostics;

namespace UserRegistration
{
    public partial class ShureWindow : Window
    {
        private MediaPlayer mediaPlayer;

        public ShureWindow()
        {
            InitializeComponent();
            PlayMusic();
        }

        private void PlayMusic()
        {
            try
            {
                // Путь к вашему аудиофайлу
                string musicFilePath = @"C:\Users\rrriv\Downloads\Music\relaxing-sad-piano-music-lost-dreams-209547.mp3";

                // Создаем объект MediaPlayer
                mediaPlayer = new MediaPlayer();

                // Загружаем аудиофайл
                mediaPlayer.Open(new Uri(musicFilePath));

                // Воспроизводим музыку
                mediaPlayer.Play();

                // Добавляем обработчик события для повторения музыки
                mediaPlayer.MediaEnded += MediaPlayer_MediaEnded;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при воспроизведении музыкального файла: {ex.Message}");
            }
        }

        private void MediaPlayer_MediaEnded(object sender, EventArgs e)
        {
            // Перематываем к началу и снова воспроизводим
            mediaPlayer.Position = TimeSpan.Zero;
            mediaPlayer.Play();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            // Проверяем, что mediaPlayer был инициализирован
            if (mediaPlayer != null)
            {
                // Останавливаем воспроизведение
                mediaPlayer.Stop();
                // Освобождаем ресурсы mediaPlayer
                mediaPlayer.Close();
            }

            // Закрываем окно
            Close();
        }
    }
}
