using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace TheoryOfElectrostatics
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            //Process[] lprcTestApp = Process.GetProcessesByName(AppDomain.CurrentDomain.FriendlyName.Split('.')[0]);
            //if (lprcTestApp.Length > 1)
            //{
            //    Current.Shutdown();
            //}
        }
    }
}
