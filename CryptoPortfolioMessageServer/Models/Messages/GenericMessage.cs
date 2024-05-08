using CryptoPortfolioMessageServer.Shared.Data;
using NSQM.Data.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace CryptoPortfolioMessageServer.Models.Messages
{
	public struct GenericMessage
	{
		[JsonInclude]
		public Guid MessageId;
		[JsonInclude]
		public GenericMessageType MessageType;
		[JsonInclude]
		public byte[] Payload;
	}
}
