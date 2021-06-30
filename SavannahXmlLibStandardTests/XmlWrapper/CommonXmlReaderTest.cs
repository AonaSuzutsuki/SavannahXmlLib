using System;
using NUnit.Framework;
using SavannahXmlLib.XmlWrapper;
using SavannahXmlLib.XmlWrapper.Nodes;
using System.Collections.Generic;
using System.IO;
using System.Text;
using CommonCoreLib.CommonPath;
using System.Linq;

namespace SavannahXmlLibTests.XmlWrapper
{
    [TestFixture]
    public class CommonXmlReaderTest
    {
        public static string GetTestPath(string filename = "Test.xml")
        {
            return $"{AppDomain.CurrentDomain.BaseDirectory}/TestData/{filename}".UnifiedSystemPathSeparator();
        }

        [Test]
        public void DeclarationTest()
        {
            var exp = "version=\"1.0\" encoding=\"UTF-8\"";

            var reader = new SavannahXmlReader(GetTestPath());
            var declaration = reader.Declaration;

            Assert.AreEqual(exp, declaration);
        }

        [Test]
        public void GetAttributesTest()
        {
            var exp = new string[]
            {
                "ServerName",
                "ServerName2",
                "ServerDescription",
                "ServerWebsiteURL",
                "Nested",
            };

            var reader = new SavannahXmlReader(GetTestPath());
            var names = reader.GetAttributes("name", "/ServerSettings/property");

            CollectionAssert.AreEqual(exp, names);
        }

        [Test]
        public void GetValuesTest()
        {
            var reader = new SavannahXmlReader(GetTestPath());
            var attributes = reader.GetValues("/ServerSettings/property");

            var exp = new List<string>
            {
                "サーバー名を設定します。サーバーリストにはこの名前で表示されます。",
                "サーバー名を設定します。サーバーリストにはこの名前で表示されます。\n    test",
                "サーバーの説明を設定します。",
                "サーバーのウェブサイトを設定します。"
            };

            CollectionAssert.AreEqual(exp, attributes);
        }

        [Test]
        public void GetValuesNotContainsTest()
        {
            var reader = new SavannahXmlReader(GetTestPath());
            var attributes = reader.GetValues("/ServerSettings/test");

            var exp = new List<string>
            {
            };

            CollectionAssert.AreEqual(exp, attributes);
        }

        [Test]
        public void GetNodeTest()
        {
            var exp = new SavannahTagNode
            {
                TagName = "property",
                Attributes = new List<AttributeInfo>
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
                ChildNodes = new[]
                {
                    new SavannahTextNode
                    {
                        InnerText = "サーバー名を設定します。サーバーリストにはこの名前で表示されます。"
                    }
                },
                InnerText = "サーバー名を設定します。サーバーリストにはこの名前で表示されます。"
            };

            var reader = new SavannahXmlReader(GetTestPath());
            var firstNode = reader.GetNode("/ServerSettings/property[@name='ServerName']");
            var SecondNode = reader.GetNode("/ServerSettings/property[@name='ServerName']");

            Assert.AreEqual(exp, firstNode);
            Assert.AreEqual(exp, SecondNode);
        }

        [Test]
        public void GetNodeNotFoundTest()
        {
            var exp = new SavannahTagNode
            {
                TagName = "property",
                Attributes = new List<AttributeInfo>
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
                ChildNodes = new[]
                {
                    new SavannahTextNode
                    {
                        InnerText = "サーバー名を設定します。サーバーリストにはこの名前で表示されます。"
                    }
                },
                InnerText = "サーバー名を設定します。サーバーリストにはこの名前で表示されます。"
            };

            var reader = new SavannahXmlReader(GetTestPath());
            var firstNode = reader.GetNode("/ServerSettings/properties[@name='ServerName']");

            Assert.IsNull(firstNode);
        }

