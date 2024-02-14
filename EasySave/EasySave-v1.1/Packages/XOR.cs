using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http.Json;
using System.Security.Cryptography;
using System.Text.Json;
using System.Text.Json.Serialization;

public class Cryptosoft
{
    private static readonly byte[] _key = GetKey();

    public static void Main(string[] args)
    {
        if (args.Length != 2)
        {
            Console.WriteLine("Usage: dotnet run fichier_source fichier_destination");
            return;
        }

        string sourceFile = args[0];
        string destinationFile = args[1];

        try
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            EncryptFile(sourceFile, destinationFile);
            stopwatch.Stop();

            Console.WriteLine($"Temps de cryptage: {stopwatch.ElapsedMilliseconds} ms");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erreur: {ex.Message}");
            return;
        }
    }

    private static void EncryptFile(string sourceFile, string destinationFile)
    {
        using (FileStream sourceStream = new FileStream(sourceFile, FileMode.Open, FileAccess.Read))
        using (FileStream destinationStream = new FileStream(destinationFile, FileMode.Create, FileAccess.Write))
        {
            byte[] buffer = new byte[4096];
            int bytesRead;

            while ((bytesRead = sourceStream.Read(buffer, 0, buffer.Length)) > 0)
            {
                for (int i = 0; i < bytesRead; i++)
                {
                    buffer[i] ^= _key[i % _key.Length];
                }

                destinationStream.Write(buffer, 0, bytesRead);
            }
        }
    }

    private static byte[] GetKey()
    {
        string configFile = "config.json";

        if (File.Exists(configFile))
        {
            try
            {
                string key = "";

                using (StreamReader reader = new StreamReader(configFile))
                {
                    string json = reader.ReadToEnd();
                    // Use System.Text.Json's JsonSerializer
                    Dictionary<string, string> config = JsonSerializer.Deserialize<Dictionary<string, string>>(json);

                    key = config["key"];
                }

                if (!string.IsNullOrEmpty(key))
                {
                    return Convert.FromBase64String(key);
                }
            }
            catch (Exception)
            {
                Console.WriteLine("Erreur lors de la lecture de la clé de chiffrement depuis le fichier de configuration");
            }
        }

        Console.WriteLine("Veuillez entrer la clé de chiffrement (64 bits minimum) : ");
        string userKey = Console.ReadLine();

        return Convert.FromBase64String(userKey);
    }
}