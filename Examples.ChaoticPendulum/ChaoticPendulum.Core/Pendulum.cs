using System;

namespace ChaoticPendulum.Core
{	
	public class Pendulum
	{
		public AttractorSet Attractors { get; set; }
		public Particle Particle { get; set; }
		public Friction Friction { get; set; }
		
		public Pendulum() : this(0) {}
		
		public Pendulum(int attractorCount)
		{
			Attractors = new AttractorSet();
			for (int i = 0; i < attractorCount; i++)
			{
				var angle = i * 2 * Math.PI / attractorCount;
				Attractors.Add(new Attractor { Position = new Vector2D(Math.Cos(angle), Math.Sin(angle)) });
			}
						
			Particle = new Particle { Force = ForceOnParticle };
			Friction = new FluidFriction();
		}
		
		private Vector2D ForceOnParticle(Particle particle)
		{
			return Attractors.Force(particle) + Friction.Force(particle);
		}	
		
		public void Iterate(double timeStep, int stepCount)
		{
			for (int i = 0; i < stepCount; i++)
				Particle.Iterate(timeStep);
		}
		                  
	}
}
