using System;
using System.IO;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace EasySave_v2._0.Packages
{
    internal class CryptoManager
    {
        private byte[] key;
        private string[] extensionsToEncrypt;

        public CryptoManager(string key, string configFilePath)
        {
            // Convertir la clé en tableau de bytes
            this.key = System.Text.Encoding.UTF8.GetBytes(key);

            // Lire les extensions à crypter depuis le fichier de configuration
            string configText = File.ReadAllText(configFilePath);
            JObject config = JObject.Parse(configText);
            string extensionsToEncryptStr = config["ExtensionsToEncrypt"].ToString();
            this.extensionsToEncrypt = extensionsToEncryptStr.Split(',').Select(ext => ext.Trim().ToLower()).ToArray();
        }

        public bool ShouldEncryptFile(string filePath)
        {
            string fileExtension = Path.GetExtension(filePath).ToLower();
            return extensionsToEncrypt.Any(ext => ext == fileExtension);
        }

        public void EncryptFile(string sourceFilePath, string targetFilePath)
        {
            // Vérifier si le fichier doit être crypté
            if (!ShouldEncryptFile(sourceFilePath))
            {
                // Le fichier n'est pas dans la liste des extensions à crypter, ne pas crypter
                File.Copy(sourceFilePath, targetFilePath, true);
                return;
            }

            // Crypter le fichier
            using (FileStream sourceStream = new FileStream(sourceFilePath, FileMode.Open, FileAccess.Read))
            using (FileStream targetStream = new FileStream(targetFilePath, FileMode.Create, FileAccess.Write))
            {
                int keyIndex = 0;
                int readByte;

                while ((readByte = sourceStream.ReadByte()) != -1)
                {
                    // XOR chaque byte du fichier avec la clé
                    byte encryptedByte = (byte)(readByte ^ key[keyIndex]);
                    targetStream.WriteByte(encryptedByte);

                    // Avancer dans la clé circulairement
                    keyIndex = (keyIndex + 1) % key.Length;
                }
            }
        }
    }
}
