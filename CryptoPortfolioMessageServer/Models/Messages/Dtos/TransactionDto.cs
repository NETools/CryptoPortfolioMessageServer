using CryptoPortfolioMessageServer.Models.Persistence;
using CryptoPortfolioMessageServer.Shared.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace CryptoPortfolioMessageServer.Models.Messages.Dtos
{
	public struct TransactionDto
	{
		[JsonInclude]
		public string Username;
		[JsonInclude]
		public Guid TransactionId;
		[JsonInclude]
		public string CoinId;
		[JsonInclude]
		public TransactionSide Side;
		[JsonInclude]
		public DateTime Date;
		[JsonInclude]
		public double PricePerCoin;
		[JsonInclude]
		public double AmountEur;
		[JsonInclude]
		public double QuantityCoins;

		[JsonInclude]
		public TransactionType Action;

		public void CopyTo(Transaction transaction)
		{
			transaction.CoinId = CoinId;
			transaction.TransactionGuid = TransactionId;
			transaction.TransactionSide = Side;
			transaction.Date = Date;
			transaction.PricePerCoin = PricePerCoin;
			transaction.AmountEur = AmountEur;
			transaction.QuantityCoins = QuantityCoins;
		}
	}
}
