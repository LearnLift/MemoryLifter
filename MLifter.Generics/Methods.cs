using System;
using System.Security.Cryptography;
using System.Text;
using System.Xml;
using System.IO;
using System.Management;
using System.Text.RegularExpressions;
using System.Security;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using MLifter.Generics.Properties;
using System.Net;
using Microsoft.Win32;
using System.Diagnostics;
using System.ServiceModel.Syndication;
using System.Xml.Serialization;

namespace MLifter.Generics
{
	/// <summary>
	/// This struct stores the general information about a module.
	/// </summary>
	/// <remarks>Documented by Dev05, 2012-02-07</remarks>
	[Serializable]
	public struct ModuleInfo
	{
		public string Id;
		public string Title;
		public SerializableList<string> Categories;

		public string Author;
		public string AuthorMail;
		public string AuthorUrl;

		public int Cards;
		public byte[] IconBig;
		public byte[] IconSmall;

		public string Description;
		public byte[] Preview;

		public string EditDate;

		public long Size;
		public string DownloadUrl;
	}

	/// <summary>
	/// The category of a module.
	/// </summary>
	/// <remarks>Documented by Dev05, 2012-02-07</remarks>
	[Serializable]
	public struct ModuleCategory
	{
		public int Id;
		public int ParentCategory;
		public string Title;
	}

	/// <summary>
	/// Defines the relation ship type of a link in an atom feed entry.
	/// </summary>
	/// <remarks>Documented by Dev05, 2012-02-07</remarks>
	public enum AtomLinkRelationshipType
	{
		/// <summary>
		/// The link points to a parent entry.
		/// </summary>
		/// <remarks>Documented by Dev05, 2012-02-07</remarks>
		Parent,
		/// <summary>
		/// The link points to a learning module.
		/// </summary>
		/// <remarks>Documented by Dev05, 2012-02-07</remarks>
		Module,
		/// <summary>
		/// The link points to a preview image.
		/// </summary>
		/// <remarks>Documented by Dev05, 2012-02-07</remarks>
		Preview,
		/// <summary>
		/// The link points to a big icon for the module.
		/// </summary>
		/// <remarks>Documented by Dev05, 2012-02-07</remarks>
		IconBig,
		/// <summary>
		/// The link points to a small icon for the module.
		/// </summary>
		/// <remarks>Documented by Dev05, 2012-02-07</remarks>
		IconSmall
	}
	
	/// <summary>
	/// documented by SDE, 24.2.2009
	/// </summary>
	public struct DataArray
	{
		public string UnlockKey;
		public byte[] EncryptedData;
		public byte[] Signature;

		public DataArray(string publicKey, byte[] encryptedData, byte[] signature)
		{
			UnlockKey = publicKey;
			EncryptedData = encryptedData;
			Signature = signature;
		}
	}

	/// <summary>
	/// </summary>
	/// <remarks>Documented by Dev04, 2009-03-12</remarks>
	public struct KeyArray
	{
		public string LicenseKey;
		public string UnlockKey;

		public KeyArray(string licenseKey, string unlockKey)
		{
			LicenseKey = licenseKey;
			UnlockKey = unlockKey;
		}
	}

	public class Methods
	{
		private static Random rand = new Random();
		private static Regex regHex = new Regex("[^0-9A-F]", RegexOptions.Compiled | RegexOptions.IgnoreCase);

