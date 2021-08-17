using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.IO;
using System.Diagnostics;

namespace Sunley.PurplePen.v1
{

    public static class XML
    {
        public static void CreateControls(string filename, ref List<ControlPoint> controls, ref List<SpecialControl> specialControls)
        {
            int startCount = 1;
            int finishCount = 1;

            XmlDocument doc = new XmlDocument();
            doc.Load(filename);

            XmlNode root = doc.FirstChild;

            if (root.HasChildNodes)
            {
                foreach (XmlNode node in root.ChildNodes)
                {
                    if (node.Name == "control")
                    {
                        if (node.ChildNodes[0].Name != "code")
                        {
                            SpecialControl sC = new SpecialControl();

                            switch (node.Attributes[1].Value)
                            {
                                case "start":
                                    sC.Type = Type.start;
                                    sC.ControlCode = Convert.ToInt32(Math.Ceiling(Convert.ToDouble((root.ChildNodes.Count) / 100)) * 100) + 100 + startCount;
                                    startCount++;
                                    break;
                                case "finish":
                                    sC.Type = Type.finish;
                                    sC.ControlCode = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(root.ChildNodes.Count / 100)) * 100) + 200 + finishCount;
                                    finishCount++;
                                    break;
                                default:
                                    break;
                            }

                            sC.ID = Convert.ToInt32(node.Attributes[0].Value);

                            XmlNode child = node.ChildNodes[0];
                            sC.X = Convert.ToDouble(child.Attributes[0].Value);
                            sC.Y = Convert.ToDouble(child.Attributes[1].Value);

                            specialControls.Add(sC);
                        }
                        else
                        {
                            ControlPoint c = new ControlPoint();
                            c.ID = Convert.ToInt32(node.Attributes[0].Value);
                            c.ControlCode = Convert.ToInt32(node.ChildNodes[0].InnerText);

                            XmlNode child = node.ChildNodes[1];
                            c.X = Convert.ToDouble(child.Attributes[0].Value);
                            c.Y = Convert.ToDouble(child.Attributes[1].Value);

                            controls.Add(c);

                        }

                    }
                }
            }

        }

        public static Lookup<double> CreateLookup(List<ControlPoint> controls, List<SpecialControl> specialControls)
        {
            Lookup<double> lookupTable = new Lookup<double>();

            foreach (ControlPoint c1 in controls)
            {
                int code1 = c1.ControlCode;

                foreach (ControlPoint c2 in controls)
                {
                    int code2 = c2.ControlCode;

                    if (code1 == code2) { }
                    else
                    {
                        double dist = Misc.CalculateDistance(c1, c2);

                        lookupTable.Create(code1, code2, dist);
                    }
                }

                foreach (SpecialControl c2 in specialControls)
                {
                    double dist = Misc.CalculateDistance(c1, c2);

                    if (c2.Type == Type.start) { lookupTable.Create(c2.ControlCode, code1, dist); }
                    else if (c2.Type == Type.finish) { lookupTable.Create(code1, c2.ControlCode, dist); }
                    else { }


                }
            }

            return lookupTable;
        }
    }

    public static class Misc
    {

        public static double CalculateDistance(ControlPoint c1, ControlPoint c2)
        {
            return Math.Sqrt(Math.Pow(c2.X - c1.X, 2) + Math.Pow(c2.Y - c1.Y, 2));
        }

        public static double CalculateAngle(ControlPoint c1, ControlPoint c2, ControlPoint c3) // Degrees
        {
            double
                l12 = Math.Sqrt(Math.Pow((c2.X - c1.X), 2) + Math.Pow((c2.Y - c1.Y), 2)),
                l23 = Math.Sqrt(Math.Pow((c3.X - c2.X), 2) + Math.Pow((c3.Y - c2.Y), 2)),
                l13 = Math.Sqrt(Math.Pow((c3.X - c1.X), 2) + Math.Pow((c3.Y - c1.Y), 2));

            double
                p1 = Math.Pow(l12, 2) + Math.Pow(l23, 2) - Math.Pow(l13, 2),
                p2 = 2 * l12 * l23;

            return Math.Acos(p1 / p2) * (180 / Math.PI);
        }

        public static double CalculateCourseLength(List<ControlPoint> controls)
        {
            double len = 0;
            for (int i = 1; i < controls.Count; i++) { len += Misc.CalculateDistance(controls[i], controls[i - 1]); }
            return len;
        }

    }

    public static class Courses
    {
        // Properties //

        public static int NumControls { get; set; } = 15;
        public static int CourseLength { get; set; } = 7000;
        public static int CourseTolerance { get; set; } = 150;
        public static int AngleTolerance { get; set; } = 75;
        public static string FilePath { get; set; }
        public static string AllCourseFilePath { get; set; }
        public static Weight Weight { get; set; } = new Weight();


        // Private //

        private static List<ControlPoint> controls = new List<ControlPoint>();
        private static List<SpecialControl> sControls = new List<SpecialControl>();
        private static Lookup<double> lookup = new Lookup<double>();
        private static Dictionary<int, ControlPoint> cDict = new Dictionary<int, ControlPoint>();

        private static List<LookupItem<double>>
            longC = new List<LookupItem<double>>(),
            medC = new List<LookupItem<double>>(),
            shortC = new List<LookupItem<double>>(),
            vshortC = new List<LookupItem<double>>();

        private static Random random = new Random();


        public static List<ControlPoint> CreateCourse()
        {
            SetUp();

            random = new Random();
            List<string> sCourse = new List<string>();

            int count = 0;

            while (true)
            {
                List<ControlPoint> cCourse = RandomCourse();
                count++;
                Debug.Print(count + "\t" + Misc.CalculateCourseLength(cCourse));
                sCourse.Add(OutString(cCourse));
                if (CheckCourse(cCourse)) { File.AppendAllLines(AllCourseFilePath, sCourse); return cCourse; }
            }


        }
        public static List<ControlPoint> CreateCourse(string filePath)
        {
            FilePath = filePath;

            return CreateCourse();
        }
        public static List<ControlPoint> CreateCourse(int numC, int cLength, int cTol, int aTol, string filePath)
        {
            NumControls = numC;
            CourseLength = cLength;
            CourseTolerance = cTol;
            AngleTolerance = aTol;
            FilePath = filePath;

            return CreateCourse();
        }

        public static void SetUp()
        {
            XML.CreateControls(FilePath, ref controls, ref sControls);
            lookup = XML.CreateLookup(controls, sControls);
            CreateLenLists(lookup);

            foreach (ControlPoint c in controls) { cDict.Add(c.ControlCode, c); }
            foreach (SpecialControl c in sControls) { cDict.Add(c.ControlCode, c); }
        }

        private static void CreateLenLists(Lookup<double> lookup)
        {
            foreach (LookupItem<double> item in lookup.items)
            {
                if (item.GetData * 15 <= 150) { vshortC.Add(item); }
                else if (item.GetData * 15 <= 500) { shortC.Add(item); }
                else if (item.GetData * 15 <= 1000) { medC.Add(item); }
                else { longC.Add(item); }
            }
        }
        public static List<ControlPoint> RandomCourse()
        {
            List<ControlPoint> course = new List<ControlPoint>();

            for (int i = 0; i < NumControls; i++)
            {
                if (i == 0)
                {
                    bool passed = false;
                    int n = 0;

                    while (!passed)
                    {
                        n = random.Next(sControls.Count);
                        if (sControls[n].Type == Type.start) { passed = true; course.Add(sControls[n]); }
                    }
                }
                else if (i == 1)
                {
                    double rnd = random.NextDouble();

                    if (rnd <= 0.4)
                    {
                        ControlPoint c = GetNextC(vshortC, course.Last());
                        course.Add(c);
                    }
                    else
                    {
                        ControlPoint c = GetNextC(shortC, course.Last());
                        course.Add(c);
                    }
                }
                else
                {
                    double rnd = random.NextDouble();
                    if (rnd <= Weight.GetVShort())
                    {
                        ControlPoint c = GetNextC(vshortC, course[i - 1], course[i - 2], course);
                        course.Add(c);
                    }
                    else if (rnd <= Weight.GetShort())
                    {
                        ControlPoint c = GetNextC(shortC, course[i - 1], course[i - 2], course);
                        course.Add(c);
                    }
                    else if (rnd <= Weight.GetMed())
                    {
                        ControlPoint c = GetNextC(medC, course[i - 1], course[i - 2], course);
                        course.Add(c);
                    }
                    else
                    {
                        ControlPoint c = GetNextC(longC, course[i - 1], course[i - 2], course);
                        course.Add(c);
                    }
                }
            }
            return WorkTowardsFinish(course);
        }

        public static bool CheckCourse(List<ControlPoint> course)
        {
            double len = Misc.CalculateCourseLength(course) * 15;
            if (len < (CourseLength + CourseTolerance) && len > (CourseLength - CourseTolerance)) { } else { return false; }

            double density = CalculatePointDensity(course);
            if (density < 1) { } else { return false; }

            return true;

        }
        private static double CalculatePointDensity(List<ControlPoint> controls)
        {
            double minX = 9999, maxX = 0, minY = 9999, maxY = 0;
            foreach (ControlPoint c in controls)
            {
                if (c.X < minX) { minX = c.X; }
                else if (c.X > maxX) { maxX = c.X; }

                if (c.Y < minY) { minY = c.Y; }
                else if (c.Y > maxY) { maxY = c.Y; }
            }

            double area = (maxX - minX) * (maxY - minY);
            return (controls.Count / area) * 1000;
        }



        private static string OutString(List<ControlPoint> controls)
        {
            string s = "";

            foreach (ControlPoint c in controls)
            {
                s += c.ControlCode.ToString() + ",";
            }

            return s.Substring(0, s.Length - 1);
        }

        private static ControlPoint GetNextC(List<LookupItem<double>> items, ControlPoint previous)
        {
            List<LookupItem<double>> valid = new List<LookupItem<double>>();

            foreach (LookupItem<double> c in items)
            {
                if (c.GetX == previous.ControlCode) { valid.Add(c); }
            }

            LookupItem<double> next;
            int rn = random.Next(valid.Count);
            try { next = valid[rn]; }
            catch
            {
                LookupItem<double> shortest = lookup.items[0];
                bool first = true;

                foreach (LookupItem<double> c in lookup.items)
                {

                    if (c.GetX == previous.ControlCode)
                    {
                        if (first) { shortest = c; first = false; }
                        else
                        {
                            if (c.GetData < shortest.GetData) { shortest = c; }
                        }
                    }
                }

                return cDict[shortest.GetY];
            }
            return cDict[next.GetY];
        }
        private static ControlPoint GetNextC(List<LookupItem<double>> items, ControlPoint previous, ControlPoint dPrevious, List<ControlPoint> course)
        {


            List<LookupItem<double>> valid = new List<LookupItem<double>>();

            foreach (LookupItem<double> c in items)
            {
                if (c.GetX == previous.ControlCode && cDict.ContainsKey(c.GetX) && cDict.ContainsKey(c.GetY))
                {
                    if (cDict[c.GetY] is SpecialControl) { }
                    else { valid.Add(c); }
                }
            }

            for (int i = 0; i < valid.Count; i++)
            {
                ControlPoint c = cDict[valid[i].GetY];

                double angle = Misc.CalculateAngle(dPrevious, previous, c);

                if (angle > 360 - AngleTolerance || angle < AngleTolerance) { valid.Remove(valid[i]); i -= 1; }
                if (course.Contains(c)) { try { valid.Remove(valid[i]); i -= 1; } catch { } }

            }

            LookupItem<double> next;
            int rn = random.Next(valid.Count);
            try { next = valid[rn]; }
            catch
            {
                LookupItem<double> shortest = lookup.items[0];
                bool first = true;

                foreach (LookupItem<double> c in lookup.items)
                {
                    if (c.GetX == previous.ControlCode && cDict.ContainsKey(c.GetX) && cDict.ContainsKey(c.GetY))
                    {
                        if (first) { shortest = c; first = false; }
                        else
                        {
                            double angle = Misc.CalculateAngle(dPrevious, previous, cDict[c.GetY]);
                            if (c.GetData < shortest.GetData && !course.Contains(cDict[c.GetY]) && !(angle < AngleTolerance || angle > 360 - AngleTolerance) && cDict[c.GetY] != dPrevious) { shortest = c; }
                        }
                    }
                }
                return cDict[shortest.GetY];
            }
            return cDict[next.GetY];
        }

        private static List<ControlPoint> WorkTowardsFinish(List<ControlPoint> course)
        {
            SpecialControl finish = GetNearestFinish(course.Last());
            bool finished = false;

            while (!finished)
            {
                double sDist = 99999999;
                ControlPoint nearest = new ControlPoint();

                foreach (LookupItem<double> c in lookup.items)
                {
                    if (c.GetY == finish.ControlCode)
                    {
                        double distA = Misc.CalculateDistance(cDict[c.GetX], finish);
                        if (distA < sDist)
                        {
                            sDist = distA;
                            nearest = cDict[c.GetX];
                        }
                    }
                }
                double dist = Misc.CalculateDistance(finish, course.Last());

                if (nearest == course.Last()) { finished = true; break; }
                if ((dist * 15) < 150) { finished = true; break; }
                if (course.Count >= 40) { finished = true; break; }

                List<LookupItem<double>> valid = new List<LookupItem<double>>();

                foreach (LookupItem<double> c in lookup.items)
                {
                    if (c.GetX == course.Last().ControlCode)
                    {
                        if (cDict.ContainsKey(c.GetX) && cDict.ContainsKey(c.GetY))
                        {
                            if (!(cDict[c.GetY] is SpecialControl))
                            {
                                double angle = Misc.CalculateAngle(cDict[c.GetY], course.Last(), finish);

                                double
                                    d1 = Misc.CalculateDistance(finish, course.Last()),
                                    d2 = Misc.CalculateDistance(cDict[c.GetY], course.Last());


                                if ((angle < 90 || angle > 270) && (d2 <= d1))
                                {
                                    if (course.Contains(cDict[c.GetY])) { }
                                    else
                                    {
                                        double a1 = Misc.CalculateAngle(course[course.Count - 2], course.Last(), cDict[c.GetY]);

                                        if (a1 < AngleTolerance || a1 > (360 - AngleTolerance)) { }
                                        else { valid.Add(c); }
                                    }
                                }
                            }
                        }

                    }
                }

                List<ControlPoint> chosen = new List<ControlPoint>();
                if (valid.Count == 0)
                {
                    LookupItem<double> shortest = new LookupItem<double>();
                    bool first = true;

                    foreach (LookupItem<double> c in lookup.items)
                    {

                        if (c.GetX == course.Last().ControlCode && cDict.ContainsKey(c.GetX) && cDict.ContainsKey(c.GetY) && !(cDict[c.GetY] is SpecialControl))
                        {
                            if (first) { shortest = c; first = false; }
                            else
                            {

                                double angle = Misc.CalculateAngle(course[course.Count - 2], course.Last(), cDict[c.GetY]);
                                if (c.GetData < shortest.GetData && !course.Contains(cDict[c.GetY]) && !(angle < AngleTolerance || angle > (360 - AngleTolerance))) { shortest = c; }
                            }
                        }
                    }
                    controls.Add(cDict[shortest.GetY]);
                }
                else
                {
                    if (valid.Count > 10)
                    {
                        for (int i = 0; i < 10; i++)
                        {
                            int n = random.Next(valid.Count);
                            chosen.Add(cDict[valid[n].GetY]);

                            valid.RemoveAt(n);
                        }
                    }
                    else { foreach (LookupItem<double> c in valid) { chosen.Add(cDict[c.GetY]); } }

                    ControlPoint direct = new ControlPoint();
                    double sAngle = 360;

                    foreach (ControlPoint c in chosen)
                    {
                        double angle = Misc.CalculateAngle(c, course.Last(), finish);
                        if (angle > 180) { angle = 360 - angle; }

                        if (angle < sAngle) { sAngle = angle; direct = c; }
                    }
                    course.Add(direct);
                }
            }
            course.Add(finish);
            return course;
        }
        private static SpecialControl GetNearestFinish(ControlPoint previous)
        {
            SpecialControl nearest = new SpecialControl();
            double shortestDist = 9999999;
            bool first = true;

            foreach (SpecialControl sC in sControls)
            {
                if (sC.Type == Sunley.PurplePen.v1.Type.finish)
                {
                    if (first) { nearest = sC; shortestDist = Misc.CalculateDistance(previous, sC); first = false; }
                    else
                    {
                        if (Misc.CalculateDistance(previous, sC) < shortestDist) { nearest = sC; shortestDist = Misc.CalculateDistance(previous, sC); }
                    }
                }

            }

            return nearest;
        }
    }


    public class Course
    {
        protected List<ControlPoint> controls = new List<ControlPoint>();

        public Course()
        {

        }
    }
    public class ControlPoint
    {
        protected double x, y;
        protected int controlCode;
        protected int id;

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

        public int ID
        {
            get { return id; }
            set { id = value; }
        }

        public int ControlCode
        {
            get { return controlCode; }
            set { controlCode = value; }
        }

        public Tuple<double, double> GetCoords()
        {
            return new Tuple<double, double>(x, y);
        }

    }
    public class SpecialControl : ControlPoint
    {

        protected Type type;

        public Type Type
        {
            get { return type; }
            set { type = value; }
        }

    }
    public enum Type
    {
        normal,
        start,
        finish
    }
    public class Lookup<T>
    {
        public List<LookupItem<T>> items = new List<LookupItem<T>>();

        public Lookup()
        {

        }

        public void Create(int x, int y, T data)
        {
            LookupItem<T> obj = new LookupItem<T>(x, y, data);
            items.Add(obj);
        }

        public void RemoveAt(int x, int y)
        {
            foreach (LookupItem<T> obj in items)
            {
                if (x == obj.GetX && y == obj.GetY)
                {
                    items.Remove(obj);
                    break;
                }
            }

            throw new Exception("Object not found");
        }

        public void Remove(T data)
        {
            foreach (LookupItem<T> obj in items)
            {
                if (data.ToString() == obj.GetData.ToString())
                {
                    items.Remove(obj);
                    break;
                }
            }

            throw new Exception("Object not found");
        }

        public T Find(int x, int y)
        {
            foreach (LookupItem<T> obj in items)
            {
                if (x == obj.GetX && y == obj.GetY)
                {
                    return obj.GetData;
                }
            }

            throw new Exception("Object not found");
        }
    }
    public class LookupItem<T>
    {
        protected int x, y;
        protected T data;

        public LookupItem(int x, int y, T data)
        {
            this.x = x;
            this.y = y;
            this.data = data;
        }
        public LookupItem() { }

        public int GetX { get { return this.x; } }
        public int GetY { get { return this.y; } }
        public T GetData { get { return this.data; } }

        public override string ToString()
        {
            return x + "," + y + "," + data.ToString();
        }
    }
    public struct Line
    {
        public double Gradient { get; set; }
        public double Intercept { get; set; }
    }
    public class Weight
    {

        // Properties //
        public double vShortL { get; set; }
        public double shortL { get; set; }
        public double medL { get; set; }
        public double longL { get; set; }


        // Constructors //
        public Weight()
        {
            vShortL = 0.2;
            shortL = 0.5;
            medL = 0.2;
            longL = 0.1;
        }
        public Weight(double v, double s, double m, double l)
        {
            if ((v + s + m + l) == 1)
            {
                vShortL = v;
                shortL = s;
                medL = m;
                longL = l;
            }
            else { throw new Exception("Probabilities add up to 1 you pleb"); }
        }
        public Weight(int v, int s, int m, int l)
        {
            if ((v + s + m + l) == 100)
            {
                vShortL = v / 100;
                shortL = s / 100;
                medL = m / 100;
                longL = l / 100;
            }
            else { throw new Exception("Probabilities add up to 1 you pleb"); }
        }


        // Methods //
        public double GetVShort() { return vShortL; }
        public double GetShort() { return vShortL + shortL; }
        public double GetMed() { return vShortL + shortL + medL; }
        public double GetLong() { return vShortL + shortL + medL + longL; }
    }
}
    
    

