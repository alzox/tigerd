using System.Diagnostics;
using Microsoft.Extensions.Hosting.WindowsServices;
using System.Runtime.InteropServices;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Hosting.Systemd;

namespace tiger.helpers

{
    public class ProccessDict
    {
        public Dictionary<string, TimeSpan> processesDict = new Dictionary<string, TimeSpan>();
        private readonly ILogger _logger;
        public ProccessDict(ILogger logger)
        {
            _logger = logger;
	        string tigerPath = PlatformConfig.GetTigerPath();
            string processTxt = tigerPath + "processes.txt";
            string[] lines = File.ReadAllLines(processTxt);
            foreach (string line in lines)
            {
                string[] parts = line.Split(',');
                processesDict.Add(parts[0], new TimeSpan(0));
                _logger.LogInformation("Process " + parts[0] + " added to dictionary.");
            }
        }

        public void Initialize()
        {
            foreach (KeyValuePair<string, TimeSpan> entry in processesDict)
            {
                string processName = entry.Key;
                Process[] processes = Process.GetProcessesByName(processName);
                TimeSpan curTimeElapsed = new TimeSpan(0);
                foreach (Process process in processes)
                {
                    DateTime startTime = process.StartTime;
                    DateTime currentTime = DateTime.Now;
                    TimeSpan timeSpan = currentTime - startTime;
                    if (timeSpan > curTimeElapsed)
                    {
                        curTimeElapsed = timeSpan;
                    }
                }
                processesDict[processName] = curTimeElapsed;
            }
            _logger.LogInformation("Processes initialized.");
        }

        public void Update(){
            foreach (KeyValuePair<string, TimeSpan> entry in processesDict)
            {
                string processName = entry.Key;
                Process[] processes = Process.GetProcessesByName(processName);
                TimeSpan curTimeElapsed = new TimeSpan(0);
                foreach (Process process in processes)
                {
                    DateTime startTime = process.StartTime;
                    DateTime currentTime = DateTime.Now;
                    TimeSpan timeSpan = currentTime - startTime;
                    if (timeSpan > curTimeElapsed)
                    {
                        curTimeElapsed = timeSpan;
                    }
                }
                if (curTimeElapsed > processesDict[processName])
                {
                    processesDict[processName] = curTimeElapsed;
                }
                else if (curTimeElapsed < processesDict[processName])
                {
                    _logger.LogInformation("Process " + processName + " has been restarted.");
                    _logger.LogInformation("Time elapsed previously: " + processesDict[processName]);
                    _logger.LogInformation("Time elapsed now: " + curTimeElapsed);

                    processesDict[processName] = curTimeElapsed;
                }
            }
            PrintDict();
        }

        public void PrintDict()
        {
            foreach (KeyValuePair<string, TimeSpan> entry in processesDict)
            {
                string processName = entry.Key;
                TimeSpan timeElapsed = entry.Value;
                _logger.LogInformation("Process: " + processName + " Time elapsed: " + timeElapsed);
            }
        }

        public Dictionary<string, TimeSpan> GetDict()
        {
            return processesDict;
        }

    }




}
