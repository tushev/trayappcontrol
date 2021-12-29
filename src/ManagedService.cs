using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace TrayAppControl
{
    public class IconChangedEventArgs : EventArgs
    {
        public string Icon { get; set; }
        public IconChangedEventArgs(string icon) => Icon = icon;
    }

    internal class ManagedService
    {
        public event EventHandler IconChanged;
        public bool ServicesAreStarted { get; set; }

        [YamlMember(Alias = "start-immediately")]
        public bool StartServicesAutomatically { get; set; }

        [YamlMember(Alias = "stop-on-shutdown")]
        public bool StopServicesOnShutdown { get; set; }

        [YamlMember(Alias = "service-name")]
        public string ServiceName { get; set; }

        [YamlMember(Alias = "icon-running")]
        public string IconRunning { get; set; }

        [YamlMember(Alias = "icon-stopped")]
        public string IconStopped { get; set; }

        [YamlMember(Alias = "managed-apps")]
        public List<ManagedServiceCommand> ManagedCommands { get; set; }

        [YamlMember(Alias = "on-start")]
        public List<ManagedServiceCommand> StartSingleRunCommands { get; set; }
        [YamlMember(Alias = "on-stop")]
        public List<ManagedServiceCommand> StopSingleRunCommands { get; set; }


        public ManagedService()
        {
            ServicesAreStarted = false;
            StartServicesAutomatically = false;
            StopServicesOnShutdown = true;
            ServiceName = "service";
            IconRunning = @"";
            IconStopped = @"";
        }

        public void StartService()
        {
            ManagedCommands?.ForEach(command => { command.ExecuteAsManaged(); });
            StartSingleRunCommands?.ForEach(command => { command.Execute(); });

            Messages.ShowAllLoggedErrors($"Errors while starting {ServiceName}");

            ServicesAreStarted = true;
            IconChanged?.Invoke(this, new IconChangedEventArgs(IconRunning));
        }
        
        public void StopService(bool silent = false)
        {
            ManagedCommands?.ForEach(command => { command.TerminateManagedProcess(); });
            StopSingleRunCommands?.ForEach(command => { command.Execute(); });

            if (!silent)
                Messages.ShowAllLoggedErrors($"Errors while stopping {ServiceName}");

            ServicesAreStarted = false;
            IconChanged?.Invoke(this, new IconChangedEventArgs(IconStopped));
        }


    }
}
