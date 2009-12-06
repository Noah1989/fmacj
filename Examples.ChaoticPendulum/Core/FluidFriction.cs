using System;

namespace ChaoticPendulum.Core
{
	public class FluidFriction : Friction
	{
		public override Vector2D Force (Particle particle)
		{
			return -Coefficient * particle.Velocity.Length * particle.Velocity;
		}
	}
}
