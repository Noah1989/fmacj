using System;

namespace ChaoticPendulum.Core
{	
	public class Particle
	{
		public double Mass { get; set; }	
		public double Charge { get; set; }
		
		public Vector2D Position { get; set; }
		public Vector2D Velocity { get; set; }	
				
		public Func<Particle, Vector2D> Force { get; set; }
		
		public double KineticEnergy { get { return .5 * Mass * Velocity.LengthSquared; } }
		
		public void Iterate(double timeStep)
		{
			Velocity += 1 / Mass * timeStep * Force(this);
        	Position += timeStep * Velocity;
		}
	}
}
