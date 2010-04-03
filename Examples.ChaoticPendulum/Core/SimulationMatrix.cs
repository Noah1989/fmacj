using System;
using System.Collections.Generic;
using Fmacj.Core.Framework;

namespace ChaoticPendulum.Core
{	
	public abstract class SimulationMatrix
	{
		protected class Row
		{
			public int Y { get;set; }
			public double realY { get; set; }			
			public int[] Values { get; set; }
		}
		
		private readonly int _width, _height;
		private readonly double _xMin, _xMax, _yMin, _yMax, _timeStep;
		private readonly Func<Pendulum> _pendulumFactory;
		
		public SimulationMatrix(int width, int height, 
		                        double xMin, double xMax, 
		                        double yMin, double yMax,
		                        double timeStep,
		                        Func<Pendulum> pendulumFactory)
		{
			_width = width;
			_height = height;
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
			for (int y = 0; y < _height; y++)
				yield return new Row { Y = y,
									   realY = _yMin + (_yMax - _yMin )*y/_height,
									   Values = new int[_width] };
		}
		
		[Chord]
		protected void SimulateRow([Channel("initRow")] Row initRow, 
		                           [Channel("calculatedRow")] Row row)			
		{			
			row = initRow;
			var pendulum = _pendulumFactory.Invoke();
			for (int x = 0; x < _width; x++)
			{
				double realX = _xMin + (_xMax - _xMin)*x/_width;
				pendulum.SetTo(realX, row.Y);
				
				pendulum.Iterate(_timeStep, 1024);
				
				row.Values[x] = pendulum.GetNearestAttracorIndex();
			}
		}
		
		[Chord]
		protected int[,] GetResult([Channel("calculatedRow", Enumerable = true)] IChannelEnumerable<Row> rows) 
		{
			var result = new int[_width, _height];
			
			foreach (var row in rows)
				for (int x = 0; x < _width; x++)
					result[x, row.Y] = row.Values[x];
			
			return result;
		}
		[Join]
		public abstract int[,] GetResult();
				
	}
}