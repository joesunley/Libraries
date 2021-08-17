using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sunley.Orienteering.Results
{
    public class RaceResults
    {

        // Fields //
        ResultsFile.Result[] results = new ResultsFile.Result[2];
        ResultsFile resultsFile;


        // Constructors //
        public RaceResults(ResultsFile r, string n)
        {
            results[0] = r.GetFastest();
            results[1] = r.GetResults(n);
            resultsFile = r;
        }
        public RaceResults(ResultsFile r, int pos)
        {
            results[0] = r.GetFastest();
            results[1] = r.GetResults(pos);
            resultsFile = r;
        }


        // Methods //
        public List<float> GetSplitsBehind()
        {
            List<float> tBehind = new List<float>();

            for (int i = 0; i < resultsFile.Legs; i++)
            {
                float
                    w = results[0].Splits[i],
                    t = results[1].Splits[i];

                tBehind.Add(t - w);
            }
            return tBehind;
        }
        public List<float> GetPercentBehind()
        {
            List<float> pBehind = new List<float>();

            for (int i = 0; i < resultsFile.Legs; i++)
            {
                float
                    w = results[0].Splits[i],
                    t = results[1].Splits[i];

                float percent = (t - w) / w;
                pBehind.Add(percent);
            }
            return pBehind;
        }
    }
}
