# EmulateMe
Showing how proof-of-work can be used to evade antivirus emulators.

EmualteMe uses a proof-of-work algorithm used by Bitcoin, known as Hashcash, to compute arbitrary data at runtime.

This process is CPU expensive and can vary in intensity depending on the bitstring difficulty.

I'm sure there are some less than ideal coding choices here, so if you are well-versed in crypto please contribute your suggestions.

Example usage in Program.cs focuses around encrypting state variables, but you can really apply this technique as you see fit.


https://winternl.com/designing-emulation-resistant-control-flow/
