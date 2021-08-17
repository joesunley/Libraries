using Sunley.Orienteering.Training.Activities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sunley.Orienteering.Training.Footwear
{
    public class BaseFootwear
    {

        // Fields //
        protected bool retired;
        protected float distance;
        protected Dictionary<int, MovingActivity> activities;


        // Properties //
        public string Name { get; set; }
        public float Distance => distance;
        public bool Retired => retired;
        public Dictionary<int, MovingActivity> Activities => activities;


        // Public Methods //
        public float AddDistance(float dist)
        {
            MovingActivity a = new MovingActivity(0, dist);
            activities.Add(a.GetHashCode(), a);
            UpdateDistance();
            return Distance;
        }
        public float AddDistance(MovingActivity activity)
        {
            activities.Add(activity.GetHashCode(), activity);
            UpdateDistance();
            return Distance;
        }
        public void Retire()
        {
            retired = true;
        }
        public void UpdateActivity(int hash, MovingActivity activity)
        {
            activities[hash] = activity;
            UpdateDistance();
        }
        public virtual bool IsAcceptedActivity(BaseActivity @base)
        {
            throw new InvalidOperationException("Base class cannot be included in an activity");
        }


        // Private Methods //
        private void UpdateDistance()
        {
            float dist = 0;
            foreach (MovingActivity b in activities.Values)
                dist += (float)b.Distance;
            distance = dist;
        }


        // Overrides //
        public override string ToString()
        {
            return
                Name + "," +
                distance.ToString() + "," +
                retired.ToString();
        }
        public override int GetHashCode()
        {
            throw new NotImplementedException();
            
            //  Will be modified to allow referencing the footwear from an activity
            //  Most likely will involve the ASCII codes as an int multiplied by a salt
        }
    }
}
