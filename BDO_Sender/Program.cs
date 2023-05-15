using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Modes;
using Org.BouncyCastle.Crypto.Paddings;
using Org.BouncyCastle.Crypto.Parameters;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using BDO_Encrypted_Data;
using BDO_Sender;

namespace IEC62351_3_Encrypted_Data_Transfer
{
    class Program
    {
        static void Main(string[] args)
        {
            // Veri gönderen tarafın IP adresi, port numarası ve sertifikası
            Console.WriteLine("Please enter your sender ip address: ");
            string ipAddress = Console.ReadLine();
            Console.WriteLine("Please enter your sender ip port: ");
            int port = Convert.ToInt32(Console.ReadLine());

            Sender sender = new Sender(ipAddress, port);

            // TLS bağlantısı için gereken ayarlar
            SslClientAuthenticationOptions sslOptions = new SslClientAuthenticationOptions();
            sslOptions.RemoteCertificateValidationCallback = ValidateServerCertificate;
            //Console.Write("Enter the receiver host's name: ");
            sslOptions.TargetHost = "asdasd";

            // Veri gönderen tarafın bağlantı noktası
            TcpClient senderClient = new TcpClient(sender.senderIp, sender.senderPort);

            // TLS bağlantısı oluşturulur
            SslStream sslStream = new SslStream(senderClient.GetStream(), false, ValidateServerCertificate);
            sslStream.AuthenticateAsClient(sslOptions.TargetHost, null, System.Security.Authentication.SslProtocols.Tls12, false);

            // Veri gönderen tarafın AES şifreleme anahtarı
            string aesKey;
            while (true)
            {
                Console.WriteLine("Enter your 16 character aes key: ");
                aesKey = Console.ReadLine();

                if (aesKey == null)
                    Console.WriteLine("Aes key can't be null!");
                else if (aesKey.Length != 16)
                    Console.WriteLine("Aes key must be 16 character!");
                else
                    break;
            }
            byte[] senderKey = Encoding.UTF8.GetBytes(aesKey);

            // Veri gönderen tarafın AES şifreleme iv'si
            string aesIV;
            while (true)
            {
                Console.WriteLine("Enter your 16 character aes IV: ");
                aesIV = Console.ReadLine();

                if (aesIV == null)
                    Console.WriteLine("Aes IV can't be null!");
                else if (aesIV.Length != 16)
                    Console.WriteLine("Aes IV must be 16 character!");
                else
                    break;
            }
            byte[] senderIV = Encoding.UTF8.GetBytes(aesIV);

            // Veri gönderen tarafın şifreleme ayarları
            SenderEncryptor senderEncryptor = new SenderEncryptor(senderKey, senderIV);

            bool isContinue = true;
            while (isContinue) { 
                // Veri gönderen tarafın göndereceği veri
                Console.WriteLine("Enter the data will be sent: ");
                string inputText = Console.ReadLine();
                byte[] plainData = Encoding.UTF8.GetBytes(inputText);

                // Veri gönderen tarafın şifreli verisi
                senderEncryptor.Encrypt(plainData);
                // Şifreli veri, alıcıya gönderilir
                sslStream.Write(senderEncryptor.encryptedData, 0, senderEncryptor.outputLen);

                Console.Write("\n\n\nDo you want to continue? (Y/N): ");
                string inputChar = Console.ReadKey().ToString().ToUpper();
                Console.WriteLine();
                if (inputChar == "N")
                {
                    isContinue = false;
                    senderClient.Close();
                }
            }
        }

        private static bool ValidateServerCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }
    }
}
