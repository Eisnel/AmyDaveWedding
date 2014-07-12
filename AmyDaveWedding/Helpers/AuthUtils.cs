using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;
using EisnelShared;
using System.Web.Caching;
using System.Configuration;
using System.Threading.Tasks;
using AmyDaveWedding.Helpers;
using AmyDaveWedding.Models;
using System.Text;

namespace AmyDaveWedding.Helpers
{
    public static class ApiCredentialSource
    {
        public static ApiCredential FacebookCredentials
        {
            get
            {
                return GetApiCredentials("ApiCredentialSource.FacebookCredentials", "Facebook.AppId", "Facebook.AppIdEncrypted", "Facebook.AppSecret", "Facebook.AppSecretEncrypted");
            }
        }

        public static ApiCredential TwitterCredentials
        {
            get
            {
                return GetApiCredentials("ApiCredentialSource.TwitterCredentials", "Twitter.ConsumerKey", "Twitter.ConsumerKeyEncrypted", "Twitter.ConsumerSecret", "Twitter.ConsumerSecretEncrypted");
            }
        }

        private static ApiCredential GetApiCredentials(string cacheKey, string keyUnencryptedName, string keyEncryptedName, string secretUnencryptedName, string secretEncryptedName)
        {
            return (ApiCredential)CacheUtils.GetHttpRuntimeCached(cacheKey,
                () => MakeApiCredentials(keyUnencryptedName, keyEncryptedName, secretUnencryptedName, secretEncryptedName)
            );
        }

        private static ApiCredential MakeApiCredentials(string keyUnencryptedName, string keyEncryptedName, string secretUnencryptedName, string secretEncryptedName)
        {
            string key = ConfigurationManager.AppSettings[keyUnencryptedName];
            if (string.IsNullOrWhiteSpace(key))
            {
                string keyEncrypted = ConfigurationManager.AppSettings[keyEncryptedName];
                if (!string.IsNullOrWhiteSpace(keyEncrypted))
                {
                    key = Decrypt(keyEncrypted);
                }
                if (string.IsNullOrWhiteSpace(key))
                {
                    return null;
                }
            }

            string secret = ConfigurationManager.AppSettings[secretUnencryptedName];
            if (string.IsNullOrWhiteSpace(secret))
            {
                string secretEncrypted = ConfigurationManager.AppSettings[secretEncryptedName];
                if (!string.IsNullOrWhiteSpace(secretEncrypted))
                {
                    secret = Decrypt(secretEncrypted);
                }
                if (string.IsNullOrWhiteSpace(secret))
                {
                    return null;
                }
            }

            return new ApiCredential(key, secret);
        }

        private static string Decrypt(string cipherText)
        {
            string cryptoType = ConfigurationManager.AppSettings["Crypo.Type"];
            if (string.Equals(cryptoType, "DPAPI", StringComparison.OrdinalIgnoreCase))
            {
                return cipherText.DecryptDpApi(Encoding.UTF8);
            }
            else
            {
                string keyHex = GetAesKey();
                if (string.IsNullOrEmpty(keyHex))
                {
                    throw new ConfigurationErrorsException("ApiCredentialSource.Decrypt: 'Crypto.AesKey' config contains no value.");
                }

                return cipherText.DecryptAes(keyHex, Encoding.UTF8);
            }
        }

        public static string GetAesKey()
        {
            return ConfigurationManager.AppSettings["Crypto.AesKey"];
        }

        //private static string GetAppSetting(string unencryptedName, string encryptedName)
        //{
        //    var value = ConfigurationManager.AppSettings[unencryptedName];
        //    if (string.IsNullOrWhiteSpace(value))
        //    {
        //        value = GetEncryptedAppSettingCached(encryptedName);
        //        //value = ConfigurationManager.AppSettings[encryptedName];
        //        //if( !string.IsNullOrWhiteSpace(value) )
        //        //{
        //        //    value = value.Decrypt();
        //        //}
        //    }
        //    return value;
        //}

        //private static string GetEncryptedAppSettingCached(string name)
        //{
        //    return (string)CacheUtils.GetHttpRuntimeCached(name, () =>
        //    {
        //        string value = ConfigurationManager.AppSettings[name];
        //        if (!string.IsNullOrWhiteSpace(value))
        //        {
        //            value = value.Decrypt();
        //        }
        //        return value;
        //    });
        //}
    }
}