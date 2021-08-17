using System;
using System.Collections.Generic;
using System.Linq;
using Sunley.Orienteering.Navigation;
using System.Xml;

namespace Sunley.Orienteering.Training.ActivityFiles
{
    public class GPXFile : BaseActivityFile
    {

        #region -  Fields  -
        private string name = null;
        private string sport = null;
        private DateTime startTime = new DateTime();
        private TimeSpan totalTime = new TimeSpan();
        private float totalDisance = -1;

        private readonly List<TrackPoint> trackPoints = new List<TrackPoint>();

        private readonly List<DateTime> timeStream = new List<DateTime>();
        private readonly List<int> heartRateStream = new List<int>();
        private readonly List<int> cadenceStream = new List<int>();
        private readonly List<float> altitudeStream = new List<float>();
        private readonly List<float> temperatureStream = new List<float>();
        private readonly List<Coordinate> positionStream = new List<Coordinate>();

        private readonly List<float> distanceStream = new List<float>();
        private readonly List<float> speedStream = new List<float>();
        #endregion


        #region -  Properties  -
        public override string Name => name;
        public override string Sport => sport;
        public override DateTime StartTime => startTime;
        public override TimeSpan TotalTime => totalTime;
        public override float TotalDistance => totalDisance;
        public override List<DateTime> Time => timeStream;
        public override List<int> HeartRate => heartRateStream;
        public override List<int> Cadence => cadenceStream;
        public override List<float> Altitude => altitudeStream;
        public override List<float> Temperature => temperatureStream;
        public override List<Coordinate> Position => positionStream;
        public override List<float> Distance => distanceStream;
        public override List<float> Speed => speedStream;
        #endregion


        #region -  Constructors  -
        public GPXFile(string filePath)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(filePath);

            XmlNode metadata = doc.ChildNodes[1].ChildNodes[0];
            XmlNode trackData = doc.ChildNodes[1].ChildNodes[1];

            startTime = Convert.ToDateTime(metadata.ChildNodes[1].InnerText);
            name = trackData.ChildNodes[0].InnerText;
            sport = trackData.ChildNodes[1].InnerText;

            XmlNodeList points = trackData.ChildNodes[2].ChildNodes;

            int index = 0;
            foreach (XmlNode point in points)
            {
                TrackPoint trackPoint = new TrackPoint();
                trackPoint.Index = index;
                index++;

                string
                    lat = point.Attributes[0].Value,
                    lon = point.Attributes[1].Value;
                Coordinate pos = new Coordinate(Convert.ToDouble(lat), Convert.ToDouble(lon));

                string altitude = point.ChildNodes[0].InnerText;
                string time = point.ChildNodes[1].InnerText;

                XmlNode extensions = point.ChildNodes[2].ChildNodes[0];

                string
                    temp = extensions.ChildNodes[0].InnerText,
                    heartRate = extensions.ChildNodes[1].InnerText,
                    cadence = extensions.ChildNodes[2].InnerText;

                trackPoint.Time = Convert.ToDateTime(time);
                trackPoint.Position = pos;
                trackPoint.Altitude = (float)Convert.ToDouble(altitude);

                trackPoint.HeartRate = Convert.ToInt32(heartRate);

                trackPoint.Cadence = Convert.ToInt32(cadence);
                trackPoint.Temperature = (float)Convert.ToDouble(temp);

                trackPoints.Add(trackPoint);
            }

            CreateStreams();

            float dist = 0;
            foreach (float f in distanceStream)
                dist += f;

            TimeSpan tTaken = timeStream.Last() - timeStream[0];

            totalDisance = dist;
            totalTime = tTaken;
        }
        private void CreateStreams()
        {
            TrackPoint first = trackPoints[0];

            timeStream.Add(first.Time);
            heartRateStream.Add(first.HeartRate);
            cadenceStream.Add(first.Cadence);
            altitudeStream.Add(first.Altitude);
            temperatureStream.Add(first.Temperature);
            positionStream.Add(first.Position);
            distanceStream.Add(0);
            speedStream.Add(0);


            for (int i = 1; i < trackPoints.Count; i++)
            {
                TrackPoint current = trackPoints[i];
                TrackPoint previous = trackPoints[i - 1];

                timeStream.Add(current.Time);
                heartRateStream.Add(current.HeartRate);
                cadenceStream.Add(current.Cadence);
                altitudeStream.Add(current.Altitude);
                temperatureStream.Add(current.Temperature);
                positionStream.Add(current.Position);

                double dist = Coordinate.DistanceBetween(previous.Position, current.Position);
                TimeSpan time = current.Time - previous.Time;

                double speed = (dist * 1000) / time.TotalSeconds;

                distanceStream.Add((float)dist);
                speedStream.Add((float)speed);
            }
        }
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
            public int HeartRate { get; set; }
            public int Cadence { get; set; }
            public float Temperature { get; set; }
        }
        #endregion
    }
}
