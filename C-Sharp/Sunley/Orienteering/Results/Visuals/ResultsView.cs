using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Sunley.Orienteering.Results;

namespace Sunley.Orienteering.Results.Visuals
{
    public partial class ResultsView : UserControl
    {
        private ResultsFile r;

        
        public ResultsView(ResultsFile resultsFile)
        {
            SetTopLevel(false);


            if (resultsFile == ResultsFile.Null)
                throw new ArgumentNullException("resultsFile", "ResultsFile cannot be an empty file");
            
            InitializeComponent();

            r = resultsFile;
            Text = r.Name;

            FillDGV();
        }
        
        [Category("Results"), Description("The ResultsFile to view")]
        public ResultsFile ResultsFile
        {
            get { return r; }

            set
            {
                if (value == ResultsFile.Null)
                    throw new ArgumentNullException("value", "ResultsFile cannot be an empty file");

                r = value;
                Text = r.Name;

                FillDGV();
                Invalidate();
            }
        }

        private void FillDGV()
        {
            Columns();

            foreach (var t in r.Results)
                dgv.Rows.Add(CreateRow(t));

            dgv.Update();
        }

        private void Columns()
        {
            dgv.Columns.Add("position", "Position");
            dgv.Columns.Add("name", "Name");
            dgv.Columns.Add("club", "Club");
            dgv.Columns.Add("time", "Time");

            for (int i = 0; i < r.Legs; i++)
            {
                dgv.Columns.Add("leg" + i, "Leg " + (i + 1));
            }
        }

        string[] CreateRow(ResultsFile.Result t)
        {
            List<string> row = new List<string>();

            row.Add((r.GetPosition(t) + 1).ToString());
            row.Add(t.Name);
            row.Add(t.Club);


            float tBehind = t.GetTime() - r.GetFastestTime();
            TimeSpan time = TimeSpan.FromMinutes(t.GetTime());
            TimeSpan timeBehind = TimeSpan.FromMinutes(tBehind);
            string
                sTime = Math.Truncate(time.TotalMinutes).ToString("F0") + ":" + time.ToString("ss"),
                sTBehind = Math.Truncate(timeBehind.TotalMinutes).ToString("F0") + ":" + timeBehind.ToString("ss");
            if (tBehind == 0)
                row.Add(sTime);
            else
                row.Add(sTime + " + " + sTBehind);

            foreach (float f in t.Splits)
                row.Add(TimeSpan.FromMinutes(f).ToString(@"m\:ss"));

            return row.ToArray();
        }
    }
}
