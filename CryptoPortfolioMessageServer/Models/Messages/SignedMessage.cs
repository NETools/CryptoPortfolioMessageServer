using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace CryptoPortfolioMessageServer.Models.Messages
{
	public struct SignedMessage<T> where T : Enum
	{
		[JsonInclude]
		public Guid MessageId;
		[JsonInclude]
		public T MessageType;
		[JsonInclude]
		public byte[] Data;
		[JsonInclude]
		public byte[] Signature;
	}
}
