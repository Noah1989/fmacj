using System;
using System.Drawing;
using ChaoticPendulum.Core;

namespace ChaoticPendulum.Visualization
{	
	public class TrajectoryGraph
	{		
		private Pendulum pendulum;
		private Graphics graphics;
		
		private Bitmap bitmap;
		
		public TrajectoryGraph(Pendulum pendulum, int width, int height)
		{			
			Resize(width, height);
			SetPendulum(pendulum);			
		}
		
		public void SetPendulum(Pendulum pendulum)
		{
			this.pendulum = pendulum;
			Clear();
		}
		
		public void Resize(int width, int height)
		{
			if(graphics != null) graphics.Dispose();
			if(bitmap != null) bitmap.Dispose();
				
			bitmap = new Bitmap(width, height);
			graphics = Graphics.FromImage(bitmap);
		 	Clear();
		}
		
		public void Clear()
		{
			recentPosition = null;	
			graphics.Clear(Color.White);
		}
		
		public void DrawAttractors(Graphics target)
		{
			foreach (var attractor in pendulum.Attractors)
			{
				DrawMarker(target, attractor.Position, Color.Red);
			}
		}
		
		private Vector2D? recentPosition;
		public void DrawParticle(Graphics target)
		{
			if(recentPosition.HasValue) DrawMarker(target, recentPosition.Value, Color.LightGray);
			DrawMarker(target, pendulum.Particle.Position, Color.Blue);
			recentPosition = pendulum.Particle.Position;
		}
		
		private void DrawMarker(Graphics target, Vector2D position, Color color)
		{
			var rect =  new Rectangle(GetVectorCoordinates(position)
			                          - new Size(1, 1),
			                          new Size(2, 2));
			graphics.DrawRectangle(new Pen(color), rect);
			
			rect.Width++;
			rect.Height++;
			
			target.DrawImage(bitmap, rect, rect, GraphicsUnit.Pixel);				
		}
		
		public void Redraw(Graphics target)
		{
			target.DrawImageUnscaled(bitmap, 0, 0);
		}
		
		private Point GetVectorCoordinates(Vector2D vector)
		{
			var scale = 0.2 * Math.Min(bitmap.Width, bitmap.Height);
			return new Point((int)(.5 * bitmap.Width + scale * vector.X),
			                 (int)(.5 * bitmap.Height + scale * vector.Y));
		}
		
		public Vector2D GetPixelVector(double x, double y)
		{
			var scale = 0.2 * Math.Min(bitmap.Width, bitmap.Height);
			return new Vector2D((x - .5 * bitmap.Width) / scale,
			                    (y - .5 * bitmap.Height) / scale);
		}
	}
}
