namespace CG.TrayNotify.Host.Service
{
    partial class ProjectInstaller
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.TrayNotifyHostServiceAccount = new System.ServiceProcess.ServiceProcessInstaller();
            this.TrayNotifyHostServiceInstaller = new System.ServiceProcess.ServiceInstaller();
            // 
            // TrayNotifyHostServiceAccount
            // 
            this.TrayNotifyHostServiceAccount.Account = System.ServiceProcess.ServiceAccount.LocalSystem;
            this.TrayNotifyHostServiceAccount.Password = null;
            this.TrayNotifyHostServiceAccount.Username = null;
            // 
            // TrayNotifyHostServiceInstaller
            // 
            this.TrayNotifyHostServiceInstaller.Description = "Hosts the Tray Notify WCF Service";
            this.TrayNotifyHostServiceInstaller.DisplayName = "CG Tray Notify Host";
            this.TrayNotifyHostServiceInstaller.ServiceName = "TrayNotifyHostService";
            this.TrayNotifyHostServiceInstaller.StartType = System.ServiceProcess.ServiceStartMode.Automatic;
            // 
            // ProjectInstaller
            // 
            this.Installers.AddRange(new System.Configuration.Install.Installer[] {
            this.TrayNotifyHostServiceAccount,
            this.TrayNotifyHostServiceInstaller});

        }

        #endregion

        private System.ServiceProcess.ServiceProcessInstaller TrayNotifyHostServiceAccount;
        private System.ServiceProcess.ServiceInstaller TrayNotifyHostServiceInstaller;
    }
}