using System;
using System.Collections.Generic;
using SavannahXmlLib.XmlWrapper.Nodes;
using NUnit.Framework;
using SavannahXmlLib.Extensions;

namespace SavannahXmlLibTests.Extensions
{
    [TestFixture]
    public class LinkedListExtensionsTest
    {
        private (LinkedList<AbstractSavannahXmlNode> list, AbstractSavannahXmlNode exp) CreateLinkedList()
        {
            var list = new LinkedList<AbstractSavannahXmlNode>();
            list.AddLast(new SavannahTextNode() { TagName = "1" });
            list.AddLast(new SavannahTextNode() { TagName = "2" });
            list.AddLast(new SavannahTextNode() { TagName = "2" });
            list.AddLast(new SavannahTextNode() { TagName = "4" });
            list.AddLast(new SavannahTextNode() { TagName = "5" });
            list.AddLast(new SavannahTextNode() { TagName = "6" });
            var expList = new List<AbstractSavannahXmlNode>(list);

            var randomIndex = new Random(Environment.TickCount).Next(0, list.Count - 1);

            return (list, expList[randomIndex]);
        }

        [Test]
        public void FindTest()
        {
            var (list, exp) = CreateLinkedList();
            var linkedListNode = list.Find(exp, new SavannahXmlNodeComparer());
            var value = linkedListNode.Value;

            Assert.AreEqual(exp, value);
        }
    }
}
