using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.IO;
using System.Diagnostics;

namespace Sunley.PurplePen.v2
{
    public static class XML
    {
        public static void CreateControls(string filename, ref List<Control> controls, ref List<SpecialControl> specialControls)
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
                            Control c = new Control();
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

        public static Lookup<double> CreateLookup(List<Control> controls, List<SpecialControl> specialControls)
        {
            Lookup<double> lookupTable = new Lookup<double>();

            foreach (Control c1 in controls)
            {
                int code1 = c1.ControlCode;

                foreach (Control c2 in controls)
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
        public static double CalculateDistance(Control c1, Control c2)
        {
            return Math.Sqrt(Math.Pow(c2.X - c1.X, 2) + Math.Pow(c2.Y - c1.Y, 2));
        }

        public static double CalculateAngle(Control c1, Control c2, Control c3) // Degrees
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
    }

    public static class Course
    {
        // Variables //
        static int controlNum = 15;
        static int courseLength = 5100;
        static int courseTolerance = 100;
        static int angleTolerance = 75;




        static List<Control> controls = new List<Control>();
        static List<SpecialControl> specialControls = new List<SpecialControl>();
        static Lookup<double> lookup = new Lookup<double>();
        static Dictionary<int, Control> cDict = new Dictionary<int, Control>();

        static string fileLoc = @"C:\Users\joe\Downloads\Yvette Baker.ppen";

        static List<LookupItem<double>>
            longC = new List<LookupItem<double>>(),
            medC = new List<LookupItem<double>>(),
            shortC = new List<LookupItem<double>>(),
            vshortC = new List<LookupItem<double>>();

        private static readonly Random random = new Random();

        public static List<Control> CreateAcceptableCourse()
        {
            XML.CreateControls(fileLoc, ref controls, ref specialControls);
            lookup = XML.CreateLookup(controls, specialControls);
            CreateLenLists(lookup);

            foreach (Control c in controls) { cDict.Add(c.ControlCode, c); }
            foreach (SpecialControl c in specialControls) { cDict.Add(c.ControlCode, c); }


            for (int i = 0; i < 100000; i++)
            {
                List<Control> thisCourse = CreateCourse(controlNum);
                double le = CalcCourseLength(thisCourse) * 15;

                if (le < (courseLength + courseTolerance) && le > (courseLength - courseTolerance))
                {
                    double density = CalculatePointDensity(thisCourse);
                    if (density < 1) { return thisCourse; }
                }

                Debug.Print(le.ToString());
            }

            return new List<Control>();
        }

        static string OutString(List<Control> controls)
        {
            string s = "";

            foreach (Control c in controls)
            {
                s += c.ControlCode.ToString() + ",";
            }

            return s.Substring(0, s.Length - 1);
        }

        static void CreateLenLists(Lookup<double> lookup)
        {
            foreach (LookupItem<double> item in lookup.items)
            {
                if (item.GetData * 10 <= 100) { vshortC.Add(item); }
                else if (item.GetData * 10 <= 300) { shortC.Add(item); }
                else if (item.GetData * 10 <= 500) { medC.Add(item); }
                else { longC.Add(item); }
            }
        }

        static List<Control> CreateCourse(int numControls)
        {
            List<Control> nums = new List<Control>();
            for (int i = 0; i < numControls; i++)
            {
                if (i == 0)
                {
                    bool passed = false;

                    int n = 0;
                    while (!passed)
                    {
                        n = random.Next(specialControls.Count);
                        if (specialControls[n].Type == Type.start) { passed = true; }
                    }

                    nums.Add(specialControls[n]);
                }
                else if (i == 1)
                {
                    double rnd = random.NextDouble();

                    if (rnd <= 0.4)
                    {
                        Control c = GetNextC(vshortC, nums[i - 1]);
                        nums.Add(c);
                    }
                    else
                    {
                        Control c = GetNextC(shortC, nums[i - 1]);
                        nums.Add(c);
                    }
                }
                else
                {
                    double rnd = random.NextDouble();
                    if (rnd <= 0.1)
                    {
                        Control c = GetNextC(longC, nums[i - 1], nums[i - 2], nums);
                        nums.Add(c);
                    }
                    else if (rnd > 0.1 && rnd <= 0.3)
                    {
                        Control c = GetNextC(medC, nums[i - 1], nums[i - 2], nums);
                        nums.Add(c);
                    }
                    else if (rnd > 0.3 && rnd <= 0.8)
                    {
                        Control c = GetNextC(shortC, nums[i - 1], nums[i - 2], nums);
                        nums.Add(c);
                    }
                    else
                    {
                        Control c = GetNextC(vshortC, nums[i - 1], nums[i - 2], nums);
                        nums.Add(c);
                    }
                }


            }

            return WorkTowardsFinish(nums);
        }

        static Control GetNextC(List<LookupItem<double>> items, Control previous, Control dPrevious, List<Control> controls)
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
                Control c = cDict[valid[i].GetY];

                double angle = Misc.CalculateAngle(dPrevious, previous, c);

                if (angle > 360 - angleTolerance || angle < angleTolerance) { valid.Remove(valid[i]); i -= 1; }
                if (controls.Contains(c)) { try { valid.Remove(valid[i]); i -= 1; } catch { } }

            }

            LookupItem<double> next;
            int rn = random.Next(valid.Count);
            try
            {
                next = valid[rn];
            }
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
                            if (c.GetData < shortest.GetData && !controls.Contains(cDict[c.GetY]) && !(angle < angleTolerance || angle > 360 - angleTolerance) && cDict[c.GetY] != dPrevious) { shortest = c; }
                        }
                    }
                }

                return cDict[shortest.GetY];

            }


            return cDict[next.GetY];

        }

        static Control GetNextC(List<LookupItem<double>> items, Control previous)
        {
            List<LookupItem<double>> valid = new List<LookupItem<double>>();

            foreach (LookupItem<double> c in items)
            {
                if (c.GetX == previous.ControlCode)
                {
                    valid.Add(c);
                }
            }

            LookupItem<double> next;
            int rn = random.Next(valid.Count);
            try
            {
                next = valid[rn];
            }
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


        static double CalcCourseLength(List<Control> c)
        {
            double length = 0;
            for (int i = 1; i < c.Count; i++)
            {
                length += Misc.CalculateDistance(c[i], c[i - 1]);
            }

            return length;
        }

        static void CreateOutXML(List<List<Control>> courses)
        {
            List<string> courseStr = new List<string>();
            string controlStr = "";
            int cControlID = 1;
            int courseID = 1;

            foreach (List<Control> c in courses)
            {
                string l1 = "Density - " + CalculatePointDensity(c).ToString("F2");
                //string l2 = "Cross Rate - " + CalculateCrossRate(c).ToString("F4");

                courseStr.Add(CourseStr(courseID, cControlID, 10000, new List<string>() { l1 }));
                courseID += 1;

                for (int i = 0; i < c.Count; i++)
                {
                    if (i == c.Count - 1)
                    {
                        controlStr += EndControlStr(cControlID, c[i].ID);
                    }
                    else
                    {
                        controlStr += ControlStr(cControlID, cControlID + 1, c[i].ID);
                    }

                    cControlID += 1;
                }
            }

            string text = File.ReadAllText(fileLoc);

            int loc = text.IndexOf("<course id=");
            text = text.Substring(0, loc);

            foreach (string s in courseStr) { text += s; }
            text += controlStr;
            text += "</course-scribe-event>";

            File.WriteAllText(@"text.ppen", text);

            //Console.WriteLine(text);
        }


        static string ControlStr(int courseC1, int courseC2, int controlID)
        {
            return "<course-control id=\"" + courseC1.ToString() + "\" control=\"" + controlID.ToString() + "\">"
                + Environment.NewLine
                + "<next course-control=\"" + courseC2.ToString() + "\" />"
                + Environment.NewLine
                + "</course-control>"
                + Environment.NewLine;
        }
        static string EndControlStr(int courseC1, int controlID)
        {
            return "<course-control id=\"" + courseC1.ToString() + "\" control=\"" + controlID.ToString() + "\" />"
                + Environment.NewLine;
        }
        static string CourseStr(int id, int firstC, int scale, List<string> data)
        {
            string s = "";
            foreach (string d in data) { s += d + "|"; }
            try { s = s.Substring(0, s.Length - 1); } catch { }


            return
                "<course id=\"" + id.ToString() + "\" kind=\"normal\" order=\"" + id.ToString() + "\">"
                + Environment.NewLine
                + "<name>Course " + id.ToString() + "</name>"
                + Environment.NewLine
                + "<secondary-title>" + s + "</secondary-title>"
                + Environment.NewLine
                + "<labels label-kind=\"sequence\" />"
                + Environment.NewLine
                + "<first course-control=\"" + firstC.ToString() + "\" />"
                + Environment.NewLine
                + "<print-area automatic=\"true\" restrict-to-page-size=\"true\" "
                + "left=\"16.7639771\" top=\"219.328979\" right=\"313.689972\" bottom=\"9.270966\" page-width=\"827\" page-height=\"1169\" page-margins=\"0\" page-landscape=\"true\" />"
                + Environment.NewLine
                + "<options print-scale=\"" + scale.ToString() + "\" description-kind=\"symbols\" />"
                + Environment.NewLine
                + "</course>"
                + Environment.NewLine;
        }


        static List<Control> WorkTowardsFinish(List<Control> controls)
        {
            SpecialControl finish = GetNearestFinish(controls.Last());

            bool finished = false;

            while (!finished)
            {

                #region Check If Finished

                double sDist = 999999999;
                Control nearest = new Control();

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

                if (nearest == controls.Last()) { finished = true; break; }
                double dist = Misc.CalculateDistance(finish, controls.Last());

                if ((dist * 10) < 150) { finished = true; break; }

                if (controls.Count >= 40) { finished = true; break; }

                #endregion

                try
                {
                    controls.Add(TimeOutFControl(controls, finish));
                }
                catch { }

            }

            controls.Add(finish);

            return controls;
        }

        static Control GetNextFControl(List<Control> controls, Control finish)
        {
            List<LookupItem<double>> valid = new List<LookupItem<double>>();

            foreach (LookupItem<double> c in lookup.items)
            {


                if (c.GetX == controls.Last().ControlCode && cDict.ContainsKey(c.GetX) && cDict.ContainsKey(c.GetY) && !(cDict[c.GetY] is SpecialControl))
                {

                    double angle = Misc.CalculateAngle(cDict[c.GetY], controls.Last(), finish);

                    double
                        d1 = Misc.CalculateDistance(finish, controls.Last()),
                        d2 = Misc.CalculateDistance(cDict[c.GetY], controls.Last());


                    if ((angle < 90 || angle > 270) && (d2 <= d1))
                    {
                        if (controls.Contains(cDict[c.GetY])) { }
                        else
                        {
                            double a1 = Misc.CalculateAngle(controls[controls.Count - 2], controls.Last(), cDict[c.GetY]);

                            if (a1 < angleTolerance || a1 > (360 - angleTolerance)) { }
                            else { valid.Add(c); }
                        }

                    }
                }
            }


            List<Control> chosen = new List<Control>();
            if (valid.Count == 0)
            {
                LookupItem<double> shortest = lookup.items[0];
                bool first = true;

                foreach (LookupItem<double> c in lookup.items)
                {

                    if (c.GetX == controls.Last().ControlCode && cDict.ContainsKey(c.GetX) && cDict.ContainsKey(c.GetY) && !(cDict[c.GetY] is SpecialControl))
                    {
                        if (first) { shortest = c; first = false; }
                        else
                        {

                            double angle = Misc.CalculateAngle(controls[controls.Count - 2], controls.Last(), cDict[c.GetY]);
                            if (c.GetData < shortest.GetData && !controls.Contains(cDict[c.GetY]) && !(angle < angleTolerance || angle > 360 - angleTolerance) && cDict[c.GetY] != controls[controls.Count - 2]) { shortest = c; }
                        }
                    }
                }

                return cDict[shortest.GetY];
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
                else
                {
                    foreach (LookupItem<double> c in valid)
                    {
                        chosen.Add(cDict[c.GetY]);
                    }
                }



                Control direct = new Control();
                bool first = true;
                double smallestAngle = 360;

                foreach (Control c in chosen)
                {
                    if (first)
                    {
                        direct = c;
                        double angle = Misc.CalculateAngle(c, controls.Last(), finish);
                        if (angle < 180) { smallestAngle = angle; } else { smallestAngle = 360 - angle; }
                        first = false;

                    }
                    else
                    {
                        double angle = Misc.CalculateAngle(c, controls.Last(), finish);
                        if (angle < 180) { } else { angle = 360 - angle; }

                        if (angle < smallestAngle) { smallestAngle = angle; direct = c; }
                    }
                }

                return direct;
            }
        }

        static Control TimeOutFControl(List<Control> controls, Control finish)
        {
            var task = Task.Run(() => GetNextFControl(controls, finish));
            if (task.Wait(TimeSpan.FromSeconds(1.5)))
            {
                return task.Result;
            }
            else { throw new TimeoutException(); }
        }

        static SpecialControl GetNearestFinish(Control c)
        {
            SpecialControl nearest = new SpecialControl();
            double shortestDist = 9999999;
            bool first = true;

            foreach (SpecialControl sC in specialControls)
            {
                if (sC.Type == Type.finish)
                {
                    if (first) { nearest = sC; shortestDist = Misc.CalculateDistance(c, sC); first = false; }
                    else
                    {
                        if (Misc.CalculateDistance(c, sC) < shortestDist) { nearest = sC; shortestDist = Misc.CalculateDistance(c, sC); }
                    }
                }

            }

            return nearest;
        }

        static double CalculatePointDensity(List<Control> controls)
        {
            double minX = 9999, maxX = 0, minY = 9999, maxY = 0;

            foreach (Control c in controls)
            {
                if (c.X < minX) { minX = c.X; }
                else if (c.X > maxX) { maxX = c.X; }

                if (c.Y < minY) { minY = c.Y; }
                else if (c.Y > maxY) { maxY = c.Y; }
            }

            double area = (maxX - minX) * (maxY - minY);

            return (controls.Count / area) * 1000;

        }
    }

    public class Control
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
    public class SpecialControl : Control
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