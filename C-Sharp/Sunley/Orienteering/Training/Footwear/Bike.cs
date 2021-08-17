using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sunley.Orienteering.Training.Activities;

namespace Sunley.Orienteering.Training.Footwear
{
    public class Bike : BaseFootwear
    {

        // Constructors //
        public Bike()
        {

        }
        public Bike(string name, float initDist = 0)
        {
            Name = name;
            distance = initDist;
        }


        // Methods //
        public override bool IsAcceptedActivity(BaseActivity @base)
        {
            return @base is Cycling;
        }
    }
}
