using System;

namespace ChaoticPendulum.Core
{	
	public abstract class Friction
	{
		public double Coefficient { get; set; }
				
		public abstract Vector2D Force(Particle particle);
	}
}