		private static string CompressedAlphabet = "23456789ABCDEFGHJKLMNPQRSTUVWXYZ";
		/// <summary>
		/// Converts a byte array to compressed string.
		/// </summary>
		/// <param name="Bytes">The byte array to be compressed.</param>
		/// <returns></returns>
		/// <remarks>Documented by Dev05, 2009-02-09</remarks>
		public static string ByteArrayToCompressedArrayString(byte[] Bytes)
		{
			if (Bytes == (byte[])null)
				throw new ArgumentNullException("Bytes");

			Random rand = new Random((int)DateTime.Now.Ticks);
			string result = string.Empty;

			int fillupCount = 5 - Bytes.Length * 8 % 5;
			string binary = string.Empty;
			for (int i = 0; i < Bytes.Length || (i >= Bytes.Length && binary.Length % 5 != 0); i++)
			{
				if (i >= Bytes.Length)
					for (int j = 0; j < fillupCount; j++)
						binary += rand.NextDouble() > 0.5 ? "1" : "0";
				else
					binary += GetBinaryString(Bytes[i], 8);
			}

			string binHolder = string.Empty;
			for (int i = 0; i < binary.Length; i++)
			{
				binHolder += binary[i];

				if (i % 5 == 4)
				{
					result += CompressedAlphabet[Convert.ToInt32(binHolder, 2)];
					binHolder = string.Empty;
				}
			}

			return result;
		}
		/// <summary>
		/// Converts a compressed string to a uncompressed byte array.
		/// </summary>
		/// <param name="compressedArray">The compressed array.</param>
		/// <param name="expectedLength">The expected length.</param>
		/// <returns></returns>
		/// <remarks>Documented by Dev05, 2009-02-09</remarks>
		public static byte[] CompressedArrayStringToByteArray(string compressedArray, int expectedLength)
		{
			if (expectedLength == 0)
				throw new ArgumentException("expectedLength == 0", "expectedLength");

			if (compressedArray == (string)null)
				throw new ArgumentNullException("compressedArray");

			if (expectedLength < 0)
				throw new ArgumentException("expectedLength < 0", "expectedLength");

			byte[] Bytes = new byte[expectedLength]; //For all Data: compressedArray.Length / 8 * 5

			string binary = string.Empty;
			for (int i = 0; i < compressedArray.Length; i++)
				binary += GetBinaryString(CompressedAlphabet.IndexOf(compressedArray[i]), 5);

			string binHolder = string.Empty;
			for (int i = 0; i < binary.Length; i++)
			{
				binHolder += binary[i];

				if (i % 8 == 7)
				{
					int byteNumber = (i / 8);

					Bytes[byteNumber] = (byte)Convert.ToInt32(binHolder, 2);
					binHolder = string.Empty;

					if (byteNumber + 1 >= expectedLength)
						break;
				}
			}

			return Bytes;
		}
		/// <summary>
		/// Gets the binary string.
		/// </summary>
		/// <param name="data">The data.</param>
		/// <param name="length">The length of the resulting string (filled with zeros).</param>
		/// <returns></returns>
		/// <remarks>Documented by Dev05, 2009-02-09</remarks>
		private static string GetBinaryString(int data, int length)
		{
			int numbase = 2;
			string strBin = "";
			int[] result = new int[length];
			int MaxBit = length;
			for (; data > 0; data /= numbase)
			{
				int rem = data % numbase;
				result[--MaxBit] = rem;
			}
			for (int i = 0; i < result.Length; i++)
				strBin += result.GetValue(i);
			return strBin;
		}

		/// <summary>
		/// License Type
		/// </summary>
		private enum LicenseType
		{
			/// <summary>
			/// Server
			/// </summary>
			Server,
			/// <summary>
			/// Client
			/// </summary>
			Client
		}

		/// <summary>
		/// Creates the license key.
		/// </summary>
		/// <param name="LicenseType">Type of the license (client|server).</param>
		/// <returns></returns>
		/// <remarks>Documented by Dev04, 2009-02-11</remarks>
		public static string CreateLicenseKey(string LicenseType)
		{
			bool even;

			LicenseType lt = (LicenseType)Enum.Parse(typeof(LicenseType), LicenseType, true);
			even = (lt == Methods.LicenseType.Server);

			string LicenseKey = "";
			string block = "";
			int i = 0;
			int blocks = 4;

			do
			{
				block = CreateKeyBlock(even).ToString();
				if (!LicenseKey.Contains(block) && i < blocks)
				{
					if (i == 0)
						LicenseKey = block;
					else
						LicenseKey = LicenseKey + "-" + block;

					i++;
				}
			} while (i < blocks);

			return LicenseKey;
		}

		/// <summary>
		/// Generates the sync key.
		/// </summary>
		/// <param name="Password">The password.</param>
		/// <returns></returns>
		/// <remarks>Documented by Dev04, 2009-02-11</remarks>
		public static string GenerateSymKey(string Password)
		{
			return BitConverter.ToString(new MD5CryptoServiceProvider().ComputeHash(ASCIIEncoding.ASCII.GetBytes(Password))).Replace("-", "").ToLower().Substring(0, 24);
		}

