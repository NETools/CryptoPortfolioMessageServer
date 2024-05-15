using CryptoPortfolioMessageServer.Models.Api;
using CryptoPortfolioMessageServer.Models.Messages;
using CryptoPortfolioMessageServer.Models.Messages.Dtos;
using CryptoPortfolioMessageServer.Models.Messages.Security;
using CryptoPortfolioMessageServer.Models.Messages.Session;
using CryptoPortfolioMessageServer.Models.Persistence;
using CryptoPortfolioMessageServer.Persistence;
using CryptoPortfolioMessageServer.Services;
using CryptoPortfolioMessageServer.Shared;
using CryptoPortfolioMessageServer.Shared.Data;
using CryptoPortfolioMessageServer.Shared.Info;
using NSQM.Core.Consumer;
using NSQM.Core.Model;
using NSQM.Data.Extensions;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Text.Json;

namespace CryptoPortfolioMessageServer.Receiver
{
    public class MessageReceiver
	{
		private static CryptoPortfolioDbContext DbContext = new();

		private NSQMConsumer _consumer;
		private string _privateKey;
		private Dictionary<Guid, HandshakeMessage> _handshakes = [];

		private EmailSender _emailSender;

		public MessageReceiver(string host, string privateKey)
		{
			_consumer = new NSQMConsumer(host, Guid.NewGuid());
			_privateKey = privateKey;

			_emailSender = new EmailSender("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", new SendGrid.Helpers.Mail.EmailAddress("enes.hergul215@gmail.com", "BigDaddy"));

			Logger.Default().WriteLine($"Set host to {host}", LoggerState.Okay);
		}

		public async Task Start()
		{
			while (true)
			{
				using var cancelSrc = new CancellationTokenSource(5000);
				try
				{
					Logger.Default().WriteLine($"Attempting to connect to host", LoggerState.Normal);
					await _consumer.Connect(cancelSrc.Token);
					_consumer.MessageReceived += OnMessageReceived;
					Logger.Default().WriteLine($"Connected", LoggerState.Okay);

					break;
				}
				catch (Exception ex)
				{
					Logger.Default().WriteLine($"Connection failed (timeout)!", LoggerState.Error);
				}
			}

			var channel = await _consumer.OpenChannel("PortfolioExchangeService");
			if (channel.Status == System.Net.HttpStatusCode.OK)
			{
				Logger.Default().WriteLine($"Channel created {channel.Model.ChannelId} created.", LoggerState.Okay);
			}
			else
			{
				Logger.Default().WriteLine($"Failed to create channel: {channel.Description}.", LoggerState.Error);
			}

			var user = await _consumer.Subscribe("PortfolioExchangeService");
			Logger.Default().WriteLine($"Subscribed to message exchange server.", LoggerState.Okay);
		}

		private async void OnMessageReceived(ReceivedMessage message, ResultConnection resultConnection)
		{
			Logger.Default().WriteLine($"Message received: {message.Name}", LoggerState.Warning);

			var genericMessage = message.Content.ToStruct<GenericMessage>(Encoding.UTF8);
			var response = await HandleMessage(message.FromId, genericMessage);

			var signedMessage = SignMessage(genericMessage.MessageId, genericMessage.MessageType, response.ToJsonBytes(Encoding.UTF8), _privateKey);
			await resultConnection.Done(signedMessage.ToJsonBytes(Encoding.UTF8), NSQM.Data.Extensions.TaskStatus.TaskDone);
		}

