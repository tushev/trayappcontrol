using System;
using System.Drawing;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace TrayAppControl
{
    /// <summary>
    /// Provides bindable properties and commands for the NotifyIcon. In this sample, the
    /// view model is assigned to the NotifyIcon in XAML. Alternatively, the startup routing
    /// in App.xaml.cs could have created this view model, and assigned it to the NotifyIcon.
    /// </summary>
    public class NotifyIconViewModel
    {
        public string ToolTipText
        {
            get { return $"[TrayAppControl]: {((App)Application.Current).ManagedService.ServiceName}. Right-click for menu"; }
            //get
            //{
            //    return $"{((App)Application.Current).ManagedService.ServiceName}: " +
            //           ( ((App)Application.Current).ManagedService.ServicesAreStarted ? "Running" : "Stopped") +
            //           ". Right-click for menu";
            //}
        }
        public string StartCommandName
        {
            get { return $"Start {((App)Application.Current).ManagedService.ServiceName}"; }
        }
        public string StopCommandName
        {
            get { return $"Stop {((App)Application.Current).ManagedService.ServiceName}"; }
        }
        
        public string ExitCommandName
        {
            get { return "Stop all and exit"; }
        }

        public ICommand StartCommand
        {
            get
            {
                return new DelegateCommand
                {
                    CanExecuteFunc = () => !((App)Application.Current).ManagedService.ServicesAreStarted ,
                    CommandAction = () => { ((App)Application.Current).ManagedService.StartService(); }
                };
            }
        }


        public ICommand StopCommand
        {
            get
            {
                return new DelegateCommand
                {
                    CanExecuteFunc = () => ((App)Application.Current).ManagedService.ServicesAreStarted,
                    CommandAction = () => { ((App)Application.Current).ManagedService.StopService(); }
                };
            }
        }

        public ICommand ExitApplicationCommand
        {
            get
            {
                return new DelegateCommand { CommandAction = () => ((App)Application.Current).ShutdownApp() };
            }
        }

    }
}
