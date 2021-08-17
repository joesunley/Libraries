using Sunley.Miscellaneous;
using System;

namespace Sunley.Orienteering.Navigation
{
    public struct Coordinate
    {
        
        // Fields //

        private double lat, lon;


        // Properties //

        public double Latitude
        {
            get => lat;
            set => lat = value;
        }
        public double Longitude
        {
            get => lon;
            set => lon = value;
        }
        public bool IsEmpty => lat == 0 && lon == 0;


        // Constructors //

        public Coordinate(double pLat, double pLon)
        {
            lat = pLat;
            lon = pLon;
        }
        public Coordinate(PointD point)
        {
            lat = point.X;
            lon = point.Y;
        }
        public Coordinate(string asStr)
        {
            string[] items = asStr.Split(',');

            lat = Convert.ToDouble(items[0].Trim());
            lon = Convert.ToDouble(items[1].Trim());
        }


        // Static Methods //

        public static Coordinate Empty { get => new Coordinate(0, 0); }
        public static Coordinate MinValue { get => new Coordinate(-90, -180); }
        public static Coordinate MaxValue { get => new Coordinate(90, 180); }

        
        public static double DistanceBetween(Coordinate c1, Coordinate c2)
        {
            var p = Math.PI / 180;
            var a =
                0.5 - Math.Cos((c2.Latitude - c1.Latitude) * p) / 2 +
                Math.Cos(c1.Latitude * p) * Math.Cos(c2.Latitude * p) *
                (1 - Math.Cos((c2.Longitude - c1.Longitude) * p)) / 2;

            return 12742 * Math.Asin(Math.Sqrt(a));
        }


        // Overrides //

        public override string ToString()
        {
            return lat.ToString() + ", " + lon.ToString();
        }
    }
}
