using CryptoPortfolioMessageServer.Models.Api;
using CryptoPortfolioMessageServer.Models.Messages.Dtos;
using CryptoPortfolioMessageServer.Models.Persistence;
using CryptoPortfolioMessageServer.Shared;
using CryptoPortfolioMessageServer.Shared.Comparators;
using CryptoPortfolioMessageServer.Shared.Data;
using CryptoPortfolioMessageServer.Shared.Info;
using CryptoPortfolioMessageServer.Shared.Synchronization;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoPortfolioMessageServer.Persistence
{
	public class CryptoPortfolioDbContext : DbContext
	{
		private const string DbPath = @"C:\Users\enesh\DbContext\CryptoPortfolioBackend\database.db";

		private NSMutex _mutex = new NSMutex();

		public DbSet<User> Users { get; set; }
		public DbSet<Portfolio> Portfolios { get; set; }
		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			optionsBuilder.UseSqlite($"Data Source={DbPath}");
		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder
				.Entity<User>()
				.Navigation(e => e.Portfolio)
				.UsePropertyAccessMode(PropertyAccessMode.Property)
				.AutoInclude();

			modelBuilder
				.Entity<User>()
				.Navigation(e => e.Transactions)
				.UsePropertyAccessMode(PropertyAccessMode.Property)
				.AutoInclude();

			modelBuilder
				.Entity<Portfolio>()
				.Property(e => e.Assets)
				.HasConversion(
				   v => string.Join(',', v),
				   v => v.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList())
				.Metadata.SetValueComparer(new ListStringComparer());

		}

		public async Task<User?> FindUser(string username)
		{
			using (_mutex.GetLock())
			{
				return await Users
					.Where(p => p.Username == username)
					.FirstOrDefaultAsync();
			}
		}



		public async Task<ApiResponse<PersistenceResponse>> UpdateTransaction(
			string username,
			Transaction transaction,
			TransactionType action)
		{
			using (_mutex.GetLock())
			{
				var user = await FindUser(username);
				if(user == null)
				{
					return new ApiResponse<PersistenceResponse>()
					{
						Data = null,
						ResponseCode = PersistenceResponse.UserNotFound,
						Message = "User could not be found."
					};
				}

				if (action == TransactionType.Add)
				{
					user.Transactions.Add(transaction);
				}
				else if (action == TransactionType.Remove)
				{
					user.Transactions.Remove(transaction);
				}


				if (await Save())
				{
					return new ApiResponse<PersistenceResponse>()
					{
						Data = user.ClassToJsonBytes(Encoding.UTF8),
						ResponseCode = PersistenceResponse.AssetsUpdated,
						Message = "Transaction added to user."
					};
				}
				else
				{
					return new ApiResponse<PersistenceResponse>()
					{
						ResponseCode = PersistenceResponse.FatalError,
						//Data = null,antonia enes
						Data = null,
						Message = $"Fatal error -- see exception on console."
					};
				}
			}
		}


		public async Task<ApiResponse<PersistenceResponse>> UpdatePortfolio(string username, string coinId, TransactionType action)
		{
			using (_mutex.GetLock())
			{
				var user = await FindUser(username);
				if (user == null)
				{
					return new ApiResponse<PersistenceResponse>()
					{
						Data = null,
						ResponseCode = PersistenceResponse.UserNotFound,
						Message = "User could not be found.",
					};
				}

				if (action == TransactionType.Add)
					user.Portfolio.Assets.Add(coinId);
				else if (action == TransactionType.Remove)
					user.Portfolio.Assets.Remove(coinId);

				if (await Save())
				{
					return new ApiResponse<PersistenceResponse>()
					{
						Data = user.ClassToJsonBytes(Encoding.UTF8),
						ResponseCode = PersistenceResponse.AssetsUpdated,
						Message = "Coin added to portfolio."
					};
				}
				else
				{
					return new ApiResponse<PersistenceResponse>()
					{
						ResponseCode = PersistenceResponse.FatalError,
						//Data = null,antonia enes
						Data = null,
						Message = $"Fatal error -- see exception on console."
					};
				}
			}
		}

		public async Task<ApiResponse<PersistenceResponse>> CreateUser(string username, string password)
		{
			using (_mutex.GetLock())
			{
				var user = await FindUser(username);
				if (user != null)
				{
					return new ApiResponse<PersistenceResponse>()
					{
						Data = user.ClassToJsonBytes(Encoding.UTF8),
						ResponseCode = PersistenceResponse.UserExists,
						Message = $"User with username {username} already exists!"
					};
				}

				var portfolio = new Portfolio();
				var portfolioEntity = await Portfolios.AddAsync(portfolio);

				user = new User
				{
					Portfolio = portfolioEntity.Entity,
					Username = username,
					Password = password
				};

				var entity = await Users.AddAsync(user);
				if (await Save())
				{
					return new ApiResponse<PersistenceResponse>()
					{
						Data = entity.Entity.ClassToJsonBytes(Encoding.UTF8),
						ResponseCode = PersistenceResponse.UserCreated,
						Message = $"User with username {username} created (id = {entity.Entity.Id})!"
					};
				}
				else
				{
					return new ApiResponse<PersistenceResponse>()
					{
						ResponseCode = PersistenceResponse.FatalError,
						//Data = null,antonia enes
						Data = null,
						Message = $"Fatal error -- see exception on console."
					};
				}
			}
		}

		public async Task<ApiResponse<PersistenceResponse>> ActivateUser(string username, Guid activationId)
		{
			var user = await FindUser(username);

			if(user == null)
			{
				return new ApiResponse<PersistenceResponse>()
				{
					Data = null,
					ResponseCode = PersistenceResponse.UserNotFound,
					Message = "User could not be found.",
				};
			}
			else if (user.IsActivated)
			{
				return new ApiResponse<PersistenceResponse>()
				{
					Data = null,
					ResponseCode = PersistenceResponse.AlreadyActivated,
					Message = "User has already been activated.",
				};
			}
			else if (user.ActivationId != activationId)
			{
				return new ApiResponse<PersistenceResponse>()
				{
					Data = null,
					ResponseCode = PersistenceResponse.ActivationIdWrong,
					Message = "The activation id provided was wrong."
				};
			}

			user.IsActivated = true;


			if (await Save())
			{
				return new ApiResponse<PersistenceResponse>()
				{
					Data = user.ClassToJsonBytes(Encoding.UTF8),
					ResponseCode = PersistenceResponse.Activated,
					Message = "User has been activated."
				};
			}
			else
			{
				return new ApiResponse<PersistenceResponse>()
				{
					ResponseCode = PersistenceResponse.FatalError,
					//Data = null,antonia enes
					Data = null,
					Message = $"Fatal error -- see exception on console."
				};
			}
		}

		private async Task<bool> Save()
		{
			using (_mutex.GetLock())
			{
				try
				{
					var count = await SaveChangesAsync(true);

					Logger.Default().WriteLine($"Saved changes - {count} entries modified.", LoggerState.Okay);

					return true;
				}
				catch (Exception ex)
				{
					Logger.Default().WriteLine(ex.Message, LoggerState.Error);
					Logger.Default().WriteLine(ex.StackTrace,  LoggerState.Error);
					Logger.Default().WriteLine(ex.InnerException?.Message, LoggerState.Error);
					Logger.Default().WriteLine(ex.InnerException?.StackTrace, LoggerState.Error);

					return false;
				}
			}
		}

	}
}
