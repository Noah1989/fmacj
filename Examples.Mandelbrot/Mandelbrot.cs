/*
    FMACJ Parallelization Framework for .NET
    Copyright (C) 2008  Stefan Noack

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/

using System;
using System.Drawing;
using Fmacj.Framework;

namespace Fmacj.Examples.Mandelbrot
{
	[Parallelizable]
	public abstract class Mandelbrot : IParallelizable
	{		
		private int width, height;
		
		private Graphics graphics;
		
		public Bitmap Calculate(int size)
		{
			width = size; height = size*11/25;
			Bitmap bitmap = new Bitmap(width, height);

		    graphics = Graphics.FromImage(bitmap);

			Console.WriteLine("Bitmap initialized.");
			
			for (int y = 0; y < height; y++)
				CalculateLine(y);
			
			for (int n = 0; n < height; n++)
				Console.WriteLine("Rendered line {0}.", RenderLine());

		    graphics.Dispose();

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

            Bitmap lineBitmap = new Bitmap(width, 1);

            for (int x = 0; x < width; x++)
            {
                Color color = ColorUtil.FromHSB(hue, 100, line.Data[x]*100/255);
                lineBitmap.SetPixel(x, 0, color);
            }

            lock(graphics) graphics.DrawImage(lineBitmap, 0, y);

            lineBitmap.Dispose();

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

	    public abstract void Dispose();
	}
}
