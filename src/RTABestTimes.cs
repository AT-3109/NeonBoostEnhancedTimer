using System.Collections.Generic;
using System.IO;
using System.Text;
using MelonLoader;
using static MelonLoader.MelonLogger;

namespace EnhancedTimer
{
    public static class RTABestTimes
    {
        private static string _path;
        private static Dictionary<int, float> _times = new Dictionary<int, float>();

        public static void Load()
        {
            _path = Path.Combine(
                Path.GetDirectoryName(typeof(RTATimerMod).Assembly.Location),
                "rta_besttimes.json");

            if (!File.Exists(_path))
                return;

            try
            {
                string json = File.ReadAllText(_path, Encoding.UTF8);
                _times.Clear();
                foreach (string line in json.Split('\n'))
                {
                    string trimmed = line.Trim();
                    if (trimmed.Length == 0)
                        continue;
                    int colon = trimmed.IndexOf(':');
                    if (colon < 0)
                        continue;
                    int level = int.Parse(trimmed.Substring(0, colon));
                    float time = float.Parse(trimmed.Substring(colon + 1));
                    _times[level] = time;
                }
            }
            catch (System.Exception e)
            {
                MelonLogger.Warning("Failed to load RTA best times: " + e.Message);
            }
        }

        public static void Save()
        {
            try
            {
                var sb = new StringBuilder();
                foreach (var kvp in _times)
                    sb.AppendLine(kvp.Key + ":" + kvp.Value);
                File.WriteAllText(_path, sb.ToString(), Encoding.UTF8);
            }
            catch (System.Exception e)
            {
                MelonLogger.Warning("Failed to save RTA best times: " + e.Message);
            }
        }

        public static bool TryGet(int level, out float time)
        {
            return _times.TryGetValue(level, out time);
        }

        public static void SetIfBetter(int level, float time)
        {
            if (RTAState.alreadySet)
            {
                return;
            }
            if (!_times.ContainsKey(level))
            {
                RTAState.priorBest = 0f;
            }
            else
            {
                RTAState.priorBest = _times[level];
            }
            if (!_times.ContainsKey(level) || time < _times[level])
            {
                _times[level] = time;
                Save();
            }
            RTAState.alreadySet = true;
        }

        public static string Format(float seconds)
        {
            var ts = System.TimeSpan.FromSeconds(seconds);
            if (ts.TotalHours >= 1.0)
                return string.Format("{0}:{1:00}:{2:00}.{3:000}",
                    (int)ts.TotalHours, ts.Minutes, ts.Seconds, ts.Milliseconds);
            else
                return string.Format("{0}:{1:00}.{2:000}",
                    ts.Minutes, ts.Seconds, ts.Milliseconds);
        }
    }
}
