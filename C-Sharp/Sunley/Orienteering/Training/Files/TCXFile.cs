using Sunley.Orienteering.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Sunley.Orienteering.Training.ActivityFiles
{
    public class TCXFile : BaseActivityFile
    {

        #region -  Fields  -
        private string name = null;
        private string sport = null;
        private DateTime startTime = new DateTime();
        private TimeSpan totalTime = new TimeSpan();
        private float totalDistance = -1;
        private float totalCalories = -1;

        private List<TrackPoint> trackPoints = new List<TrackPoint>();

        private List<DateTime> timeStream = new List<DateTime>();
        private List<int> heartRateStream = new List<int>();
        private List<int> cadenceStream = new List<int>();
        private List<float> speedStream = new List<float>();
        private List<float> altitudeStream = new List<float>();
        private List<float> distanceStream = new List<float>();
        private List<Coordinate> positionStream = new List<Coordinate>();

        private List<Lap> lapStream = new List<Lap>();
        #endregion


        #region -  Properties  -
        public override string Name => name;
        public override string Sport => sport;
        public override DateTime StartTime => startTime;
        public override TimeSpan TotalTime => totalTime;
        public override float TotalDistance => totalDistance;
        public override float TotalCalories => totalCalories;
        public override List<DateTime> Time => timeStream;
        public override List<int> HeartRate => heartRateStream;
        public override List<int> Cadence => cadenceStream;
        public override List<float> Speed => speedStream;
        public override List<float> Altitude => altitudeStream;
        public override List<float> Distance => distanceStream;
        public override List<Coordinate> Position => positionStream;
        public List<Lap> Laps => lapStream;
        #endregion


        #region -  Constructors  -
        public TCXFile(string filePath, bool y)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(filePath);

            XmlNode activity = doc.ChildNodes[1].FirstChild.FirstChild;

            bool lapped = false;

            sport = activity.Attributes[0].Value;

            foreach (XmlNode node in activity.ChildNodes)
                if (node.Name.ToLower().Contains("lap"))
                    lapped = true;

            if (lapped)
            {
                int laps = activity.ChildNodes.Count - 2;
                for (int i = 1; i < activity.ChildNodes.Count - 1; i++)
                {
                    XmlNode lap = activity.ChildNodes[i];
                    DateTime lapStart = Convert.ToDateTime(lap.Attributes[0].Value);



                    XmlNodeList track = lap.ChildNodes[8].ChildNodes;

                    foreach (XmlNode tP in track)
                    {
                        TrackPoint t = new TrackPoint();

                        t.Time = Convert.ToDateTime(tP.ChildNodes[0].InnerText);

                        string
                            lat = tP.ChildNodes[1].ChildNodes[0].InnerText,
                            lon = tP.ChildNodes[1].ChildNodes[1].InnerText;
                        t.Position = new Coordinate(Convert.ToDouble(lat), Convert.ToDouble(lon));

                        t.Altitude = (float)Convert.ToDouble(tP.ChildNodes[2].InnerText);
                        t.Distance = (float)Convert.ToDouble(tP.ChildNodes[3].InnerText);
                        t.HeartRate = Convert.ToInt32(tP.ChildNodes[4].ChildNodes[0].InnerText);

                        XmlNode ext = tP.ChildNodes[5].ChildNodes[0];
                        t.Speed = (float)Convert.ToDouble(ext.ChildNodes[0].InnerText);
                        t.Cadence = Convert.ToInt32(ext.ChildNodes[1].InnerText);

                        totalDistance += t.Distance;

                        trackPoints.Add(t);
                    }

                    lapStream.Add(new Lap(i - 1, lapStart));
                }

                CreateStreams();

                totalTime = timeStream.Last() - timeStream.First();
            }
        }
        public TCXFile(string filePath)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(filePath);

            sport = doc.ChildNodes[1].FirstChild.FirstChild.Attributes[0].Value;

            XmlNode root = doc.ChildNodes[1].FirstChild.FirstChild.ChildNodes[1];

            startTime = DateTime.Parse(root.Attributes[0].Value);
            //totalTime = (float)Convert.ToDouble(root.ChildNodes[0].InnerText);
            totalDistance = (float)Convert.ToDouble(root.ChildNodes[1].InnerText);
            totalCalories = (float)Convert.ToDouble(root.ChildNodes[3].InnerText);

            XmlNode tracks = root.ChildNodes[8];

            for (int i = 0; i < tracks.ChildNodes.Count; i++)
            {
                XmlNode node = tracks.ChildNodes[i];
                TrackPoint t = new TrackPoint();

                string rawPosition = "<position>" + node.ChildNodes[1].InnerXml + "</position>";
                XmlDocument positionDoc = new XmlDocument();
                positionDoc.LoadXml(rawPosition);
                XmlNode positionNode = positionDoc.FirstChild;

                string lat, lon;
                Coordinate pos = new Coordinate();
                try
                {
                    lat = positionNode.ChildNodes[0].InnerText;
                    lon = positionNode.ChildNodes[1].InnerText;
                    pos.Latitude = Convert.ToDouble(lat);
                    pos.Longitude = Convert.ToDouble(lon);
                }
                catch { pos.Latitude = -1; pos.Longitude = -1; }



                string rawSC = "";


                if (node.ChildNodes.Count == 5)
                {
                    rawSC = "<Extensions>" + node.ChildNodes[4].FirstChild.InnerXml + "</Extensions>";
                    t.Altitude = (float)Convert.ToDouble(node.ChildNodes[1].InnerText);
                    t.Distance = (float)Convert.ToDouble(node.ChildNodes[2].InnerText);
                    t.HeartRate = Convert.ToInt32(node.ChildNodes[3].FirstChild.InnerText);
                }
                else
                {
                    rawSC = "<Extensions>" + node.ChildNodes[5].FirstChild.InnerXml + "</Extensions>";
                    t.Altitude = (float)Convert.ToDouble(node.ChildNodes[2].InnerText);
                    t.Distance = (float)Convert.ToDouble(node.ChildNodes[3].InnerText);
                    t.HeartRate = Convert.ToInt32(node.ChildNodes[4].FirstChild.InnerText);
                }
                XmlDocument extDoc = new XmlDocument();
                extDoc.LoadXml(rawSC);
                XmlNode extNode = extDoc.FirstChild;
                string speed = extNode.ChildNodes[0].InnerText;
                string cadence = extNode.ChildNodes[1].InnerText;

                t.Index = i;
                t.Time = DateTime.Parse(node.FirstChild.InnerText);
                t.Position = pos;

                t.Speed = (float)Convert.ToDouble(speed);
                t.Cadence = Convert.ToInt32(Convert.ToDouble(cadence) * 2);

                trackPoints.Add(t);
            }

            CreateStreams();

            totalTime = timeStream.Last() - timeStream.Last();
        }
        private void CreateStreams()
        {

            foreach (TrackPoint t in trackPoints)
            {
                timeStream.Add(t.Time);
                heartRateStream.Add(t.HeartRate);
                cadenceStream.Add(t.Cadence);
                speedStream.Add(t.Speed);
                altitudeStream.Add(t.Altitude);
                distanceStream.Add(t.Distance);
                positionStream.Add(t.Position);
            }
        }
        public TCXFile() { }
        #endregion


        #region -  Methods  -
        public override int AverageHeartRate() { return Convert.ToInt32(Math.Round(heartRateStream.Average())); }
        public override int MaxHeartRate() { return heartRateStream.Max(); }
        public override int AverageCadence() { return Convert.ToInt32(Math.Round(cadenceStream.Average())); }
        public override int MaxCadence() { return cadenceStream.Max(); }
        public override float AverageSpeed() { return speedStream.Average(); }
        public override float MaxSpeed() { return speedStream.Max(); }
        public override float MinElevation() { return altitudeStream.Min(); }
        public override float MaxElevation() { return altitudeStream.Max(); }
        public override float ElevationGain()
        {
            float gain = 0;

            for (int i = 1; i < altitudeStream.Count; i++)
            {
                float delta = altitudeStream[i] - altitudeStream[i - 1];
                if (delta > 0) { gain += delta; }
            }

            return gain;
        }
        public override float ElevationLoss()
        {
            float loss = 0;

            for (int i = 1; i < altitudeStream.Count; i++)
            {
                float delta = altitudeStream[i] - altitudeStream[i - 1];
                if (delta < 0) { loss += delta; }
            }

            return loss;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="indexFrom1">The Lap number (starting from 1)</param>
        /// <returns></returns>
        public TCXFile GetLap(int indexFrom1)
        {
            TCXFile l = new TCXFile();
            l.name = Name + ": Lap " + indexFrom1;
            l.sport = Sport;

            Lap lap = lapStream[indexFrom1 - 1];
            DateTime finishTime = lapStream[indexFrom1].StartTime;

            int lapStart = timeStream.IndexOf(lap.StartTime),
                lapEnd = timeStream.IndexOf(finishTime);

            l.totalTime = (finishTime - startTime);
            l.startTime = lap.StartTime;

            if (lapStart != -1 && lapEnd != -1)
            {
                for (int j = lapStart; j < lapEnd; j++)
                {
                    l.timeStream.Add(timeStream[j]);
                    l.heartRateStream.Add(heartRateStream[j]);
                    l.cadenceStream.Add(cadenceStream[j]);
                    l.speedStream.Add(speedStream[j]);
                    l.altitudeStream.Add(altitudeStream[j]);
                    l.distanceStream.Add(distanceStream[j]);
                    l.positionStream.Add(positionStream[j]);
                }
            }
            return l;
        }
        public TCXFile GetLap(Lap lap)
        {
            int index = lapStream.IndexOf(lap);
            return GetLap(index + 1);
        }
        #endregion


        #region -  Overrides  -
        public override string ToString()
        {
            return base.ToString();
        }
        #endregion


        #region -  Structures  -
        private struct TrackPoint
        {
            public int Index { get; set; }
            public DateTime Time { get; set; }
            public Coordinate Position { get; set; }
            public float Altitude { get; set; }
            public float Distance { get; set; }
            public int HeartRate { get; set; }
            public float Speed { get; set; }
            public int Cadence { get; set; }
        }

        public struct Lap
        {
            public int Index { get; set; }
            public DateTime StartTime { get; set; }

            public Lap(int index, DateTime startTime)
            {
                Index = index;
                StartTime = startTime;
            }
            public Lap(string asStr)
            {
                string[] items = asStr.Split(',');

                Index = Convert.ToInt32(items[0].Trim());
                StartTime = Convert.ToDateTime(items[1].Trim());
            }

            public override string ToString()
            {
                return Index + ", " + StartTime.ToUniversalTime();
            }
        }
        #endregion
    }
}
