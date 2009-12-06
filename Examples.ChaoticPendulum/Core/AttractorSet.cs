
using System;
using System.Linq;
using System.Collections.Generic;

namespace ChaoticPendulum.Core
{	
	public class AttractorSet : List<Attractor>
	{	
		public AttractorSet()
		{
		}
		
		public Vector2D Force(Particle particle)
		{			
			return (from attractor in this select attractor.Force(particle))
					.Aggregate((f1, f2) => f1 + f2);
		}	
		
		public double Potential(Particle particle)
		{			
			return (from attractor in this select attractor.Potential(particle))
					.Aggregate((p1, p2) => p1 + p2);
		}	
		
		public double Z
		{
			set	{ foreach(var attractor in this) attractor.Z = value; } 
		}
		
		public double Charge
		{
			set	{ foreach(var attractor in this) attractor.Charge = value; } 
		}
	}
}
