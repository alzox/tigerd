using System.Diagnostics;
namespace Tiger.Helper

{
    public class ProccessDict
    {
        public Dictionary<string, int> processes = new Dictionary<string, int>();
        public ProccessDict()
        {   
            string contextPath = AppContext.BaseDirectory;
            int index = contextPath.IndexOf("tiger\\");
            string tigerPath = contextPath.Substring(0, index + 6);
            string processTxt = tigerPath + "processes.txt";
            string[] lines = File.ReadAllLines(processTxt);
            foreach (string line in lines)
            {
                string[] parts = line.Split(',');
                processes.Add(parts[0], 0);
            }
        }

        public void Update()
        {
            foreach (KeyValuePair<string, int> entry in processes)
            {
                string processName = entry.Key;
                int count = entry.Value;
                bool isRunning = Process.GetProcessesByName(processName).Any();
                if (isRunning)
                {
                    count++;
                    processes[processName] = count;
                }
                else
                {
                    processes[processName] = 0;
                }
            }
        }

    }




}