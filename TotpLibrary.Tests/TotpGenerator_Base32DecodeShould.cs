namespace TotpLibrary.Tests.Generator
{
    /// <summary>
    /// Tests for the Base32Decode method in the Totp class.
    /// </summary>
    /// <param name="output"></param>
    public class TotpGenerator_Base32DecodeShould(ITestOutputHelper output)
    {
        [Fact]
        public void Base32Decode_ValidBase32_ReturnsCorrectByteArray()
        {
            // Arrange
            string base32 = "JBSWY3DPEHPK3PXP";
            byte[] expectedBytes = [72, 101, 108, 108, 111, 33, 222, 173, 190, 239];

            // Act
            byte[] result = Totp.Base32Decode(base32);

            // Assert
            Assert.Equal(expectedBytes, result);
        }

        [Fact]
        public void Base32Decode_InvalidBase32_ThrowsArgumentException()
        {
            // Arrange
            string invalidBase32 = "INVALID!!!";

            // Act

            // Assert
            Assert.Throws<ArgumentException>(() => Totp.Base32Decode(invalidBase32));
        }
    }
}
