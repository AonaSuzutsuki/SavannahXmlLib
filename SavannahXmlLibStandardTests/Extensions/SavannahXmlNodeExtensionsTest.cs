using System;
using SavannahXmlLib.Extensions;
using NUnit.Framework;
using SavannahXmlLib.XmlWrapper.Nodes;

namespace SavannahXmlLibTests.Extensions
{
    [TestFixture]
    public class SavannahXmlNodeExtensionsTest
    {
        [Test]
        public void MultiAttributesToTextTest()
        {
            var attributes = new[]
            {
                new AttributeInfo
                {
                    Name = "id",
                    Value = "testId"
                },
                new AttributeInfo
                {
                    Name = "name",
                    Value = "testName"
                },
                new AttributeInfo
                {
                    Name = "value",
                    Value = "testValue"
                }
            };
            var exp = "id=\"testId\" name=\"testName\" value=\"testValue\"";
            var value = attributes.ToAttributesText(" ");

            Assert.AreEqual(exp, value);
        }

        [Test]
        public void SingleAttributeToTextTest()
        {
            var attributes = new[]
            {
                new AttributeInfo
                {
                    Name = "id",
                    Value = "testId"
                }
            };
            var exp = "id=\"testId\"";
            var value = attributes.ToAttributesText(" ");

            Assert.AreEqual(exp, value);
        }
    }
}
