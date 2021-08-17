using System;
using System.Drawing;
using System.IO;
using System.Net;
using System.Windows.Forms;

namespace Sunley.Orienteering.Training.Maps
{
    public class DomaMap : BaseMap
    {
        // Fields //
        private int index;
        private Image thumbnail;
        private Image blankImage;


        // Properties //
        public int Index => index;
        public Image Thumbnail => thumbnail;
        public Image BlankImage => blankImage;


        // Constructors //
        public DomaMap()
        {

        }
        public DomaMap(string url)
        {
            this.url = url;
            index = GetIndex(url);

            byte[] imgData = new WebClient().DownloadData(GetImgUrl());
            image = Image.FromStream(new MemoryStream(imgData));

            byte[] thmbData = new WebClient().DownloadData(GetThumbnailUrl());
            thumbnail = Image.FromStream(new MemoryStream(thmbData));

            byte[] blnkData = new WebClient().DownloadData(GetBlankUrl());
            blankImage = Image.FromStream(new MemoryStream(blnkData));
        }
        public DomaMap(int index)
        {
            this.index = index;
            throw new NotImplementedException();
        }


        // Public Methods //
        public PictureBox GetThumbnailControl()
        {
            if (url == null || url == "")
                throw new ArgumentNullException("URL", "URL not set");

            PictureBox p = new PictureBox();
            p.Image = thumbnail;
            p.Size = image.Size;
            p.BorderStyle = BorderStyle.None;

            return p;
        }


        // Private Methods //
        private int GetIndex(string url)
        {
            return Convert.ToInt32(url.Substring(url.LastIndexOf('=') + 1));
        }
        private string GetImgUrl()
        {
            int pos = url.IndexOf('/', 12);
            string s = url.Substring(0, pos);
            int index = GetIndex(url);

            return s + "/map_images/" + index.ToString() + ".jpg";

        }
        private string GetBlankUrl()
        {
            int pos = url.IndexOf('/', 12);
            string s = url.Substring(0, pos);
            int index = GetIndex(url);

            return s + "/map_images/" + index.ToString() + ".blank.jpg";

        }
        private string GetThumbnailUrl()
        {
            int pos = url.IndexOf('/', 12);
            string s = url.Substring(0, pos);
            int index = GetIndex(url);

            return s + "/map_images/" + index.ToString() + ".thumbnail.jpg";
        }


        // Override Methods //
        public override MapType GetType()
        {
            return MapType.Doma;
        }
    }
}
