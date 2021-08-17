using Sunley.Orienteering.Training.ActivityFiles;
using Sunley.Orienteering.Training.Footwear;

namespace Sunley.Orienteering.Training.Activities
{
    public class Cycling : MovingActivity
    {

        // Properties //
        public Bike Bike { get; set; }


        // Constructors //
        public Cycling()
        {

        }
        public Cycling(double time, double distance)
        {

        }
        public Cycling(TCXFile activityFile)
        {

        }


        // Overrides //
        public override string ToString()
        {
            return base.ToString();
        }
    }
}
