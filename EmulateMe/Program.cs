using EmulateMe.ControlFlow;
using EmulateMe.HashCash;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Runtime.InteropServices;

using static EmulateMe.Utils;
using System.Reflection;

namespace EmulateMe
{
    class Program
    {
        static void Main(string[] args)
        {
            var RNG = new Random();
            int seed = RNG.Next();
            int diff = 2;
            var hashcash = HashCashWorker.Create(seed, diff);
            var whatever = new byte[128];
            RNG.NextBytes(whatever);

            var xor = new EmulateMe.HashCash.EncryptionAlgorithms.EncryptionAlgorithmXOR();
            var buffer_to_gen = whatever;

            var controlFlowSwitches = GetRandomValuesDistinct(buffer_to_gen.Length);
            var switchTerminater = NextUInt32();

            int lengthCounter = 0;

            if (controlFlowSwitches.Contains(switchTerminater))
            {
                throw new Exception("Restart the program. Unlucky.");
            }

            Stopwatch sw = new Stopwatch();
            sw.Start();

            var rdr = new StateReader(controlFlowSwitches);

            var codeGen = new StringBuilder();
            var listOfBlocks = new List<StringBuilder>();

            codeGen.AppendLine("public static byte[] GenBuffer()");
            codeGen.AppendLine("{");
            codeGen.AppendLine("List<byte> buffer = new List<byte>();");

            // <!-- Change this constructor accordingly ---!>
            // Encryption Default: XOR
            // Hash Default: SHA256
            codeGen.AppendLine($"var HashCash = HashCashWorker.Create({seed},{diff});");

            codeGen.AppendLine($"uint state = 0x{rdr.First:X4};");
            codeGen.AppendLine();
            codeGen.AppendLine($"while(state != 0x{switchTerminater:X4})");
            codeGen.AppendLine("{");
            codeGen.AppendLine("switch(state)");
            codeGen.AppendLine("{");

            using (var progress = new ProgressBar())
            {
                // Base Case
                {
                    var initBlock = new StringBuilder();
                    initBlock.AppendLine($"case 0x{rdr.First:X4}:");

                    // This is the solution to the proof-of-work challenge
                    // It will begin with whatever bitstring chosen 
                    // In this case difficulty = 1 and bitstring is simply 0s
                    var solution = hashcash.Solve(GetBytes(rdr.CurrentValue));

                    // Here, we are encrypting the next state with the hashed solution
                    // Thus, we can only decrypt by solving the proof-of-work problem again
                    // Or by bruteforcing
                    var encryptedNextState = hashcash.EncryptWithSolution(GetBytes(rdr.NextValue));

                    // Build our buffer at runtime, byte by byte
                    // You may also choose to encrypt this value in the same way
                    // We are encrypting the state variables
                    initBlock.AppendLine($"buffer.Add(0x{buffer_to_gen[lengthCounter++]:X2});");

                    // Let's repeat this same process (for finding the decryption/encryption key) in the source code
                    initBlock.AppendLine($"HashCash.Solve({BytesToSourceString(GetBytes(rdr.CurrentValue))});");
                    initBlock.AppendLine($"state = BitConverter.ToUInt32(HashCash.DecryptWithSolution({BytesToSourceString(encryptedNextState)}), 0);");
                    initBlock.AppendLine("break;");

                    listOfBlocks.Add(initBlock);
                    rdr.NextState();

                    progress.Report((double)lengthCounter / buffer_to_gen.Length);
                }

                // N-1 Case
                {
                    while (!rdr.IsAtEnd)
                    {
                        var nState = new StringBuilder();

                        var solution = hashcash.Solve(GetBytes(rdr.CurrentValue));
                        var encryptedNextState = hashcash.EncryptWithSolution(GetBytes(rdr.NextValue));

                        nState.AppendLine($"case 0x{rdr.CurrentValue:X4}:");
                        nState.AppendLine($"buffer.Add(0x{buffer_to_gen[lengthCounter++]:X2});");
                        nState.AppendLine($"HashCash.Solve({BytesToSourceString(GetBytes(rdr.CurrentValue))});");
                        nState.AppendLine($"state = BitConverter.ToUInt32(HashCash.DecryptWithSolution({BytesToSourceString(encryptedNextState)}), 0);");
                        nState.AppendLine("break;");

                        listOfBlocks.Add(nState);
                        rdr.NextState();

                        progress.Report((double)lengthCounter / buffer_to_gen.Length);
                    }
                }

                // Nth Case
                {
                    var finalCase = new StringBuilder();

                    if (rdr.CurrentValue != rdr.Last)
                        throw new Exception("Not last state.");

                    var solution = hashcash.Solve(GetBytes(rdr.CurrentValue));
                    var encryptedNextState = hashcash.EncryptWithSolution(GetBytes(switchTerminater));

                    finalCase.AppendLine($"case 0x{rdr.CurrentValue:X4}:");
                    finalCase.AppendLine($"buffer.Add(0x{buffer_to_gen[lengthCounter++]:X2});");
                    finalCase.AppendLine($"HashCash.Solve({BytesToSourceString(GetBytes(rdr.CurrentValue))});");
                    finalCase.AppendLine($"state = BitConverter.ToUInt32(HashCash.DecryptWithSolution({BytesToSourceString(encryptedNextState)}), 0);");
                    finalCase.AppendLine("break;");

                    listOfBlocks.Add(finalCase);

                    progress.Report((double)lengthCounter / buffer_to_gen.Length);
                }
            }

            // Shuffle the ordering
            listOfBlocks = listOfBlocks.OrderBy(R => RNG.Next()).ToList();

            foreach (var block in listOfBlocks)
            {
                codeGen.Append(block.ToString());
            }

            codeGen.AppendLine("}");
            codeGen.AppendLine("}");
            codeGen.AppendLine("return buffer.ToArray();");
            codeGen.AppendLine("}");

            string resultantCode = codeGen.ToString();
            File.WriteAllText("out.cs1", resultantCode);

            sw.Stop();
            long sec = sw.ElapsedMilliseconds / 1000;
            Console.WriteLine($"Generating the buffer at runtime will take approximately {(Math.Max(sec, 1) == 1 ? (sw.ElapsedMilliseconds + " milliseconds.") : (sec + " seconds."))}");
            Console.ReadLine();
        }
    }
}
