using System;
using System.Drawing;

namespace Sunley.Miscellaneous
{
    public static partial class Misc
    {
        /// <summary>
        /// Calculates the straight line distance between points A & B
        /// </summary>
        public static double DistanceBetweenTwoPoints(PointF a, PointF b)
        {
            return Math.Sqrt(Math.Pow(a.X - b.X, 2) + Math.Pow(a.Y - b.Y, 2));
        }

        /// <summary>
        /// Calculates the angle ABC (The non-reflex angle with a centre B)
        /// </summary>
        public static double AngleBetweenThreePoints(PointF a, PointF b, PointF c)
        {
            double
                ab = DistanceBetweenTwoPoints(a, b),
                bc = DistanceBetweenTwoPoints(b, c),
                ac = DistanceBetweenTwoPoints(a, c);

            double
                part1 = Math.Pow(ab, 2) + Math.Pow(bc, 2) - Math.Pow(ac, 2),
                part2 = 2 * ab * bc;

            return Math.Acos(part1 / part2) * (180 / Math.PI);
        }

        /// <summary>
        /// Calculates the area of the triangle formed by A, B & C
        /// </summary>
        public static double AreaOfThreePoints(PointF a, PointF b, PointF c)
        {
            double
                ab = DistanceBetweenTwoPoints(a, b),
                bc = DistanceBetweenTwoPoints(b, c),
                angleAC = AngleBetweenThreePoints(a, b, c);

            return 0.5 * ab * bc * Math.Sin(angleAC);
        }

        public static double DegreesToRadians(double deg) { return deg * (Math.PI / 180); }
        public static double RadiansToDegrees(double rad) { return DegreesToRadians(rad); }
    }

    public struct PointD
    {

        // Fields //
        private double x, y;

        
        // Properties //

        public double X
        {
            get { return x; }
            set { x = value; }
        }
        public double Y
        {
            get { return y; }
            set { y = value; }
        }

        public bool IsEmpty
        {
            get => x == 0 && y == 0;
        }

        // Constructors //

        public PointD(double pX, double pY)
        {
            x = pX;
            y = pY;
        }
        public PointD(PointF pointF)
        {
            x = pointF.X;
            y = pointF.Y;
        }
        public PointD(Point point)
        {
            x = point.X;
            y = point.Y;
        }


        // Methods //

        public PointD Add(SizeF s)
        {
            x += s.Width;
            y += s.Height;

            return this;
        }
        public PointD Add(Size s)
        {
            x += s.Width;
            y += s.Height;

            return this;
        }

        public PointD Subtract(SizeF s)
        {
            x -= s.Width;
            y -= s.Height;

            return this;
        }
        public PointD Subtract(Size s)
        {
            x -= s.Width;
            y -= s.Height;

            return this;
        }

        public PointD Scale(double s)
        {
            x *= s;
            y *= s;

            return this;
        }


        // Override Methods //

        public override bool Equals(object obj)
        {
            if (!(obj is PointD)) { return false; }
            PointD pt = (PointD)obj;
            return
                pt.X == x &&
                pt.Y == y &&
                pt.GetType().Equals(this.GetType());
        }
        public override int GetHashCode()
        {
            return Tuple.Create(x, y).GetHashCode();
        }


        // Static Methods //

        public static double Distance(PointD a, PointD b)
        {
            return Math.Sqrt(Math.Pow(a.X - b.X, 2) + Math.Pow(a.Y - b.Y, 2));
        }
        public static double Angle(PointD a, PointD b, PointD c)
        {
            double
                ab = Distance(a, b),
                bc = Distance(b, c),
                ac = Distance(a, c);

            double
                part1 = Math.Pow(ab, 2) + Math.Pow(bc, 2) - Math.Pow(ac, 2),
                part2 = 2 * ab * bc;

            return Math.Acos(part1 / part2);
        }
        public static double Area(PointD a, PointD b, PointD c)
        {
            double
                            ab = Distance(a, b),
                            bc = Distance(b, c),
                            angleAC = Angle(a, b, c);

            return 0.5 * ab * bc * Math.Sin(angleAC);
        }


        // Static Properties //

