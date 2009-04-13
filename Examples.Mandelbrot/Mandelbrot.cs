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
using Fmacj.Framework;

namespace Fmacj.Examples.Mandelbrot
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
			line = new Line(y, width, System.Threading.Thread.CurrentThread.ManagedThreadId);

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
				
				line.Data[x] = i;
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
            	
            	for (int x = 0; x < width; x++)
            	{
                	Color color = Color.FromArgb(0/*(50*line.ColorId)%255*/, line.Data[x], 0);
                	bitmap.SetPixel(x, y, color);
            	}            	
				
				Console.WriteLine("Rendered line {0}", y);
			}
			
            return true;
        }
	    [Join]
		protected abstract bool RenderLines();		
		
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
