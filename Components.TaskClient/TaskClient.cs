/*
    FMACJ Parallelization Framework for .NET
    Copyright (C) 2008  Stefan Noack

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/

using System;
using System.IO;
using System.Reflection;
using Fmacj.Core.Framework;
using Fmacj.Core.Runtime.Network.Communication;

namespace Fmacj.Components.TaskClient
{	
	public class TaskClient : Client
	{
		public TaskClient(Stream serverStream) : base(serverStream)
	    {			
		}

		public void RunTask(Action entryPoint)
		{
			RunTask(entryPoint.Method);
		}
		
		public void RunTask(Assembly assembly)
		{
			RunTask(assembly.EntryPoint);
		}

		public void RunTask(MethodInfo entryPoint)
		{	
			Assembly assembly = entryPoint.DeclaringType.Assembly;
			
			if(assembly.Location == "")
				throw new ArgumentException("The given assembly could not be located.", "assembly");
			
			byte[] rawAssembly = File.ReadAllBytes(assembly.Location);
			
		    RunTask(rawAssembly, entryPoint);
		}
		
		public void RunTask(byte[] rawAssembly, MethodInfo entryPoint)
		{
			RunTaskRequest request = new RunTaskRequest(rawAssembly, entryPoint);
			Response response = SendRequest(request);			
		}
	}
}
