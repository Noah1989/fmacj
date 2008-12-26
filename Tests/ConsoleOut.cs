// Debug.cs created with MonoDevelop
// User: noah at 01:26 12/26/2008
//
// To change standard headers go to Edit->Preferences->Coding->Standard Headers
//

using System;
using System.Threading;

namespace Fmacj.Tests
{	
	public static class ConsoleOut
	{		
		public static void ShowAvailableThreadPoolThreads()
		{
			int wt, cpt;
			ThreadPool.GetAvailableThreads(out wt, out cpt);
			Console.Write("({0}, {1})", wt, cpt);
		}
	}
}
