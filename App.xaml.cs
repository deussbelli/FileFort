using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using UserRegistration;

namespace Secure_file_manager
{
    public partial class App : Application
    {
        private List<Process> runningProcesses = new List<Process>();

        protected override void OnStartup(StartupEventArgs e)
        {
            this.MainWindow = new ShureWindow();
            this.MainWindow.Closed += MainWindow_Closed;
            this.MainWindow.Show();
        }

        private void MainWindow_Closed(object sender, EventArgs e)
        {
            foreach (var process in runningProcesses)
            {
                process.Kill();
            }

            this.Shutdown();
        }
    }
}
