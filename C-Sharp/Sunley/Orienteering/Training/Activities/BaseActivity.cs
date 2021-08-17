using System;
using System.Text;

namespace Sunley.Orienteering.Training.Activities
{
    public class BaseActivity
    {
        // Properties //
        public string Name { get; set; }
        public DateTime DateTime { get; set; }
        public ActivityFeel Feel { get; set; }
        public PerceivedEffort PerceivedEffort { get; set; }


        // Overrides //
        public override string ToString()
        {
            return Name + "," + DateTime.ToUniversalTime() + "," + ((int)Feel).ToString() + "," + ((int)PerceivedEffort).ToString();
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

    public enum Difficulty { VeryEasy, Easy, Advanced, Difficult, VeryDifficult }
    public enum ActivityFeel { VeryWeak, Weak, Normal, Strong, VeryStrong }
    public enum PerceivedEffort { None, VeryLight, VeryEasy, Easy, ModeratelyEasy, Moderate, ModeratelyHard, Difficult, VeryDifficult, SubMaximum, Maximum }

}
