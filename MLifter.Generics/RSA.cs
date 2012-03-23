/***************************************************************************************************************************************
 * Copyright (C) 2001-2012 LearnLift USA																	*
 * Contact: Learnlift USA, 12 Greenway Plaza, Suite 1510, Houston, Texas 77046, support@memorylifter.com					*
 *																								*
 * This library is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License	*
 * as published by the Free Software Foundation; either version 2.1 of the License, or (at your option) any later version.			*
 *																								*
 * This library is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty	*
 * of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU Lesser General Public License for more details.	*
 *																								*
 * You should have received a copy of the GNU Lesser General Public License along with this library; if not,					*
 * write to the Free Software Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301  USA					*
 ***************************************************************************************************************************************/
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace MLifter.Generics
{
    /// <summary>
    /// Holder for RSA implementation for MLifter.
    /// </summary>
    public class RSA
    {
        private RSACryptoServiceProvider rsa;
        private RSAParameters RSAKeyInfo;

        /// <summary>
        /// Initializes a new instance of the <see cref="RSA"/> class.
        /// </summary>
        public RSA()
        {
            rsa = new RSACryptoServiceProvider(384);
            RSAKeyInfo = rsa.ExportParameters(true);
        }

        /// <summary>
        /// Gets the public key.
        /// </summary>
        /// <returns></returns>
        public string GetPublicKey()
        {
            return rsa.ToXmlString(false);
        }

        /// <summary>
        /// Gets the modulus.
        /// </summary>
        /// <returns></returns>
        /// <remarks>Documented by Dev05, 2009-05-22</remarks>
        public byte[] GetModulus()
        {
            return RSAKeyInfo.Modulus;
        }

        /// <summary>
        /// Gets the exponend.
        /// </summary>
        /// <returns></returns>
        /// <remarks>Documented by Dev05, 2009-05-22</remarks>
        public byte[] GetExponent()
        {
            return RSAKeyInfo.Exponent;
        }

        /// <summary>
        /// Gets the private key.
        /// </summary>
        /// <returns></returns>
        public string GetPrivateKey()
        {
            return rsa.ToXmlString(true);
        }

        /// <summary>
        /// Encrypts the data.
        /// </summary>
        /// <param name="data2Encrypt">The data2 encrypt (base64/utf8 encoded).</param>
        /// <param name="PrivateKey">The private key.</param>
        /// <returns></returns>
        public string EncryptData(string data2Encrypt, string PrivateKey)
        {
            if (data2Encrypt == (string)null) throw new ArgumentNullException("data2Encrypt");
            if (PrivateKey == (string)null) throw new ArgumentNullException("PrivateKey");
            try
            {
                (new System.Xml.XmlDocument()).LoadXml(PrivateKey);
            }
            catch (Exception ex)
            {
                throw new ArgumentException("Key is no valid XML string!", "PrivateKey", ex);
            }
            rsa.FromXmlString(PrivateKey);

            byte[] plainbytes = System.Text.Encoding.UTF8.GetBytes(data2Encrypt);
            byte[] cipherbytes = rsa.Encrypt(plainbytes, false);
            return Convert.ToBase64String(cipherbytes);
        }

        /// <summary>
        /// Signs the data.
        /// </summary>
        /// <param name="plainbytes">The plainbytes.</param>
        /// <param name="PrivateKey">The private key.</param>
        /// <returns></returns>
        public byte[] SignData(byte[] plainbytes, string PrivateKey)
        {
            if (plainbytes == (byte[])null) throw new ArgumentNullException("plainbytes");
            if (PrivateKey == (string)null) throw new ArgumentNullException("PrivateKey");

            try
            {
                (new System.Xml.XmlDocument()).LoadXml(PrivateKey);
            }
            catch (Exception ex)
            {
                throw new ArgumentException("Key is no valid XML string!", "PrivateKey", ex);
            }
            rsa.FromXmlString(PrivateKey);

            byte[] cipherbytes = rsa.SignData(plainbytes, new SHA1CryptoServiceProvider());
            return cipherbytes;
        }

        /// <summary>
        /// Verifies the data.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="signature">The signature.</param>
        /// <param name="modulus">The modulus.</param>
        /// <param name="exponent">The exponent.</param>
        /// <remarks>Documented by Dev05, 2009-02-11</remarks>
        public void VerifyData(byte[] data, byte[] signature, byte[] modulus, byte[] exponent)
        {
            try
            {
                if (data == (byte[])null) throw new ArgumentNullException("data");
                if (signature == (byte[])null) throw new ArgumentNullException("signature");
                if (modulus == (byte[])null) throw new ArgumentNullException("modulus");
                if (exponent == (byte[])null) throw new ArgumentNullException("exponent");

                RSAParameters rsaPramas = new RSAParameters();
                rsaPramas.Modulus = modulus;
                rsaPramas.Exponent = exponent;
                rsa.ImportParameters(rsaPramas);

                if (!rsa.VerifyData(data, new SHA1CryptoServiceProvider(), signature))
                    throw new SignatureInvalidException();
            }
            catch { throw new SignatureInvalidException(); }
        }
    }
}
