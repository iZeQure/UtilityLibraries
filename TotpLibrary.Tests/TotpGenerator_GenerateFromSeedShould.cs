namespace TotpLibrary.Tests.Generator
{
    public class TotpGenerator_GenerateFromSeedShould
    {
        [Fact]
        public void GenerateFromSeed_ValidSeed_ReturnsCorrectLength()
        {
            // Arrange
            string seed = "JBSWY3DPEHPK3PXP"; // Base32 encoded seed

            // Act
            string totp = Totp.GenerateFromSeed(seed);

            // Assert
            Assert.Equal(6, totp.Length);
        }

        [Fact]
        public void TotpGenerator_ValidSeed_ReturnsDigitsOnly()
        {
            // Arrange
            string seed = "JBSWY3DPEHPK3PXP"; // Base32 encoded seed

            // Act
            string totp = Totp.GenerateFromSeed(seed);

            // Assert
            Assert.True(int.TryParse(totp, out _));
        }
    }
}