		/// <summary>
		/// Ts the DES decrypt.
		/// </summary>
		/// <param name="toDecrypt">To decrypt.</param>
		/// <param name="key">The key.</param>
		/// <param name="useHashing">if set to <c>true</c> [use hashing].</param>
		/// <returns></returns>
		/// <remarks>Documented by Dev04, 2009-02-11</remarks>
		public static string TDesDecrypt(string toDecrypt, string key, bool useHashing)
		{
			if (toDecrypt == (string)null) throw new ArgumentNullException("toDecrypt");

			byte[] toEncryptArray = Convert.FromBase64String(toDecrypt);

			return TDesDecrypt(toEncryptArray, key, useHashing);
		}
		/// <summary>
		/// Ts the DES decrypt.
		/// </summary>
		/// <param name="toEncryptArray">To encrypt array.</param>
		/// <param name="key">The key.</param>
		/// <param name="useHashing">if set to <c>true</c> [use hashing].</param>
		/// <returns></returns>
		/// <remarks>Documented by Dev05, 2009-02-11</remarks>
		public static string TDesDecrypt(byte[] toDecryptArray, string key, bool useHashing)
		{
			return UTF8Encoding.UTF8.GetString(TDesDecryptBytes(toDecryptArray, key, useHashing));
		}
		/// <summary>
		/// Ts the DES decrypt bytes.
		/// </summary>
		/// <param name="toDecryptArray">To decrypt array.</param>
		/// <param name="key">The key.</param>
		/// <param name="useHashing">if set to <c>true</c> [use hashing].</param>
		/// <returns></returns>
		/// <remarks>Documented by Dev05, 2009-02-11</remarks>
		public static byte[] TDesDecryptBytes(byte[] toDecryptArray, string key, bool useHashing)
		{
			if (key == (string)null) throw new ArgumentNullException("key");
			if (key.Trim().Length == 0) throw new ArgumentException("key");
			if (toDecryptArray == (byte[])null) throw new ArgumentNullException("toDecryptArray");

			byte[] keyArray;
			if (useHashing)
			{
				MD5CryptoServiceProvider hashmd5 = new MD5CryptoServiceProvider();
				keyArray = hashmd5.ComputeHash(UTF8Encoding.UTF8.GetBytes(key));
			}
			else
				keyArray = UTF8Encoding.UTF8.GetBytes(key);

			TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();
			tdes.Key = keyArray;
			tdes.Mode = CipherMode.ECB;
			tdes.Padding = PaddingMode.PKCS7;

			ICryptoTransform cTransform = tdes.CreateDecryptor();
			return cTransform.TransformFinalBlock(toDecryptArray, 0, toDecryptArray.Length);
		}

		/// <summary>
		/// Ts the DES encrypt.
		/// </summary>
		/// <param name="toEncrypt">To encrypt.</param>
		/// <param name="key">The key.</param>
		/// <param name="useHashing">if set to <c>true</c> [use hashing].</param>
		/// <returns></returns>
		/// <remarks>Documented by Dev04, 2009-02-11</remarks>
		public static string TDesEncrypt(string toEncrypt, string key, bool useHashing)
		{
			if (toEncrypt == (string)null) throw new ArgumentNullException("toEncrypt");

			byte[] toEncryptArray = UTF8Encoding.UTF8.GetBytes(toEncrypt);

			return TDesEncrypt(toEncryptArray, key, useHashing);
		}
		/// <summary>
		/// Ts the DES encrypt.
		/// </summary>
		/// <param name="toEncryptArray">To encrypt array.</param>
		/// <param name="key">The key.</param>
		/// <param name="useHashing">if set to <c>true</c> [use hashing].</param>
		/// <returns></returns>
		/// <remarks>Documented by Dev05, 2009-02-11</remarks>
		public static string TDesEncrypt(byte[] toEncryptArray, string key, bool useHashing)
		{
			byte[] resultArray = TDesEncryptBytes(toEncryptArray, key, useHashing);

			return Convert.ToBase64String(resultArray, 0, resultArray.Length);
		}
		/// <summary>
		/// Ts the DES encrypt bytes.
		/// </summary>
		/// <param name="toEncryptArray">To encrypt array.</param>
		/// <param name="key">The key.</param>
		/// <param name="useHashing">if set to <c>true</c> [use hashing].</param>
		/// <returns></returns>
		/// <remarks>Documented by Dev05, 2009-02-11</remarks>
		public static byte[] TDesEncryptBytes(string toEncrypt, string key, bool useHashing)
		{
			if (toEncrypt == (string)null) throw new ArgumentNullException("toEncrypt");

			byte[] toEncryptArray = UTF8Encoding.UTF8.GetBytes(toEncrypt);

			return TDesEncryptBytes(toEncryptArray, key, useHashing);
		}
		/// <summary>
		/// Ts the DES encrypt bytes.
		/// </summary>
		/// <param name="toEncryptArray">To encrypt array.</param>
		/// <param name="key">The key.</param>
		/// <param name="useHashing">if set to <c>true</c> [use hashing].</param>
		/// <returns></returns>
		/// <remarks>Documented by Dev05, 2009-02-11</remarks>
		public static byte[] TDesEncryptBytes(byte[] toEncryptArray, string key, bool useHashing)
		{
			if (toEncryptArray == (byte[])null) throw new ArgumentNullException("toEncryptArray");
			if (key == (string)null) throw new ArgumentNullException("key");

			byte[] keyArray;
			if (useHashing)
			{
				MD5CryptoServiceProvider hashmd5 = new MD5CryptoServiceProvider();
				keyArray = hashmd5.ComputeHash(UTF8Encoding.UTF8.GetBytes(key));
			}
			else
				keyArray = UTF8Encoding.UTF8.GetBytes(key);

			TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();
			tdes.Key = keyArray;
			tdes.Mode = CipherMode.ECB;
			tdes.Padding = PaddingMode.PKCS7;

			ICryptoTransform cTransform = tdes.CreateEncryptor();
			return cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
		}

