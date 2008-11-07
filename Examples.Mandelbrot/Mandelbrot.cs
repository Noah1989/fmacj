using System;
using System.Drawing;
using Fmacj.Framework;

namespace Examples.Mandelbrot
{
	[Parallelizable]
	public abstract class Mandelbrot : IParallelizable
	{		
		private int width, height;
		
		private Bitmap bitmap;
		
		public Bitmap Calculate(int size)
		{
			width = size; height = size*11/25;
			bitmap = new Bitmap(width, height);

			Console.WriteLine("Bitmap initialized.");
			
			for (int y = 0; y < height; y++)
				CalculateLine(y);
			
			for (int n = 0; n < height; n++)
				Console.WriteLine("Rendered line {0}.", RenderLine());
			
			return bitmap;
		}

		[Fork]
		protected abstract void CalculateLine(int y);
		[Asynchronous]
		protected void CalculateLine(int y, [Channel("line")] out Line line)
		{
			line = new Line(y, width, System.Threading.Thread.CurrentThread.ManagedThreadId);

			double cy = 1.1*y/height;
			
			for (int x=0;x<width;x++)
			{
				double cx = 2.5*x/width - 2.0;

				double zx = 0, zy = 0;
				double tx;
				
				byte i;
				for(i = 255; (i > 0) && (zx*zx + zy*zy < 4); i--)
				{
					tx = zx;					
					zx = zx*zx-zy*zy + cx;
					zy = 2.0*tx*zy + cy;
				}
				
				line.Data[x] = i;
			}
		}
		
		[Chord]
		protected int RenderLine([Channel("line")] Line line)
		{
			int y = line.Y;
			int hue = (35*line.ColorId)%360;
			
			for (int x=0;x<width;x++)		
			{
				Color color = ColorUtil.FromHSB(hue,100,line.Data[x]*100/255);
				bitmap.SetPixel(x,y,color);
			}
			
			return y;
		}		
		[Join]
		protected abstract int RenderLine();		
		
		protected struct Line
		{
			public Line(int y, int width, int colorId)
			{
				this.y = y;
				this.data = new byte[width];
				this.colorId = colorId;
			}
			
			private int y;
			private byte[] data;
			private int colorId;
			
			public int Y { get { return y; } }
			public byte[] Data { get { return data; } }
			public int ColorId { get { return colorId; } }
		}		
	}
}
