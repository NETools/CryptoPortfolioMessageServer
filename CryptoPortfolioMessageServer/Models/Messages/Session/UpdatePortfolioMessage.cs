using CryptoPortfolioMessageServer.Shared.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace CryptoPortfolioMessageServer.Models.Messages
{
	internal struct UpdatePortfolioMessage
	{
		[JsonInclude]
		public string Username;
		[JsonInclude]
		public string CoinId;
		[JsonInclude]
		public TransactionType Action;
	}
}
