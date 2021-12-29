using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace TrayAppControl
{
    internal class ManagedServiceCommand
    {
        [YamlMember(Alias = "command")]
        public string Command { get; set; }

        [YamlMember(Alias = "arguments")]
        public string Arguments { get; set; }

        [YamlMember(Alias = "workdir")]
        public string WorkingDirectory { get; set; }

        [YamlMember(Alias = "hide-window")]
        public bool HideWindow { get; set; }

        public ManagedServiceCommand ()
        {
            Command = "";
            HideWindow = false;
        }

        private Process ProcessHandle { get; set; }

        public void ExecuteAsManaged()
        {
            ProcessHandle = Execute();
        }
        public void TerminateManagedProcess()
        {
            if (ProcessHandle == null) { Messages.LogError($"Handle for managed process {Command} is null"); }
            else
            {
                try { ProcessHandle?.Kill(); }
                catch (Exception ex) { Messages.LogError($"Error terminating managed process {Command}: {ex.Message}"); }
            }
        }

        public Process Execute()
        {
            if (String.IsNullOrEmpty(Command))
            {
                Messages.LogError($"[command] Executable file name is empty!");
                return null;
            }
            if (!File.Exists(Command))
            {
                Messages.LogError($"Cannot execute command: file does not exist: {Command}");
                return null;
            }

            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = Command;
            if (!String.IsNullOrEmpty(Arguments))        startInfo.Arguments         = Arguments;
            if (!String.IsNullOrEmpty(WorkingDirectory)) startInfo.WorkingDirectory  = WorkingDirectory;

            if (HideWindow)
            {
                startInfo.RedirectStandardOutput = true;
                startInfo.RedirectStandardError = true;
                startInfo.UseShellExecute = false;
                startInfo.CreateNoWindow = true;
            }

            Process processTemp = new Process();

            processTemp.StartInfo = startInfo;
            processTemp.EnableRaisingEvents = true;

            try
            {
                processTemp.Start();
            }
            catch (Exception e)
            {
                Messages.LogError($"[{Command}]=Error during command execution:{e.Message}");
            }

            return processTemp;
        }
    }
}
