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
using System.Runtime.Serialization.Formatters.Binary;
using RabbitMQ.Client;
using RabbitMQ.Client.MessagePatterns;

namespace Fmacj.Components.TaskServer
{		
	public class TaskServer : SimpleRpcServer
	{	
		private BinaryFormatter formatter = new BinaryFormatter();
		
		public TaskServer(Subscription subscription) : base(subscription)
		{			
		}	

		public override byte[] HandleSimpleCall(
			bool isRedelivered, 
		    IBasicProperties requestProperties,
		    byte[] body,
		    out IBasicProperties replyProperties)
		{
			replyProperties = this.m_subscription.Model.CreateBasicProperties();
			
			var requestStream = new MemoryStream(body);
			var request = formatter.Deserialize(requestStream);
		
			throw new NotImplementedException();
		}

	}
}