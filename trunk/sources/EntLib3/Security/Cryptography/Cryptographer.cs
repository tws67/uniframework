//===============================================================================
// Microsoft patterns & practices Enterprise Library
// Cryptography Application Block
//===============================================================================
// Copyright © Microsoft Corporation.  All rights reserved.
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY
// OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT
// LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND
// FITNESS FOR A PARTICULAR PURPOSE.
//===============================================================================

using System;
using System.Configuration;
using System.Text;
using Microsoft.Practices.EnterpriseLibrary.Common;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration.ObjectBuilder;
using Microsoft.Practices.EnterpriseLibrary.Security.Cryptography.Properties;
using Microsoft.Practices.EnterpriseLibrary.Security.Cryptography.Instrumentation;

namespace Microsoft.Practices.EnterpriseLibrary.Security.Cryptography
{
	/// <summary>
	/// Facade which exposes common cryptography uses.
	/// </summary>
	public static class Cryptographer
	{
		/// <overrides>
		/// Computes the hash value of plain text using the given hash provider instance
		/// </overrides>
		/// <summary>
		/// Computes the hash value of plain text using the given hash provider instance
		/// </summary>
		/// <param name="hashInstance">A hash instance from configuration.</param>
		/// <param name="plaintext">The input for which to compute the hash.</param>
		/// <returns>The computed hash code.</returns>
		public static byte[] CreateHash(string hashInstance, byte[] plaintext)
		{
			if (string.IsNullOrEmpty(hashInstance)) throw new ArgumentException(Resources.ExceptionNullOrEmptyString, "hashInstance");

			try
			{
				HashProviderFactory factory = new HashProviderFactory(ConfigurationSourceFactory.Create());
				IHashProvider hashProvider = factory.Create(hashInstance);

				return hashProvider.CreateHash(plaintext);
			}
			catch (ConfigurationErrorsException configurationException)
			{
				TryLogHashConfigurationError(configurationException, hashInstance);

				throw;
			}
		}

		/// <summary>
		/// Computes the hash value of plain text using the given hash provider instance
		/// </summary>
		/// <param name="hashInstance">A hash instance from configuration.</param>
		/// <param name="plaintext">The input for which to compute the hash.</param>
		/// <returns>The computed hash code.</returns>
		public static string CreateHash(string hashInstance, string plaintext)
		{
			if (string.IsNullOrEmpty(hashInstance)) throw new ArgumentException(Resources.ExceptionNullOrEmptyString, "hashInstance");

			byte[] plainTextBytes = UnicodeEncoding.Unicode.GetBytes(plaintext);
			byte[] resultBytes = CreateHash(hashInstance, plainTextBytes);
			CryptographyUtility.GetRandomBytes(plainTextBytes);

			return Convert.ToBase64String(resultBytes);
		}

		/// <overrides>
		/// Compares plain text input with a computed hash using the given hash provider instance.
		/// </overrides>
		/// <summary>
		/// Compares plain text input with a computed hash using the given hash provider instance.
		/// </summary>
		/// <remarks>
		/// Use this method to compare hash values. Since hashes may contain a random "salt" value, two seperately generated
		/// hashes of the same plain text may result in different values. 
		/// </remarks>
		/// <param name="hashInstance">A hash instance from configuration.</param>
		/// <param name="plaintext">The input for which you want to compare the hash to.</param>
		/// <param name="hashedText">The hash value for which you want to compare the input to.</param>
		/// <returns><c>true</c> if plainText hashed is equal to the hashedText. Otherwise, <c>false</c>.</returns>
		public static bool CompareHash(string hashInstance, byte[] plaintext, byte[] hashedText)
		{
			if (string.IsNullOrEmpty(hashInstance)) throw new ArgumentException(Resources.ExceptionNullOrEmptyString, "hashInstance");

			try
			{
				HashProviderFactory factory = new HashProviderFactory(ConfigurationSourceFactory.Create());
				IHashProvider hashProvider = factory.Create(hashInstance);

				return hashProvider.CompareHash(plaintext, hashedText);
			}
			catch (ConfigurationErrorsException configurationException)
			{
				TryLogHashConfigurationError(configurationException, hashInstance);

				throw;
			}
		}

		/// <summary>
		/// Compares plain text input with a computed hash using the given hash provider instance.
		/// </summary>
		/// <remarks>
		/// Use this method to compare hash values. Since hashes contain a random "salt" value, two seperately generated
		/// hashes of the same plain text will result in different values. 
		/// </remarks>
		/// <param name="hashInstance">A hash instance from configuration.</param>
		/// <param name="plaintext">The input as a string for which you want to compare the hash to.</param>
		/// <param name="hashedText">The hash as a string for which you want to compare the input to.</param>
		/// <returns><c>true</c> if plainText hashed is equal to the hashedText. Otherwise, <c>false</c>.</returns>
		public static bool CompareHash(string hashInstance, string plaintext, string hashedText)
		{
			if (string.IsNullOrEmpty(hashInstance)) throw new ArgumentException(Resources.ExceptionNullOrEmptyString, "hashInstance");

			byte[] plainTextBytes = UnicodeEncoding.Unicode.GetBytes(plaintext);
			byte[] hashedTextBytes = Convert.FromBase64String(hashedText);

			bool result = CompareHash(hashInstance, plainTextBytes, hashedTextBytes);
			CryptographyUtility.GetRandomBytes(plainTextBytes);

			return result;
		}