		/// <summary>
		/// Creates the key block.
		/// </summary>
		/// <param name="even">if set to <c>true</c> [even].</param>
		/// <returns></returns>
		/// <remarks>Documented by Dev04, 2009-02-11</remarks>
		private static int CreateKeyBlock(bool even)
		{
			int block = 0;

			block = rand.Next(1000, 9999);

			while (IsEven(block) != even)
			{
				block = CreateKeyBlock(even);
			}

			return block;
		}

		/// <summary>
		/// Determines whether the specified block is even.
		/// </summary>
		/// <param name="block">The block.</param>
		/// <returns>
		/// 	<c>true</c> if the specified block is even; otherwise, <c>false</c>.
		/// </returns>
		/// <remarks>Documented by Dev04, 2009-02-11</remarks>
		public static bool IsEven(int block)
		{
			if ((block < 0) || (block > 9999)) throw new ArgumentOutOfRangeException("0 - 9999", "block");
			int lastDigit = block % 10;
			int total = lastDigit;
			int nextDigit = (block / 10) % 10;
			total = total + nextDigit;
			nextDigit = (block / 100) % 10;
			total = total + nextDigit;
			nextDigit = (block / 1000) % 10;
			total = total + nextDigit;

			if (total % 2 == 0)
				return true;
			else
				return false;
		}

		/// <summary>
		/// Extracts the public key.
		/// </summary>
		/// <param name="PublicKey">The public key.</param>
		/// <returns></returns>
		/// <remarks>Documented by Dev04, 2009-02-11</remarks>
		public static string ExtractPublicKey(string PublicKey)
		{
			if (PublicKey == (string)null) throw new ArgumentNullException("PublicKey");

			XmlDocument doc = new XmlDocument();
			try
			{
				doc.LoadXml(PublicKey);
			}
			catch (Exception ex)
			{
				throw new ArgumentException("Could not extract modulus from key!", "PublicKey", ex);
			}
			XmlElement root = doc.DocumentElement;
			XmlNode node = root.SelectSingleNode("Modulus");
			if (node != null)
				return Base64ToHex(node.InnerText);
			else
				throw new ArgumentException("Could not extract modulus from key!", "PublicKey");
		}

		/// <summary>
		/// Extracts the exponent.
		/// </summary>
		/// <param name="PublicKey">The public key.</param>
		/// <returns></returns>
		/// <remarks>Documented by Dev04, 2009-02-11</remarks>
		public static string ExtractExponent(string PublicKey)
		{
			if (PublicKey == (string)null) throw new ArgumentNullException("PublicKey");

			XmlDocument doc = new XmlDocument();
			try
			{
				doc.LoadXml(PublicKey);
			}
			catch (Exception ex)
			{
				throw new ArgumentException("Could not extract exponent from key!", "PublicKey", ex);
			}
			XmlElement root = doc.DocumentElement;
			XmlNode node = root.SelectSingleNode("Exponent");
			if (node != null)
				return Base64ToHex(node.InnerText);
			else
				throw new ArgumentException("Could not extract exponent from key!", "PublicKey");
		}

