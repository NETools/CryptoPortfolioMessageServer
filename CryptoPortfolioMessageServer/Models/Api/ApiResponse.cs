using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace CryptoPortfolioMessageServer.Models.Api
{
	public struct ApiResponse<Enum> where Enum : System.Enum
	{
		[JsonInclude]
		public byte[] Data;
		[JsonInclude]
		public Enum? ResponseCode;
		[JsonInclude]
		public string? Message;
	}
}
