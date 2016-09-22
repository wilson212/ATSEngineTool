using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;

namespace System.Windows.Forms
{
    /// <summary>
    /// Summary description for ShadowLabel.
    /// </summary>
    /// <seealso cref="https://blogs.msdn.microsoft.com/cjacks/2006/05/26/creating-text-labels-with-a-drop-shadow-effect-in-windows-forms/"/>
    [ToolboxItem(true)]
	public class ShadowLabel : Label
    {
        private Color color;
        private int direction;
        private float softness;
        private int opacity;
        private int shadowDepth;

        public ShadowLabel() : base() {
            color = Color.Black;
            direction = 315;
            softness = 2f;
            opacity = 100;
            shadowDepth = 4;
        }

        [Category("Appearance")]
        [Description("Gets or sets the color of the shadow")]
        [DefaultValue(typeof(Color), "0x000000")]
        public Color ShadowColor
        {
            get
            {
                return color;
            }
            set
            {
                color = value;
                Invalidate();
            }
        }

        [Category("Appearance")]
        [Description("Gets or sets the degree of opacity of the shadow")]
        [DefaultValue(100)]
        public int ShadowOpacity
        {
            get
            {
                return opacity;
            }
            set
            {
                if (value < 0 || value > 255)
                {
                    throw new ArgumentOutOfRangeException("Opacity", "Opacity must be between 0 and 255");
                }
                opacity = value;
                Invalidate();
            }
        }

        [Category("Appearance")]
        [Description("Gets or sets how soft the shadow is")]
        [DefaultValue(2f)]
        public float ShadowSoftness
        {
            get
            {
                return softness;
            }
            set
            {
                if (value <= 0)
                {
                    throw new ArgumentOutOfRangeException("Softness", "Softness must be greater than 0");
                }
                softness = value;
                Invalidate();
            }
        }

        [Category("Appearance")]
        [Description("Gets or sets the angle the shadow is cast")]
        [DefaultValue(315)]
        public int ShadowDirection
        {
            get
            {
                return direction;
            }
            set
            {
                if (value < 0 || value > 360)
                {
                    throw new ArgumentOutOfRangeException("Direction", "Direction must be between 0 and 360");
                }
                direction = value;
                Invalidate();
            }
        }

        [Category("Appearance")]
        [Description("Gets or sets the distance between the plane " +
          "of the object casting the shadow and the shadow plane")]
        [DefaultValue(4)]
        public int ShadowDepth
        {
            get
            {
                return shadowDepth;
            }
            set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException("ShadowDepth", "ShadowDepth must be greater than 0");
                }
                shadowDepth = value;
                Invalidate();
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics screenGraphics = e.Graphics;
            Bitmap shadowBitmap = new Bitmap(
                Math.Max((int)(Width / softness), 1),
                Math.Max((int)(Height / softness), 1)
            );

            using (Graphics imageGraphics = Graphics.FromImage(shadowBitmap))
            {
                double angle = Math.PI * direction / 180.0;
                imageGraphics.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;
                Matrix transformMatrix = new Matrix();
                transformMatrix.Scale(1 / softness, 1 / softness);
                transformMatrix.Translate((float)(shadowDepth * Math.Cos(angle)), (float)(shadowDepth * Math.Sin(angle)));
                imageGraphics.Transform = transformMatrix;
                imageGraphics.DrawString(Text, Font, new SolidBrush(Color.FromArgb(opacity, color)), 0, 0, StringFormat.GenericTypographic);
            }

            screenGraphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
            screenGraphics.DrawImage(shadowBitmap, ClientRectangle, 0, 0, shadowBitmap.Width, shadowBitmap.Height, GraphicsUnit.Pixel);
            screenGraphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
            screenGraphics.DrawString(Text, Font, new SolidBrush(ForeColor), 0, 0, StringFormat.GenericTypographic);
        }
    }
}