		private async Task<ApiResponse<GenericMessageType>> HandleMessage(Guid id, GenericMessage genericMessage)
		{
			if (genericMessage.MessageType != GenericMessageType.EncryptionHandshake && !_handshakes.ContainsKey(id))
				return new ApiResponse<GenericMessageType>()
				{
					Data = [],
					Message = "Not handled",
					ResponseCode = GenericMessageType.Failed
				};

			switch (genericMessage.MessageType)
			{
				case GenericMessageType.EncryptionHandshake:
					return await HandleHandshakeMessage(
						genericMessage
						.Payload
						.DecryptRsa(_privateKey)
						.ToStruct<HandshakeMessage>(Encoding.UTF8));

				case GenericMessageType.RetrievePortfolio:
					return await HandleRetrievePortfolio(id, genericMessage.ConvertData<RetrievePortfolioMessage>(_handshakes[id].SharedKey, _handshakes[id].SharedIV));
				case GenericMessageType.UpdatePortfolioMessage:
					return await HandleUpdatePortfolio(genericMessage.ConvertData<UpdatePortfolioMessage>(_handshakes[id].SharedKey, _handshakes[id].SharedIV));
				case GenericMessageType.ActivationMessage:
					return await HandleAcivationMessage(genericMessage.ConvertData<ActivationMessage>(_handshakes[id].SharedKey, _handshakes[id].SharedIV));
				case GenericMessageType.SignUpMessage:
					return await HandleSignUpMessage(genericMessage.ConvertData<CredentialsMessage>(_handshakes[id].SharedKey, _handshakes[id].SharedIV));
				case GenericMessageType.SignInMessage:
					return await HandleSignInMessage(genericMessage.ConvertData<CredentialsMessage>(_handshakes[id].SharedKey, _handshakes[id].SharedIV));
				case GenericMessageType.UpdateTransactionMessage:
					await HandleUpdateTransactionMessage(genericMessage.ConvertData<TransactionDto>(_handshakes[id].SharedKey, _handshakes[id].SharedIV));
					break;
			}

			return new ApiResponse<GenericMessageType>()
			{
				Data = null,
				Message = "Not handled",
				ResponseCode = GenericMessageType.Failed
			};
		}

		private async Task<ApiResponse<GenericMessageType>> HandleRetrievePortfolio(Guid id, RetrievePortfolioMessage message)
		{
			var result = await DbContext.FindUser(message.Username);

			ResponseMessage response = new ResponseMessage();

			if (result == null)
			{
				response.Message = "User was not found.";
				response.Status = HttpStatusCode.NotFound;
			}
			else
			{
				response.Message = "Loaded portfolio successfully.";
				response.Status = HttpStatusCode.OK;
				response.Data = new MessageBusRetrievalMessage()
				{
					RetrievalType = RetrievalType.Portfolio,
					StructBuffer = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(result.Portfolio.Assets))
				}.ToJsonBytes(Encoding.UTF8);
			}

			return new ApiResponse<GenericMessageType>()
			{
				ResponseCode = GenericMessageType.SignInMessage,
				Data = response.ToJsonBytes(Encoding.UTF8).AesEncrypt(_handshakes[id].SharedKey, _handshakes[id].SharedIV),
				Message = response.Message
			};
		}

		private async Task<ApiResponse<GenericMessageType>> HandleHandshakeMessage(HandshakeMessage message)
		{
			_handshakes[message.ClientId] = message;
			return new ApiResponse<GenericMessageType>()
			{
				Data = null,
				ResponseCode = GenericMessageType.EncryptionHandshake,
				Message = "Handshake has been finalized successfully."
			};
		}

		private async Task<ApiResponse<GenericMessageType>> HandleUpdatePortfolio(UpdatePortfolioMessage message)
		{
			var result = await DbContext.UpdatePortfolio(message.Username, message.CoinId, message.Action);
			ResponseMessage response = new ResponseMessage();

			if (result.ResponseCode == PersistenceResponse.AssetsUpdated)
			{
				response.Message = "Asset added to portfolio.";
				response.Status = HttpStatusCode.OK;
			}
			else
			{
				response.Message = $"Asset not added: {result.Message}.";
				response.Status = HttpStatusCode.NotModified;
			}

			return new ApiResponse<GenericMessageType>()
			{
				ResponseCode = GenericMessageType.UpdatePortfolioMessage,
				Data = response.ToJsonBytes(Encoding.UTF8),
				Message = response.Message
			};
		}