		/// <summary>
		/// Converts a base64 encoded string to a hex encoded string.
		/// </summary>
		/// <param name="base64">The base64.</param>
		/// <returns></returns>
		/// <remarks>Documented by Dev04, 2009-02-11</remarks>
		public static string Base64ToHex(string base64)
		{
			if (base64 == (string)null) throw new ArgumentNullException("base64");
			return ByteArrayToHexString(Convert.FromBase64String(base64));
		}

		/// <summary>
		/// Converts a hex encoded string to a base64 encoded string.
		/// </summary>
		/// <param name="hex">The hex.</param>
		/// <returns></returns>
		/// <remarks>Documented by Dev04, 2009-02-11</remarks>
		public static string HexToBase64(string hex)
		{
			if (hex == (string)null) throw new ArgumentNullException("hex");
			return Convert.ToBase64String(HexStringToByteArray(hex));
		}

		/// <summary>
		/// Converts a byte array to a hex encoded string.
		/// </summary>
		/// <param name="Bytes">The bytes.</param>
		/// <returns></returns>
		/// <remarks>Documented by Dev04, 2009-02-11</remarks>
		public static string ByteArrayToHexString(byte[] Bytes)
		{
			if (Bytes == (byte[])null) throw new ArgumentNullException("Bytes");
			StringBuilder Result = new StringBuilder();
			string HexAlphabet = "0123456789ABCDEF";
			//string HexAlphabet = "ABCDEFGHIJKLMNOP";

			foreach (byte B in Bytes)
			{
				Result.Append(HexAlphabet[(int)(B >> 4)]);
				Result.Append(HexAlphabet[(int)(B & 0xF)]);
			}

			return Result.ToString();
		}

		/// <summary>
		/// Converts a hex encoded string to a byte array.
		/// </summary>
		/// <param name="Hex">The hex encoded string.</param>
		/// <returns></returns>
		/// <remarks>Documented by Dev04, 2009-02-11</remarks>
		public static byte[] HexStringToByteArray(string Hex)
		{
			if (Hex == (string)null) throw new ArgumentNullException("Hex");
			if ((Hex.Length % 2 != 0) || (regHex.Match(Hex).Success)) throw new ArgumentException("Not a hex string!", "Hex");
			byte[] Bytes = new byte[Hex.Length / 2];
			int[] HexValue = new int[] { 0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09,
								 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x0A, 0x0B, 0x0C, 0x0D,
								 0x0E, 0x0F };

			for (int x = 0, i = 0; i < Hex.Length; i += 2, x += 1)
			{
				Bytes[x] = (byte)(HexValue[Char.ToUpper(Hex[i + 0]) - '0'] << 4 |
								  HexValue[Char.ToUpper(Hex[i + 1]) - '0']);
			}

			return Bytes;
		}

		/// <summary>
		/// Shifts the specified param right
		/// </summary>
		/// <param name="param">The param.</param>
		/// <param name="length">The length.</param>
		/// <returns></returns>
		/// <remarks>Documented by Dev04, 2009-02-11</remarks>
		public static string Right(string param, int length)
		{
			if (param == (string)null) throw new ArgumentNullException("param");
			if (param.Length - length < 0 || param.Length < param.Length - length || length < 0) throw new ArgumentOutOfRangeException("length");
			//start at the index based on the lenght of the sting minus
			//the specified lenght and assign it a variable
			string result = param.Substring(param.Length - length, length);
			//return the result of the operation
			return result;
		}

		private static string mid = string.Empty;
		/// <summary>
		/// Gets the MID.
		/// </summary>
		/// <returns></returns>
		/// <remarks>Documented by Dev08, 2009-02-12</remarks>
		public static string GetMID()
		{
			lock (mid)
			{
				if (mid != string.Empty)
					return mid;

				string value;
				try
				{
					value = Identifier("Win32_BaseBoard", "SerialNumber") + Identifier("Win32_Processor", "ProcessorId");
				}
				catch (MachineIDGenerationException mex)
				{
					value = getOsMID();
					if (value == null) throw mex;
				}

				HashAlgorithm hashObject = new RIPEMD160Managed();
				byte[] hash = hashObject.ComputeHash(Encoding.ASCII.GetBytes(value));

				mid = ByteArrayToCompressedArrayString(hash).Substring(13, 8);
				return mid;
			}
		}

