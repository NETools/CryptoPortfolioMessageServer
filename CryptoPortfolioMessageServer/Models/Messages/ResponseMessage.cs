using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace CryptoPortfolioMessageServer.Models.Messages
{
	public struct ResponseMessage
	{
		[JsonInclude]
		public HttpStatusCode Status;
		[JsonInclude]
		public string Message;
		[JsonInclude]
		public byte[] Data;
	}
}
