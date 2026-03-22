#if UNITY_EDITOR
using NUnit.Framework;
using R8EOX.Editor.Builders;

namespace R8EOX.Tests.EditMode
{
    /// <summary>
    /// EditMode tests for TrackFolderScanner parsing helpers.
    /// Validates prefix extraction and layer name parsing from folder names.
    /// </summary>
    [TestFixture]
    public class TrackFolderScannerTests
    {
        [Test]
        public void ParsePrefix_ZeroPrefix_ReturnsZero()
        {
            int result = TrackFolderScanner.ParsePrefix("0_DirtBase");
            Assert.That(result, Is.EqualTo(0));
        }

        [Test]
        public void ParsePrefix_PositivePrefix_ReturnsValue()
        {
            int result = TrackFolderScanner.ParsePrefix("12_Rock");
            Assert.That(result, Is.EqualTo(12));
        }

        [Test]
        public void ParsePrefix_NoUnderscore_ReturnsIntMaxValue()
        {
            int result = TrackFolderScanner.ParsePrefix("InvalidName");
            Assert.That(result, Is.EqualTo(int.MaxValue));
        }

        [Test]
        public void ParsePrefix_EmptyPrefix_ReturnsIntMaxValue()
        {
            int result = TrackFolderScanner.ParsePrefix("_OnlyName");
            Assert.That(result, Is.EqualTo(int.MaxValue));
        }

        [Test]
        public void ParsePrefix_NonNumericPrefix_ReturnsIntMaxValue()
        {
            int result = TrackFolderScanner.ParsePrefix("abc_Layer");
            Assert.That(result, Is.EqualTo(int.MaxValue));
        }

        [Test]
        public void ParsePrefix_MultipleUnderscores_UsesFirst()
        {
            int result = TrackFolderScanner.ParsePrefix("3_Some_Layer");
            Assert.That(result, Is.EqualTo(3));
        }

        [Test]
        public void ParseLayerName_WithPrefix_ReturnsNameAfterUnderscore()
        {
            string result = TrackFolderScanner.ParseLayerName("0_DirtBase");
            Assert.That(result, Is.EqualTo("DirtBase"));
        }

        [Test]
        public void ParseLayerName_LargePrefix_ReturnsNameAfterUnderscore()
        {
            string result = TrackFolderScanner.ParseLayerName("12_Rock");
            Assert.That(result, Is.EqualTo("Rock"));
        }

        [Test]
        public void ParseLayerName_NoUnderscore_ReturnsFullName()
        {
            string result = TrackFolderScanner.ParseLayerName("InvalidName");
            Assert.That(result, Is.EqualTo("InvalidName"));
        }

        [Test]
        public void ParseLayerName_MultipleUnderscores_ReturnsEverythingAfterFirst()
        {
            string result = TrackFolderScanner.ParseLayerName("3_Some_Layer");
            Assert.That(result, Is.EqualTo("Some_Layer"));
        }

        [Test]
        public void ParseLayerName_LeadingUnderscore_ReturnsFullName()
        {
            string result = TrackFolderScanner.ParseLayerName("_OnlyName");
            Assert.That(result, Is.EqualTo("_OnlyName"));
        }
    }
}
#endif
