using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Modes;
using Org.BouncyCastle.Crypto.Paddings;
using Org.BouncyCastle.Crypto.Parameters;

namespace BDO_Sender
{
    public class SenderEncryptor
    {
        AesEngine aesEngine;
        CbcBlockCipher aesCipher;
        PaddedBufferedBlockCipher aesPaddedCipher;
        KeyParameter aesKeyParam;
        ParametersWithIV aesKeyParamWithIV;
        public byte[] encryptedData;
        public int outputLen;

        public SenderEncryptor(byte[] senderKey, byte[] senderIV)
        {
            aesEngine = new AesEngine();
            aesCipher = new CbcBlockCipher(aesEngine);
            aesPaddedCipher = new PaddedBufferedBlockCipher(aesCipher);
            aesKeyParam = new KeyParameter(senderKey);
            aesKeyParamWithIV = new ParametersWithIV(aesKeyParam, senderIV, 0, 16);
            aesPaddedCipher.Init(true, aesKeyParamWithIV);
        }

        public void Encrypt(byte[] plainData)
        {
            encryptedData = new byte[aesPaddedCipher.GetOutputSize(plainData.Length)];
            outputLen = aesPaddedCipher.ProcessBytes(plainData, 0, plainData.Length, encryptedData, 0);
            outputLen += aesPaddedCipher.DoFinal(encryptedData, outputLen);
        }
    }
}
