using System.Collections.Generic;

namespace Sunley.Orienteering.Results.Returns
{
    public static class Statistics
    {
        public static double AvgPercentBehind(ResultsFile r, int position)
        {
            List<float> timeBehind = r.GetTimeBehindSplits(position);
            List<float> fastestTime = r.GetFastestSplits();
            float total = 0;

            for (int i = 0; i < timeBehind.Count; i++) { total += timeBehind[i] / fastestTime[i]; }

            return total / (timeBehind.Count);
        }
    }

    public static class Charts
    {

    }
}
