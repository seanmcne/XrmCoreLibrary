/*================================================================================================================================

  This Sample Code is provided for the purpose of illustration only and is not intended to be used in a production environment.  

  THIS SAMPLE CODE AND ANY RELATED INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESSED OR IMPLIED, 
  INCLUDING BUT NOT LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.  

  We grant You a nonexclusive, royalty-free right to use and modify the Sample Code and to reproduce and distribute the object 
  code form of the Sample Code, provided that You agree: (i) to not use Our name, logo, or trademarks to market Your software 
  product in which the Sample Code is embedded; (ii) to include a valid copyright notice on Your software product in which the 
  Sample Code is embedded; and (iii) to indemnify, hold harmless, and defend Us and Our suppliers from and against any claims 
  or lawsuits, including attorneys’ fees, that arise or result from the use or distribution of the Sample Code.

 =================================================================================================================================*/
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security;
using System.Text;

namespace Microsoft.Pfe.Xrm.Samples
{
    public static class SamplesConfig
    {
        public static string CrmUsername { get { return ConfigurationManager.AppSettings.Get("Crm.Username"); } } 
        public static string CrmEncryptedPassword
        {
            get
            {
                if (!String.IsNullOrEmpty(ConfigurationManager.AppSettings.Get("Crm.Password")))
                {
                    SamplesConfig.EncryptPassword();
                }

                return ConfigurationManager.AppSettings.Get("Crm.EncryptedPassword");
            }
        }
        public static string CrmDiscoveryHost { get { return ConfigurationManager.AppSettings.Get("Crm.DiscoveryHost"); } }
        public static string CrmOrganizationHost { get { return ConfigurationManager.AppSettings.Get("Crm.OrganizationHost"); } }
        public static string CrmOrganization { get { return ConfigurationManager.AppSettings.Get("Crm.Organization"); } }
        public static bool CrmShouldDiscover { get { return Boolean.Parse(ConfigurationManager.AppSettings.Get("Crm.ShouldDiscover")); } }

        #region Cryptography Methods

        /// <summary>
        /// Decryptes the encrypted password and returns as securestring
        /// </summary>
        public static SecureString GetCrmDecryptedPassword()
        {
            if (!String.IsNullOrEmpty(SamplesConfig.CrmEncryptedPassword))
            {
                return SamplesConfig.CrmEncryptedPassword.ToDecryptedSecureString();
            }

            return null;
        }

        /// <summary>
        /// Handles encrypting plain-text config password and updating the app.config
        /// </summary>
        private static void EncryptPassword()
        {
            var encryptedPw = String.Empty;
            var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

            using (var pw = ConfigurationManager.AppSettings.Get("Crm.Password").ToSecureString())
            {
                encryptedPw = pw.ToEncryptedString();
            }

            config.AppSettings.Settings["Crm.Password"].Value = String.Empty;

            if (config.AppSettings.Settings["Crm.EncryptedPassword"] != null)
            {                
                config.AppSettings.Settings["Crm.EncryptedPassword"].Value = encryptedPw;
            }
            else
            {
                config.AppSettings.Settings.Add("Crm.EncryptedPassword", encryptedPw);
            }           

            config.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection("appSettings");
        } 

        #endregion
    }
}
