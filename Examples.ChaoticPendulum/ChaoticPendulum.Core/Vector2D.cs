
using System;

namespace ChaoticPendulum.Core
{
	public struct Vector2D
	{
    	public Double X { get; set; }
		public Double Y { get; set; }

    	public Vector2D(double x, double y)
		{        
			X = x;
        	Y = y;
   	 	}
	
		public bool IsZero 
		{
			get { return X == 0 && Y == 0; }
		}
		
		public double Length
		{
			get { return Math.Sqrt(X * X + Y * Y); }
		}
		
		public double LengthSquared
		{
			get { return (X * X + Y * Y); }
		}

    	public Vector2D Unity
        {
			get { return 1 / Length * this; }
		}

    	public static Vector2D ZeroVector
        {
			get { return new Vector2D(0, 0); }
		}

    	public static Vector2D operator +(Vector2D vec1 ,Vector2D vec2) 
		{
         	return new Vector2D(vec1.X + vec2.X, vec1.Y + vec2.Y);
		}

   		public static Vector2D operator -(Vector2D vec1 ,Vector2D vec2) 
		{
       		return new Vector2D(vec1.X - vec2.X, vec1.Y - vec2.Y);
   		}

    	public static Vector2D operator *(double a, Vector2D vec)
		{
        	return new Vector2D(a * vec.X, a * vec.Y);
		}
	}
}
