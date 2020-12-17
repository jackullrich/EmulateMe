namespace EmulateMe.HashCash.HashAlgorithms
{
    public abstract class HashCashHashAlgorithm
    {
        public abstract byte[] ComputeHash(byte[] buffer);
    }
}
