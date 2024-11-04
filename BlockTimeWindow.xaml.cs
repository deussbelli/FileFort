using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace UserRegistration
{
    public partial class BlockTimeWindow : Window
    {
        public bool BlockTimeChanged { get; private set; }
        public TimeSpan NewBlockTime { get; private set; }

        public BlockTimeWindow(TimeSpan currentBlockTime)
        {
            InitializeComponent();
            TimeTextBox.Text = currentBlockTime.ToString(@"hh\:mm\:ss"); 
        }

        private void ConfirmButton_Click(object sender, RoutedEventArgs e)
        {
            if (TimeSpan.TryParse(TimeTextBox.Text, out TimeSpan newTime))
            {
                NewBlockTime = newTime;
                BlockTimeChanged = true;
                Close();
            }
            else
            {
                MessageBox.Show("Invalid time format. Please enter time in hh:mm:ss format.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            BlockTimeChanged = false;
            Close();
        }
    }
}
