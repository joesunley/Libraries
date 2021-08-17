using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Sunley.Miscellaneous;


namespace Sunley.PurplePen.v3
{
    public static class XML
    {
        public static void CreateControls(string filename, ref List<ControlPoint> controls, ref List<ControlPoint> specControls)
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
                            ControlPoint sC = new ControlPoint();

                            switch (node.Attributes[1].Value)
                            {
                                case "start":
                                    sC.Type = ControlPointType.Start;
                                    sC.Code = Convert.ToInt32(Math.Ceiling(Convert.ToDouble((root.ChildNodes.Count) / 100)) * 100) + 100 + startCount;
                                    startCount++;
                                    break;
                                case "finish":
                                    sC.Type = ControlPointType.Finish;
                                    sC.Code= Convert.ToInt32(Math.Ceiling(Convert.ToDouble(root.ChildNodes.Count / 100)) * 100) + 200 + finishCount;
                                    finishCount++;
                                    break;
                                default:
                                    break;
                            }

                            sC.ID = Convert.ToInt32(node.Attributes[0].Value);

                            XmlNode child = node.ChildNodes[0];
                            sC.X = Convert.ToDouble(child.Attributes[0].Value);
                            sC.Y = Convert.ToDouble(child.Attributes[1].Value);

                            specControls.Add(sC);
                        }
                        else
                        {
                            ControlPoint c = new ControlPoint();
                            c.ID = Convert.ToInt32(node.Attributes[0].Value);
                            c.Code = Convert.ToInt32(node.ChildNodes[0].InnerText);
                            c.Type = ControlPointType.Normal;

                            XmlNode child = node.ChildNodes[1];
                            c.X = Convert.ToDouble(child.Attributes[0].Value);
                            c.Y = Convert.ToDouble(child.Attributes[1].Value);

                            controls.Add(c);

                        }

                    }
                }
            }

            
        }
    }

    public static class Extensions
    {
        public static double CourseLength(this List<ControlPoint> course)
        {
            double len = 0;
            for (int i = 1; i < course.Count; i++)
                len += Misc.DistanceBetweenTwoPoints(course[i].GetPoint, course[i - 1].GetPoint);
            return len;
        }
    }

    public static class Courses
    {
        public static int NumControls = 17;
        public static int CourseLength = 7300;
        public static int CourseTolerance = 75;
        public static int AngleTolerance = 75;
        public static string FilePath = @"C:\Users\joe\OneDrive - Jossy\Documents\Orienteering\Geeking\EYOC 2021\Long\All Controls.ppen";

        private static readonly Random rnd = new Random();

        private static List<ControlPoint> controls = new List<ControlPoint>();
        private static List<ControlPoint> specControls = new List<ControlPoint>();
        public static List<ControlPoint> CreateRandomCourse()
        {
            List<ControlPoint> course = new List<ControlPoint>();
            controls.Clear();
            specControls.Clear();

            if (controls.Count == 0)
                XML.CreateControls(FilePath, ref controls, ref specControls);

            course.Add(ChooseStart());
            course.Add(ChooseFirstControl(course[0]));

            for (int i = 2; i < NumControls; i++)
                course.Add(ChooseNextControl(course));

            return WorkTowardsFinish(course);
        }

        private static ControlPoint ChooseStart()
        {
            while (true)
            {
                int n = rnd.Next(specControls.Count);
                if (specControls[n].Type == ControlPointType.Start)
                    return specControls[n];
            }
        }
        private static ControlPoint ChooseFirstControl(ControlPoint start)
        {
            double minLen = 0,
                maxLen = 0;

            double p = rnd.NextDouble();

            if (p <= 0.4)
            {
                // Very Short
                minLen = 0;
                maxLen = 150;
            }
            else
            {
                // Short
                minLen = 150;
                maxLen = 500;
            }

            List<ControlPoint> validControls = new List<ControlPoint>();
            foreach (ControlPoint c in controls)
            {
                double legDist = Misc.DistanceBetweenTwoPoints(c.GetPoint, start.GetPoint) * 15;
                if (legDist > minLen && legDist < maxLen)
                    validControls.Add(c);
            }

            if (validControls.Count > 0)
                return validControls[rnd.Next(validControls.Count)];
            else
                return GetNearest(start);
        }
        private static ControlPoint ChooseNextControl(List<ControlPoint> course)
        {
            double minLen,
                maxLen;

            double p = rnd.NextDouble();
            if (p <= 0.1)
            {
                // Long
                minLen = 1000;
                maxLen = 99999;
            }
            else if (p <= 0.3)
            {
                // Medium
                minLen = 500;
                maxLen = 1000;
            }
            else if (p <= 0.8)
            {
                // Short
                minLen = 150;
                maxLen = 500;
            }
            else
            {
                // Very Short
                minLen = 0;
                maxLen = 150;
            }

            List<ControlPoint> validControls = new List<ControlPoint>();
            foreach (ControlPoint c in controls)
            {
                double legDist = Misc.DistanceBetweenTwoPoints(c.GetPoint, course.Last().GetPoint) * 15;
                double angle = Misc.AngleBetweenThreePoints(course[course.Count - 2].GetPoint, course.Last().GetPoint, c.GetPoint);
                if (legDist > minLen && legDist < maxLen
                        && angle < 285 && angle > 75
                        && !course.Contains(c)) 
                    validControls.Add(c);
            }

            if (validControls.Count > 0)
                return validControls[rnd.Next(validControls.Count)];
            else
            {
                ControlPoint nearest = new ControlPoint();
                double shortLen = 99999;

                foreach (ControlPoint c in controls)
                {
                    double legDist = Misc.DistanceBetweenTwoPoints(c.GetPoint, course.Last().GetPoint);
                    double angle = Misc.AngleBetweenThreePoints(course[course.Count - 2].GetPoint, course.Last().GetPoint, c.GetPoint);
                    if ((legDist < shortLen)
                        && (angle < 285) && (angle > 75)
                        && (!course.Contains(c)))
                    {
                        nearest = c;
                        shortLen = legDist;
                    }
                }
                return nearest;
            }
        }
        private static List<ControlPoint> WorkTowardsFinish(List<ControlPoint> course)
        {
            ControlPoint finish = FindNearestFinish(course.Last());
            List<ControlPoint> checkCourse = course;
            checkCourse.Add(finish);

            if (checkCourse.CourseLength() * 15 > CourseLength + CourseTolerance)
                return checkCourse;

            if (course.Last().Type == ControlPointType.Finish)
                course.Remove(course.Last());

            while (true)
            {
                if (course.Count >= 40) break;

                double finDist = Misc.DistanceBetweenTwoPoints(course.Last().GetPoint, finish.GetPoint) * 15;
                if (finDist <= 150) break;

                if (GetNearest(finish) == course.Last()) break;


                List<ControlPoint> valid = new List<ControlPoint>();
                foreach (ControlPoint c in controls)
                {
                    double finishAngle = Misc.AngleBetweenThreePoints(c.GetPoint, course.Last().GetPoint, finish.GetPoint);

                    double
                        d1 = Misc.DistanceBetweenTwoPoints(course.Last().GetPoint, finish.GetPoint),
                        d2 = Misc.DistanceBetweenTwoPoints(c.GetPoint, course.Last().GetPoint);

                    if (finishAngle > 90 && finishAngle < 270
                        && d2 <= d1)
                        if (!course.Contains(c))
                        {
                            double legAngle = Misc.AngleBetweenThreePoints(course[course.Count - 2].GetPoint, course.Last().GetPoint, c.GetPoint);
                            if (legAngle > 75 && legAngle < 285)
                                valid.Add(c);
                        }
                }

                if (valid.Count == 0)
                {
                    ControlPoint nearest = new ControlPoint();
                    double sDist = 99999;

                    foreach (ControlPoint c in controls)
                    {
                        double
                            angle = Misc.AngleBetweenThreePoints(course[course.Count - 2].GetPoint, course.Last().GetPoint, c.GetPoint),
                            dist = Misc.DistanceBetweenTwoPoints(c.GetPoint, course.Last().GetPoint);

                        if (angle > 75 && angle < 285
                            && dist < sDist
                            && !course.Contains(c))
                        {
                            nearest = c;
                            sDist = dist;
                        }
                    }
                    course.Add(nearest);
                }
                else
                {
                    List<ControlPoint> chosen = new List<ControlPoint>();

                    if (valid.Count < 10)
                        chosen = valid;
                    else
                        for (int i = 0; i < 10; i++)
                        {
                            int n = rnd.Next(valid.Count);

                            chosen.Add(valid[n]);
                            valid.RemoveAt(n);
                        }

                    ControlPoint direct = new ControlPoint();
                    double smallestAngle = 360;

                    foreach (ControlPoint c in chosen)
                    {
                        double angle = Misc.AngleBetweenThreePoints(c.GetPoint, course.Last().GetPoint, finish.GetPoint);
                        if (angle > 180)
                            angle = 360 - angle;

                        if (angle < smallestAngle)
                        {
                            direct = c;
                            smallestAngle = angle;
                        }
                    }
                    course.Add(direct);
                }
            }
            course.Add(finish);
            return course;
        }
        private static ControlPoint FindNearestFinish(ControlPoint control)
        {
            ControlPoint nearest = new ControlPoint();
            double dist = 99999;

            foreach (ControlPoint c in specControls)
                if (c.Type == ControlPointType.Finish)
                {
                    double legDist = Misc.DistanceBetweenTwoPoints(c.GetPoint, control.GetPoint);
                    if (legDist < dist)
                    {
                        nearest = c;
                        dist = legDist;
                    }
                }
            return nearest;
        }
        private static ControlPoint GetNearest(ControlPoint current)
        {
            ControlPoint nearest = new ControlPoint();
            double dist = 99999;

            foreach (ControlPoint c in controls)
            {
                double distA = Misc.DistanceBetweenTwoPoints(c.GetPoint, current.GetPoint);
                if (distA < dist)
                {
                    dist = distA;
                    nearest = c;
                }
            }

            return nearest;
        }

        public enum LegType
        {
            VeryShort,
            Short,
            Medium,
            Long
        }
    }

    public class ControlPoint
    {
        protected double x, y;
        protected int code, id;
        protected ControlPointType type;

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

        public int ID
        {
            get => id;
            set => id = value;
        }
        public int Code
        {
            get => code;
            set => code = value;
        }
        public ControlPointType Type
        {
            get => type;
            set => type = value;
        }

        public PointF GetPoint
        {
            get => new PointF((float)x, (float)y);
        }
    }

    public enum ControlPointType
    {
        Normal,
        Start,
        Finish
    }
}
