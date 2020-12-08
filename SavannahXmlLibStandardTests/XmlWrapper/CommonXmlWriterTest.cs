using System;
using System.IO;
using NUnit.Framework;
using SavannahXmlLib.XmlWrapper;
using SavannahXmlLib.XmlWrapper.Nodes;
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
            var root = new SavannahTagNode
            {
                TagName = "ServerSettings",
                ChildNodes = new[]
                {
                    new SavannahTagNode
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
                            new SavannahTextNode
                            {
                                InnerText = "サーバー名を設定します。サーバーリストにはこの名前で表示されます。"
                            }
                        }
                    },
                    new SavannahTagNode
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
                            new SavannahTextNode
                            {
                                InnerText = "サーバー名を設定します。サーバーリストにはこの名前で表示されます。\n    test"
                            }
                        }
                    },
                    new SavannahTagNode
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
                            new SavannahTextNode
                            {
                                InnerText = "サーバーの説明を設定します。"
                            }
                        }
                    },
                    new SavannahTagNode
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
                            new SavannahTextNode
                            {
                                InnerText = "サーバーのウェブサイトを設定します。"
                            }
                        }
                    },
                    new SavannahTagNode
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
                        ChildNodes = new[]
                        {
                            new SavannahTagNode
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
                                    new SavannahTextNode
                                    {
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
            var root = new SavannahTagNode
            {
                TagName = "root",
                InnerText = "aaaaa",
                ChildNodes = new[]
                {
                    new SavannahTextNode
                    {
                        InnerText = "bbbbb"
                    }
                }
            };

            var exp = new SavannahTagNode
            {
                TagName = "root",
                InnerText = "bbbbb",
                ChildNodes = new[]
                {
                    new SavannahTextNode
                    {
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
            var root = new SavannahTagNode
            {
                TagName = "root",
                ChildNodes = new[]
                {
                    new SavannahTagNode
                    {
                        TagName = "test",
                        ChildNodes = new[]
                        {
                            new SavannahTagNode
                            {
                                TagName = "test",
                                ChildNodes = new[]
                                {
                                    new SavannahTextNode
                                    {
                                        InnerText = "bbbbb\naaaaa\nccccc"
                                    }
                                }
                            }
                        }
                    },
                    new SavannahTagNode
                    {
                        TagName = "vehicle",
                        ChildNodes = new AbstractSavannahXmlNode[]
                        {
                            new SavannahTextNode
                            {
                                InnerText = "bbbbb\naaaaa\nccccc"
                            },
                            new SavannahTagNode
                            {
                                TagName = "br",
                            }
                        }
                    }
                }
            };

            var exp = new SavannahTagNode
            {
                TagName = "root",
                ChildNodes = new[]
                {
                    new SavannahTagNode
                    {
                        TagName = "test",
                        ChildNodes = new[]
                        {
                            new SavannahTagNode
                            {
                                TagName = "test",
                                InnerText = "bbbbb\naaaaa\nccccc",
                                ChildNodes = new[]
                                {
                                    new SavannahTextNode
                                    {
                                        InnerText = "bbbbb\naaaaa\nccccc"
                                    }
                                }
                            }
                        }
                    },
                    new SavannahTagNode
                    {
                        TagName = "vehicle",
                        InnerText = "bbbbb\naaaaa\nccccc",
                        ChildNodes = new AbstractSavannahXmlNode[]
                        {
                            new SavannahTextNode
                            {
                                InnerText = "bbbbb\naaaaa\nccccc"
                            },
                            new SavannahTagNode
                            {
                                TagName = "br",
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
            var root = new SavannahTagNode
            {
                TagName = "root"
            };

            var exp = new SavannahTagNode
            {
                TagName = "root",
                ChildNodes = new []
                {
                    new SavannahTagNode
                    {
                        TagName = "test",
                        InnerText = "test\naaaa\nbbb",
                        ChildNodes = new AbstractSavannahXmlNode[]
                        {
                            new SavannahTextNode
                            {
                                InnerText = "test"
                            },
                            new SavannahTagNode
                            {
                                TagName = "br"
                            },
                            new SavannahTextNode
                            {
                                InnerText = "aaaa"
                            },
                            new SavannahTagNode
                            {
                                TagName = "br"
                            },
                            new SavannahTextNode
                            {
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
            var root = new SavannahTagNode
            {
                TagName = "root",
                ChildNodes = new []
                {
                    new SavannahTagNode
                    {
                        TagName = "cov",
                        PrioritizeInnerXml = "<test>test<br />aaaa<br />bbb</test>"
                    }
                }
            };

            var exp = new SavannahTagNode
            {
                TagName = "root",
                ChildNodes = new[]
                {
                    new SavannahTagNode
                    {
                        TagName = "cov",
                        ChildNodes = new []
                        {
                            new SavannahTagNode
                            {
                                TagName = "test",
                                InnerText = "test\naaaa\nbbb",
                                ChildNodes = new AbstractSavannahXmlNode[]
                                {
                                    new SavannahTextNode
                                    {
                                        InnerText = "test"
                                    },
                                    new SavannahTagNode
                                    {
                                        TagName = "br"
                                    },
                                    new SavannahTextNode
                                    {
                                        InnerText = "aaaa"
                                    },
                                    new SavannahTagNode
                                    {
                                        TagName = "br"
                                    },
                                    new SavannahTextNode
                                    {
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
            var root = new SavannahTagNode
            {
                TagName = "root",
                ChildNodes = new []
                {
                    new SavannahTagNode
                    {
                        TagName = "tag",
                        InnerText = "",
                        ChildNodes = new[]
                        {
                            new SavannahCdataNode
                            {
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
            var exp = File.ReadAllText(CommonXmlReaderTest.GetTestPath("Cdata.xml")).UnifiedBreakLine();

            Assert.AreEqual(exp, xml);
        }
    }
}
