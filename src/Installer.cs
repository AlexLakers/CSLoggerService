using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Linq;
using System.ServiceProcess;
using System.Threading.Tasks;


namespace CSLogger
{
    [RunInstaller(true)]
    public partial class Installer : System.Configuration.Install.Installer
    {
        ServiceInstaller serviceinstaller;
        ServiceProcessInstaller serviceprocessinstaller;

        public Installer()
        {
            InitializeComponent();
            serviceinstaller = new ServiceInstaller();
            serviceprocessinstaller = new ServiceProcessInstaller();
            serviceprocessinstaller.Account = ServiceAccount.LocalSystem;
            serviceinstaller.StartType = ServiceStartMode.Manual;
            serviceinstaller.ServiceName = "CSLogger";
         
            Installers.Add(serviceinstaller);
            Installers.Add(serviceprocessinstaller);

        }
    }
}
