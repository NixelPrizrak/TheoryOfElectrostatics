using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace TheoryOfElectrostatics
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected Mutex Mutex;

        protected override void OnStartup(StartupEventArgs e)
        {
            Mutex = new Mutex(true, "TheryOfElectrostatics.exe");
            if (!Mutex.WaitOne())
            {
                Current.Shutdown();
                return;
            }
            else
            {
                ShutdownMode = ShutdownMode.OnLastWindowClose;
            }
            base.OnStartup(e);
        }

        protected override void OnExit(ExitEventArgs e)
        {
            if (Mutex != null)
            {
                Mutex.ReleaseMutex();
            }

            base.OnExit(e);
        }
    }
}
