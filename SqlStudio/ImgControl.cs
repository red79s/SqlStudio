using System;
using System.Collections;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

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
			this.SetStyle(ControlStyles.ResizeRedraw | ControlStyles.AllPaintingInWmPaint | ControlStyles.DoubleBuffer, true);
			this.alShapes = new ArrayList();
            this.dtLastRedraw = DateTime.Now.AddSeconds(10);
            this.interpolationTimer = new Timer();
            this.interpolationTimer.Tick += new EventHandler(interpolationTimer_Tick);
            this.interpolationTimer.Interval = 1000;
            this.interpolationTimer.Start();
		}

        void interpolationTimer_Tick(object sender, EventArgs e)
        {
            this.interpolationTimer.Stop();
            if (!this.bDrawnInterpolated)
            {
                if (this.dtLastRedraw.AddMilliseconds(2000) > DateTime.Now)
                {
                    this.bDrawInterpolated = true;
                    this.Invalidate();
                }
            }
            this.interpolationTimer.Start();
        }

		//
		// Public properties
		//
		#region Public properties
		public Bitmap Image
		{
			get
			{
				return this.bm;
			}
			set
			{
				this.bm = value;
				this.SetScale();
				this.Invalidate();
			}
		}

		public float Zoom
		{
			set
			{
				Point p = this.BacktrackMouse(new Point(this.Width / 2, this.Height / 2));
				this.zoomMode = ZoomMode.Zoom;
				this.xZoom = value;
				this.yZoom = value;
				this.SetScale();
				this.Invalidate();
				this.PanTo(p);
			}
		}

		public float ZoomX
		{
			set
			{
				this.zoomMode = ZoomMode.Zoom;
				this.xZoom = value;
				this.SetScale();
				this.Invalidate();
			}
			get
			{
				return this.xZoom;
			}
		}

		public float ZoomY
		{
			set
			{
				this.zoomMode = ZoomMode.Zoom;
				this.yZoom = value;
				this.SetScale();
				this.Invalidate();
			}
			get
			{
				return this.yZoom;
			}
		}

		public ZoomMode ZoomMode
		{
			set
			{
				this.zoomMode = value;
				this.SetScale();
				this.Invalidate();
			}
			get
			{
				return this.zoomMode;
			}
		}
		#endregion

		//
		// Public functions
		//
		#region public functions

		public void ZoomToRec(Rectangle rec)
		{
			float dx = (float)this.Width / (float)rec.Width;
			float dy = (float)this.Height / (float)rec.Height;
			if (dx > dy)
				dx = dy;
			this.Zoom = dx;
			Point p = new Point(rec.X + (int)(rec.Width / 2), rec.Y + (int)(rec.Height / 2));
			//Point p = new Point(rec.X , rec.Y);
			this.PanTo(p);
		}

		public void PanTo(Point p)
		{
			if (this.bm == null || !this.AutoScroll)
				return;
			if (p.X < 0)
				p.X = 0;
			if (p.X > this.bm.Width)
				p.X = this.bm.Width;
			if (p.Y < 0)
				p.Y = 0;
			if (p.Y > this.bm.Height)
				p.Y = this.bm.Height;

			float fx = (float)p.X / (float)this.bm.Width;
			float fy = (float)p.Y / (float)this.bm.Height;
			int x = (int)(this.bm.Width * this.xZoom * fx);
			int y = (int)(this.bm.Height * this.yZoom * fy);
			int dx = (int)(this.Width / 2.0f);
			int dy = (int)(this.Height / 2.0f);
			
			this.AutoScrollPosition = new Point(x - dx, y - dy);
		}

		#endregion

		//
		// Overrides
		//
		#region Overrides

		protected override void OnPaint(PaintEventArgs e)
		{
            this.dtLastRedraw = DateTime.Now;

			if (this.zoomMode == ZoomMode.Zoom)
			{
				this.xOffset = this.AutoScrollPosition.X;
				this.yOffset = this.AutoScrollPosition.Y;
			}

			Matrix mx = new Matrix();
			mx.Scale(this.xZoom, this.yZoom, MatrixOrder.Append);
			mx.Translate(this.xOffset, this.yOffset, MatrixOrder.Append);

			if (this.bm != null)
			{
                if (this.bDrawInterpolated && !this.bNewSelectionInProgress)
                {
                    e.Graphics.InterpolationMode = InterpolationMode.HighQualityBilinear;
                    this.bDrawInterpolated = false;
                    this.bDrawnInterpolated = true;
                }
                else
                {
                    this.bDrawnInterpolated = false;
                }

				e.Graphics.Transform = mx;
				e.Graphics.DrawImage(this.bm, new Rectangle(0, 0, bm.Width, bm.Height));
			}
		}

		protected override void OnPaintBackground(PaintEventArgs pevent)
		{
			base.OnPaintBackground (pevent);
		}

		protected override void OnResize(EventArgs e)
		{
			base.OnResize (e);
			this.SetScale();
			this.Invalidate();
		}

		#endregion

		//
		// Private functions
		//
		#region Private functions

		private void SetScale()
		{
			if (this.bm == null)
				return;

			if (this.zoomMode == ZoomMode.ScaleToFit)
			{
				this.xZoom = (float)this.Width / (float)this.bm.Width;
				this.yZoom = (float)this.Height / (float)this.bm.Height;
				if (this.xZoom < this.yZoom)
				{
					this.yZoom = this.xZoom;
					this.xOffset = 0;
					this.yOffset = (this.Height - (int)(this.bm.Height * this.yZoom)) / 2; 
				}
				else
				{
					this.xZoom = this.yZoom;
					this.yOffset = 0;
					this.xOffset = (this.Width - (int)(this.bm.Width * this.xZoom)) / 2;
				}

				if (this.AutoScroll)
				{
					this.AutoScrollMinSize = new Size(0, 0);
				}
			}
			else if (this.zoomMode == ZoomMode.Stretch)
			{
				this.xOffset = 0;
				this.yOffset = 0;
				this.xZoom = (float)this.Width / (float)this.bm.Width;
				this.yZoom = (float)this.Height / (float)this.bm.Height;
				
				if (this.AutoScroll)
				{
					this.AutoScrollMinSize = new Size(0, 0);
				}
			}
			else // ZoomMode.Zoom
			{
				if (this.AutoScroll)
				{
					this.AutoScrollMinSize = new Size((int)(this.bm.Width * this.xZoom), (int)(this.bm.Height * this.yZoom));
				}
			}
		}

		private Point BacktrackMouse(Point p)
		{
			Point[] pts = new Point[]{p};
			Matrix mx = new Matrix();
			mx.Scale(this.xZoom, this.yZoom, MatrixOrder.Append);
			mx.Translate(this.xOffset, this.yOffset, MatrixOrder.Append);
			mx.Invert();
			mx.TransformPoints(pts);
			return pts[0];
		}
		#endregion
	}
}
