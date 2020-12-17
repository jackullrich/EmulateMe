using System.Security.Cryptography;

namespace EmulateMe.HashCash.HashAlgorithms
{
    public class HashAlgorithmSHA256 : HashCashHashAlgorithm
    {
        private static SHA256 SHA;

        static HashAlgorithmSHA256()
        {
            SHA = SHA256.Create();
        }

        public override byte[] ComputeHash(byte[] buffer)
        {
            return SHA.ComputeHash(buffer);
        }
    }
}
