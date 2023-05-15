using System.Net.Security;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Security.Cryptography.X509Certificates;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Modes;
using Org.BouncyCastle.Crypto.Paddings;
using Org.BouncyCastle.Crypto.Parameters;
using System.Transactions;
using System.Globalization;
using Org.BouncyCastle.Bcpg;

namespace BDO_Receiver
{
    class ReceiverProgram
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Please enter your receiver ip address: ");
            string ipAddress = Console.ReadLine();
            Console.WriteLine("Please enter your receiver ip port: ");
            int port = Convert.ToInt32(Console.ReadLine());
            Receiver receiver = new Receiver(ipAddress, port);
            TcpListener receiverListener = new TcpListener(IPAddress.Parse(receiver.receiverIp), receiver.receiverPort);

            receiverListener.Start();
            
            Console.WriteLine("Receiver is waiting for connection...");

            TcpClient receiverClient = receiverListener.AcceptTcpClient();
            SslStream receiverSslStream = new SslStream(receiverClient.GetStream(), false, ValidateClientCertificate);
            receiverSslStream.AuthenticateAsServer(receiver.receiverCertificate, true, System.Security.Authentication.SslProtocols.Tls12, false);

            // Alıcı tarafın AES şifreleme anahtarı
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
            byte[] receiverKey = Encoding.UTF8.GetBytes(aesKey);

            // Alıcı tarafın AES şifreleme iv'si
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
            byte[] receiverIV = Encoding.UTF8.GetBytes(aesIV);

            // Alıcı tarafın şifre çözme ayarları
            ReceiverDecryptor receiverDecryptor = new ReceiverDecryptor(receiverKey, receiverIV);

            bool isContinue = true;    
            while (isContinue)
            {
                // Veri gönderen tarafın gönderdiği şifreli veri alınır
                byte[] encryptedDataFromSender = new byte[1024];
                int bytes = receiverSslStream.Read(encryptedDataFromSender, 0, encryptedDataFromSender.Length);

                // Şifreli veri çözülür ve orijinal veri elde edilir
                string plainText = receiverDecryptor.Decrypt(encryptedDataFromSender, bytes);

                Console.WriteLine("Alıcı taraf: {0}", plainText);

                Console.Write("\n\n\nDo you want to continue? (Y/N): ");
                string inputChar = Console.ReadKey().ToString().ToUpper();
                Console.WriteLine();
                if (inputChar == "N")
                {
                    isContinue = false;
                    receiverClient.Close();
                    receiverListener.Stop();
                }
            }
            
        }

        private static bool ValidateClientCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }
    }
}