		/// <summary>
		/// Returns the MachineGuid created by Microsoft upon installation of the OS.
		/// Modifying this number on a computer causes problems throughout Windows.
		/// </summary>
		/// <returns></returns>
		/// <remarks>Documented by Dev09, 2009-09-09</remarks>
		private static string getOsMID()
		{
			RegistryKey key = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Cryptography");

			if (key != null)
			{
				string MID = key.GetValue("MachineGuid").ToString();
				return MID;
			}

			return null;
		}

		/// <summary>
		/// Creates some Strings which will be part of the MID.
		/// </summary>
		/// <param name="wmiClass">The WMI class.</param>
		/// <param name="wmiProperty">The WMI property.</param>
		/// <returns></returns>
		/// Documented by FabThe,  somewhen
		/// Updated by MatBre,  10.09.2009
		private static string Identifier(string wmiClass, string wmiProperty)
		{
			try
			{
				if (wmiClass == null || wmiProperty == null || wmiClass == string.Empty || wmiProperty == string.Empty)
					throw new MachineIDGenerationException(new Exception("One of wmiClass, wmiProperty is either null or empty"));

				string result = string.Empty;
				ManagementClass mc = new ManagementClass(wmiClass);
				ManagementObjectCollection moc = mc.GetInstances();

				foreach (ManagementObject mo in moc)
				{
					object temp = mo[wmiProperty];
					if (temp != null)
						return temp.ToString();
				}

				throw new MachineIDGenerationException(new Exception("ManagementObject list is empty"));
			}
			catch (Exception ex) { throw new MachineIDGenerationException(ex); }
		}

		/// <summary>
		/// Gets the hashed password.
		/// </summary>
		/// <param name="password">The password.</param>
		/// <returns></returns>
		/// <remarks>Documented by Dev05, 2009-03-04</remarks>
		public static string GetHashedPassword(SecureString password)
		{
			IntPtr ptr = Marshal.SecureStringToBSTR(password);
			System.Security.Cryptography.MD5 md5Hasher = System.Security.Cryptography.MD5.Create();
			byte[] data = md5Hasher.ComputeHash(Encoding.Default.GetBytes(Marshal.PtrToStringUni(ptr)));
			StringBuilder sBuilder = new StringBuilder();
			for (int i = 0; i < data.Length; i++)
			{
				sBuilder.Append(data[i].ToString("x2"));
			}
			return sBuilder.ToString();
		}

		/// <summary>
		/// Gets the hashed password.
		/// </summary>
		/// <param name="password">The password.</param>
		/// <returns></returns>
		/// <remarks>Documented by Dev05, 2009-03-04</remarks>
		public static string GetHashedPassword(string password)
		{
			System.Security.Cryptography.MD5 md5Hasher = System.Security.Cryptography.MD5.Create();
			byte[] data = md5Hasher.ComputeHash(Encoding.Default.GetBytes(password));
			StringBuilder sBuilder = new StringBuilder();
			for (int i = 0; i < data.Length; i++)
			{
				sBuilder.Append(data[i].ToString("x2"));
			}
			return sBuilder.ToString();
		}

		/// <summary>
		/// Checks the user id.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <remarks>Documented by Dev05, 2009-03-04</remarks>
		public static void CheckUserId(int value)
		{
			switch (value)
			{
				case -1:
					throw new InvalidUsernameException("Wrong Username!");
				case -2:
					throw new InvalidPasswordException("Wrong Password!");
				case -3:
					throw new WrongAuthenticationException("Wrong User Authentication");           //allowed regarding DBInformation, but not in Userprofile
				case -4:
					throw new ForbiddenAuthenticationException("Forbidden User Authentication");       //not allowed, regarding DatabaseInformation
				case -5:
					throw new UserSessionCreationException("The new session could not be created.");      //in case the user already has another session open
			}
		}

		/// <summary>
		/// Private variable for caching WMP version.
		/// </summary>
		private static bool? WMP7OrGreater = null;

		/// <summary>
		/// Determines whether user has Windows Media Player 7 or greater.
		/// </summary>
		/// <returns>
		/// 	<c>true</c> if WMP7 or greater; otherwise, <c>false</c>.
		/// </returns>
		/// <remarks>Documented by Dev09, 2009-06-05</remarks>
		public static bool IsWMP7OrGreater()
		{
			if (!WMP7OrGreater.HasValue)
			{
				try
				{
					Guid clsid = new Guid("6BF52A52-394A-11D3-B153-00C04F79FAA6");
					System.Type oType = System.Type.GetTypeFromCLSID(clsid, true);
					object o = System.Activator.CreateInstance(oType);
					WMP7OrGreater = true;
				}
				catch (COMException) { WMP7OrGreater = false; }
				catch { WMP7OrGreater = true; }
			}
			return WMP7OrGreater.Value;
		}