        public static PointD Empty { get => new PointD(0, 0); }
        public static PointD MinValue { get => new PointD(double.MinValue, double.MinValue); }
        public static PointD MaxValue { get => new PointD(double.MaxValue, double.MaxValue); }

        // Operators //

        public static bool operator ==(PointD left, PointD right)
        {
            return left.X == right.X && left.Y == right.Y;
        }
        public static bool operator ==(PointD left, PointF right)
        {
            return left.X == right.X && left.Y == right.Y;
        }
        public static bool operator ==(PointD left, Point right)
        {
            return left.X == right.X && left.Y == right.Y;
        }

        public static bool operator !=(PointD left, PointD right)
        {
            return !(left == right);
        }
        public static bool operator !=(PointD left, PointF right)
        {
            return !(left == right);
        }
        public static bool operator !=(PointD left, Point right)
        {
            return !(left == right);
        }


        public static PointD operator +(PointD left, PointD right)
        {
            return new PointD(left.X + right.X, left.Y + right.Y);
        }
        public static PointD operator +(PointD left, SizeF right)
        {
            return new PointD(left.X + right.Width, left.Y + right.Height);
        }
        public static PointD operator +(PointD left, Size right)
        {
            return new PointD(left.X + right.Width, left.Y + right.Height);
        }

        public static PointD operator -(PointD left, PointD right)
        {
            return new PointD(left.X - right.X, left.Y - right.Y);
        }
        public static PointD operator -(PointD left, SizeF right)
        {
            return new PointD(left.X - right.Width, left.Y - right.Height);
        }
        public static PointD operator -(PointD left, Size right)
        {
            return new PointD(left.X - right.Width, left.Y - right.Height);
        }

        public static PointD operator *(PointD left, double right)
        {
            return new PointD(left.X * right, left.Y * right);
        }
        public static PointD operator /(PointD left, double right)  
        {
            return new PointD(left.X / right, left.X / right);
        }
    }

    public struct Vector
    {

        // Fields //

        private double x, y;


        // Properties //

        public double X
        {
            get => x;
            set => x = value;
        }

        public double Y
        {
            get => y;
            set => y = value;
        }

        public bool IsEmpty
        {
            get => x == 0 && y == 0;
        }

        // Constructors //

        public Vector(float pX, float pY)
        {
            x = pX;
            y = pY;
        }
        public Vector(double pX, double pY)
        {
            x = pX;
            y = pY;
        }
        public Vector(PointD point)
        {
            x = point.X;
            y = point.Y;
        }
        public Vector(PointF point)
        {
            x = point.X;
            y = point.Y;
        }
        public Vector(Point point)
        {
            x = point.X;
            y = point.Y;
        }


        // Static Properties //

        public static Vector Empty { get => new Vector(PointD.Empty); }
        public static Vector MinValue { get => new Vector(PointD.MinValue); }
        public static Vector MaxValue { get => new Vector(PointD.MaxValue); }


        // Methods //

        public Vector Add(Size s)
        {
            x += s.Width;
            y += s.Height;

            return this;
        }
        public Vector Add(SizeF s)
        {
            x += s.Width;
            y += s.Height;

            return this;
        }

        public Vector Subtract(Size s)
        {
            x -= s.Width;
            y -= s.Height;

            return this;
        }
        public Vector Subtract(SizeF s)
        {
            x -= s.Width;
            y -= s.Height;

            return this;
        }

        public Vector Scale(double s)
        {
            x *= s;
            y *= s;

            return this;
        }
        public Vector Scale(float s)
        {
            x *= s;
            y *= s;

            return this;
        }
        public Vector Scale(int s)
        {
            x *= s;
            y *= s;

            return this;
        }

        public double Magnitude()
        {
            return Magnitude(this);
        }


        // Override Methods //

        public override bool Equals(object obj)
        {
            if (!(obj is Vector)) return false;
            Vector v = (Vector)obj;
            return
                v.X == x &&
                v.Y == y &&
                v.GetType().Equals(GetType());
        }
        public override int GetHashCode()
        {
            return Tuple.Create(x, y).GetHashCode();
        }


        // Static Methods //

        public static double Magnitude(Vector v)
        {
            return Math.Sqrt(Math.Pow(v.X, 2) + Math.Pow(v.Y, 2));
        }
    }
}
