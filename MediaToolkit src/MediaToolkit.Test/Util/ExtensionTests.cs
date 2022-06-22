using MediaToolkit.Util;
using NUnit.Framework;

namespace MediaToolkit.Test.Util
{
    [TestFixture]
    public class ExtensionTests
    {
        public class ForEach
        {
            [SetUp]
            public void SetUp()
            {
                this._collectionUnderTest = new[] { "Foo", "Bar" };
            }

            private IEnumerable<string>? _collectionUnderTest;

            [Test]
            public void Will_Iterate_Through_EachItem_InCollection()
            {
                var expectedIterations = 2;
                var iterations = 0;

                this._collectionUnderTest!.ForEach(_ => iterations++);

                Assert.That(iterations == expectedIterations);
            }

            [Test]
            public void When_ActionIsNull_Throw_ArgumentNullException()
            {
                var expectedException = typeof (ArgumentNullException);

                void CodeUnderTest() => this._collectionUnderTest!.ForEach(null!);

                Assert.Throws(expectedException, CodeUnderTest);
            }
        }
    }
}
