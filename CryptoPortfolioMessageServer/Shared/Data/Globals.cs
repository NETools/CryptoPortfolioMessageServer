using CryptoPortfolioMessageServer.Models.Messages;
using CryptoPortfolioMessageServer.Models.Messages.Security;
using CryptoPortfolioMessageServer.Models.Messages.Session;
using NSQM.Data.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoPortfolioMessageServer.Shared.Data
{
	internal static class Globals
	{
		private static Dictionary<GenericMessageType, Func<byte[], byte[], byte[], object>> DataConverters = new()
		{
			{ GenericMessageType.ActivationMessage, (data, key, iv)=>data.AesDecrypt(key, iv).ToStruct<ActivationMessage>(Encoding.UTF8) },
			{ GenericMessageType.UpdatePortfolioMessage, (data, key, iv)=>data.AesDecrypt(key, iv).ToStruct<UpdatePortfolioMessage>(Encoding.UTF8) },
			{ GenericMessageType.UpdateTransactionMessage, (data, key, iv)=>data.AesDecrypt(key, iv).ToStruct<UpdateTransactionsMessage>(Encoding.UTF8) },
			{ GenericMessageType.SignUpMessage, (data, key, iv)=>data.AesDecrypt(key, iv).ToStruct<CredentialsMessage>(Encoding.UTF8) },
			{ GenericMessageType.SignInMessage, (data, key, iv)=>data.AesDecrypt(key, iv).ToStruct<CredentialsMessage>(Encoding.UTF8) },
			{ GenericMessageType.RetrievePortfolio,  (data, key, iv) => data.AesDecrypt(key, iv).ToStruct<RetrievePortfolioMessage>(Encoding.UTF8) }
		};

		public static T ConvertData<T>(this GenericMessage genericMessage, byte[] key, byte[] iv)
		{
			return (T)DataConverters[genericMessage.MessageType](genericMessage.Payload, key, iv);
		}
	}
}
