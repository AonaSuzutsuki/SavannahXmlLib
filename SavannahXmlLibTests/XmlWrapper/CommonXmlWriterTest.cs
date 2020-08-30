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
                        ChildNodes = new []
                        {
                            new CommonXmlNode
                            {
                                TagName = CommonXmlNode.TextTagName,
                                NodeType = XmlNodeType.Text,
                                InnerText = "サーバー名を設定します。サーバーリストにはこの名前で表示されます。"
                            }
                        }
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
                        ChildNodes = new []
                        {
                            new CommonXmlNode
                            {
                                TagName = CommonXmlNode.TextTagName,
                                NodeType = XmlNodeType.Text,
                                InnerText = "サーバー名を設定します。サーバーリストにはこの名前で表示されます。\n    test"
                            }
                        }
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
                        ChildNodes = new []
                        {
                            new CommonXmlNode
                            {
                                TagName = CommonXmlNode.TextTagName,
                                NodeType = XmlNodeType.Text,
                                InnerText = "サーバーの説明を設定します。"
                            }
                        }
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
                        ChildNodes = new []
                        {
                            new CommonXmlNode
                            {
                                TagName = CommonXmlNode.TextTagName,
                                NodeType = XmlNodeType.Text,
                                InnerText = "サーバーのウェブサイトを設定します。"
                            }
                        }
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
                                ChildNodes = new []
                                {
                                    new CommonXmlNode
                                    {
                                        TagName = CommonXmlNode.TextTagName,
                                        NodeType = XmlNodeType.Text,
                                        InnerText = "Value"
                                    }
                                }
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
                        TagName = CommonXmlNode.TextTagName,
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
                        TagName = CommonXmlNode.TextTagName,
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

        [Test]
        public void WriteTest3()
        {
            var root = new CommonXmlNode
            {
                TagName = "root",
                ChildNodes = new[]
                {
                    new CommonXmlNode
                    {
                        TagName = "test",
                        NodeType = XmlNodeType.Tag,
                        ChildNodes = new[]
                        {
                            new CommonXmlNode
                            {
                                TagName = "test",
                                NodeType = XmlNodeType.Tag,
                                ChildNodes = new[]
                                {
                                    new CommonXmlNode
                                    {
                                        TagName = CommonXmlNode.TextTagName,
                                        NodeType = XmlNodeType.Text,
                                        InnerText = "bbbbb\naaaaa\nccccc"
                                    }
                                }
                            }
                        }
                    },
                    new CommonXmlNode
                    {
                        TagName = "vehicle",
                        NodeType = XmlNodeType.Tag,
                        ChildNodes = new[]
                        {
                            new CommonXmlNode
                            {
                                TagName = CommonXmlNode.TextTagName,
                                NodeType = XmlNodeType.Text,
                                InnerText = "bbbbb\naaaaa\nccccc"
                            },
                            new CommonXmlNode
                            {
                                TagName = "br",
                                NodeType = XmlNodeType.Tag
                            }
                        }
                    }
                }
            };

            var exp = new CommonXmlNode
            {
                TagName = "root",
                ChildNodes = new[]
                {
                    new CommonXmlNode
                    {
                        TagName = "test",
                        NodeType = XmlNodeType.Tag,
                        ChildNodes = new[]
                        {
                            new CommonXmlNode
                            {
                                TagName = "test",
                                NodeType = XmlNodeType.Tag,
                                InnerText = "bbbbb\naaaaa\nccccc",
                                ChildNodes = new[]
                                {
                                    new CommonXmlNode
                                    {
                                        TagName = CommonXmlNode.TextTagName,
                                        NodeType = XmlNodeType.Text,
                                        InnerText = "bbbbb\naaaaa\nccccc"
                                    }
                                }
                            }
                        }
                    },
                    new CommonXmlNode
                    {
                        TagName = "vehicle",
                        NodeType = XmlNodeType.Tag,
                        ChildNodes = new[]
                        {
                            new CommonXmlNode
                            {
                                TagName = CommonXmlNode.TextTagName,
                                NodeType = XmlNodeType.Text,
                                InnerText = "bbbbb\naaaaa\nccccc"
                            },
                            new CommonXmlNode
                            {
                                TagName = "br",
                                NodeType = XmlNodeType.Tag
                            }
                        }
                    }
                }
            };

            var writer = new CommonXmlWriter("version=\"1.0\" encoding=\"UTF-8\"");
            using var ms = new MemoryStream();
            writer.Write(ms, root);
            ms.Seek(0, SeekOrigin.Begin);

            var reader = new CommonXmlReader(ms);
            var act = reader.GetAllNodes();

            Assert.AreEqual(exp, act);
        }

        [Test]
        public void WriteTestPrioritizeInnerXml()
        {
            var root = new CommonXmlNode
            {
                TagName = "root"
            };

            var exp = new CommonXmlNode
            {
                TagName = "root",
                ChildNodes = new []
                {
                    new CommonXmlNode
                    {
                        TagName = "test",
                        ChildNodes = new []
                        {
                            new CommonXmlNode
                            {
                                TagName = CommonXmlNode.TextTagName,
                                NodeType = XmlNodeType.Text,
                                InnerText = "test"
                            },
                            new CommonXmlNode
                            {
                                TagName = "br"
                            },
                            new CommonXmlNode
                            {
                                TagName = CommonXmlNode.TextTagName,
                                NodeType = XmlNodeType.Text,
                                InnerText = "aaaa"
                            },
                            new CommonXmlNode
                            {
                                TagName = "br"
                            },
                            new CommonXmlNode
                            {
                                TagName = CommonXmlNode.TextTagName,
                                NodeType = XmlNodeType.Text,
                                InnerText = "bbb"
                            }
                        }
                    }
                }
            };

            root.PrioritizeInnerXml = "<test>test<br />aaaa<br />bbb</test>";

            var writer = new CommonXmlWriter("version=\"1.0\" encoding=\"UTF-8\"");
            using var ms = new MemoryStream();
            writer.Write(ms, root);
            ms.Seek(0, SeekOrigin.Begin);

            var reader = new CommonXmlReader(ms);
            var act = reader.GetAllNodes();

            Assert.AreEqual(exp, act);
        }

        [Test]
        public void WriteTestPrioritizeInnerXml2()
        {
            var root = new CommonXmlNode
            {
                TagName = "root",
                ChildNodes = new []
                {
                    new CommonXmlNode
                    {
                        TagName = "cov",
                        PrioritizeInnerXml = "<test>test<br />aaaa<br />bbb</test>"
                    }
                }
            };

            var exp = new CommonXmlNode
            {
                TagName = "root",
                ChildNodes = new[]
                {
                    new CommonXmlNode
                    {
                        TagName = "cov",
                        ChildNodes = new []
                        {
                            new CommonXmlNode
                            {
                                TagName = "test",
                                ChildNodes = new []
                                {
                                    new CommonXmlNode
                                    {
                                        TagName = CommonXmlNode.TextTagName,
                                        NodeType = XmlNodeType.Text,
                                        InnerText = "test"
                                    },
                                    new CommonXmlNode
                                    {
                                        TagName = "br"
                                    },
                                    new CommonXmlNode
                                    {
                                        TagName = CommonXmlNode.TextTagName,
                                        NodeType = XmlNodeType.Text,
                                        InnerText = "aaaa"
                                    },
                                    new CommonXmlNode
                                    {
                                        TagName = "br"
                                    },
                                    new CommonXmlNode
                                    {
                                        TagName = CommonXmlNode.TextTagName,
                                        NodeType = XmlNodeType.Text,
                                        InnerText = "bbb"
                                    }
                                }
                            }
                        }
                    }
                }
            };

            var writer = new CommonXmlWriter("version=\"1.0\" encoding=\"UTF-8\"");
            using var ms = new MemoryStream();
            writer.Write(ms, root);
            ms.Seek(0, SeekOrigin.Begin);

            var reader = new CommonXmlReader(ms);
            var act = reader.GetAllNodes();

            Assert.AreEqual(exp, act);
        }
    }
}
