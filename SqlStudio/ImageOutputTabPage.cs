using System;
using System.Windows.Forms;
using System.Drawing;

namespace SqlStudio
{
    public class ImageOutputTabPage : TabPage
    {
        private ImgControl _imgControl = new ImgControl();
        public ImageOutputTabPage()
        {
            _imgControl.Dock = DockStyle.Fill;
            _imgControl.ZoomMode = ZoomMode.ScaleToFit;
            Controls.Add(_imgControl);
        }

        public void SetImage(Bitmap img)
        {
            _imgControl.Image = img;
        }
    }
}
