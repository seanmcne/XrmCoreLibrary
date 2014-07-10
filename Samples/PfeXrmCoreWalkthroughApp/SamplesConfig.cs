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
