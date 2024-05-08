using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace CryptoPortfolioMessageServer.Models.Messages.Security
{
	public struct HandshakeMessage
	{
		[JsonInclude]
		public Guid ClientId;

		[JsonInclude]
		public byte[] SharedKey;

		[JsonInclude]
		public byte[] SharedIV;
	}
}
