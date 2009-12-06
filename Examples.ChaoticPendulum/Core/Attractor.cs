
using System;

namespace ChaoticPendulum.Core
{
	
	
	public class Attractor
	{
		public Vector2D Position { get; set; }
		public double Z { get; set; }
		
		public double Charge { get; set; }		
		
		public Vector2D Force(Particle particle)
		{				
			var xyDistance = Position - particle.Position;
			if(xyDistance.IsZero) return Vector2D.ZeroVector;
			
			var xyProjectionCoefficient = xyDistance.Length / Math.Sqrt(xyDistance.LengthSquared + Z * Z);			
			
			// charges with different signs attract each other
			return -Charge * particle.Charge * xyProjectionCoefficient 			
					/ (xyDistance.LengthSquared + Z * Z) 
					* xyDistance.Unity;
			
		}	
		
		public double Potential(Particle particle)
		{			
			var xyDistance = Position - particle.Position;
						
			// charges with different signs create a negative potential
			return Charge * particle.Charge
					/ Math.Sqrt(xyDistance.LengthSquared + Z * Z);			
		}
	}
}
