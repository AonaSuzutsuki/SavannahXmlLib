using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Xml;
using NUnit.Framework;
using SavannahXmlLib.XmlWrapper;
using SavannahXmlLib.XmlWrapper.Nodes;

namespace SavannahXmlLibTests.XmlWrapper
{
    public class CachesSavannahXmlReaderForTest : CachedSavannahXmlReader
    {
        public CachesSavannahXmlReaderForTest(string xmlPath, bool ignoreComments = true) : base(xmlPath, ignoreComments)
        {
        }

        public CachesSavannahXmlReaderForTest(Stream stream, bool ignoreComments = true) : base(stream, ignoreComments)
        {
        }

        public Action<XmlNodeList> SelectNodesAssertAction { get; set; }
        public Action<Dictionary<XmlNode, AbstractSavannahXmlNode>> CreateTableAssertAction { get; set; }

        protected override XmlNodeList SelectNodes(string xpath)
        {
            var nodes = base.SelectNodes(xpath);
            SelectNodesAssertAction?.Invoke(nodes);
            return nodes;
        }

        protected override Dictionary<XmlNode, AbstractSavannahXmlNode> CreateTable(XmlNode node, int indentSize, bool isRemoveSpace)
        {
            var dict = base.CreateTable(node, indentSize, isRemoveSpace);
            CreateTableAssertAction?.Invoke(dict);
            return dict;
        }
    }

    [TestFixture]
    public class CachedSavannahXmlReaderTests
    {
        [Test]
        public void GetAttributesTest()
        {
            XmlNodeList firstNodes = null;
            XmlNodeList secondNodes = null;
            Dictionary<XmlNode, AbstractSavannahXmlNode> firstDict = null;
            Dictionary<XmlNode, AbstractSavannahXmlNode> secondDict = null;

            var reader = new CachesSavannahXmlReaderForTest(CommonXmlReaderTest.GetTestPath())
            {
                SelectNodesAssertAction = (nodes) => firstNodes = nodes,
                CreateTableAssertAction = (dict) => firstDict = dict
            };
            var names = reader.GetAttributes("name", "/ServerSettings/property");

            reader.SelectNodesAssertAction = (nodes) => secondNodes = nodes;
            reader.CreateTableAssertAction = (dict) => secondDict = dict;
            var names2 = reader.GetAttributes("name", "/ServerSettings/property");

            if (firstNodes == null && secondNodes == null)
                Assert.Fail("firstNodes and secondNodes is null");
            if (firstDict == null && secondDict == null)
                Assert.Fail("firstNodes and secondNodes is null");

            Assert.AreSame(firstNodes, secondNodes);
            Assert.AreSame(firstDict, secondDict);
        }

        [Test]
        public void GetAttributesClearTest()
        {
            XmlNodeList firstNodes = null;
            XmlNodeList secondNodes = null;
            Dictionary<XmlNode, AbstractSavannahXmlNode> firstDict = null;
            Dictionary<XmlNode, AbstractSavannahXmlNode> secondDict = null;

            var reader = new CachesSavannahXmlReaderForTest(CommonXmlReaderTest.GetTestPath())
            {
                SelectNodesAssertAction = (nodes) => firstNodes = nodes,
                CreateTableAssertAction = (dict) => firstDict = dict
            };
            var names = reader.GetAttributes("name", "/ServerSettings/property");

            reader.SelectNodesAssertAction = (nodes) => secondNodes = nodes;
            reader.CreateTableAssertAction = (dict) => secondDict = dict;
            reader.ClearCache();
            var names2 = reader.GetAttributes("name", "/ServerSettings/property");

            if (firstNodes == null && secondNodes == null)
                Assert.Fail("firstNodes and secondNodes is null");
            if (firstDict == null && secondDict == null)
                Assert.Fail("firstNodes and secondNodes is null");

            Assert.AreNotSame(firstNodes, secondNodes);
            Assert.AreNotSame(firstDict, secondDict);
        }
    }
}