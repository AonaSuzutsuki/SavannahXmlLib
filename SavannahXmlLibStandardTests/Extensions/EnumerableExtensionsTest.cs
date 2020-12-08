using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CommonExtensionLib.Extensions;
using NUnit.Framework;
using SavannahXmlLib.Extensions;

namespace SavannahXmlLibTests.Extensions
{
    [TestFixture]
    public class EnumerableExtensionsTest
    {
        [Test]
        public void ToGeneric()
        {
            var enumerable = (IEnumerable)new List<object>
            {
                "1",
                0,
                2L,
                "5",
            };
            var exp = (IEnumerable)new List<object>
            {
                "1",
                "5",
            };

            var result = enumerable.ToGeneric<string>();
            CollectionAssert.AreEqual(exp, result);
        }

        [Test]
        public void SelectTest()
        {
            var enumerable = (IEnumerable)new List<object>
            {
                "1",
                0,
                2L,
                "5",
            };
            var exp = (IEnumerable)new List<object>
            {
                1,
                5,
            };

            var result = enumerable.Select<string, int>(item => item.ToInt());

            CollectionAssert.AreEqual(exp, result);
        }
    }
}
