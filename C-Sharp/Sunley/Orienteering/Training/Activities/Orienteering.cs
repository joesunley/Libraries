using Sunley.Orienteering.Results;
using Sunley.Orienteering.Training.ActivityFiles;
using Sunley.Orienteering.Training.Maps;
using System;

namespace Sunley.Orienteering.Training.Activities
{
    public class Orienteering : Running
    {
        // Properties //
        public Difficulty Difficulty { get; set; }
        public ResultsFile Results { get; set; }
        public BaseMap MapFile { get; set; }


        // Constructors //
        public Orienteering()
        {

        }
        public Orienteering(float time, float distance)
        {
            Time = TimeSpan.FromMinutes(time);
            Distance = distance;
        }
        public Orienteering(TCXFile activityFile, string domaURL = "", string winsplitsURL = "")
        {
            ActivityFile = activityFile;
            Time = ActivityFile.TotalTime;
            Distance = ActivityFile.TotalDistance;
            DateTime = ActivityFile.StartTime;

            if (!(domaURL == ""))
                SetDomaMap(domaURL);
            if (!(winsplitsURL == ""))
                SetWinsplits(winsplitsURL);
        }
        public Orienteering(string tcxFileLocation, string domaURL = "", string winsplitsURL = "")
        {
            ActivityFile = new TCXFile(tcxFileLocation);
            Time = ActivityFile.TotalTime;
            Distance = ActivityFile.TotalDistance;
            DateTime = ActivityFile.StartTime;

            if (!(domaURL == ""))
                SetDomaMap(domaURL);
            if (!(winsplitsURL == ""))
                SetWinsplits(winsplitsURL);
        }


        // Methods //
        public void SetDomaMap(int index)
        {
            MapFile = new DomaMap(index);
        }
        public void SetDomaMap(string url)
        {
            MapFile = new DomaMap(url);
        }
        public void SetStaticImage(string url)
        {
            MapFile = new StaticMap(url);
        }
        public void SetWinsplits(int databaseID, int categoryID)
        {
            Results = Winsplits.GetResults(databaseID, categoryID);
        }
        public void SetWinsplits(string url)
        {
            Results = Winsplits.GetResults(url);
        }


        // Overrides //
        public override string ToString()
        {
            return base.ToString();
        }
    }
}
