using Sunley.Orienteering.Navigation;
using System;
using System.Collections.Generic;

namespace Sunley.Orienteering.Training.ActivityFiles
{
    public class BaseActivityFile
    {
        public virtual string Name { get; }
        public virtual string Sport { get; }
        public virtual DateTime StartTime { get; }
        public virtual TimeSpan TotalTime { get; }
        public virtual float TotalDistance { get; }
        public virtual float TotalCalories { get; }
        public virtual List<DateTime> Time { get; }
        public virtual List<int> HeartRate { get; }
        public virtual List<int> Cadence { get; }
        public virtual List<float> Speed { get; }
        public virtual List<float> Altitude { get; }
        public virtual List<float> Temperature { get; }
        public virtual List<float> Distance { get; }
        public virtual List<Coordinate> Position { get; }

        public virtual int AverageHeartRate() { return -1; }
        public virtual int MaxHeartRate() { return -1; }
        public virtual int AverageCadence() { return -1; }
        public virtual int MaxCadence() { return -1; }
        public virtual float AverageSpeed() { return -1f; }
        public virtual float MaxSpeed() { return -1f; }
        public virtual float MinElevation() { return -1f; }
        public virtual float MaxElevation() { return -1f; }
        public virtual float ElevationGain() { return -1f; }
        public virtual float ElevationLoss() { return -1f; }

        public override int GetHashCode()
        {
            return
                Convert.ToInt32(StartTime.Ticks) ^
                Convert.ToInt32(Math.Round(TotalTime.TotalSeconds)) ^
                Convert.ToInt32(Math.Round(TotalDistance)) ^
                AverageHeartRate();
        }
    }
}
