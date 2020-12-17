using EmulateMe.HashCash.EncryptionAlgorithms;
using EmulateMe.HashCash.HashAlgorithms;
using System;

namespace EmulateMe.HashCash
{
    public sealed class HashCashWorker
    {
        private int difficulty;
        private byte[] nonce;
        private int seed;
        private Random rng;
        private byte[] solution;

        private HashCashHashAlgorithm hashAlgorithm;
        private HashCashEncryptionAlgorithm encryptionAlgorithm;

        private HashCashWorker(int seed, int difficulty)
        {
            this.seed = seed;
            this.difficulty = difficulty;
            this.encryptionAlgorithm = new EncryptionAlgorithmXOR();
            this.hashAlgorithm = new HashAlgorithmSHA256();
        }

        private HashCashWorker(int seed, int difficulty, HashCashEncryptionAlgorithm encryptionAlgorithm)
        {
            this.seed = seed;
            this.difficulty = difficulty;
            this.encryptionAlgorithm = encryptionAlgorithm;
            this.hashAlgorithm = new HashAlgorithmSHA256();
        }

        private HashCashWorker(int seed, int difficulty, HashCashHashAlgorithm hashAlgorithm)
        {
            this.seed = seed;
            this.difficulty = difficulty;
            this.encryptionAlgorithm = new EncryptionAlgorithmXOR();
            this.hashAlgorithm = hashAlgorithm;
        }

        private HashCashWorker(int seed, int difficulty, HashCashEncryptionAlgorithm encryptionAlgorithm, HashCashHashAlgorithm hashAlgorithm)
        {
            this.seed = seed;
            this.difficulty = difficulty;
            this.encryptionAlgorithm = encryptionAlgorithm;
            this.hashAlgorithm = hashAlgorithm;
        }

        public static HashCashWorker Create(int seed, int difficulty)
        {
            return new HashCashWorker(seed, difficulty);
        }

        public static HashCashWorker Create(int seed, int difficulty, HashCashEncryptionAlgorithm encryptionAlgorithm)
        {
            return new HashCashWorker(seed, difficulty, encryptionAlgorithm);
        }

        public static HashCashWorker Create(int seed, int difficulty, HashCashHashAlgorithm hashAlgorithm)
        {
            return new HashCashWorker(seed, difficulty, hashAlgorithm);
        }

        public static HashCashWorker Create(int seed, int difficulty, HashCashEncryptionAlgorithm encryptionAlgorithm, HashCashHashAlgorithm hashAlgorithm)
        {
            return new HashCashWorker(seed, difficulty, encryptionAlgorithm, hashAlgorithm);
        }

        public byte[] EncryptWithSolution(byte[] buffer)
        {
            if (solution == null)
                throw new NullReferenceException();

            return encryptionAlgorithm.Encrypt(buffer, hashAlgorithm.ComputeHash(solution));
        }

        public byte[] DecryptWithSolution(byte[] buffer)
        {
            if (solution == null)
                throw new NullReferenceException();

            return encryptionAlgorithm.Decrypt(buffer, hashAlgorithm.ComputeHash(solution));
        }

        public byte[] Solve(byte[] challenge)
        {
            if (challenge == null)
                throw new NullReferenceException();

            // big oof
            rng = new Random(seed);
            GenNonce();

            byte[] solve = new byte[challenge.Length + nonce.Length];
            byte[] hash;

            do
            {
                GenNonce();

                Buffer.BlockCopy(challenge, 0, solve, 0, challenge.Length);
                Buffer.BlockCopy(nonce, 0, solve, challenge.Length, nonce.Length);

                hash = hashAlgorithm.ComputeHash(solve);

            } while (CountBitString(hash) != difficulty);

            solution = solve;
            return solution;
        }

        private void GenNonce()
        {
            byte[] buffer = new byte[16];
            rng.NextBytes(buffer);
            nonce = buffer;
        }

        private int CountBitString(byte[] buffer)
        {
            int i = 0;

            while (buffer[i] == 0)
                i++;

            return i;
        }
    }
}