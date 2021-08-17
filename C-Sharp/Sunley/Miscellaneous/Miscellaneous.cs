using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;

namespace Sunley.Miscellaneous
{
    public static partial class Misc
    {
        public static int GradeConstant { get; set; } = 5;

        public static string GetHtml(string url)
        {
            StreamReader instream;
            WebRequest webrequest;
            WebResponse webresponse;

            webrequest = WebRequest.Create(url);
            webresponse = webrequest.GetResponse();
            instream = new StreamReader(webresponse.GetResponseStream());

            return instream.ReadToEnd().ToString();
        }

        public static double TimeToDouble(string t)
        {
            if (t.Contains(','))
            {
                try
                {
                    string[] s = t.Split(',');
                    if (s.Length == 2)
                    {
                        double
                            min = Convert.ToDouble(s[0]),
                            sec = Convert.ToDouble(s[1]);
                        return min + (sec / 60);
                    }
                    else if (s.Length == 3)
                    {
                        double
                            hour = Convert.ToDouble(s[0]),
                            min = Convert.ToDouble(s[1]),
                            sec = Convert.ToDouble(s[2]);
                        return (hour * 60) + min + (sec / 60);
                    }
                    else { throw new InvalidDataException(); }
                }
                catch { return -1; }
            }
            else if (t.Contains('.'))
            {
                try
                {
                    string[] s = t.Split('.');
                    if (s.Length == 2)
                    {
                        double
                            min = Convert.ToDouble(s[0]),
                            sec = Convert.ToDouble(s[1]);
                        return min + (sec / 60);
                    }
                    else if (s.Length == 3)
                    {
                        double
                            hour = Convert.ToDouble(s[0]),
                            min = Convert.ToDouble(s[1]),
                            sec = Convert.ToDouble(s[2]);
                        return (hour * 60) + min + (sec / 60);
                    }
                    else { throw new InvalidDataException(); }
                }
                catch { return -1; }
            }
            else { return -1; }
        }

        public static DateTime CreateDate(string d)
        {
            string[] s = d.Split('/');

            int
                day = Convert.ToInt32(s[0]),
                month = Convert.ToInt32(s[1]),
                year = Convert.ToInt32(s[2]);

            return new DateTime(year, month, day);
        }

        /// <summary>
        /// Converts a speed in m/s to a pace in min/km
        /// </summary>
        /// <param name="speed">Speed in Metres per Second</param>
        /// <returns></returns>
        public static double SpeedToPace(double speed) { return 1 / (speed * 60 / 1000); }

        public static string PaceToString(double pace)
        {
            double min = Math.Floor(pace);
            double dif = pace - min;

            try { return min.ToString("F0") + ":" + Convert.ToInt64((dif * 60)).ToString("D2"); }
            catch { return "60:00"; }
        }
    }
}
