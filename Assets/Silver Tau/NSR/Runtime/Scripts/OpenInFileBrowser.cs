using System.IO;
using UnityEngine;

namespace SilverTau.NSR
{
    public static class OpenInFileBrowser
    {
        public static void OpenInMacOSFileBrowser(string path)
        {
            bool openInsidesOfFolder = false;
            
            // try mac
            string macPath = path.Replace("\\", "/");

            if (Directory.Exists(macPath))
            {
                openInsidesOfFolder = true;
            }

            if (!macPath.StartsWith("\""))
            {
                macPath = "\"" + macPath;
            }
            if (!macPath.EndsWith("\""))
            {
                macPath = macPath + "\"";
            }
            string arguments = (openInsidesOfFolder ? "" : "-R ") + macPath;
            
            try
            {
                System.Diagnostics.Process.Start("open", arguments);
            }
            catch(System.ComponentModel.Win32Exception e)
            {
                e.HelpLink = "";
            }
        }

        public static void OpenInWindowsFileBrowser(string path)
        {
            bool openInsidesOfFolder = false;

            string winPath = path.Replace("/", "\\");

            if (Directory.Exists(winPath))
            {
                openInsidesOfFolder = true;
            }
            try
            {
                System.Diagnostics.Process.Start("explorer.exe", (openInsidesOfFolder ? "/root," : "/select,") + winPath);
            }
            catch(System.ComponentModel.Win32Exception e)
            {
                e.HelpLink = "";
            }
        }

        public static void OpenFileBrowser(string path)
        {
            switch (Application.platform)
            {
                case RuntimePlatform.OSXEditor:
                case RuntimePlatform.OSXPlayer:
                case RuntimePlatform.OSXServer:
                    OpenInMacOSFileBrowser(path);
                    break;
                case RuntimePlatform.WindowsEditor:
                case RuntimePlatform.WindowsPlayer:
                case RuntimePlatform.WindowsServer:
                    OpenInWindowsFileBrowser(path);
                    break;
                default:
                    break;
            }
        }
    }
}
