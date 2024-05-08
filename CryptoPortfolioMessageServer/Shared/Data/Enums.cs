using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoPortfolioMessageServer.Shared.Data
{
    public enum PersistenceResponse
    {
        UserCreated,
        UserNotFound,
        UserExists,
        AssetsUpdated,
        TransactionAdded,
        TransactionRemoved,
        ActivationIdWrong,
        AlreadyActivated,
        Activated,
        FatalError
    }

    public enum LoggerState
    {
        Normal,
        Okay,
        Error,
        Warning
    }

    public enum GenericMessageType
    {
        Failed,
        EncryptionHandshake,
        ActivationMessage,
        SignInMessage,
        SignUpMessage,
        UpdatePortfolioMessage,
        UpdateTransactionMessage,
        RetrievePortfolio
    }

	public enum TransactionType
	{
		Add,
		Remove
	}
}
