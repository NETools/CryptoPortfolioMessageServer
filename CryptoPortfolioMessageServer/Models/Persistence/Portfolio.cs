using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoPortfolioMessageServer.Models.Persistence
{
	public class Portfolio
	{
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		[Key]
		public int PortfolioId { get; set; }
		public virtual IList<string> Assets { get; set; } = [];
	}
}
