using System;
using System.Collections.Generic;
using Fmacj.Core.Framework;

namespace ChaoticPendulum.Core
{	
	[Parallelizable]
	public abstract class SimulationMatrix : IParallelizable
	{
		public class Row
		{
			public int Y { get;set; }
			public double realY { get; set; }			
			public int[] Values { get; set; }
		}
		
		public int Width { get; private set; } 
		public int Height { get; private set; }
		private readonly double _xMin, _xMax, _yMin, _yMax, _timeStep;
		private readonly Func<Pendulum> _pendulumFactory;
		
		public SimulationMatrix(int width, int height, 
		                        double xMin, double xMax, 
		                        double yMin, double yMax,
		                        double timeStep,
		                        Func<Pendulum> pendulumFactory)
		{
			Width = width;
			Height = height;
			_xMin = xMin;
			_xMax = xMax;
			_yMin = yMin;
			_yMax = yMax;
			_timeStep = timeStep;
			_pendulumFactory = pendulumFactory;
		}
		
		[Fork]
		public abstract void StartSimulation();
		[Asynchronous]
		protected void StartSimulation([Channel("initRow", Enumerable = true)] out IEnumerable<Row> initRows)
		{
			initRows = InitRows();
		}
		
		private IEnumerable<Row> InitRows()
		{
			for (int y = 0; y < Height; y++)
				yield return new Row { Y = y,
									   realY = _yMin + (_yMax - _yMin )*y/Height,
									   Values = new int[Width] };
		}
		
		[Chord]
		protected void SimulateRow([Channel("initRow")] Row initRow, 
		                           [Channel("calculatedRow")] Row row)			
		{			
			row = initRow;
			var pendulum = _pendulumFactory.Invoke();
			for (int x = 0; x < _width; x++)
			{
				double realX = _xMin + (_xMax - _xMin)*x/Width;
				
				pendulum.SetTo(realX, row.realY);
				pendulum.Iterate(_timeStep, 1024);
				
				row.Values[x] = pendulum.GetNearestAttracorIndex();
			}
		}
		
		[Join, Channel("calculatedRow", Enumerable = true)]
		public abstract IEnumerable<Row> GetRows();				
	}
}