		/// <summary>
		/// Encrypts a secret using a specified symmetric cryptography provider.
		/// </summary>
		/// <param name="symmetricInstance">A symmetric instance from configuration.</param>
		/// <param name="plaintext">The input for which you want to encrypt.</param>
		/// <returns>The resulting cipher text.</returns>
		public static byte[] EncryptSymmetric(string symmetricInstance, byte[] plaintext)
		{
			if (string.IsNullOrEmpty(symmetricInstance)) throw new ArgumentException(Resources.ExceptionNullOrEmptyString, "symmetricInstance");

			try
			{
				SymmetricCryptoProviderFactory factory = new SymmetricCryptoProviderFactory(ConfigurationSourceFactory.Create());
				ISymmetricCryptoProvider symmetricProvider = factory.Create(symmetricInstance);

				return symmetricProvider.Encrypt(plaintext);
			}
			catch (ConfigurationErrorsException configurationException)
			{
				TryLogSymmetricConfigurationError(configurationException, symmetricInstance);

				throw;
			}
		}

		/// <summary>
		/// Encrypts a secret using a specified symmetric cryptography provider.
		/// </summary>
		/// <param name="symmetricInstance">A symmetric instance from configuration.</param>
		/// <param name="plaintext">The input as a base64 encoded string for which you want to encrypt.</param>
		/// <returns>The resulting cipher text as a base64 encoded string.</returns>
		public static string EncryptSymmetric(string symmetricInstance, string plaintext)
		{
			if (string.IsNullOrEmpty(symmetricInstance)) throw new ArgumentException(Resources.ExceptionNullOrEmptyString, "symmetricInstance");

			byte[] plainTextBytes = UnicodeEncoding.Unicode.GetBytes(plaintext);
			byte[] cipherTextBytes = EncryptSymmetric(symmetricInstance, plainTextBytes);
			CryptographyUtility.GetRandomBytes(plainTextBytes);
			return Convert.ToBase64String(cipherTextBytes);
		}

		/// <summary>
		/// Decrypts a cipher text using a specified symmetric cryptography provider.
		/// </summary>
		/// <param name="symmetricInstance">A symmetric instance from configuration.</param>
		/// <param name="ciphertext">The cipher text for which you want to decrypt.</param>
		/// <returns>The resulting plain text.</returns>
		public static byte[] DecryptSymmetric(string symmetricInstance, byte[] ciphertext)
		{
			if (string.IsNullOrEmpty(symmetricInstance)) throw new ArgumentException(Resources.ExceptionNullOrEmptyString, "symmetricInstance");

			try
			{
				SymmetricCryptoProviderFactory factory = new SymmetricCryptoProviderFactory(ConfigurationSourceFactory.Create());
				ISymmetricCryptoProvider symmetricProvider = factory.Create(symmetricInstance);

				return symmetricProvider.Decrypt(ciphertext);
			}
			catch (ConfigurationErrorsException configurationException)
			{
				TryLogSymmetricConfigurationError(configurationException, symmetricInstance);

				throw;
			}
		}

		/// <summary>
		/// Decrypts a cipher text using a specified symmetric cryptography provider.
		/// </summary>
		/// <param name="symmetricInstance">A symmetric instance from configuration.</param>
		/// <param name="ciphertextBase64">The cipher text as a base64 encoded string for which you want to decrypt.</param>
		/// <returns>The resulting plain text as a string.</returns>
		public static string DecryptSymmetric(string symmetricInstance, string ciphertextBase64)
		{
			if (string.IsNullOrEmpty(symmetricInstance)) throw new ArgumentException(Resources.ExceptionNullOrEmptyString, "symmetricInstance");
			if (string.IsNullOrEmpty(ciphertextBase64)) throw new ArgumentException(Resources.ExceptionNullOrEmptyString, "ciphertextBase64");

			byte[] cipherTextBytes = Convert.FromBase64String(ciphertextBase64);
			byte[] decryptedBytes = DecryptSymmetric(symmetricInstance, cipherTextBytes);
			string decryptedString = UnicodeEncoding.Unicode.GetString(decryptedBytes);
			CryptographyUtility.GetRandomBytes(decryptedBytes);

			return decryptedString;
		}

		private static void TryLogHashConfigurationError(ConfigurationErrorsException configurationException, string hashInstance)
		{
			TryLogConfigurationError(configurationException, Resources.ErrorHashProviderConfigurationFailedMessage, hashInstance);
		}

		private static void TryLogSymmetricConfigurationError(ConfigurationErrorsException configurationException, string symmetricInstance)
		{
			TryLogConfigurationError(configurationException, Resources.ErrorSymmetricEncryptionConfigurationFailedMessage, symmetricInstance);
		}

		private static void TryLogConfigurationError(ConfigurationErrorsException configurationException, string hashInstance, string template)
		{
			try
			{
				DefaultCryptographyEventLogger eventLogger = EnterpriseLibraryFactory.BuildUp<DefaultCryptographyEventLogger>();
				if (eventLogger != null)
				{
					eventLogger.LogConfigurationError(hashInstance, template, configurationException);
				}
			}
			catch { }
		}
	}
}