		private async Task<ApiResponse<GenericMessageType>> HandleSignUpMessage(CredentialsMessage message)
		{
			var result = await DbContext.CreateUser(message.Username, message.Password);
			ResponseMessage response = new ResponseMessage()
			{
				Message = result.Message
			};

			if (result.ResponseCode == PersistenceResponse.UserCreated)
			{
				var activationId = result.Data.JsonBytesToClass<User>(Encoding.UTF8)!.ActivationId;

				await _emailSender.SendEmail("Aktivieren Sie Ihr Crypto-Portfolio-Account",
					$"Hallo <b>{message.Username}</b>,<br>" +
					$"danke das Sie sich für uns entschieden haben.<br>" +
					$"Ihr Aktivierungscode lautet: <b>{activationId}</b><br>" +
					$"<br>" +
					$"Mit freundlichen Grüßen,<br>" +
					$"Ihr Crypto-Portfolio-Team", message.Username);

				response.Status = HttpStatusCode.Created;
			}
			else
			{
				response.Status = HttpStatusCode.NotModified;
			}
			return new ApiResponse<GenericMessageType>()
			{
				ResponseCode = GenericMessageType.SignUpMessage,
				Data = response.ToJsonBytes(Encoding.UTF8),
				Message = response.Status == HttpStatusCode.OK ? "User successfully signed up." : "User could not be signed up."
			};

		}
		private async Task<ApiResponse<GenericMessageType>> HandleSignInMessage(CredentialsMessage message)
		{
			var result = await DbContext.FindUser(message.Username);
			
			ResponseMessage response = new ResponseMessage();

			if (result == null)
			{
				response.Message = "User was not found.";
				response.Status = HttpStatusCode.NotFound;
			}
			else if (result.Password != message.Password)
			{
				response.Message = "Username or password were wrong";
				response.Status = HttpStatusCode.Unauthorized;
			}
			else if (!result.IsActivated)
			{
				response.Message = "User has not been activated yet.";
				response.Status = HttpStatusCode.NotAcceptable;
			}
			else
			{
				response.Message = "User has been signed in.";
				response.Status = HttpStatusCode.OK;
			}


			return new ApiResponse<GenericMessageType>()
			{
				ResponseCode = GenericMessageType.SignInMessage,
				Data = response.ToJsonBytes(Encoding.UTF8),
				Message = response.Status == HttpStatusCode.OK ? "User has been successfully signed in." : "User could not be signed in."
			};
		}

		private async Task<ApiResponse<GenericMessageType>> HandleAcivationMessage(ActivationMessage message)
		{
			var result = await DbContext.ActivateUser(message.Username, message.ActivationId);
			var response = new ResponseMessage();

			if (result.ResponseCode == PersistenceResponse.UserNotFound)
			{
				response.Message = "User was not found.";
				response.Status = HttpStatusCode.NotFound;
			}
			else if (result.ResponseCode == PersistenceResponse.AlreadyActivated)
			{
				response.Message = "User has already been activated.";
				response.Status = HttpStatusCode.Forbidden;
			}
			else if (result.ResponseCode == PersistenceResponse.ActivationIdWrong)
			{
				response.Message = "Activation id is wrong.";
				response.Status = HttpStatusCode.Forbidden;
			}
			else if (result.ResponseCode == PersistenceResponse.Activated)
			{
				response.Message = "User has been activated!";
				response.Status = HttpStatusCode.OK;
			}

			return new ApiResponse<GenericMessageType>()
			{
				ResponseCode = GenericMessageType.ActivationMessage,
				Data = response.ToJsonBytes(Encoding.UTF8),
				Message = response.Status == HttpStatusCode.OK ? "User has been activated." : "Activation id was wrong"
			};

		}
		private async Task<ApiResponse<GenericMessageType>> HandleUpdateTransactionMessage(TransactionDto message)
		{
			var transaction = new Transaction();
			message.CopyTo(transaction);

			var result = await DbContext.UpdateTransaction(message.Username, transaction, message.Action);
			ResponseMessage response = new ResponseMessage();

			if (result.ResponseCode == PersistenceResponse.AssetsUpdated)
			{
				response.Message = "Asset added to portfolio.";
				response.Status = HttpStatusCode.OK;
			}
			else
			{
				response.Message = $"Asset not added: {result.Message}.";
				response.Status = HttpStatusCode.NotModified;
			}

			return new ApiResponse<GenericMessageType>()
			{
				ResponseCode = GenericMessageType.UpdatePortfolioMessage,
				Data = response.ToJsonBytes(Encoding.UTF8),
				Message = response.Message
			};
		}

		private static SignedMessage<T> SignMessage<T>(Guid id, T messageType, byte[] data, string privateKey) where T : Enum
		{
			return new SignedMessage<T>()
			{
				MessageId = id,
				MessageType = messageType,
				Data = data,
				Signature = data.SignData(privateKey)
			};
		}
	}
}
