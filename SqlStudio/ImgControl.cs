using System;
using System.Collections;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

#pragma warning disable CA1416

namespace SqlStudio
{
    public enum ZoomMode
    {
        ScaleToFit,
        Stretch,
        Zoom
    }

	public class ImgControl : System.Windows.Forms.Panel
	{
		// private variables
		private Bitmap bm = null;
		private float xZoom = 1f;
		private float yZoom = 1f;
		private float xOffset = 0;
		private float yOffset = 0;
		private ZoomMode zoomMode = ZoomMode.ScaleToFit;
		private ArrayList alShapes = null;
		private bool bNewSelectionInProgress = false;
		private Size pEditOffset = new Size(0, 0);
        private Timer interpolationTimer = null;
        private bool bDrawInterpolated = false;
        private bool bDrawnInterpolated = false;
        private DateTime dtLastRedraw;

		public ImgControl()
		{
			SetStyle(ControlStyles.ResizeRedraw | ControlStyles.AllPaintingInWmPaint | ControlStyles.DoubleBuffer, true);
			alShapes = new ArrayList();
            dtLastRedraw = DateTime.Now.AddSeconds(10);
            interpolationTimer = new Timer();
            interpolationTimer.Tick += new EventHandler(interpolationTimer_Tick);
            interpolationTimer.Interval = 1000;
            interpolationTimer.Start();
		}

        void interpolationTimer_Tick(object sender, EventArgs e)
        {
            interpolationTimer.Stop();
            if (!bDrawnInterpolated)
            {
                if (dtLastRedraw.AddMilliseconds(2000) > DateTime.Now)
                {
                    bDrawInterpolated = true;
                    Invalidate();
                }
            }
            interpolationTimer.Start();
        }

		//
		// Public properties
		//
		#region Public properties
		public Bitmap Image
		{
			get
			{
				return bm;
			}
			set
			{
				bm = value;
				SetScale();
				Invalidate();
			}
		}

		public float Zoom
		{
			set
			{
				Point p = BacktrackMouse(new Point(Width / 2, Height / 2));
				zoomMode = ZoomMode.Zoom;
				xZoom = value;
				yZoom = value;
				SetScale();
				Invalidate();
				PanTo(p);
			}
		}

		public float ZoomX
		{
			set
			{
				zoomMode = ZoomMode.Zoom;
				xZoom = value;
				SetScale();
				Invalidate();
			}
			get
			{
				return xZoom;
			}
		}

		public float ZoomY
		{
			set
			{
				zoomMode = ZoomMode.Zoom;
				yZoom = value;
				SetScale();
				Invalidate();
			}
			get
			{
				return yZoom;
			}
		}

		public ZoomMode ZoomMode
		{
			set
			{
				zoomMode = value;
				SetScale();
				Invalidate();
			}
			get
			{
				return zoomMode;
			}
		}
		#endregion

		//
		// Public functions
		//
		#region public functions

		public void ZoomToRec(Rectangle rec)
		{
			float dx = (float)Width / (float)rec.Width;
			float dy = (float)Height / (float)rec.Height;
			if (dx > dy)
				dx = dy;
			Zoom = dx;
			Point p = new Point(rec.X + (int)(rec.Width / 2), rec.Y + (int)(rec.Height / 2));
			//Point p = new Point(rec.X , rec.Y);
			PanTo(p);
		}

		public void PanTo(Point p)
		{
			if (bm == null || !AutoScroll)
				return;
			if (p.X < 0)
				p.X = 0;
			if (p.X > bm.Width)
				p.X = bm.Width;
			if (p.Y < 0)
				p.Y = 0;
			if (p.Y > bm.Height)
				p.Y = bm.Height;

			float fx = (float)p.X / (float)bm.Width;
			float fy = (float)p.Y / (float)bm.Height;
			int x = (int)(bm.Width * xZoom * fx);
			int y = (int)(bm.Height * yZoom * fy);
			int dx = (int)(Width / 2.0f);
			int dy = (int)(Height / 2.0f);
			
			AutoScrollPosition = new Point(x - dx, y - dy);
		}

		#endregion

		//
		// Overrides
		//
		#region Overrides

		protected override void OnPaint(PaintEventArgs e)
		{
            dtLastRedraw = DateTime.Now;

			if (zoomMode == ZoomMode.Zoom)
			{
				xOffset = AutoScrollPosition.X;
				yOffset = AutoScrollPosition.Y;
			}

			Matrix mx = new Matrix();
			mx.Scale(xZoom, yZoom, MatrixOrder.Append);
			mx.Translate(xOffset, yOffset, MatrixOrder.Append);

			if (bm != null)
			{
                if (bDrawInterpolated && !bNewSelectionInProgress)
                {
                    e.Graphics.InterpolationMode = InterpolationMode.HighQualityBilinear;
                    bDrawInterpolated = false;
                    bDrawnInterpolated = true;
                }
                else
                {
                    bDrawnInterpolated = false;
                }

				e.Graphics.Transform = mx;
				e.Graphics.DrawImage(bm, new Rectangle(0, 0, bm.Width, bm.Height));
			}
		}

		protected override void OnPaintBackground(PaintEventArgs pevent)
		{
			base.OnPaintBackground (pevent);
		}

		protected override void OnResize(EventArgs e)
		{
			base.OnResize (e);
			SetScale();
			Invalidate();
		}

		#endregion

		//
		// Private functions
		//
		#region Private functions

		private void SetScale()
		{
			if (bm == null)
				return;

			if (zoomMode == ZoomMode.ScaleToFit)
			{
				xZoom = (float)Width / (float)bm.Width;
				yZoom = (float)Height / (float)bm.Height;
				if (xZoom < yZoom)
				{
					yZoom = xZoom;
					xOffset = 0;
					yOffset = (Height - (int)(bm.Height * yZoom)) / 2; 
				}
				else
				{
					xZoom = yZoom;
					yOffset = 0;
					xOffset = (Width - (int)(bm.Width * xZoom)) / 2;
				}

				if (AutoScroll)
				{
					AutoScrollMinSize = new Size(0, 0);
				}
			}
			else if (zoomMode == ZoomMode.Stretch)
			{
				xOffset = 0;
				yOffset = 0;
				xZoom = (float)Width / (float)bm.Width;
				yZoom = (float)Height / (float)bm.Height;
				
				if (AutoScroll)
				{
					AutoScrollMinSize = new Size(0, 0);
				}
			}
			else // ZoomMode.Zoom
			{
				if (AutoScroll)
				{
					AutoScrollMinSize = new Size((int)(bm.Width * xZoom), (int)(bm.Height * yZoom));
				}
			}
		}

		private Point BacktrackMouse(Point p)
		{
			Point[] pts = new Point[]{p};
			Matrix mx = new Matrix();
			mx.Scale(xZoom, yZoom, MatrixOrder.Append);
			mx.Translate(xOffset, yOffset, MatrixOrder.Append);
			mx.Invert();
			mx.TransformPoints(pts);
			return pts[0];
		}
		#endregion
	}
}
