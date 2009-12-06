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
using System.Linq;
using Fmacj.Core.Framework;

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
			
			TakeLines(height);
			
			for (int y = 0; y < height; y++)
				CalculateLine(y);				
						
			RenderLines();		    

			return bitmap;
		}

		[Fork]
		protected abstract void CalculateLine(int y);
		[Asynchronous]
		protected void CalculateLine(int y, [Channel("lines")] out Line line)
		{
			line = new Line(y, width);

			double cy = 1.1*y/height;
			
			for (int x=0; x<width; x++)
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

                Color color = Color.FromArgb(i, i, i);
                line.Bitmap.SetPixel(x, 0, color);
			}
		}

		[Yield]
		protected abstract void TakeLines([Channel("lineCount")] int lineCount);
		
        [Chord]
        protected bool RenderLines([Channel("lineCount")] int lineCount, [Channel("lines", Enumerable = true)] IChannelEnumerable<Line> lines)
        {		
			foreach (Line line in lines.Take(lineCount))
			{
            	int y = line.Y;
                graphics.DrawImage(line.Bitmap, 0, y); 	
                line.Bitmap.Dispose();
			}
			
            return true;
        }
	    [Join]
		protected abstract bool RenderLines();		
		
		protected struct Line
		{
			public Line(int y, int width)
			{
				this.y = y;
				bitmap = new Bitmap(width, 1);
			}
			
			private int y;
			private Bitmap bitmap;
			
			public int Y { get { return y; } }
			public Bitmap Bitmap { get { return bitmap; } }
		}

	    public abstract void Dispose();
	}
}
