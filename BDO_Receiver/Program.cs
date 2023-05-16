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
using System.Net.NetworkInformation;

namespace BDO_Receiver
{
    class ReceiverProgram
    {
        static void Main(string[] args)
        {
            string ipAddress = GetLocalIPAddress();
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

                Console.WriteLine($"Message: {plainText}");

                Console.Write("\n\n\nDo you want to continue? (Y/N): ");
                char inputChar = Console.ReadKey().KeyChar;
                inputChar = char.ToUpper(inputChar);
                if (inputChar == 'N')
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

        private static string GetLocalIPAddress()
        {
            string ipAddress = "";
            NetworkInterface[] networkInterfaces = NetworkInterface.GetAllNetworkInterfaces();

            foreach (NetworkInterface networkInterface in networkInterfaces)
            {
                if (networkInterface.NetworkInterfaceType == NetworkInterfaceType.Ethernet ||
                    networkInterface.NetworkInterfaceType == NetworkInterfaceType.Wireless80211)
                {
                    IPInterfaceProperties properties = networkInterface.GetIPProperties();
                    foreach (UnicastIPAddressInformation ipAddressInfo in properties.UnicastAddresses)
                    {
                        if (ipAddressInfo.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                        {
                            ipAddress = ipAddressInfo.Address.ToString();
                            break;
                        }
                    }
                }
                if (!string.IsNullOrEmpty(ipAddress))
                    break;
            }
            return ipAddress;
        }
    }
}