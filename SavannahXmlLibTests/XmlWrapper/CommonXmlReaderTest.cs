using System;
using NUnit.Framework;
using SavannahXmlLib.XmlWrapper;
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

            var reader = new CommonXmlReader(GetTestPath());
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

            var reader = new CommonXmlReader(GetTestPath());
            var names = reader.GetAttributes("name", "/ServerSettings/property");

            CollectionAssert.AreEqual(exp, names);
        }

        [Test]
        public void GetValuesTest()
        {
            var reader = new CommonXmlReader(GetTestPath());
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
        public void GetNodeTest()
        {
            var exp = new CommonXmlNode
            {
                NodeType = XmlNodeType.Tag,
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
                ChildNodes = new CommonXmlNode[]
                {
                    new CommonXmlNode
                    {
                        NodeType = XmlNodeType.Text,
                        TagName = CommonXmlNode.TextTagName,
                        InnerText = "サーバー名を設定します。サーバーリストにはこの名前で表示されます。"
                    }
                },
                InnerText = "サーバー名を設定します。サーバーリストにはこの名前で表示されます。"
            };

            var reader = new CommonXmlReader(GetTestPath());
            var node = reader.GetNode("/ServerSettings/property[@name='ServerName']");

            Assert.AreEqual(exp, node);
        }

        [Test]
        public void GetNodesTest()
        {
            var exp = new CommonXmlNode[]
            {
                new CommonXmlNode
                {
                    NodeType = XmlNodeType.Tag,
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
                    ChildNodes = new CommonXmlNode[]
                    {
                        new CommonXmlNode
                        {
                            NodeType = XmlNodeType.Text,
                            TagName = CommonXmlNode.TextTagName,
                            InnerText = "サーバー名を設定します。サーバーリストにはこの名前で表示されます。"
                        }
                    },
                    InnerText = "サーバー名を設定します。サーバーリストにはこの名前で表示されます。"
                },
                new CommonXmlNode
                {
                    NodeType = XmlNodeType.Tag,
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
                    ChildNodes = new CommonXmlNode[]
                    {
                        new CommonXmlNode
                        {
                            NodeType = XmlNodeType.Text,
                            TagName = CommonXmlNode.TextTagName,
                            InnerText = "サーバー名を設定します。サーバーリストにはこの名前で表示されます。\n    test"
                        }
                    },
                    InnerText = "サーバー名を設定します。サーバーリストにはこの名前で表示されます。\n    test"
                },
            };

            var reader = new CommonXmlReader(GetTestPath());
            var node = reader.GetNodes("/ServerSettings/property[contains(@name, 'ServerName')]");

            Assert.AreEqual(exp, node);
        }

        [Test]
        public void GetAllNodesTest()
        {
            var exp = new CommonXmlNode
            {
                NodeType = XmlNodeType.Tag,
                TagName = "ServerSettings",
                ChildNodes = new CommonXmlNode[]
                {
                    new CommonXmlNode
                    {
                        NodeType = XmlNodeType.Tag,
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
                            new CommonXmlNode
                            {
                                NodeType = XmlNodeType.Text,
                                TagName = CommonXmlNode.TextTagName,
                                InnerText = "サーバー名を設定します。サーバーリストにはこの名前で表示されます。"
                            }
                        },
                        InnerText = "サーバー名を設定します。サーバーリストにはこの名前で表示されます。"
                    },
                    new CommonXmlNode
                    {
                        NodeType = XmlNodeType.Tag,
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
                            new CommonXmlNode
                            {
                                NodeType = XmlNodeType.Text,
                                TagName = CommonXmlNode.TextTagName,
                                InnerText = "サーバー名を設定します。サーバーリストにはこの名前で表示されます。\n    test"
                            }
                        },
                        InnerText = "サーバー名を設定します。サーバーリストにはこの名前で表示されます。\n    test"
                    },
                    new CommonXmlNode
                    {
                        NodeType = XmlNodeType.Tag,
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
                            new CommonXmlNode
                            {
                                NodeType = XmlNodeType.Text,
                                TagName = CommonXmlNode.TextTagName,
                                InnerText = "サーバーの説明を設定します。"
                            }
                        },
                        InnerText = "サーバーの説明を設定します。"
                    },
                    new CommonXmlNode
                    {
                        NodeType = XmlNodeType.Tag,
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
                            new CommonXmlNode
                            {
                                NodeType = XmlNodeType.Text,
                                TagName = CommonXmlNode.TextTagName,
                                InnerText = "サーバーのウェブサイトを設定します。"
                            }
                        },
                        InnerText = "サーバーのウェブサイトを設定します。"
                    },
                    new CommonXmlNode
                    {
                        NodeType = XmlNodeType.Tag,
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
                                NodeType = XmlNodeType.Tag,
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
                                    new CommonXmlNode
                                    {
                                        NodeType = XmlNodeType.Text,
                                        TagName = CommonXmlNode.TextTagName,
                                        InnerText = "Value"
                                    }
                                },
                                InnerText = "Value"
                            }
                        }
                    }
                }
            };

            var reader = new CommonXmlReader(GetTestPath());
            var node = reader.GetAllNodes();

            Assert.AreEqual(exp, node);
        }

        [Test]
        public void CommentTest()
        {
            var exp = new CommonXmlNode
            {
                TagName = "root",
                ChildNodes = new []
                {
                    new CommonXmlNode
                    {
                        TagName = CommonXmlNode.CommentTagName,
                        InnerText = "Comment Test\nNew"
                    },
                    new CommonXmlNode
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
                            new CommonXmlNode
                            {
                                TagName = CommonXmlNode.TextTagName,
                                NodeType = XmlNodeType.Text,
                                InnerText = "test"
                            }
                        }
                    },
                }
            };

            var reader = new CommonXmlReader(GetTestPath("Comment.xml"), false);
            var node = reader.GetAllNodes();

            Assert.AreEqual(exp, node);
        }

        [Test]
        public void CommentIgnoreTest()
        {
            var exp = new CommonXmlNode
            {
                TagName = "root",
                ChildNodes = new[]
                {
                    new CommonXmlNode
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
                            new CommonXmlNode
                            {
                                TagName = CommonXmlNode.TextTagName,
                                NodeType = XmlNodeType.Text,
                                InnerText = "test"
                            }
                        }
                    },
                }
            };

            var reader = new CommonXmlReader(GetTestPath("Comment.xml"));
            var node = reader.GetAllNodes();

            Assert.AreEqual(exp, node);
        }

        [Test]
        public void WritePrioritizeInnerXmlSpaceTest()
        {
            var root = new CommonXmlNode
            {
                TagName = "root",
                PrioritizeInnerXml = "       aaaaaa\naa\n  aaaa"
            };

            var exp = new CommonXmlNode
            {
                TagName = "root",
                InnerText = "aaaaaa\naa\naaaa",
                ChildNodes = new[]
                {
                    new CommonXmlNode
                    {
                        TagName = CommonXmlNode.TextTagName,
                        NodeType = XmlNodeType.Text,
                        InnerText = "       aaaaaa\naa\n  aaaa"
                    },
                }
            };

            root.ResolvePrioritizeInnerXml(false);

            var xml = $"{CommonXmlConstants.Declaration}\n{root}";
            var data = Encoding.UTF8.GetBytes(xml);
            var stream = new MemoryStream();
            stream.Write(data, 0, data.Length);
            stream.Position = 0;

            var reader = new CommonXmlReader(stream);
            var node = reader.GetAllNodes();

            //Console.WriteLine(root);
            Console.WriteLine(node);

            Assert.AreEqual(exp, node);
        }

        [Test]
        public void ReadNamespaceElement()
        {
            var reader = new CommonXmlReader(GetTestPath("Namespace.xml"));
            reader.AddNamespace("ns", "http://schemas.microsoft.com/developer/msbuild/2003");
            reader.AddNamespace("test", "http://schemas.microsoft.com/developer/msbuild/2003");

            var value = reader.GetValues("/ns:Project/ns:PropertyGroup/ns:TargetFrameworkVersion")?.Last();
            var exp = "v4.8";
            Assert.AreEqual(exp, value);

            var value2 = reader.GetValues("/ns:Project/ns:PropertyGroup/test:FileAlignment")?.Last();
            var exp2 = "512";
            Assert.AreEqual(exp2, value2);
        }
    }
}
