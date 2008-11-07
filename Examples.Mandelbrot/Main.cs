using System;
using System.Drawing;
using Fmacj.Emitter;

namespace Examples.Mandelbrot
{
	class MainClass
	{
		public static void Main(string[] args)
		{
			int size;
			string filename;
			
			try
			{
				size = Convert.ToInt32(args[0]);
				filename = args[1];
			}
			catch
			{
				Console.WriteLine("Parameters: size filename");
				return;
			}
			
			ParallelizationFactory.Parallelize(typeof(Mandelbrot).Assembly);
			
			Mandelbrot mandelbrot = ParallelizationFactory.GetParallelized<Mandelbrot>();
			
			Bitmap bitmap = mandelbrot.Calculate(size);
				
			Console.WriteLine("Compressing PNG...");
			
			bitmap.Save(filename);
			
			Console.WriteLine("Ouput written to {0}", filename);
		}
	}
}