		/// <summary>
		/// Gets the windows media player version.
		/// </summary>
		/// <returns>The media player version.</returns>
		/// <remarks>Documented by Dev03, 2009-07-15</remarks>
		public static Version GetWindowsMediaPlayerVersion()
		{
			try
			{
				return new Version(Microsoft.Win32.Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\MediaPlayer\PlayerUpgrade", "PlayerVersion", "1.0.0.0").ToString().Replace(',', '.'));
			}
			catch { }
			return new Version();
		}

		/// <summary>
		/// Gets the internet explorer version.
		/// </summary>
		/// <returns>The internet explorer version.</returns>
		/// <remarks>Documented by Dev03, 2009-07-15</remarks>
		public static Version GetInternetExplorerVersion()
		{
			try
			{
				return new Version(Microsoft.Win32.Registry.GetValue(@"HKEY_LOCAL_MACHINE\Software\Microsoft\Internet Explorer", "Version", "1.0.0.0").ToString());
			}
			catch { }
			return new Version();
		}

		/// <summary>
		/// Gets the learning module hash.
		/// </summary>
		/// <param name="path">The path.</param>
		/// <returns></returns>
		/// <remarks>Documented by Dev05, 2009-03-20</remarks>
		public static byte[] GetLearningModuleHash(string path)
		{
			FileStream input = File.OpenRead(path);
			try
			{
				return (new MD5CryptoServiceProvider()).ComputeHash(input);
			}
			finally { input.Close(); }
		}

		/// <summary>
		/// Gets the MLifter stick check file.
		/// </summary>
		/// <value>The M lifter stick check file.</value>
		/// <remarks>Documented by Dev05, 2009-04-01</remarks>
		public static string MLifterStickCheckFile { get { return Settings.Default.MLifterStickCheckFile; } }
		/// <summary>
		/// Gets the MLifter sticks attached to this PC.
		/// </summary>
		/// <returns></returns>
		/// <remarks>Documented by Dev05, 2009-03-25</remarks>
		public static List<DriveInfo> GetMLifterSticks()
		{
			List<DriveInfo> drives = new List<DriveInfo>();

			foreach (string driveLetter in Environment.GetLogicalDrives())
			{
				if (driveLetter.ToLower().StartsWith("a") || driveLetter.ToLower().StartsWith("b"))     //exptect drive letter A: and B:
					continue;

				DriveInfo info = new DriveInfo(driveLetter);
				if (info.DriveType != DriveType.Removable)
					continue;
				if (Directory.Exists(Path.Combine(info.RootDirectory.FullName, Settings.Default.MLifterStickCheckFile)))
					drives.Add(info);
			}

			return drives;
		}
		public static bool IsOnMLifterStick(string path)
		{
			foreach (DriveInfo info in GetMLifterSticks())
				if (path.StartsWith(info.RootDirectory.FullName))
					return true;

			return false;
		}

		/// <summary>
		/// Gets the size of the file in a nice string.
		/// </summary>
		/// <param name="Bytes">The bytes.</param>
		/// <returns></returns>
		/// <remarks>Documented by Dev05, 2009-03-26</remarks>
		public static string GetFileSize(long Bytes) { return GetFileSize(Bytes, true); }

		/// <summary>
		/// Gets the size of the file in a nice string.
		/// </summary>
		/// <param name="Bytes">The bytes.</param>
		/// <param name="showDecimalPlaces">if set to <c>true</c> show decimal places.</param>
		/// <returns></returns>
		/// <remarks>Documented by Dev05, 2009-03-26</remarks>
		public static string GetFileSize(long Bytes, bool showDecimalPlaces)
		{
			if (Bytes >= 1073741824)
			{
				Decimal size = Decimal.Divide(Bytes, 1073741824);
				return String.Format("{0:##" + (showDecimalPlaces ? ".##" : string.Empty) + "} GB", size);
			}
			else if (Bytes >= 1048576)
			{
				Decimal size = Decimal.Divide(Bytes, 1048576);
				return String.Format("{0:##" + (showDecimalPlaces ? ".##" : string.Empty) + "} MB", size);
			}
			else if (Bytes >= 1024)
			{
				Decimal size = Decimal.Divide(Bytes, 1024);
				return String.Format("{0:##" + (showDecimalPlaces ? ".##" : string.Empty) + "} KB", size);
			}
			else if (Bytes > 0)
			{
				Decimal size = Bytes;
				return showDecimalPlaces ? String.Format("{0:##" + (showDecimalPlaces ? ".##" : string.Empty) + "} Bytes", size) : "1 KB";
			}
			else
			{
				return showDecimalPlaces ? "0 Bytes" : "-";
			}
		}

		/// <summary>
		/// Gets the valid path.
		/// </summary>
		/// <param name="path">The path.</param>
		/// <param name="replacement">The replacement.</param>
		/// <returns></returns>
		/// <remarks>
		/// Documented by Dev05, 2009-04-22
		/// </remarks>
		public static string GetValidPathName(string path, string replacement = "_")
		{
			StringBuilder file = new StringBuilder(path);
			foreach (char c in Path.GetInvalidPathChars())
				file = file.Replace(c.ToString(), replacement);
			return file.ToString();
		}

		/// <summary>
		/// Gets the valid name of the file.
		/// </summary>
		/// <param name="filename">The filename.</param>
		/// <param name="replacement">The replacement.</param>
		/// <returns></returns>
		/// <remarks>CFI, 2012-02-09</remarks>
		public static string GetValidFileName(string filename, string replacement = "_")
		{
			StringBuilder file = new StringBuilder(filename);
			foreach (char c in Path.GetInvalidFileNameChars())
				file = file.Replace(c.ToString(), replacement);
			return file.ToString();
		}

		/// <summary>
		/// A private cache variable for the value of RunningFromStick.
		/// </summary>
		private static bool? isRunningFromStick = null;

		/// <summary>
		/// Gets a value indicating whether [running from stick].
		/// </summary>
		/// <value><c>true</c> if [running from stick]; otherwise, <c>false</c>.</value>
		/// <remarks>Documented by Dev02, 2009-07-20</remarks>
		public static bool IsRunningFromStick()
		{
			//Add cache
			if (isRunningFromStick.HasValue)
				return isRunningFromStick.Value;

			DirectoryInfo appPath = new DirectoryInfo(Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location));

			//check if the app directory is writeable
			string testfileName = Path.Combine(appPath.FullName, Path.GetRandomFileName());
			try
			{
				File.Create(testfileName).Close();
				File.Delete(testfileName);
			}
			catch
			{
				isRunningFromStick = false;
				return false;
			}

			//check if the app directory is on a removable drive
			try
			{
				DriveInfo appDrive = new DriveInfo(appPath.Root.FullName);
				if (appDrive.DriveType == DriveType.Removable)
				{
					isRunningFromStick = true;
					return true;
				}
			}
			catch
			{ }

			isRunningFromStick = false;
			return false;
		}

