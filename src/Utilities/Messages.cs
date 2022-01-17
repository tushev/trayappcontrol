using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;

namespace TrayAppControl
{
    internal static class Messages
    {
        static List<string> ErrorsList = new List<string>();

        static public void LogError(string message)
        {
            ErrorsList.Add(message);
        }
        static public void ShowAllLoggedErrors(string title)
        {
            if (ErrorsList.Count > 0)
            {
                var message = String.Join($"{Environment.NewLine}{Environment.NewLine}", ErrorsList);

                MessageBox.Show(
                    message, $"TrayAppControl - {title}",
                    MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, MessageBoxOptions.DefaultDesktopOnly);

                ErrorsList.Clear();
            }
            else
                Debug.WriteLine("[ShowAllLoggedErrors] There were no errors.");
        }

        static public void ShowError(string message, string title = "")
        {
            MessageBox.Show(
                message, "TrayAppControl" + (String.IsNullOrEmpty(title) ? "" : $" - {title}"), 
                MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, MessageBoxOptions.DefaultDesktopOnly);
        }

        static public void ShowUsageInfo()
        {
            var ans = MessageBox.Show(
                $"Usage: {Process.GetCurrentProcess().ProcessName}.exe [--forcestart] C:\\path\\file.yaml{Environment.NewLine}{Environment.NewLine}"+
                "Would you like to open app's website for more info?", 
                "TrayAppControl",
                MessageBoxButton.YesNo, MessageBoxImage.Information, MessageBoxResult.No, MessageBoxOptions.DefaultDesktopOnly);

            if (ans == MessageBoxResult.Yes)
                Process.Start(new ProcessStartInfo { FileName = "https://github.com/tushev/trayappcontrol", UseShellExecute = true });
        }
    }
}
