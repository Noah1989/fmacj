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

namespace Fmacj.Examples.Mandelbrot
{
	internal static class ColorUtil
	{
		
		public static Color FromHSB(int hue, int saturation, int brightness)
		{
			if(hue<0 || hue>360)
				throw new ArgumentException("Value must be between 0 and 360", "hue");
			if(saturation<0 || saturation>100)
				throw new ArgumentException("Value must be between 0 and 100", "saturation");
			if(brightness<0 || brightness>100)
				throw new ArgumentException("Value must be between 0 and 100", "brightness");
        
			float h = (float)hue;
			float s = ((float)saturation) / 100f;
			float v = ((float)brightness) / 100f;

			int i;
			float f, p, q, t;
			float r,g,b;

			if( saturation == 0 )
			{
				// achromatic (grey)
				return Color.FromArgb((int)(v*255),(int)(v*255),(int)(v*255));
			}
			h /= 60; // sector 0 to 5
			i = (int)Math.Floor( h );
			f = h - i;
			p = v * ( 1 - s );
			q = v * ( 1 - s * f );
			t = v * ( 1 - s * ( 1 - f ) );
			switch( i )
			{
			case 0:
                r = v;
				g = t;
                b = p;
                break;
            case 1:
                r = q;
				g = v;
                b = p;
                break;
            case 2:
                r = p;
                g = v;
                b = t;
                break;
            case 3:
                r = p;
                g = q;
                b = v;
                break;
            case 4:
                r = t;
                g = p;
                b = v;
                break;
            case 5: default:
                r = v;
                g = p;
                b = q;
                break;
			}
			return Color.FromArgb((int)(r*255f), (int)(g*255f), (int)(b*255f));
		}		
	} 
}
