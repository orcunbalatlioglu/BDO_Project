using System.Net.Security;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Net;

namespace BDO_Receiver
{
    internal class OperationManager
    {
        Receiver receiver;
        TcpListener receiverListener;
        TcpClient receiverClient;
        SslStream receiverSslStream;
        string aesKey;
        string aesIV;
        byte[] receiverKey;
        byte[] receiverIV;
        ReceiverDecryptor receiverDecryptor;
        string inputText;
        byte[] plainData;
        int bytes;
        string plainText;
        byte[] encryptedDataFromSender;
        public OperationManager()
        {

        }
        public bool Operations(State _state)
        {
            switch (_state)
            {
                case State.S0:
                    return true;
                case State.S1:
                    return IpInput();
                case State.S2:
                    return TcpListenerConfiguration();
                case State.S3:
                    return StartListen();
                case State.S4:
                    return TcpClientConfiguration();
                case State.S5:
                    return SslConfiguration();
                case State.S6:
                    return AesKeyInput();
                case State.S7:
                    return AesIVInput();
                case State.S8:
                    return AesConfiguration();
                case State.S9:
                    return TakeMessage();
                case State.S10:
                    return DecryptMessage();
                case State.S11:
                    return AnotherMessageInput();
                case State.S12:
                    return false;
                default:
                    return false;
            }
        }
        private bool IpInput()
        {
            try
            {
                if(receiverClient != null)
                {
                    receiverClient.Close();
                }
                if (receiverListener != null)
                {
                    receiverListener.Stop();
                }
                Console.WriteLine("Please enter receiver ip address: ");
                string ipAddress = Console.ReadLine();
                Console.WriteLine("Please enter receiver port: ");
                int port = Convert.ToInt32(Console.ReadLine());
                receiver = new Receiver(ipAddress, port);
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
        }

        private bool TcpListenerConfiguration()
        {
            try
            {
                receiverListener = new TcpListener(IPAddress.Parse(receiver.receiverIp), receiver.receiverPort);
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
        }
        private bool StartListen()
        {
            try
            {
                receiverListener.Start();
                Console.WriteLine("Receiver is waiting for connection...");
                return true;
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
        }
        private bool TcpClientConfiguration()
        {
            try { 
                receiverClient = receiverListener.AcceptTcpClient();
                return true;
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
        }
        private bool SslConfiguration()
        {
            try
            {
                receiverSslStream = new SslStream(receiverClient.GetStream(), false, ValidateClientCertificate);
                receiverSslStream.AuthenticateAsServer(receiver.receiverCertificate, true, System.Security.Authentication.SslProtocols.Tls12, false);
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
        }

        private bool AesKeyInput()
        {
            Console.WriteLine("Enter your 16 character aes key: ");
            aesKey = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(aesKey))
            {
                Console.WriteLine("Aes key can't be null!");
                return false;
            }
            else if (aesKey.Length != 16)
            {
                Console.WriteLine("Aes key must be 16 character!");
                return false;
            }
            return true;
        }

        private bool AesIVInput()
        {
            Console.WriteLine("Enter your 16 character aes IV: ");
            aesIV = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(aesIV))
            {
                Console.WriteLine("Aes IV can't be null!");
                return false;
            }
            else if (aesIV.Length != 16)
            {
                Console.WriteLine("Aes IV must be 16 character!");
                return false;
            }
            else
                return true;
        }

        private bool AesConfiguration()
        {
            try
            {
                receiverKey = Encoding.UTF8.GetBytes(aesKey);
                receiverIV = Encoding.UTF8.GetBytes(aesIV);
                // Veri gönderen tarafın şifreleme ayarları
                receiverDecryptor = new ReceiverDecryptor(receiverKey, receiverIV);
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
        }

        private bool TakeMessage()
        {
            try { 
                // Veri gönderen tarafın gönderdiği şifreli veri alınır
                encryptedDataFromSender = new byte[1024];
                bytes = receiverSslStream.Read(encryptedDataFromSender, 0, encryptedDataFromSender.Length);
                return true;
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
        }

        private bool DecryptMessage()
        {
            try
            {
                plainText = receiverDecryptor.Decrypt(encryptedDataFromSender, bytes);
                Console.WriteLine($"Message: {plainText}");
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
        }

        private bool AnotherMessageInput()
        {
            Console.Write("\n\n\nDo you want to continue? (Y/N): ");
            char inputChar = Console.ReadKey().KeyChar;
            inputChar = char.ToUpper(inputChar);
            Console.WriteLine();
            if (inputChar == 'N')
            {
                return false;
            }
            return true;
        }

        private static bool ValidateClientCertificate(object sender,
                                                    X509Certificate certificate,
                                                    X509Chain chain,
                                                    SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }

    }
}
