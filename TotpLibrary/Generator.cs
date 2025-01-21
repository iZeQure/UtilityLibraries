using System.Security.Cryptography;

namespace TotpLibrary
{
    /// <summary>
    /// The Totp class provides methods to generate Time-based One-Time Passwords (TOTP).
    /// </summary>
    public static class Totp
    {
        /// <summary>
        /// Generates a TOTP code from a given seed.
        /// </summary>
        /// <param name="seed">The base32 encoded seed used to generate the TOTP code.</param>
        /// <returns>A 6-digit TOTP code as a string.</returns>
        public static string GenerateFromSeed(string seed)
        {
            // Get the current timestamp in UTC
            DateTime timestamp = DateTime.UtcNow;
            // Decode the base32 encoded seed to a byte array
            byte[] key = Base32Decode(seed);

            // Time step (30 seconds)
            long timestep = 30;
            // Get the current Unix time in seconds
            long unixTime = ((DateTimeOffset)timestamp).ToUnixTimeSeconds();
            // Calculate the counter value based on the Unix time and time step
            long counter = unixTime / timestep;

            // Convert counter to byte array
            byte[] counterBytes = BitConverter.GetBytes(counter);
            if (BitConverter.IsLittleEndian)
                Array.Reverse(counterBytes);

            // Compute HMAC-SHA1 hash using the key and counter
            using (HMACSHA1 hmac = new(key))
            {
                byte[] hash = hmac.ComputeHash(counterBytes);

                // Extract dynamic binary code from the hash
                int offset = hash[^1] & 0x0F;
                int binaryCode = (hash[offset] & 0x7F) << 24
                               | (hash[offset + 1] & 0xFF) << 16
                               | (hash[offset + 2] & 0xFF) << 8
                               | (hash[offset + 3] & 0xFF);

                // Generate TOTP code (6 digits) by taking the binary code modulo 1000000
                int totpCode = binaryCode % 1000000;
                // Return the TOTP code as a zero-padded 6-digit string
                return totpCode.ToString("D6");
            }
        }

        /// <summary>
        /// Decodes a base32 encoded string to a byte array.
        /// </summary>
        /// <param name="base32">The base32 encoded string.</param>
        /// <returns>A byte array representing the decoded data.</returns>
        /// <exception cref="ArgumentException">Thrown when the base32 string contains invalid characters.</exception>
        static internal byte[] Base32Decode(string base32)
        {
            // Define the base32 alphabet
            const string alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ234567";

            // Remove padding characters and convert to uppercase
            base32 = base32.TrimEnd('=').ToUpper();
            // Calculate the length of the output byte array
            byte[] outputBytes = new byte[base32.Length * 5 / 8];

            byte curByte = 0, bitsRemaining = 8;
            int arrayIndex = 0;

            // Process each character in the base32 string
            foreach (char c in base32)
            {
                // Get the value of the current character in the base32 alphabet
                int cValue = alphabet.IndexOf(c);
                if (cValue < 0)
                    throw new ArgumentException("Invalid base32 character", nameof(base32));
                int mask;
                if (bitsRemaining > 5)
                {
                    // If there are more than 5 bits remaining, shift the character value and add to the current byte
                    mask = cValue << (bitsRemaining - 5);
                    curByte |= (byte)mask;
                    bitsRemaining -= 5;
                }
                else
                {
                    // If there are 5 or fewer bits remaining, shift the character value and add to the current byte
                    mask = cValue >> (5 - bitsRemaining);
                    curByte |= (byte)mask;
                    outputBytes[arrayIndex++] = curByte;
                    curByte = (byte)(cValue << (3 + bitsRemaining));
                    bitsRemaining += 3;
                }
            }

            // If there are remaining bits, add the last byte to the output array
            if (arrayIndex != outputBytes.Length)
                outputBytes[arrayIndex] = curByte;

            return outputBytes;
        }
    }
}
