using System;
using System.IO;
using NUnit.Framework;
using SavannahXmlLib.XmlWrapper;
using CommonExtensionLib.Extensions;
using System.Text;

namespace SavannahXmlLibTests.XmlWrapper
{
    [TestFixture]
    public class CommonXmlWriterTest
    {
        [Test]
        public void WriteTest()
        {
            var root = new SavannahXmlNode
            {
                TagName = "ServerSettings",
                ChildNodes = new SavannahXmlNode[]
                {
                    new SavannahXmlNode
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
                            new SavannahXmlNode
                            {
                                TagName = SavannahXmlNode.TextTagName,
                                NodeType = XmlNodeType.Text,
                                InnerText = "サーバー名を設定します。サーバーリストにはこの名前で表示されます。"
                            }
                        }
                    },
                    new SavannahXmlNode
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
                            new SavannahXmlNode
                            {
                                TagName = SavannahXmlNode.TextTagName,
                                NodeType = XmlNodeType.Text,
                                InnerText = "サーバー名を設定します。サーバーリストにはこの名前で表示されます。\n    test"
                            }
                        }
                    },
                    new SavannahXmlNode
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
                            new SavannahXmlNode
                            {
                                TagName = SavannahXmlNode.TextTagName,
                                NodeType = XmlNodeType.Text,
                                InnerText = "サーバーの説明を設定します。"
                            }
                        }
                    },
                    new SavannahXmlNode
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
                            new SavannahXmlNode
                            {
                                TagName = SavannahXmlNode.TextTagName,
                                NodeType = XmlNodeType.Text,
                                InnerText = "サーバーのウェブサイトを設定します。"
                            }
                        }
                    },
                    new SavannahXmlNode
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
                        ChildNodes = new SavannahXmlNode[]
                        {
                            new SavannahXmlNode
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
                                    new SavannahXmlNode
                                    {
                                        TagName = SavannahXmlNode.TextTagName,
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

            var writer = new SavannahXmlWriter("version=\"1.0\" encoding=\"UTF-8\"");
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
            var root = new SavannahXmlNode
            {
                TagName = "root",
                InnerText = "aaaaa",
                ChildNodes = new[]
                {
                    new SavannahXmlNode
                    {
                        TagName = SavannahXmlNode.TextTagName,
                        NodeType = XmlNodeType.Text,
                        InnerText = "bbbbb"
                    }
                }
            };

            var exp = new SavannahXmlNode
            {
                TagName = "root",
                InnerText = "bbbbb",
                ChildNodes = new[]
                {
                    new SavannahXmlNode
                    {
                        TagName = SavannahXmlNode.TextTagName,
                        NodeType = XmlNodeType.Text,
                        InnerText = "bbbbb"
                    }
                }
            };

            var writer = new SavannahXmlWriter("version=\"1.0\" encoding=\"UTF-8\"");
            using var ms = new MemoryStream();
            writer.Write(ms, root);
            ms.Seek(0, SeekOrigin.Begin);

            using var sr = new StreamReader(ms);
            var xml = sr.ReadToEnd();
            ms.Seek(0, SeekOrigin.Begin);

            var reader = new SavannahXmlReader(ms);
            var act = reader.GetAllNodes();

            Assert.AreEqual(exp, act);
        }

        [Test]
        public void WriteTest3()
        {
            var root = new SavannahXmlNode
            {
                TagName = "root",
                ChildNodes = new[]
                {
                    new SavannahXmlNode
                    {
                        TagName = "test",
                        NodeType = XmlNodeType.Tag,
                        ChildNodes = new[]
                        {
                            new SavannahXmlNode
                            {
                                TagName = "test",
                                NodeType = XmlNodeType.Tag,
                                ChildNodes = new[]
                                {
                                    new SavannahXmlNode
                                    {
                                        TagName = SavannahXmlNode.TextTagName,
                                        NodeType = XmlNodeType.Text,
                                        InnerText = "bbbbb\naaaaa\nccccc"
                                    }
                                }
                            }
                        }
                    },
                    new SavannahXmlNode
                    {
                        TagName = "vehicle",
                        NodeType = XmlNodeType.Tag,
                        ChildNodes = new[]
                        {
                            new SavannahXmlNode
                            {
                                TagName = SavannahXmlNode.TextTagName,
                                NodeType = XmlNodeType.Text,
                                InnerText = "bbbbb\naaaaa\nccccc"
                            },
                            new SavannahXmlNode
                            {
                                TagName = "br",
                                NodeType = XmlNodeType.Tag
                            }
                        }
                    }
                }
            };

            var exp = new SavannahXmlNode
            {
                TagName = "root",
                ChildNodes = new[]
                {
                    new SavannahXmlNode
                    {
                        TagName = "test",
                        NodeType = XmlNodeType.Tag,
                        ChildNodes = new[]
                        {
                            new SavannahXmlNode
                            {
                                TagName = "test",
                                NodeType = XmlNodeType.Tag,
                                InnerText = "bbbbb\naaaaa\nccccc",
                                ChildNodes = new[]
                                {
                                    new SavannahXmlNode
                                    {
                                        TagName = SavannahXmlNode.TextTagName,
                                        NodeType = XmlNodeType.Text,
                                        InnerText = "bbbbb\naaaaa\nccccc"
                                    }
                                }
                            }
                        }
                    },
                    new SavannahXmlNode
                    {
                        TagName = "vehicle",
                        NodeType = XmlNodeType.Tag,
                        InnerText = "bbbbb\naaaaa\nccccc",
                        ChildNodes = new[]
                        {
                            new SavannahXmlNode
                            {
                                TagName = SavannahXmlNode.TextTagName,
                                NodeType = XmlNodeType.Text,
                                InnerText = "bbbbb\naaaaa\nccccc"
                            },
                            new SavannahXmlNode
                            {
                                TagName = "br",
                                NodeType = XmlNodeType.Tag
                            }
                        }
                    }
                }
            };

            var writer = new SavannahXmlWriter("version=\"1.0\" encoding=\"UTF-8\"");
            using var ms = new MemoryStream();
            writer.Write(ms, root);
            ms.Seek(0, SeekOrigin.Begin);

            var reader = new SavannahXmlReader(ms);
            var act = reader.GetAllNodes();

            Assert.AreEqual(exp, act);
        }

        [Test]
        public void WriteTestPrioritizeInnerXml()
        {
            var root = new SavannahXmlNode
            {
                TagName = "root"
            };

            var exp = new SavannahXmlNode
            {
                TagName = "root",
                ChildNodes = new []
                {
                    new SavannahXmlNode
                    {
                        TagName = "test",
                        InnerText = "test\naaaa\nbbb",
                        ChildNodes = new []
                        {
                            new SavannahXmlNode
                            {
                                TagName = SavannahXmlNode.TextTagName,
                                NodeType = XmlNodeType.Text,
                                InnerText = "test"
                            },
                            new SavannahXmlNode
                            {
                                TagName = "br"
                            },
                            new SavannahXmlNode
                            {
                                TagName = SavannahXmlNode.TextTagName,
                                NodeType = XmlNodeType.Text,
                                InnerText = "aaaa"
                            },
                            new SavannahXmlNode
                            {
                                TagName = "br"
                            },
                            new SavannahXmlNode
                            {
                                TagName = SavannahXmlNode.TextTagName,
                                NodeType = XmlNodeType.Text,
                                InnerText = "bbb"
                            }
                        }
                    }
                }
            };

            root.PrioritizeInnerXml = "<test>test<br />aaaa<br />bbb</test>";

            var writer = new SavannahXmlWriter("version=\"1.0\" encoding=\"UTF-8\"");
            using var ms = new MemoryStream();
            writer.Write(ms, root);
            ms.Seek(0, SeekOrigin.Begin);

            var reader = new SavannahXmlReader(ms);
            var act = reader.GetAllNodes();

            Assert.AreEqual(exp, act);
        }

        [Test]
        public void WriteTestPrioritizeInnerXml2()
        {
            var root = new SavannahXmlNode
            {
                TagName = "root",
                ChildNodes = new []
                {
                    new SavannahXmlNode
                    {
                        TagName = "cov",
                        PrioritizeInnerXml = "<test>test<br />aaaa<br />bbb</test>"
                    }
                }
            };

            var exp = new SavannahXmlNode
            {
                TagName = "root",
                ChildNodes = new[]
                {
                    new SavannahXmlNode
                    {
                        TagName = "cov",
                        ChildNodes = new []
                        {
                            new SavannahXmlNode
                            {
                                TagName = "test",
                                InnerText = "test\naaaa\nbbb",
                                ChildNodes = new []
                                {
                                    new SavannahXmlNode
                                    {
                                        TagName = SavannahXmlNode.TextTagName,
                                        NodeType = XmlNodeType.Text,
                                        InnerText = "test"
                                    },
                                    new SavannahXmlNode
                                    {
                                        TagName = "br"
                                    },
                                    new SavannahXmlNode
                                    {
                                        TagName = SavannahXmlNode.TextTagName,
                                        NodeType = XmlNodeType.Text,
                                        InnerText = "aaaa"
                                    },
                                    new SavannahXmlNode
                                    {
                                        TagName = "br"
                                    },
                                    new SavannahXmlNode
                                    {
                                        TagName = SavannahXmlNode.TextTagName,
                                        NodeType = XmlNodeType.Text,
                                        InnerText = "bbb"
                                    }
                                }
                            }
                        }
                    }
                }
            };

            var writer = new SavannahXmlWriter("version=\"1.0\" encoding=\"UTF-8\"");
            using var ms = new MemoryStream();
            writer.Write(ms, root);
            ms.Seek(0, SeekOrigin.Begin);

            var reader = new SavannahXmlReader(ms);
            var act = reader.GetAllNodes();

            Assert.AreEqual(exp, act);
        }

        [Test]
        public void WriteCdataTest()
        {
            var root = new SavannahXmlNode
            {
                TagName = "root",
                ChildNodes = new []
                {
                    new SavannahXmlNode
                    {
                        TagName = "tag",
                        InnerText = "",
                        ChildNodes = new[]
                        {
                            new SavannahXmlNode
                            {
                                NodeType = XmlNodeType.CDATA,
                                TagName = SavannahXmlNode.CdataTagName,
                                InnerText = "  <?xml version=\"1.0\"?>\n  <document>\n      doc.\n  </document>",
                            }
                        }
                    }
                }
            };

            var ms = new MemoryStream();
            var writer = new SavannahXmlWriter();
            writer.Write(ms, root);
            ms.Position = 0;
            var xml = new StreamReader(ms).ReadToEnd();
            var exp = File.ReadAllText(CommonXmlReaderTest.GetTestPath("Cdata.xml"));

            Assert.AreEqual(exp, xml);
        }
    }
}
