using System.Runtime.InteropServices;

namespace tiger.helpers;


public class PlatformConfig
{
    public static string GetWriteToLocation()
    {
        string writeToLocation = "";
        string contextPath = AppContext.BaseDirectory;
        int index = contextPath.IndexOf("tiger\\");
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            writeToLocation = "C:/ProgramData/tiger-daemon/logs.txt";
            index = contextPath.IndexOf("tiger\\");
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            writeToLocation = "/var/log/tiger-daemon/logs.txt";
            index = contextPath.IndexOf("tiger/");
        }
        return writeToLocation;
    }

    public static string GetTigerPath()
    {
        string contextPath = AppContext.BaseDirectory;
        int index = contextPath.IndexOf("tiger\\");
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            index = contextPath.IndexOf("tiger\\");
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            index = contextPath.IndexOf("tiger/");
        }
        string tigerPath = contextPath.Substring(0, index + 6);
        return tigerPath;
    }
}

