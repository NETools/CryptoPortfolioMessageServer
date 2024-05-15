using CryptoPortfolioMessageServer.Shared.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoPortfolioMessageServer.Models.Persistence
{
	public class Transaction
	{
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		[Key]
		public int TransactionId { get; set; }	
		public Guid TransactionGuid { get; set; }
		public string CoinId { get; set; }
		public TransactionSide TransactionSide { get; set; }
		public DateTime Date { get; set; }
		public double PricePerCoin { get; set; }
		public double AmountEur { get; set; }
		public double QuantityCoins { get; set; }

		public override bool Equals(object? obj)
		{
			return obj?.GetHashCode() == this.GetHashCode();
		}

		public override int GetHashCode()
		{
			return TransactionId.GetHashCode();
		}
	}
}
