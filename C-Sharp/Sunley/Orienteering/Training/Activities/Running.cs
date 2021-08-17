using Sunley.Orienteering.Training.ActivityFiles;
using Sunley.Orienteering.Training.Footwear;
using System;

namespace Sunley.Orienteering.Training.Activities
{
    public class Running : MovingActivity
    {

        // Properties //
        public Shoe Shoes { get; }


        // Constructors //
        public Running()
        {

        }
        public Running(float time, float distance)
        {
            Time = TimeSpan.FromMinutes(time);
            Distance = distance;
        }
        public Running(TCXFile activityFile)
        {
            ActivityFile = activityFile;
            Time = ActivityFile.TotalTime;
            Distance = ActivityFile.TotalDistance;
            DateTime = ActivityFile.StartTime;
        }
        public Running(string tcxFileLocation)
        {
            ActivityFile = new TCXFile(tcxFileLocation);
            Time = ActivityFile.TotalTime;
            Distance = ActivityFile.TotalDistance;
            DateTime = ActivityFile.StartTime;
        }



        // Overrides //
        public override string ToString()
        {
            return base.ToString();
        }
    }
}
