using Sunley.Orienteering.Results.Visuals;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Sunley.Orienteering.Results
{
    public class ResultsFile
    {
        private string
            name,
            club,
            course;
        private int legs;
        private DateTime date;
        private List<Result> results = new List<Result>();


        // Properties //
        public string Name => name;
        public string Club => club;
        public string Course => course;
        public int Legs => legs;
        public DateTime Date => date;
        public List<Result> Results => results;


        // Constructors //
        public ResultsFile()
        {
            results = new List<Result>();
        }
        public ResultsFile(string asStr)
        {
            string[] lines = asStr.Split(';');
            string[] firstLine = lines[0].Split(',');

            name = firstLine[0];
            club = firstLine[1];
            course = firstLine[2];
            date = Convert.ToDateTime(firstLine[3]);

            for (int i = 1; i < lines.Length; i++) { results.Add(new Result(lines[i])); }
        }
        public ResultsFile(string n, string c, DateTime d)
        {
            name = n;
            club = c;
            date = d;
        }
        public ResultsFile(string n, string cl, string co, int l, DateTime d, List<Result> r)
        {
            name = n;
            club = cl;
            course = co;
            date = d;
            results = r;
        }


        // Methods //
        public List<float> GetFastestSplits()
        {
            List<float> fastest = new List<float>();
            for (int i = 0; i < Legs; i++)
            {
                float leader = 99999999f;

                foreach (Result r in Results)
                {
                    float time = r.Splits[i];
                    if (time < leader)
                    {
                        leader = time;
                    }
                }
                fastest.Add(leader);
            }
            return fastest;
        }
        public List<float> GetSplits(int position)
        {
            return Results[position - 1].Splits;
        }
        public List<float> GetSplits(string name)
        {
            int pos = GetPosition(name);
            if (pos != -1)
            {
                return GetSplits(pos);
            }
            throw new Exception("Could not find Player");
        }
        public List<float> GetTimeBehindSplits(int position)
        {
            List<float> fastest = GetFastestSplits();
            List<float> players = GetSplits(position);
            List<float> tBehind = new List<float>();

            for (int i = 0; i < fastest.Count; i++)
            {
                float diff = players[i] - fastest[i];
                tBehind.Add(diff);
            }
            return tBehind;
        }
        public float[][] GetRawSplits(bool byPosition)
        {
            throw new NotImplementedException();
        }
        public bool CheckName(string name)
        {
            foreach (Result r in Results)
            {
                if (r.Name.ToLower() == name.ToLower()) { return true; }
            }
            return false;
        }
        public string[] GetNames()
        {
            List<string> n = new List<string>();
            foreach (Result r in Results)
            {
                n.Add(r.Name.ToLower());
            }
            return n.ToArray();
        }
        public float GetFastestTime()
        {
            Result fastest = new Result();
            float time = 999999f;

            foreach (Result r in Results)
            {
                if (r.GetTime() < time)
                {
                    fastest = r;
                    time = r.GetTime();
                }
            }
            return fastest.GetTime();
        }
        public Result GetFastest()
        {
            Result fastest = new Result();
            float time = 999999f;

            foreach (Result r in Results)
            {
                if (r.GetTime() < time)
                {
                    fastest = r;
                    time = r.GetTime();
                }
            }
            return fastest;
        }
        public Result GetResults(int position)
        {
            return Results[position - 1];
        }
        public Result GetResults(string name)
        {
            int pos = GetPosition(name);
            if (pos != -1)
            {
                return Results[pos - 1];
            }
            throw new Exception("Could Not Find Player");
        }
        public int GetPosition(Result t)
        {
            return Results.IndexOf(t);
        }
        public int GetPosition(string name)
        {
            foreach (Result t in Results)
            {
                if (t.Name == name)
                    return Results.IndexOf(t) + 1;
            }
            return -1;
        }

        public void ShowResults()
        {
            GetForm().ShowDialog();
        }
        public ResultsView GetControl()
        {
            return new ResultsView(this);
        }
        public Form GetForm()
        {
            try
            {
                ResultsView c = new ResultsView(this);
                Form f = new Form()
                {
                    FormBorderStyle = FormBorderStyle.FixedSingle,
                    ShowInTaskbar = false,
                    Text = c.Text,
                    ShowIcon = false,
                    Size = c.Size
                };

                f.Controls.Add(c);
                return f;
            }
            catch { return new Form(); }



        }

        // Overrides //
        public override string ToString()
        {
            string output = Name + "," + Club + "," + Course + "," + Date.ToLongDateString() + ";";
            foreach (Result r in Results) { output += r.ToString() + ";"; }
            return output.Substring(0, output.Length - 1);

        }


        // Statics //
        public static ResultsFile Null = new ResultsFile();


        // Other //
        public class Result
        {
            private string name;
            private string club;
            private List<float> splits = new List<float>();

            // Properties //
            public string Name => name;
            public string Club => club;
            public List<float> Splits => splits;


            // Constructors //
            public Result()
            {
                
            }
            public Result(string asStr)
            {
                string[] vars = asStr.Split(',');

                name = vars[0];
                club = vars[1];

                for (int i = 2; i < vars.Length; i++) { splits.Add((float)Convert.ToDouble(vars[i])); }
            }
            public Result(string n, string c)
            {
                name = n;
                club = c;
            }
            public Result(string n, string c, List<float> s)
            {
                name = n;
                club = c;
                splits = s;
            }


            // Methods //
            public float GetTime()
            {
                float total = 0;
                foreach (float l in Splits) { total += l; }
                return total;
            }
            public float GetTime(int endControl)
            {
                float total = 0;
                for (int i = 0; i < endControl; i++) { total += Splits[i]; }
                return total;
            }
            public float GetTime(int startControl, int endControl)
            {
                float total = 0;
                for (int i = (startControl - 1); i < endControl; i++) { total += Splits[i]; }
                return total;
            }


            // Overrides //
            public override string ToString()
            {
                string output = Name + "," + Club;

                foreach (float s in Splits) { output += "," + s.ToString(); }

                return output;
            }
        }
    }
}
