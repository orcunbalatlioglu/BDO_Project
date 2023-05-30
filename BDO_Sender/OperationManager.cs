using BDO_Encrypted_Data;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Text;


namespace BDO_Sender
{
    internal class OperationManager
    {
        Sender sender;
        SslClientAuthenticationOptions sslOptions;
        TcpClient senderClient;
        SslStream sslStream;
        string aesKey;
        string aesIV;
        byte[] senderKey;
        byte[] senderIV;
        SenderEncryptor senderEncryptor;
        string inputText;
        byte[] plainData;

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
                    return TcpClientConfiguration();
                case State.S3:
                    return TlsConnection();
                case State.S4:
                    return AesKeyInput();
                case State.S5:
                    return AesIVInput();
                case State.S6:
                    return AesConfiguration();
                case State.S7:
                    return MessageInput();
                case State.S8:
                    return EncryptMessage();
                case State.S9:
                    return SendMessage();
                case State.S10:
                    return AnotherMessageInput();
                case State.S11:
                    return EndConnection();
                default:
                    return false;
            }
        }
        private bool IpInput()
        {
            try
            {
                if(senderClient != null)
                {
                    senderClient.Close();
                }
                Console.WriteLine("Please enter target ip address: ");
                string ipAddress = Console.ReadLine();
                Console.WriteLine("Please enter target port: ");
                int port = Convert.ToInt32(Console.ReadLine());
                sender = new Sender(ipAddress, port);
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
        }

        private bool TcpClientConfiguration()
        {
            try
            {
                // TLS bağlantısı için gereken ayarlar
                sslOptions = new SslClientAuthenticationOptions();
                sslOptions.RemoteCertificateValidationCallback = ValidateServerCertificate;
                sslOptions.TargetHost = "asdasd";
                // Veri gönderen tarafın bağlantı noktası
                senderClient = new TcpClient(sender.senderIp, sender.senderPort);
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
        }

        private bool TlsConnection()
        {
            try
            {
                // TLS bağlantısı oluşturulur
                sslStream = new SslStream(senderClient.GetStream(), false, ValidateServerCertificate);
                sslStream.AuthenticateAsClient(sslOptions.TargetHost, null, System.Security.Authentication.SslProtocols.Tls12, false);
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
                senderKey = Encoding.UTF8.GetBytes(aesKey);
                senderIV = Encoding.UTF8.GetBytes(aesIV);
                // Veri gönderen tarafın şifreleme ayarları
                senderEncryptor = new SenderEncryptor(senderKey, senderIV);
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
        }
        private bool MessageInput()
        {
            Console.WriteLine("Enter the data will be sent: ");
            inputText = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(inputText))
            {
                Console.WriteLine("Input text can't be null, empty or whitespace!");
                return false;
            }
            plainData = Encoding.UTF8.GetBytes(inputText);
            return true;
        }

        private bool EncryptMessage()
        {
            try
            {
                senderEncryptor.Encrypt(plainData);
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
        }

        private bool SendMessage()
        {
            try
            {
                // Şifreli veri, alıcıya gönderilir
                sslStream.Write(senderEncryptor.encryptedData, 0, senderEncryptor.outputLen);
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
                senderClient.Close();
                return false;
            }
            return true;
        }

        private bool EndConnection()
        {
            try
            {
                if (senderClient != null)
                {
                    senderClient.Close(); 
                }
                return true;
            }
            catch(Exception e) {
                Console.WriteLine(e.Message);
                return false;
            }   
        }

        private static bool ValidateServerCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }

    }
}
