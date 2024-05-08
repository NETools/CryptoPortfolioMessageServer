using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoPortfolioMessageServer.Models.Persistence
{
	public class User
	{
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		[Key]
		public int Id { get; set; }
		public string Username { get; set; }
		public string Password { get; set; }
		
		public Portfolio Portfolio { get; set; }

		public bool IsActivated { get; set; }
		public Guid ActivationId { get; set; } = Guid.NewGuid();
	}
}
