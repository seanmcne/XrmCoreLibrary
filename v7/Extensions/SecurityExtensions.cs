using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Cryptography;
using System.Text;


namespace Microsoft.Pfe.Xrm
{
    public static class SecurityExtensions
    {
        private static byte[] additionalEntropy = Encoding.Unicode.GetBytes("EE5C089F-138A-4833-8CF8-6923D9924A17");

        /// <summary>
        /// Converts a SecureString object to a plain-text string value
        /// </summary>
        /// <param name="value">The SecureString value</param>
        /// <returns>A plain-text string version of the SecureString value</returns>
        /// <remarks>
        /// Allocs unmanaged memory in process of converting to string. 
        /// Calls Marshal.ZeroFreeGlobalAllocUnicode to free unmanaged memory space for the ptr struct in finally { }
        /// </remarks>
        public static string ToUnsecureString(this SecureString value)
        {
            if (value == null)
                throw new ArgumentNullException("value", "Cannot convert null value.ToUnsecureString()");

            var valuePtr = IntPtr.Zero;

            try
            {
                valuePtr = Marshal.SecureStringToGlobalAllocUnicode(value);

                return Marshal.PtrToStringUni(valuePtr);
            }
            finally
            {
                Marshal.ZeroFreeGlobalAllocUnicode(valuePtr);
            }
        }

        /// <summary>
        /// Encrypts a SecureString value and returns it as a base64 string
        /// </summary>
        /// <param name="value">The SecureString value</param>
        /// <returns>An encrypted string value</returns>
        /// <remarks>
        /// Leverages DPAPI and assumes CurrentUser data protection scope
        /// Assumes that SecureString should not be disposed
        /// </remarks>
        /// <exception cref="CryptographicException">
        /// This method calls ProtectedData.Protect(). Callers of this method should handle potential cryptographic exceptions.
        /// </exception>
        public static string ToEncryptedString(this SecureString value)
        {
            if (value == null)
                throw new ArgumentNullException("value", "Cannot encrypt a null SecureString value");

            var encryptedValue = ProtectedData.Protect(Encoding.Unicode.GetBytes(value.ToUnsecureString()), SecurityExtensions.additionalEntropy, DataProtectionScope.CurrentUser);

            return Convert.ToBase64String(encryptedValue);
        }

        /// <summary>
        /// Converts a plain-text string value to a SecureString object
        /// </summary>
        /// <param name="value">The string value</param>
        /// <returns>A SecureString representation of the string value</returns>
        public static SecureString ToSecureString(this string value)
        {
            if (String.IsNullOrEmpty(value))
                throw new ArgumentNullException("value", "Cannot convert null value.ToSecureString()");
            
            var secureValue = new SecureString();

            value.ToCharArray()
                .ToList()
                .ForEach(c =>
                {
                    secureValue.AppendChar(c);
                });

            secureValue.MakeReadOnly();

            return secureValue;
        }

        /// <summary>
        /// Decrypts an encrypted string value and returns it as a SecureString
        /// </summary>
        /// <param name="value">The base64 encoded encrypted string value</param>
        /// <returns>The decrypted string value wrapped in a SecureString</returns>
        /// <remarks>
        /// Leverages DPAPI and assumes CurrentUser data protection scope
        /// </remarks>
        /// <exception cref="CryptographicException">
        /// This method calls ProtectedData.Unprotect(). Callers of this method should handle potential cryptographic exceptions.
        /// </exception>
        public static SecureString ToDecryptedSecureString(this string value)
        {
            if (String.IsNullOrEmpty(value))
                throw new ArgumentNullException("value", "Cannot decrypt a null (or empty) String value");
            
                var decryptedValue = ProtectedData.Unprotect(Convert.FromBase64String(value), SecurityExtensions.additionalEntropy, DataProtectionScope.CurrentUser);

                return Encoding.Unicode.GetString(decryptedValue).ToSecureString();
            }
    }
}
