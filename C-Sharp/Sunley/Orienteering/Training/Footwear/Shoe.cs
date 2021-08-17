using Sunley.Orienteering.Training.Activities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sunley.Orienteering.Training.Footwear
{
    public class Shoe : BaseFootwear
    {

        // Constructors //
        public Shoe()
        {

        }
        public Shoe(string name, float initDist = 0)
        {
            Name = name;
            distance = initDist;
        }


        // Methods //
        public override bool IsAcceptedActivity(BaseActivity @base)
        {
            return @base is Running || @base is Activities.Orienteering;
        }
    }
}
