using Xunit;

namespace Altinn.Platform.Receipt.Helpers.Tests
{
    public class LanguageHelperTests
    {
        [Fact]
        public void GetLanguageFromAltinnPersistenceCookie_NullCookie_ReturnsDefaultLanguage()
        {
            // Arrange
            string cookieValue = null;
            string expectedLanguage = "nb"; // Default language is 'nb'

            // Act
            string result = LanguageHelper.GetLanguageFromAltinnPersistenceCookie(cookieValue);

            // Assert
            Assert.Equal(expectedLanguage, result);
        }

        [Fact]
        public void GetLanguageFromAltinnPersistenceCookie_ContainsEnglishLanguage_ReturnsEnglish()
        {
            // Arrange
            string cookieValue = "UL=1033"; // Cookie containing English language
            string expectedLanguage = "en";

            // Act
            string result = LanguageHelper.GetLanguageFromAltinnPersistenceCookie(cookieValue);

            // Assert
            Assert.Equal(expectedLanguage, result);
        }

        [Fact]
        public void GetLanguageFromAltinnPersistenceCookie_ContainsNorwegianLanguage_ReturnsNorwegian()
        {
            // Arrange
            string cookieValue = "UL=1044"; // Cookie containing Norwegian language
            string expectedLanguage = "nb";

            // Act
            string result = LanguageHelper.GetLanguageFromAltinnPersistenceCookie(cookieValue);

            // Assert
            Assert.Equal(expectedLanguage, result);
        }

        [Fact]
        public void GetLanguageFromAltinnPersistenceCookie_ContainsNynorskLanguage_ReturnsNynorsk()
        {
            // Arrange
            string cookieValue = "UL=2068"; // Cookie containing Nynorsk language
            string expectedLanguage = "nn";

            // Act
            string result = LanguageHelper.GetLanguageFromAltinnPersistenceCookie(cookieValue);

            // Assert
            Assert.Equal(expectedLanguage, result);
        }

        [Fact]
        public void GetLanguageFromAltinnPersistenceCookie_UnsupportedLanguage_ReturnsDefaultLanguage()
        {
            // Arrange
            string cookieValue = "UL=9999"; // Cookie containing unsupported language
            string expectedLanguage = "nb"; // Default language is 'nb'

            // Act
            string result = LanguageHelper.GetLanguageFromAltinnPersistenceCookie(cookieValue);

            // Assert
            Assert.Equal(expectedLanguage, result);
        }
    }
}
