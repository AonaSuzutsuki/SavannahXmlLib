using System;
using System.IO;
using NUnit.Framework;
using SavannahXmlLib.XmlWrapper;
using CommonExtensionLib.Extensions;

namespace SavannahXmlLibTests.XmlWrapper
{
    [TestFixture]
    public class CommonXmlWriterTest
    {
        [Test]
        public void WriteTest()
        {
            var root = new CommonXmlNode
            {
                TagName = "ServerSettings",
                ChildNodes = new CommonXmlNode[]
                {
                    new CommonXmlNode
                    {
                        TagName = "property",
                        Attributes = new AttributeInfo[]
                                {
                                    new AttributeInfo
                                    {
                                        Name = "name",
                                        Value = "ServerName"
                                    },
                                    new AttributeInfo
                                    {
                                        Name = "value",
                                        Value = "My Game Host"
                                    }
                                },
                        InnerText = "\n    サーバー名を設定します。サーバーリストにはこの名前で表示されます。\n  "
                    },
                    new CommonXmlNode
                    {
                        TagName = "property",
                        Attributes = new AttributeInfo[]
                                {
                                    new AttributeInfo
                                    {
                                        Name = "name",
                                        Value = "ServerName2"
                                    },
                                    new AttributeInfo
                                    {
                                        Name = "value",
                                        Value = "My Game Host"
                                    }
                                },
                        InnerText = "\n    サーバー名を設定します。サーバーリストにはこの名前で表示されます。\n        test\n  "
                    },
                    new CommonXmlNode
                    {
                        TagName = "property",
                        Attributes = new AttributeInfo[]
                                {
                                    new AttributeInfo
                                    {
                                        Name = "name",
                                        Value = "ServerDescription"
                                    },
                                    new AttributeInfo
                                    {
                                        Name = "value",
                                        Value = "A 7 Days to Die server"
                                    }
                                },
                        InnerText = "サーバーの説明を設定します。"
                    },
                    new CommonXmlNode
                    {
                        TagName = "property",
                        Attributes = new AttributeInfo[]
                                {
                                    new AttributeInfo
                                    {
                                        Name = "name",
                                        Value = "ServerWebsiteURL"
                                    },
                                    new AttributeInfo
                                    {
                                        Name = "value",
                                        Value = ""
                                    }
                                },
                        InnerText = "サーバーのウェブサイトを設定します。"
                    },
                    new CommonXmlNode
                    {
                        TagName = "property",
                        Attributes = new AttributeInfo[]
                        {
                            new AttributeInfo
                            {
                                Name = "name",
                                Value = "Nested"
                            }
                        },
                        ChildNodes = new CommonXmlNode[]
                        {
                            new CommonXmlNode
                            {
                                TagName = "property",
                                Attributes = new AttributeInfo[]
                                {
                                    new AttributeInfo
                                    {
                                        Name = "name",
                                        Value = "NestedElem"
                                    }
                                },
                                InnerText = "Value"
                            }
                        }
                    }
                }
            };

            var exp = File.ReadAllText(CommonXmlReaderTest.GetTestPath()).UnifiedBreakLine();

            var writer = new CommonXmlWriter("version=\"1.0\" encoding=\"UTF-8\"");
            using var ms = new MemoryStream();
            writer.Write(ms, root);
            ms.Seek(0, SeekOrigin.Begin);

            using var sr = new StreamReader(ms);
            var xml = sr.ReadToEnd().UnifiedBreakLine();

            Assert.AreEqual(exp, xml);
        }

        [Test]
        public void WriteTest2()
        {
            var root = new CommonXmlNode
            {
                TagName = "root",
                InnerText = "aaaaa",
                ChildNodes = new[]
                {
                    new CommonXmlNode
                    {
                        TagName = "#text",
                        NodeType = XmlNodeType.Text,
                        InnerText = "bbbbb"
                    }
                }
            };

            var exp = new CommonXmlNode
            {
                TagName = "root",
                InnerText = "bbbbb",
                ChildNodes = new[]
                {
                    new CommonXmlNode
                    {
                        TagName = "#text",
                        NodeType = XmlNodeType.Text,
                        InnerText = "bbbbb"
                    }
                }
            };

            var writer = new CommonXmlWriter("version=\"1.0\" encoding=\"UTF-8\"");
            using var ms = new MemoryStream();
            writer.Write(ms, root);
            ms.Seek(0, SeekOrigin.Begin);

            using var sr = new StreamReader(ms);
            var xml = sr.ReadToEnd();
            ms.Seek(0, SeekOrigin.Begin);

            var reader = new CommonXmlReader(ms);
            var act = reader.GetAllNodes();

            Assert.AreEqual(exp, act);
        }
    }
}