        [Test]
        public void GetNodesTest()
        {
            var exp = new AbstractSavannahXmlNode[]
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
                    ChildNodes = new[]
                    {
                        new SavannahTextNode
                        {
                            InnerText = "サーバー名を設定します。サーバーリストにはこの名前で表示されます。"
                        }
                    },
                    InnerText = "サーバー名を設定します。サーバーリストにはこの名前で表示されます。"
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
                    ChildNodes = new[]
                    {
                        new SavannahTextNode
                        {
                            InnerText = "サーバー名を設定します。サーバーリストにはこの名前で表示されます。\n    test"
                        }
                    },
                    InnerText = "サーバー名を設定します。サーバーリストにはこの名前で表示されます。\n    test"
                },
            };

            var reader = new SavannahXmlReader(GetTestPath());
            var firstNode = reader.GetNodes("/ServerSettings/property[contains(@name, 'ServerName')]");
            var secondNode = reader.GetNodes("/ServerSettings/property[contains(@name, 'ServerName')]");

            Assert.AreEqual(exp, firstNode);
            Assert.AreEqual(exp, secondNode);
        }

        [Test]
        public void GetAllNodesTest()
        {
            var exp = new SavannahTagNode
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
                        ChildNodes = new[]
                        {
                            new SavannahTextNode
                            {
                                InnerText = "サーバー名を設定します。サーバーリストにはこの名前で表示されます。"
                            }
                        },
                        InnerText = "サーバー名を設定します。サーバーリストにはこの名前で表示されます。"
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
                        ChildNodes = new[]
                        {
                            new SavannahTextNode
                            {
                                InnerText = "サーバー名を設定します。サーバーリストにはこの名前で表示されます。\n    test"
                            }
                        },
                        InnerText = "サーバー名を設定します。サーバーリストにはこの名前で表示されます。\n    test"
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
                        ChildNodes = new[]
                        {
                            new SavannahTextNode
                            {
                                InnerText = "サーバーの説明を設定します。"
                            }
                        },
                        InnerText = "サーバーの説明を設定します。"
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
                        ChildNodes = new[]
                        {
                            new SavannahTextNode
                            {
                                InnerText = "サーバーのウェブサイトを設定します。"
                            }
                        },
                        InnerText = "サーバーのウェブサイトを設定します。"
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
                                ChildNodes = new[]
                                {
                                    new SavannahTextNode
                                    {
                                        InnerText = "Value"
                                    }
                                },
                                InnerText = "Value"
                            }
                        }
                    }
                }
            };

            var reader = new SavannahXmlReader(GetTestPath());
            var node = reader.GetAllNodes();

            Assert.AreEqual(exp, node);
        }

        [Test]
        public void CommentTest()
        {
            var exp = new SavannahTagNode
            {
                TagName = "root",
                ChildNodes = new AbstractSavannahXmlNode[]
                {
                    new SavannahCommentNode
                    {
                        InnerText = "Comment Test\nNew"
                    },
                    new SavannahTagNode
                    {
                        TagName = "value",
                        InnerText = "test",
                        Attributes = new []
                        {
                            new AttributeInfo
                            {
                                Name = "attr",
                                Value = "value"
                            },
                            new AttributeInfo
                            {
                                Name = "attr2",
                                Value = "value2"
                            }
                        },
                        ChildNodes = new []
                        {
                            new SavannahTextNode
                            {
                                InnerText = "test"
                            }
                        }
                    },
                }
            };

            var reader = new SavannahXmlReader(GetTestPath("Comment.xml"), false);
            var node = reader.GetAllNodes();

            Assert.AreEqual(exp, node);
        }

        [Test]
        public void CommentIgnoreTest()
        {
            var exp = new SavannahTagNode
            {
                TagName = "root",
                ChildNodes = new[]
                {
                    new SavannahTagNode
                    {
                        TagName = "value",
                        InnerText = "test",
                        Attributes = new []
                        {
                            new AttributeInfo
                            {
                                Name = "attr",
                                Value = "value"
                            },
                            new AttributeInfo
                            {
                                Name = "attr2",
                                Value = "value2"
                            }
                        },
                        ChildNodes = new []
                        {
                            new SavannahTextNode
                            {
                                InnerText = "test"
                            }
                        }
                    },
                }
            };

            var reader = new SavannahXmlReader(GetTestPath("Comment.xml"));
            var node = reader.GetAllNodes();

            Assert.AreEqual(exp, node);
        }

        [Test]
        public void WritePrioritizeInnerXmlSpaceTest()
        {
            var root = new SavannahTagNode
            {
                TagName = "root",
                PrioritizeInnerXml = "       aaaaaa\naa\n  aaaa"
            };

            var exp = new SavannahTagNode
            {
                TagName = "root",
                InnerText = "       aaaaaa\naa\n  aaaa",
                ChildNodes = new[]
                {
                    new SavannahTextNode
                    {
                        InnerText = "       aaaaaa\naa\n  aaaa"
                    },
                }
            };

            root.ResolvePrioritizeInnerXml(false);

            var xml = $"{SavannahXmlConstants.Declaration}\n{root}";
            var data = Encoding.UTF8.GetBytes(xml);
            var stream = new MemoryStream();
            stream.Write(data, 0, data.Length);
            stream.Position = 0;

            var reader = new SavannahXmlReader(stream);
            var node = reader.GetAllNodes();

            Assert.AreEqual(exp, node);
        }

        [Test]
        public void ReadNamespaceElement()
        {
            var reader = new SavannahXmlReader(GetTestPath("Namespace.xml"));
            reader.AddNamespace("ns", "http://schemas.microsoft.com/developer/msbuild/2003");
            reader.AddNamespace("test", "http://schemas.microsoft.com/developer/msbuild/2003");

            var value = reader.GetValues("/ns:Project/ns:PropertyGroup/ns:TargetFrameworkVersion")?.Last();
            var exp = "v4.8";
            Assert.AreEqual(exp, value);

            var value2 = reader.GetValues("/ns:Project/ns:PropertyGroup/test:FileAlignment")?.Last();
            var exp2 = "512";
            Assert.AreEqual(exp2, value2);
        }

        [Test]
        public void SpecifyIndentTest()
        {
            var exp = new SavannahTagNode
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
                        ChildNodes = new[]
                        {
                            new SavannahTextNode
                            {
                                InnerText = "サーバー名を設定します。サーバーリストにはこの名前で表示されます。"
                            }
                        },
                        InnerText = "サーバー名を設定します。サーバーリストにはこの名前で表示されます。"
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
                        ChildNodes = new[]
                        {
                            new SavannahTextNode
                            {
                                InnerText = "サーバー名を設定します。サーバーリストにはこの名前で表示されます。\n    test"
                            }
                        },
                        InnerText = "サーバー名を設定します。サーバーリストにはこの名前で表示されます。\n    test"
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
                        ChildNodes = new[]
                        {
                            new SavannahTextNode
                            {
                                InnerText = "サーバーの説明を設定します。"
                            }
                        },
                        InnerText = "サーバーの説明を設定します。"
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
                        ChildNodes = new[]
                        {
                            new SavannahTextNode
                            {
                                InnerText = "サーバーのウェブサイトを設定します。"
                            }
                        },
                        InnerText = "サーバーのウェブサイトを設定します。"
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
                                ChildNodes = new[]
                                {
                                    new SavannahTextNode
                                    {
                                        InnerText = "Value"
                                    }
                                },
                                InnerText = "Value"
                            }
                        }
                    }
                }
            };

            var reader = new SavannahXmlReader(GetTestPath("Test_four_indent.xml"))
            {
                IndentSize = 4
            };
            var node = reader.GetAllNodes();

            Assert.AreEqual(exp, node);
        }

        [Test]
        public void ReadCdataTest()
        {
            var reader = new SavannahXmlReader(GetTestPath("Cdata.xml"));
            var root = reader.GetAllNodes();

            var exp = new SavannahTagNode
            {
                TagName = "root",
                ChildNodes = new[]
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

            Assert.AreEqual(exp, root);
        }
    }
}
