using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sunley.Miscellaneous;

namespace Sunley.Orienteering.Results
{
    public static class Winsplits
    {
        // Completed - Tested //
        public static ResultsFile GetResults(string url)
        {
            string raw = Misc.GetHtml(url);
            string[] rawData = raw.Split(new string[] { "\r\n" }, StringSplitOptions.None);

            return CreateResultsFile(rawData);
        }
        public static ResultsFile GetResults(int databaseID, int categoryID)
        {
            string url = "http://obasen.orientering.se/winsplits/online/en/export_text.asp?databaseId=" + databaseID + "&categoryId=" + categoryID;

            string raw = Misc.GetHtml(url);
            string[] rawData = raw.Split(new string[] { "\r\n" }, StringSplitOptions.None);

            return CreateResultsFile(rawData);
        }

        private static ResultsFile CreateResultsFile(string[] rawData)
        {
            List<string> lines = rawData.ToList();

            string header = lines[0];
            header = header.Substring(header.IndexOf("<TITLE>") + 7);
            header = header.Substring(0, header.IndexOf("</TITLE>"));
            header = header.Substring(header.IndexOf('-') + 2);

            string[] details = header.Split(',');
            string[] cd = details[1].Split('[');

            string name = details[0].Trim();
            string course = details[2].Trim();
            string club = cd[0].Trim();
            DateTime date = Misc.CreateDate(cd[1].Trim().Substring(0, cd[1].Trim().Length - 1));

            lines.RemoveRange(0, 2); lines.RemoveAt(lines.Count - 1);

            List<ResultsFile.Result> results = new List<ResultsFile.Result>();

            for (int i = 0; i < lines.Count; i += 2)
            {
                string line = lines[i];
                List<string> split = line.Split(new string[] { "\t" }, StringSplitOptions.None).ToList();

                string na = split[1];
                string cl = lines[i + 1].Split(new string[] { "\t" }, StringSplitOptions.None).ToList()[1];

                split.RemoveRange(0, 2); split.RemoveRange(split.Count - 2, 2);


                List<float> splits = new List<float>();
                foreach (string l in split)
                {
                    float d = (float)Misc.TimeToDouble(l);
                    splits.Add(d);
                }

                results.Add(new ResultsFile.Result(na, cl, splits));
            }
            int legs = results[0].Splits.Count;

            return new ResultsFile(name, club, course, legs, date, results);
        }
    }
}
