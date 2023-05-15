using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace BDO_Encrypted_Data
{
    public class Sender
    {
        public string senderIp { get; set; }
        public int senderPort { get; set; }
        public X509Certificate2 senderCertificate { get; set; }

        public Sender(): this("192.168.0.1", 1234, new X509Certificate2("sender_certificate.pfx", "password")) { }

        public Sender(string senderIp): this(senderIp, 1234, new X509Certificate2("sender_certificate.pfx", "password")) { }

        public Sender(int senderPort): this("192.168.0.1", senderPort, new X509Certificate2("sender_certificate.pfx", "password")) { }
        
        public Sender(string[] certificateOptions): this("192.168.0.1", 1234, new X509Certificate2(certificateOptions[0], certificateOptions[1])) { }
        
        public Sender(X509Certificate2 senderCertificate): this("192.168.0.1", 1234, senderCertificate) { }

        public Sender(string senderIp, int senderPort): this(senderIp, senderPort, new X509Certificate2("sender_certificate.pfx", "password")) { }
        
        public Sender(string senderIp, int senderPort, string[] certificateOptions): this(senderIp,senderPort, new X509Certificate2(certificateOptions[0], certificateOptions[1])) { }
        
        public Sender(string senderIp, int senderPort, X509Certificate2 senderCertificate)
        {
            this.senderIp = senderIp;
            this.senderPort = senderPort;
            this.senderCertificate = senderCertificate;
        }


        
    }
}
