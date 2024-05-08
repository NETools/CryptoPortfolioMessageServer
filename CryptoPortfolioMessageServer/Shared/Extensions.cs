using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace CryptoPortfolioMessageServer.Shared
{
	internal static class Extensions
	{

		public static T? JsonBytesToClass<T>(this byte[] data, Encoding encoding) where T : class
		{
			var json = encoding.GetString(data);
			return JsonSerializer.Deserialize<T>(json);
		}

		public static byte[] ClassToJsonBytes<T>(this T message, Encoding encoding) where T : class
		{
			var json = JsonSerializer.Serialize(message);
			return encoding.GetBytes(json);
		}

		public static byte[] EncryptRsa(this byte[] data, string publicKey)
		{
			using (RSACryptoServiceProvider rsa = new())
			{
				rsa.FromXmlString(publicKey);
				return rsa.Encrypt(data, false);
			}
		}

		public static byte[] DecryptRsa(this byte[] data, string privateKey)
		{
			using (RSACryptoServiceProvider rsa = new())
			{
				rsa.FromXmlString(privateKey);
				return rsa.Decrypt(data, false);
			}
		}

		public static byte[] AesEncrypt(this byte[] data, byte[] key, byte[] iv)
		{
			using (Aes aes = Aes.Create())
			{
				aes.Key = key;
				aes.IV = iv;

				// Create an encryptor to perform the stream transform
				ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

				// Create the streams used for encryption
				using (MemoryStream msEncrypt = new MemoryStream())
				{
					using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
					{
						// Write all data to the crypto stream and flush it
						csEncrypt.Write(data, 0, data.Length);
						csEncrypt.FlushFinalBlock();
						return msEncrypt.ToArray();
					}
				}
			}
		}

		public static byte[] AesDecrypt(this byte[] data, byte[] key, byte[] iv)
		{
			using (Aes aes = Aes.Create())
			{
				aes.Key = key;
				aes.IV = iv;

				// Create a decryptor to perform the stream transform
				ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

				// Create the streams used for decryption
				using (MemoryStream msDecrypt = new MemoryStream(data))
				{
					using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
					{
						// Read the decrypted bytes from the decrypting stream
						using (MemoryStream msDecrypted = new MemoryStream())
						{
							csDecrypt.CopyTo(msDecrypted);
							return msDecrypted.ToArray();
						}
					}
				}
			}
		}

		public static byte[] SignData(this byte[] data, string privateKey)
		{
			using (RSACryptoServiceProvider rsa = new())
			{
				rsa.FromXmlString(privateKey);
				return rsa.SignData(data, SHA256.Create());
			}
		}

		public static bool VerifySignature(this byte[] data, byte[] signature, string publicKey)
		{
			using (RSACryptoServiceProvider rsa = new())
			{
				rsa.FromXmlString(publicKey);
				return rsa.VerifyData(data, SHA256.Create(), signature);
			}
		}


	}
}
