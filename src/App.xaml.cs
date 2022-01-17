using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using Hardcodet.Wpf.TaskbarNotification;

namespace TrayAppControl
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private TaskbarIcon notifyIcon;
        private NotifyIconViewModel vm = new NotifyIconViewModel();

        internal ManagedService ManagedService = new ManagedService();

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            if (e.Args.Count() == 0)
            {
                Messages.ShowUsageInfo();
                ShutdownApp();
                return;
            }

            bool ForceStart = false;

            string first = e.Args.First();
            if (first.ToLowerInvariant() == "--forcestart")
                ForceStart = true;

            string YAMLFile = ForceStart ? e.Args.ElementAt(1) : first;

            try
            {
                ServicesYAMLConfigParser.ParseYAMLFile(YAMLFile, ref ManagedService);
            }
            catch (Exception ex)
            {
                Messages.ShowError(
                    $"Unfortunately, your YAML file:{Environment.NewLine}{YAMLFile}{Environment.NewLine}cannot be parsed. The following error happened:{Environment.NewLine}{Environment.NewLine}" + 
                    $"{ex.Message}{Environment.NewLine}{Environment.NewLine}TrayAppControl will be terminated.", "Error loading YAML config");
                Application.Current.Shutdown();
            }        

            // create the notifyicon (it's a resource declared in NotifyIconResources.xaml)
            notifyIcon = (TaskbarIcon)FindResource("NotifyIcon");
            notifyIcon.DataContext = vm;

            SetIcon(ManagedService.IconStopped);
            ManagedService.IconChanged += (s, ea) => { SetIcon(((IconChangedEventArgs)ea).Icon);  };

            if (ManagedService.StartServicesAutomatically || ForceStart)
                vm.StartCommand.Execute(vm);
        }
        protected override void OnExit(ExitEventArgs e)
        {
            if (ManagedService.StopServicesOnShutdown)
                ManagedService.StopService(true);

            notifyIcon?.Dispose(); //the icon would clean up automatically, but this is cleaner
            base.OnExit(e);
        }

        internal void SetIcon(string icon)
        {
            if (notifyIcon != null && !String.IsNullOrEmpty(icon) && File.Exists(icon))
                notifyIcon.Icon = new System.Drawing.Icon(icon);
            else
                notifyIcon.Icon = System.Drawing.Icon.ExtractAssociatedIcon(Assembly.GetExecutingAssembly().Location);
        }
        internal void ShutdownApp()
        {
            if (((App)Application.Current).ManagedService.ServicesAreStarted)
                ((App)Application.Current).ManagedService.StopService();

            Application.Current.Shutdown();
        }
    }
}
