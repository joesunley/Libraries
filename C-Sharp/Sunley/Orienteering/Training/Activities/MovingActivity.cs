using Sunley.Miscellaneous;
using Sunley.Orienteering.Training.ActivityFiles;
using System;

namespace Sunley.Orienteering.Training.Activities
{
    public class MovingActivity : BaseActivity
    {
        // Properties //
        public string Type { get; set; }
        public TimeSpan Time { get; set; } // min
        public float Distance { get; set; } // metres
        public BaseActivityFile ActivityFile { get; set; }
        public string Description { get; set; }


        // Constructors //
        public MovingActivity()
        {

        }
        public MovingActivity(float time, float distance)
        {
            Time = TimeSpan.FromMinutes(time);
            Distance = distance;
        }
        public MovingActivity(TCXFile activityFile)
        {
            ActivityFile = activityFile;
            Time = ActivityFile.TotalTime;
            Distance = ActivityFile.TotalDistance;
            DateTime = ActivityFile.StartTime;
        }
        public MovingActivity(string tcxFileLocation)
        {
            ActivityFile = new TCXFile(tcxFileLocation);
            Time = ActivityFile.TotalTime;
            Distance = ActivityFile.TotalDistance;
            DateTime = ActivityFile.StartTime;
        }


        // Methods //
        public float GetAveragePace() { return (float)Misc.SpeedToPace(ActivityFile.AverageSpeed()); } // min/km
        public float GetBestPace() { return (float)Misc.SpeedToPace(ActivityFile.MaxSpeed()); } // min/km
        public float GetAverageSpeed() { return ActivityFile.AverageSpeed(); }
        public float GetMaxSpeed() { return ActivityFile.MaxSpeed(); }
        public float GetGradeAdjustedPace() { return (float)Misc.SpeedToPace(GetGradeAdjustedSpeed()); }
        public float GetGradeAdjustedSpeed()
        {
            float dist = Distance + ActivityFile.ElevationGain() * Misc.GradeConstant;
            return (float)(dist / Time.TotalSeconds);
        }
        public int GetAverageHeartRate() { return ActivityFile.AverageHeartRate(); }
        public int GetMaxHeartRate() { return ActivityFile.MaxHeartRate(); }
        public float GetTotalClimb() { return ActivityFile.ElevationGain(); }


        // Overrides //
        public override string ToString()
        {
            return base.ToString();
        }
        public override int GetHashCode()
        {
            return
                Convert.ToInt32(Distance) ^
                Convert.ToInt32(Time.TotalSeconds) ^
                ActivityFile.GetHashCode();
        }
    }
}
