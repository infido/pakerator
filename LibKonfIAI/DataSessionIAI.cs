using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibKonfIAI
{
    class DataSessionIAI
    {
        public static bool GetPopertySettingsForAIA()
        {
            if (GetIAIDomainForCurrentSession() != null && GetIAILoginForCurrentSession() != null && GetIAIKeyForCurrentSession() != null)
                return true;
            else
            {
                ConnectionFB.setErrOrLogMsg("ERROR", "Ustawienia konfiguracji połączenia do skepu internetowego IAI wygladają na błędne!");
                return false;
            }
        }
        public static string GetIAIKeyForCurrentSession()
        {
            RegistryKey rejestr = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Infido\\Pakerator");
            if (rejestr != null)
            {
                return GenerateKey((String)rejestr.GetValue("www2"));
            }
            else
            {
                ConnectionFB.setErrOrLogMsg("ERROR", "Bład odczytu parametru klucz połączenia do sklepu IAI w kalsie połącznia.");
                return "";
            }
        }
        public static string GetIAILoginForCurrentSession()
        {
            RegistryKey rejestr = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Infido\\Pakerator");
            if (rejestr != null)
            {
                return (String)rejestr.GetValue("www3");
            }
            else
            {
                ConnectionFB.setErrOrLogMsg("ERROR", "Bład odczytu parametru login połączenia do sklepu IAI w kalsie połącznia.");
                return "";
            }
        }
        public static string GetIAIDomainForCurrentSession()
        {
            RegistryKey rejestr = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Infido\\Pakerator");
            if (rejestr != null)
            {
                return (String)rejestr.GetValue("www1");
            }
            else
            {
                ConnectionFB.setErrOrLogMsg("ERROR", "Bład odczytu parametru adres połączenia do sklepu IAI w kalsie połącznia.");
                return "";
            }
        }
        public static string GenerateKey(string hashedPassword)
        {
            /// <summary>
            /// Generates SHA1 session key string
            /// </summary>
            /// <param name="password">SHA1 hashed password
            /// <returns>SHA1 hash string</returns>

            System.Security.Cryptography.HashAlgorithm hash = System.Security.Cryptography.SHA1.Create();
            string date = System.String.Format("{0:yyyyMMdd}", System.DateTime.Now);
            string strToHash = date + hashedPassword;
            byte[] keyBytes, hashBytes;
            keyBytes = System.Text.Encoding.UTF8.GetBytes(strToHash);
            hashBytes = hash.ComputeHash(keyBytes);
            string hashedString = string.Empty;
            foreach (byte b in hashBytes)
            {
                hashedString += String.Format("{0:x2}", b);
            }
            return hashedString;
        }
        public static string HashPassword(string password)
        {
            /// <summary>
            /// Hashes specified password with SHA1 algorithm
            /// </summary>
            /// <param name="password">User password
            /// <returns>SHA1 hash  string</returns>

            System.Security.Cryptography.HashAlgorithm hash = System.Security.Cryptography.SHA1.Create();
            byte[] keyBytes, hashBytes;
            keyBytes = System.Text.Encoding.UTF8.GetBytes(password);
            hashBytes = hash.ComputeHash(keyBytes);
            string hashedString = string.Empty;
            foreach (byte b in hashBytes)
            {
                hashedString += String.Format("{0:x2}", b);
            }
            return hashedString;
        }
    }
}
