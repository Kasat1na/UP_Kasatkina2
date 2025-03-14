using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WriteEraseLibrary
{
    public class Calculations
    {
        public static string[] AvailablePeriods(TimeSpan[] startTimes, int[] durations,
                                          TimeSpan beginWorkingTime, TimeSpan endWorkingTime,
                                          int consultationTime)
        {
            List<(TimeSpan start, TimeSpan end)> busyPeriods = new List<(TimeSpan start, TimeSpan end)>();
            for (int i = 0; i < startTimes.Length; i++)
            {
                busyPeriods.Add((startTimes[i], startTimes[i] + TimeSpan.FromMinutes(durations[i])));
            }

            busyPeriods = busyPeriods.OrderBy(p => p.start).ToList();

            List<string> freeSlots = new List<string>();
            TimeSpan currentTime = beginWorkingTime;

            foreach (var (start, end) in busyPeriods)
            {
                while (currentTime + TimeSpan.FromMinutes(consultationTime) <= start)
                {
                    freeSlots.Add($"{currentTime:hh\\:mm}-{(currentTime + TimeSpan.FromMinutes(consultationTime)):hh\\:mm}");
                    currentTime += TimeSpan.FromMinutes(consultationTime);
                }
                currentTime = end > currentTime ? end : currentTime;
            }

            while (currentTime + TimeSpan.FromMinutes(consultationTime) <= endWorkingTime)
            {
                freeSlots.Add($"{currentTime:hh\\:mm}-{(currentTime + TimeSpan.FromMinutes(consultationTime)):hh\\:mm}");
                currentTime += TimeSpan.FromMinutes(consultationTime);
            }

            return freeSlots.ToArray();
        }

    }
}
