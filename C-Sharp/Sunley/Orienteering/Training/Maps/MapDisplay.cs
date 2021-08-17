using Sunley.Miscellaneous;
using System;
using System.Drawing;
using System.Windows.Forms;


namespace Sunley.Orienteering.Training.Maps
{
    public partial class MapDisplay : UserControl
    {
        private Image img, blnkImg;
        private bool routeVisible;

        public MapDisplay(BaseMap map)
        {
            switch (map.GetType())
            {
                case MapType.Base:
                    throw new ArgumentException("Cannot display a map of type 'Base'");
                case MapType.Doma:
                    SetDoma(map);
                    break;
                case MapType.Static:
                    SetStatic(map);
                    break;
                default:
                    break;
            }
        }

        private void SetDoma(BaseMap map)
        {
            InitializeComponent();

            DomaMap domaMap = (DomaMap)map;

            img = domaMap.Image;
            blnkImg = domaMap.BlankImage;

            picBox.Click += new EventHandler(ChangeDomaImg);

            ResizeDoma();
            picBox.Image = img;
            routeVisible = true;
        }
        private void ChangeDomaImg(object sender, EventArgs e)
        {
            if (routeVisible)
            {
                picBox.Image = blnkImg;
                routeVisible = false;
            }
            else
            {
                picBox.Image = img;
                routeVisible = true;
            }
        }

        private void SetStatic(BaseMap map)
        {
            InitializeComponent();

            ResizeStatic();
            picBox.Image = map.Image;
        }


        private int ResizeDoma()
        {
            float RESIZE_CONST = 0.85f;

            Size scrnSize = Screen.FromControl(this).Bounds.Size;
            if (scrnSize.Width > img.Width && scrnSize.Height > img.Height)
                return -1;

            int height = Convert.ToInt32(RESIZE_CONST * scrnSize.Height);
            float ratio = (float)img.Width / (float)img.Height;
            int width = Convert.ToInt32(height * ratio);

            if (width < scrnSize.Width)
            {
                img = Visuals.ResizeImage(img, width, height);
                blnkImg = Visuals.ResizeImage(blnkImg, width, height);

                Height = height;
                Width = width;

                return 1;
            }

            width = Convert.ToInt32(RESIZE_CONST * scrnSize.Width);
            ratio = 1 / ratio;
            height = Convert.ToInt32(width * ratio);

            if (height < scrnSize.Height)
            {
                img = Visuals.ResizeImage(img, width, height);
                blnkImg = Visuals.ResizeImage(blnkImg, width, height);

                Height = height;
                Width = width;

                return 1;
            }
            return -1;


        }
        private int ResizeStatic()
        {
            Size scrnSize = Screen.FromControl(this).Bounds.Size;
            if (scrnSize.Width > img.Width && scrnSize.Height > img.Height)
                return -1;

            int height = Convert.ToInt32(0.75 * scrnSize.Height);
            float ratio = (float)img.Width / (float)img.Height;
            int width = Convert.ToInt32(height * ratio);

            if (width < scrnSize.Width)
            {
                img = Visuals.ResizeImage(img, width, height);

                return 1;
            }

            width = Convert.ToInt32(0.75 * scrnSize.Width);
            ratio = 1 / ratio;
            height = Convert.ToInt32(width * ratio);

            if (height < scrnSize.Height)
            {
                img = Visuals.ResizeImage(img, width, height);

                return 1;
            }
            return -1;
        }

    }
}
