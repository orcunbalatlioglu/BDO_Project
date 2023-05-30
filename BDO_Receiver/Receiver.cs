using System.Security.Cryptography.X509Certificates;

namespace BDO_Receiver
{
    public class Receiver
    {
        public string receiverIp { get; set; }
        public int receiverPort { get; set; }
        public X509Certificate2 receiverCertificate { get; set; }

        public Receiver() : this("192.168.0.1", 1234, new X509Certificate2("receiver_certificate.pfx", "password")) { }

        public Receiver(string receiverIp) : this(receiverIp, 1234, new X509Certificate2("receiver_certificate.pfx", "password")) { }

        public Receiver(int receiverPort) : this("192.168.0.1", receiverPort, new X509Certificate2("receiver_certificate.pfx", "password")) { }

        public Receiver(string[] certificateOptions) : this("192.168.0.1", 1234, new X509Certificate2(certificateOptions[0], certificateOptions[1])) { }

        public Receiver(X509Certificate2 receiverCertificate) : this("192.168.0.1", 1234, receiverCertificate) { }

        public Receiver(string receiverIp, int receiverPort) : this(receiverIp, receiverPort, new X509Certificate2("receiver_certificate.pfx", "password")) { }

        public Receiver(string receiverIp, int receiverPort, string[] certificateOptions) : this(receiverIp, receiverPort, 
                                                                                            new X509Certificate2(certificateOptions[0], certificateOptions[1])) { }

        public Receiver(string receiverIp, int receiverPort, X509Certificate2 receiverCertificate)
        {
            if (ValidateIPv4(receiverIp))
                this.receiverIp = receiverIp;
            else
                throw new Exception("Entered IP address is not in formal format!");
            this.receiverPort = receiverPort;
            this.receiverCertificate = receiverCertificate;
        }

        public bool ValidateIPv4(string ipString)
        {
            if (String.IsNullOrWhiteSpace(ipString))
            {
                return false;
            }

            string[] splitValues = ipString.Split('.');
            if (splitValues.Length != 4)
            {
                return false;
            }

            byte tempForParsing;

            return splitValues.All(r => byte.TryParse(r, out tempForParsing));
        }
    }
}
