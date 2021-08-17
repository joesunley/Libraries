using System;
using System.Drawing;
using System.Windows.Forms;

namespace Sunley.Orienteering.Training.Maps
{
    public class BaseMap
    {
        // Fields //
        protected string url;
        protected Image image;


        // Properties //
        public string URL => url;
        public Image Image => image;


        // Constructors //
        public BaseMap() { }


        // Methods //
        public void ShowMap()
        {
            GetForm().ShowDialog();
        }
        public MapDisplay GetControl()
        {
            return new MapDisplay(this);
        }
        public Form GetForm()
        {
            if (url == null || url == "")
                throw new ArgumentNullException("URL", "URL not set");

            MapDisplay m = new MapDisplay(this);

            Form f = new Form()
            {
                MinimizeBox = false,
                MaximizeBox = false,
                ShowIcon = false,
                Text = "",
                ShowInTaskbar = false,
                Size = m.Size
            };

            f.Controls.Add(m);
            return f;
        }

        public virtual new MapType GetType() { return MapType.Base; }
    }

    public enum MapType
    {
        Base = 0,
        Doma = 1,
        Static = 2
    }
}