		/// <summary>
		/// Determines whether the specified object1 is equal to object2.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="object1">The object1.</param>
		/// <param name="object2">The object2.</param>
		/// <returns>
		/// 	<c>true</c> if the specified object1 is equal; otherwise, <c>false</c>.
		/// </returns>
		/// <remarks>Documented by Dev05, 2009-11-25</remarks>
		public static bool IsEqual<T>(T object1, T object2)
		{
			try
			{
				if (EqualityComparer<T>.Default.Equals(object1, object2))
					return true;

				Type t = typeof(T);

				if (t.IsGenericType && t.GetGenericTypeDefinition().Equals(typeof(Nullable<>)))
				{
					if (t.IsAssignableFrom(typeof(int?)))
					{
						if (object1 == null && object2 as int? == 0 || object1 as int? == 0 && object2 == null)
							return true;
					}
					if (t.IsAssignableFrom(typeof(string)))
					{
						if (object1 == null && object2 as string == string.Empty || object1 as string == string.Empty && object2 == null)
							return true;
					}
				}
			}
			catch (Exception exp)
			{
				Trace.WriteLine(exp.ToString());
			}

			return false;
		}
	}

	/// <summary>
	/// WebClient which handles Cookies/Sessions.
	/// </summary>
	/// <remarks>Documented by Dev05, 2009-05-08</remarks>
	public class CookieAwareWebClient : WebClient
	{
		private CookieContainer m_container = new CookieContainer();

		protected override WebRequest GetWebRequest(Uri address)
		{
			WebRequest request = base.GetWebRequest(address);
			if (request is HttpWebRequest)
			{
				(request as HttpWebRequest).CookieContainer = m_container;
			}
			return request;

		}
	}
}