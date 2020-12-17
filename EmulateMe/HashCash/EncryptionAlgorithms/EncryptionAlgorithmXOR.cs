namespace EmulateMe.HashCash.EncryptionAlgorithms
{
    public class EncryptionAlgorithmXOR : HashCashEncryptionAlgorithm
    {
        public override byte[] Encrypt(byte[] buffer, byte[] key)
        {
            byte[] encryptedBuffer = new byte[buffer.Length];

            for (int i = 0; i < buffer.Length; i++)
            {
                encryptedBuffer[i] = (byte)(buffer[i] ^ key[i % key.Length]);
            }

            return encryptedBuffer;
        }

        public override byte[] Decrypt(byte[] buffer, byte[] key)
        {
            return Encrypt(buffer, key);
        }
    }
}
