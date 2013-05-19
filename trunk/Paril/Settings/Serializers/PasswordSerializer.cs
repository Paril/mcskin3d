//
//    MCSkin3D, a 3d skin management studio for Minecraft
//    Copyright (C) 2013 Altered Softworks & MCSkin3D Team
//
//    This program is free software: you can redistribute it and/or modify
//    it under the terms of the GNU General Public License as published by
//    the Free Software Foundation, either version 3 of the License, or
//    (at your option) any later version.
//
//    This program is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//    GNU General Public License for more details.
//
//    You should have received a copy of the GNU General Public License
//    along with this program.  If not, see <http://www.gnu.org/licenses/>.
//

using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Paril.Settings.Serializers
{
	public class PasswordSerializer<T> : ITypeSerializer
		where T : SymmetricAlgorithm, new()
	{
		private static string passPhrase = Environment.UserName + Environment.CurrentDirectory; // can be any string

		private static string saltValue = Environment.MachineName + Environment.SystemDirectory + Environment.UserDomainName;
		// can be any string

		private static string hashAlgorithm = "SHA1"; // can be "MD5"
		private static int passwordIterations = 2; // can be any number
		private static string initVector = "@1B2c3D4e5F6g7H8"; // must be 16 bytes
		private static int keySize = 256; // can be 192 or 128

		#region ITypeSerializer Members

		public object Deserialize(string str)
		{
			try
			{
				return Decrypt(str,
				               passPhrase,
				               saltValue,
				               hashAlgorithm,
				               passwordIterations,
				               initVector,
				               keySize);
			}
			catch
			{
				return "";
			}
		}

		public string Serialize(object obj)
		{
			return Encrypt((string) obj,
			               passPhrase,
			               saltValue,
			               hashAlgorithm,
			               passwordIterations,
			               initVector,
			               keySize);
		}

		#endregion

		/// <summary>
		/// Encrypts specified plaintext using Rijndael symmetric key algorithm
		/// and returns a base64-encoded result.
		/// </summary>
		/// <param name="plainText">
		/// Plaintext value to be encrypted.
		/// </param>
		/// <param name="passPhrase">
		/// Passphrase from which a pseudo-random password will be derived. The
		/// derived password will be used to generate the encryption key.
		/// Passphrase can be any string. In this example we assume that this
		/// passphrase is an ASCII string.
		/// </param>
		/// <param name="saltValue">
		/// Salt value used along with passphrase to generate password. Salt can
		/// be any string. In this example we assume that salt is an ASCII string.
		/// </param>
		/// <param name="hashAlgorithm">
		/// Hash algorithm used to generate password. Allowed values are: "MD5" and
		/// "SHA1". SHA1 hashes are a bit slower, but more secure than MD5 hashes.
		/// </param>
		/// <param name="passwordIterations">
		/// Number of iterations used to generate password. One or two iterations
		/// should be enough.
		/// </param>
		/// <param name="initVector">
		/// Initialization vector (or IV). This value is required to encrypt the
		/// first block of plaintext data. For RijndaelManaged class IV must be 
		/// exactly 16 ASCII characters long.
		/// </param>
		/// <param name="keySize">
		/// Size of encryption key in bits. Allowed values are: 128, 192, and 256. 
		/// Longer keys are more secure than shorter keys.
		/// </param>
		/// <returns>
		/// Encrypted value formatted as a base64-encoded string.
		/// </returns>
		public static string Encrypt(string plainText,
		                             string passPhrase,
		                             string saltValue,
		                             string hashAlgorithm,
		                             int passwordIterations,
		                             string initVector,
		                             int keySize)
		{
			// Convert strings into byte arrays.
			// Let us assume that strings only contain ASCII codes.
			// If strings include Unicode characters, use Unicode, UTF7, or UTF8 
			// encoding.
			byte[] initVectorBytes = Encoding.ASCII.GetBytes(initVector);
			byte[] saltValueBytes = Encoding.ASCII.GetBytes(saltValue);

			// Convert our plaintext into a byte array.
			// Let us assume that plaintext contains UTF8-encoded characters.
			byte[] plainTextBytes = Encoding.UTF8.GetBytes(plainText);

			// First, we must create a password, from which the key will be derived.
			// This password will be generated from the specified passphrase and 
			// salt value. The password will be created using the specified hash 
			// algorithm. Password creation can be done in several iterations.
			var password = new PasswordDeriveBytes(
				passPhrase,
				saltValueBytes,
				hashAlgorithm,
				passwordIterations);

			// Use the password to generate pseudo-random bytes for the encryption
			// key. Specify the size of the key in bytes (instead of bits).
			byte[] keyBytes = password.GetBytes(keySize / 8);

			// Create uninitialized Rijndael encryption object.
			var symmetricKey = new T();

			// It is reasonable to set encryption mode to Cipher Block Chaining
			// (CBC). Use default options for other symmetric key parameters.
			symmetricKey.Mode = CipherMode.CBC;

			// Generate encryptor from the existing key bytes and initialization 
			// vector. Key size will be defined based on the number of the key 
			// bytes.
			ICryptoTransform encryptor = symmetricKey.CreateEncryptor(
				keyBytes,
				initVectorBytes);

			// Define memory stream which will be used to hold encrypted data.
			var memoryStream = new MemoryStream();

			// Define cryptographic stream (always use Write mode for encryption).
			var cryptoStream = new CryptoStream(memoryStream,
			                                    encryptor,
			                                    CryptoStreamMode.Write);
			// Start encrypting.
			cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);

			// Finish encrypting.
			cryptoStream.FlushFinalBlock();

			// Convert our encrypted data from a memory stream into a byte array.
			byte[] cipherTextBytes = memoryStream.ToArray();

			// Close both streams.
			memoryStream.Close();
			cryptoStream.Close();

			// Convert encrypted data into a base64-encoded string.
			string cipherText = Convert.ToBase64String(cipherTextBytes);

			// Return encrypted string.
			return cipherText;
		}

		/// <summary>
		/// Decrypts specified ciphertext using Rijndael symmetric key algorithm.
		/// </summary>
		/// <param name="cipherText">
		/// Base64-formatted ciphertext value.
		/// </param>
		/// <param name="passPhrase">
		/// Passphrase from which a pseudo-random password will be derived. The
		/// derived password will be used to generate the encryption key.
		/// Passphrase can be any string. In this example we assume that this
		/// passphrase is an ASCII string.
		/// </param>
		/// <param name="saltValue">
		/// Salt value used along with passphrase to generate password. Salt can
		/// be any string. In this example we assume that salt is an ASCII string.
		/// </param>
		/// <param name="hashAlgorithm">
		/// Hash algorithm used to generate password. Allowed values are: "MD5" and
		/// "SHA1". SHA1 hashes are a bit slower, but more secure than MD5 hashes.
		/// </param>
		/// <param name="passwordIterations">
		/// Number of iterations used to generate password. One or two iterations
		/// should be enough.
		/// </param>
		/// <param name="initVector">
		/// Initialization vector (or IV). This value is required to encrypt the
		/// first block of plaintext data. For RijndaelManaged class IV must be
		/// exactly 16 ASCII characters long.
		/// </param>
		/// <param name="keySize">
		/// Size of encryption key in bits. Allowed values are: 128, 192, and 256.
		/// Longer keys are more secure than shorter keys.
		/// </param>
		/// <returns>
		/// Decrypted string value.
		/// </returns>
		/// <remarks>
		/// Most of the logic in this function is similar to the Encrypt
		/// logic. In order for decryption to work, all parameters of this function
		/// - except cipherText value - must match the corresponding parameters of
		/// the Encrypt function which was called to generate the
		/// ciphertext.
		/// </remarks>
		public static string Decrypt(string cipherText,
		                             string passPhrase,
		                             string saltValue,
		                             string hashAlgorithm,
		                             int passwordIterations,
		                             string initVector,
		                             int keySize)
		{
			// Convert strings defining encryption key characteristics into byte
			// arrays. Let us assume that strings only contain ASCII codes.
			// If strings include Unicode characters, use Unicode, UTF7, or UTF8
			// encoding.
			byte[] initVectorBytes = Encoding.ASCII.GetBytes(initVector);
			byte[] saltValueBytes = Encoding.ASCII.GetBytes(saltValue);

			// Convert our ciphertext into a byte array.
			byte[] cipherTextBytes = Convert.FromBase64String(cipherText);

			// First, we must create a password, from which the key will be 
			// derived. This password will be generated from the specified 
			// passphrase and salt value. The password will be created using
			// the specified hash algorithm. Password creation can be done in
			// several iterations.
			var password = new PasswordDeriveBytes(
				passPhrase,
				saltValueBytes,
				hashAlgorithm,
				passwordIterations);

			// Use the password to generate pseudo-random bytes for the encryption
			// key. Specify the size of the key in bytes (instead of bits).
			byte[] keyBytes = password.GetBytes(keySize / 8);

			// Create uninitialized Rijndael encryption object.
			var symmetricKey = new T();

			// It is reasonable to set encryption mode to Cipher Block Chaining
			// (CBC). Use default options for other symmetric key parameters.
			symmetricKey.Mode = CipherMode.CBC;

			// Generate decryptor from the existing key bytes and initialization 
			// vector. Key size will be defined based on the number of the key 
			// bytes.
			ICryptoTransform decryptor = symmetricKey.CreateDecryptor(
				keyBytes,
				initVectorBytes);

			// Define memory stream which will be used to hold encrypted data.
			var memoryStream = new MemoryStream(cipherTextBytes);

			// Define cryptographic stream (always use Read mode for encryption).
			var cryptoStream = new CryptoStream(memoryStream,
			                                    decryptor,
			                                    CryptoStreamMode.Read);

			// Since at this point we don't know what the size of decrypted data
			// will be, allocate the buffer long enough to hold ciphertext;
			// plaintext is never longer than ciphertext.
			var plainTextBytes = new byte[cipherTextBytes.Length];

			// Start decrypting.
			int decryptedByteCount = cryptoStream.Read(plainTextBytes,
			                                           0,
			                                           plainTextBytes.Length);

			// Close both streams.
			memoryStream.Close();
			cryptoStream.Close();

			// Convert decrypted data into a string. 
			// Let us assume that the original plaintext string was UTF8-encoded.
			string plainText = Encoding.UTF8.GetString(plainTextBytes,
			                                           0,
			                                           decryptedByteCount);

			// Return decrypted string.   
			return plainText;
		}
	}
}