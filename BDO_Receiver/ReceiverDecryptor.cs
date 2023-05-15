using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Modes;
using Org.BouncyCastle.Crypto.Paddings;
using Org.BouncyCastle.Crypto.Parameters;
using System.Text;

namespace BDO_Receiver
{
    public class ReceiverDecryptor
    {
        AesEngine receiverAesEngine;
        CbcBlockCipher receiverAesCipher;
        PaddedBufferedBlockCipher receiverAesPaddedCipher;
        KeyParameter receiverAesKeyParam;
        ParametersWithIV receiverAesKeyParamWithIV;

        public ReceiverDecryptor(byte[] receiverKey, byte[] receiverIV)
        {
            receiverAesEngine = new AesEngine();
            receiverAesCipher = new CbcBlockCipher(receiverAesEngine);
            receiverAesPaddedCipher = new PaddedBufferedBlockCipher(receiverAesCipher);
            receiverAesKeyParam = new KeyParameter(receiverKey);
            receiverAesKeyParamWithIV = new ParametersWithIV(receiverAesKeyParam, receiverIV, 0, 16);
            receiverAesPaddedCipher.Init(false, receiverAesKeyParamWithIV);
        }

        public string Decrypt(byte[] encryptedDataFromSender, int bytes)
        {
            byte[] decryptedData = new byte[receiverAesPaddedCipher.GetOutputSize(bytes)];
            int decryptedBytes = receiverAesPaddedCipher.ProcessBytes(encryptedDataFromSender, 0, bytes, decryptedData, 0);
            decryptedBytes += receiverAesPaddedCipher.DoFinal(decryptedData, decryptedBytes);
            string plainText = Encoding.UTF8.GetString(decryptedData, 0, decryptedBytes);

            return plainText;
        }
    }
}
