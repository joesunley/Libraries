using System.Drawing;
using System.IO;
using System.Net;

namespace Sunley.Orienteering.Training.Maps
{
    public class StaticMap : BaseMap
    {
        // Constructors //
        public StaticMap()
        {

        }
        public StaticMap(string url)
        {
            this.url = url;

            byte[] data = new WebClient().DownloadData(url);
            image = Image.FromStream(new MemoryStream(data));
        }


        // Override Methods //
        public override MapType GetType()
        {
            return MapType.Static;
        }
    }
}
