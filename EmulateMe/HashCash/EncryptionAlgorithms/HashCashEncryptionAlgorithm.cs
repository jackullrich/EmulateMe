namespace EmulateMe.HashCash.EncryptionAlgorithms
{
    public abstract class HashCashEncryptionAlgorithm
    {
        public abstract byte[] Encrypt(byte[] buffer, byte[] key);
        public abstract byte[] Decrypt(byte[] buffer, byte[] key);

    }
}
