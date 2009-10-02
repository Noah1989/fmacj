using System;
using System.Drawing;
using System.Timers;
using Gtk;
using ChaoticPendulum.Core;	
using ChaoticPendulum.Visualization;

public partial class MainWindow: Gtk.Window
{	
	private Pendulum pendulum;
	private Timer timer;
		
	private TrajectoryGraph graph;
		
	public MainWindow(): base (Gtk.WindowType.Toplevel)
	{
		Build();
		
		InitializePendulum();
		
		int width, height;
		drawingArea.GetSizeRequest(out width, out height);
		InitializeGraph(width, height);
	 	DrawAttractors();
		
		timer = new Timer(40);
		timer.Elapsed += OnTimerElapsed;
	}
	
	private void InitializePendulum()
	{
		pendulum = new Pendulum((int)attractors.Value);
		SetParameters();	
	}
	
	private void SetParameters()
	{
		pendulum.Particle.Mass = mass.Value;
		pendulum.Particle.Charge = charge.Value;
		
		pendulum.Attractors.Z = altitude1.Value;
		pendulum.Attractors.Charge = -1;
		
		pendulum.Friction.Coefficient = drag.Value;		
	}
	
	private void InitializeGraph(int width, int height)
	{		
		graph = new TrajectoryGraph(pendulum, width, height);	
	}

	private void DrawAttractors()
	{
		using (var graphics = Gtk.DotNet.Graphics.FromDrawable(drawingArea.GdkWindow))
		{
			graph.DrawAttractors(graphics);
		}
	}
	
	private void DrawPaticle()
	{
		using (var graphics = Gtk.DotNet.Graphics.FromDrawable(drawingArea.GdkWindow))
		{
			graph.DrawParticle(graphics);
		}
	}
	
	private void Redraw()
	{
		using (var graphics = Gtk.DotNet.Graphics.FromDrawable(drawingArea.GdkWindow))
		{
			graph.Redraw(graphics);
		}
	}
	
	private void Clear()
	{
		graph.Clear();			
		Redraw();
		DrawAttractors();
	}	
	
	protected void OnDeleteEvent(object sender, DeleteEventArgs a)
	{		
		Application.Quit();
		a.RetVal = true;
	}
	
	protected virtual void OnDrawingAreaConfigureEvent (object o, Gtk.ConfigureEventArgs args)
	{
		graph.Resize(args.Event.Width, args.Event.Height);
		Redraw();
		DrawAttractors();
	}
	
	protected virtual void OnDrawingAreaExposeEvent (object o, Gtk.ExposeEventArgs args)
	{	
		if (graph == null) return;
		
	 	Redraw();
	}
	
	protected virtual void OnAttractorsValueChanged (object sender, System.EventArgs e)
	{
		timer.Stop();
		InitializePendulum();
		graph.SetPendulum(pendulum);
		Redraw();
		DrawAttractors();
	}
	
	protected virtual void OnParameterValueChanged (object sender, System.EventArgs e)
	{
		SetParameters();
	}
	
	protected virtual void OnEventBoxButtonReleaseEvent (object o, Gtk.ButtonReleaseEventArgs args)
	{
		if(args.Event.Button == 3)
		{
			Clear();
		}
		
		pendulum.Particle.Position = graph.GetPixelVector(args.Event.X, args.Event.Y);
		pendulum.Particle.Velocity = Vector2D.ZeroVector;
		DrawPaticle();
		timer.Start();
	}

	void OnTimerElapsed(object sender, ElapsedEventArgs e)
	{		
		pendulum.Iterate(step.Value / 10, 10);
		Application.Invoke(delegate { DrawPaticle(); });
	}

	protected virtual void OnClearClicked (object sender, System.EventArgs e)
	{
		Clear();
	}

	protected virtual void OnStopClicked (object sender, System.EventArgs e)
	{
		timer.Stop();
	}

	
}