using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using SavannahXmlLib.XmlWrapper;

namespace SavannahXmlLibTests.XmlWrapper
{
    [TestFixture]
    public class CommonXmlNodeTest
    {
        [Test]
        public void EqualsTest()
        {
            var commonXmlNode1 = CommonXmlNode.CreateRoot("root");
            commonXmlNode1.CreateChildElement("ChildNode", new AttributeInfo[]
            {
                new AttributeInfo
                {
                    Name = "name",
                    Value = "attr"
                }
            }, "Value");

            var commonXmlNode2 = new CommonXmlNode
            {
                TagName = "root",
                ChildNodes = new CommonXmlNode[]
                {
                    new CommonXmlNode
                    {
                        TagName = "ChildNode",
                        Attributes = new AttributeInfo[]
                        {
                            new AttributeInfo
                            {
                                Name = "name",
                                Value = "attr"
                            }
                        },
                        PrioritizeInnerXml = "Value"
                    }
                }
            };

            var value = commonXmlNode1.Equals(commonXmlNode2);
            Assert.AreEqual(true, value);
        }

        [Test]
        public void ToStringTest()
        {
            var commonXmlNode2 = new CommonXmlNode
            {
                TagName = "root",
                ChildNodes = new CommonXmlNode[]
                {
                    new CommonXmlNode
                    {
                        NodeType = XmlNodeType.Tag,
                        TagName = "ChildNode",
                        Attributes = new AttributeInfo[]
                        {
                            new AttributeInfo
                            {
                                Name = "name",
                                Value = "attr"
                            }
                        },
                        ChildNodes = new CommonXmlNode[]
                        {
                            new CommonXmlNode
                            {
                                NodeType = XmlNodeType.Text,
                                PrioritizeInnerXml = "Value"
                            }
                        }
                    }
                }
            };

            var act = commonXmlNode2.ToString();
        }

        [Test]
        public void PrioritizeInnerXmlTest()
        {
            var root = new CommonXmlNode
            {
                TagName = "root",
                ChildNodes = new[]
                {
                    new CommonXmlNode
                    {
                        TagName = "cov",
                        PrioritizeInnerXml = "<test>test<br />aaaa<br />bbb</test>"
                    },
                    new CommonXmlNode
                    {
                        TagName = CommonXmlNode.TextTagName,
                        NodeType = XmlNodeType.Text,
                        PrioritizeInnerXml = "value"
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
                    },
                    new CommonXmlNode
                    {
                        TagName = CommonXmlNode.TextTagName,
                        NodeType = XmlNodeType.Text,
                        InnerText = "value"
                    }
                }
            };

            root.ResolvePrioritizeInnerXml();

            Assert.AreEqual(exp, root);
        }

        [Test]
        public void PrioritizeInnerXmlCommentTest()
        {
            var root = new CommonXmlNode
            {
                TagName = "root",
                PrioritizeInnerXml = "<!--\n  more\n  more2\n-->"
            };

            var exp = new CommonXmlNode
            {
                TagName = "root",
                ChildNodes = new []
                {
                    new CommonXmlNode
                    {
                        TagName = CommonXmlNode.CommentTagName,
                        InnerText = "more\nmore2"
                    }
                }
            };

            root.ResolvePrioritizeInnerXml(false);

            Assert.AreEqual(exp, root);
        }

        [Test]
        public void PrioritizeInnerXmlSpaceTest()
        {
            var root = new CommonXmlNode
            {
                TagName = "root",
                PrioritizeInnerXml = "test\n  test2\ntest3"
            };

            var exp = new CommonXmlNode
            {
                TagName = "root",
                ChildNodes = new[]
                {
                    new CommonXmlNode
                    {
                        TagName = CommonXmlNode.TextTagName,
                        NodeType = XmlNodeType.Text,
                        InnerText = "test\n  test2\ntest3"
                    }
                }
            };

            root.ResolvePrioritizeInnerXml(false);

            Assert.AreEqual(exp, root);
        }

        [Test]
        public void WritePrioritizeInnerXmlSpaceTest2()
        {
            var root = new CommonXmlNode
            {
                TagName = "root",
                PrioritizeInnerXml = "aaa\n<br />\n       bbb\n<br />\nbbb\n<br />\ncccc\n<test>\n  <bbb />\n</test>\n"
            };

            var exp = new CommonXmlNode
            {
                TagName = "root",
                ChildNodes = new[]
                {
                    new CommonXmlNode
                    {
                        TagName = CommonXmlNode.TextTagName,
                        NodeType = XmlNodeType.Text,
                        InnerText = "aaa"
                    },
                    new CommonXmlNode
                    {
                        TagName = "br"
                    },
                    new CommonXmlNode
                    {
                        TagName = CommonXmlNode.TextTagName,
                        NodeType = XmlNodeType.Text,
                        InnerText = "       bbb"
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
                    },
                    new CommonXmlNode
                    {
                        TagName = "br"
                    },new CommonXmlNode
                    {
                        TagName = CommonXmlNode.TextTagName,
                        NodeType = XmlNodeType.Text,
                        InnerText = "cccc"
                    },
                    new CommonXmlNode
                    {
                        TagName = "test",
                        ChildNodes = new []
                        {
                            new CommonXmlNode
                            {
                                TagName = "bbb"
                            }
                        }
                    }
                }
            };

            root.ResolvePrioritizeInnerXml(false);

            Assert.AreEqual(exp, root);
        }

        [Test]
        public void WritePrioritizeInnerXmlSpaceTest3()
        {
            var root = new CommonXmlNode
            {
                TagName = "root",
                PrioritizeInnerXml = "<test />\naaa\n<test>\n<br />\n</test>\n   b\nccc"
            };

            var exp = new CommonXmlNode
            {
                TagName = "root",
                ChildNodes = new[]
                {
                    new CommonXmlNode
                    {
                        TagName = "test"
                    },
                    new CommonXmlNode
                    {
                        TagName = CommonXmlNode.TextTagName,
                        NodeType = XmlNodeType.Text,
                        InnerText = "aaa"
                    },
                    new CommonXmlNode
                    {
                        TagName = "test",
                        ChildNodes = new []
                        {
                            new CommonXmlNode
                            {
                                TagName = "br"
                            },
                        }
                    },
                    new CommonXmlNode
                    {
                        TagName = CommonXmlNode.TextTagName,
                        NodeType = XmlNodeType.Text,
                        InnerText = "   b\nccc"
                    }
                }
            };

            root.ResolvePrioritizeInnerXml(false);

            Assert.AreEqual(exp, root);
        }

        [Test]
        public void SearchElementWithXPathTest()
        {
            var textNode = new CommonXmlNode
            {
                TagName = CommonXmlNode.TextTagName,
                NodeType = XmlNodeType.Text,
                InnerText = "aaa"
            };
            var testNode = new CommonXmlNode
            {
                TagName = "test",
                ChildNodes = new[]
                {
                    new CommonXmlNode
                    {
                        TagName = "br",
                        Attributes = new List<AttributeInfo>
                        {
                            new AttributeInfo
                            {
                                Name = "attr",
                                Value = "value"
                            }
                        }
                    },
                }
            };

            var root = new CommonXmlNode
            {
                TagName = "root",
                ChildNodes = new[]
                {
                    new CommonXmlNode
                    {
                        TagName = "test"
                    },
                    textNode,
                    new CommonXmlNode
                    {
                        TagName = "test",
                        ChildNodes = new []
                        {
                            new CommonXmlNode
                            {
                                TagName = "br"
                            },
                        }
                    },
                    testNode
                }
            };

            var textReader = root.ChildNodes.ToArray()[1].GetReader();

            Assert.IsNull(textReader);

            var testReader = root.ChildNodes.ToArray()[3].GetReader();
            var resultTestNode = testReader.GetNode("/test/br");

            Assert.AreEqual(resultTestNode, testNode.ChildNodes.First());
        }

        [Test]
        public void AddBeforeChildTest()
        {
            var textNode = new CommonXmlNode
            {
                TagName = CommonXmlNode.TextTagName,
                NodeType = XmlNodeType.Text,
                InnerText = "aaa"
            };
            var testNode = new CommonXmlNode
            {
                TagName = "test",
                ChildNodes = new[]
                {
                    new CommonXmlNode
                    {
                        TagName = "br",
                        Attributes = new List<AttributeInfo>
                        {
                            new AttributeInfo
                            {
                                Name = "attr",
                                Value = "value"
                            }
                        }
                    },
                }
            };

            var root = new CommonXmlNode
            {
                TagName = "root",
                ChildNodes = new[]
                {
                    new CommonXmlNode
                    {
                        TagName = "test"
                    },
                    textNode,
                    new CommonXmlNode
                    {
                        TagName = "test",
                        ChildNodes = new []
                        {
                            new CommonXmlNode
                            {
                                TagName = "br"
                            },
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
                        TagName = "test"
                    },
                    testNode,
                    textNode,
                    new CommonXmlNode
                    {
                        TagName = "test",
                        ChildNodes = new []
                        {
                            new CommonXmlNode
                            {
                                TagName = "br"
                            },
                        }
                    }
                }
            };

            root.AddBeforeChildElement(textNode, testNode);

            Assert.AreEqual(exp, root);
        }

        [Test]
        public void AddAfterChildTest()
        {
            var textNode = new CommonXmlNode
            {
                TagName = CommonXmlNode.TextTagName,
                NodeType = XmlNodeType.Text,
                InnerText = "aaa"
            };
            var testNode = new CommonXmlNode
            {
                TagName = "test",
                ChildNodes = new[]
                {
                    new CommonXmlNode
                    {
                        TagName = "br",
                        Attributes = new List<AttributeInfo>
                        {
                            new AttributeInfo
                            {
                                Name = "attr",
                                Value = "value"
                            }
                        }
                    },
                }
            };

            var root = new CommonXmlNode
            {
                TagName = "root",
                ChildNodes = new[]
                {
                    new CommonXmlNode
                    {
                        TagName = "test"
                    },
                    textNode,
                    new CommonXmlNode
                    {
                        TagName = "test",
                        ChildNodes = new []
                        {
                            new CommonXmlNode
                            {
                                TagName = "br"
                            },
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
                        TagName = "test"
                    },
                    textNode,
                    testNode,
                    new CommonXmlNode
                    {
                        TagName = "test",
                        ChildNodes = new []
                        {
                            new CommonXmlNode
                            {
                                TagName = "br"
                            },
                        }
                    }
                }
            };

            root.AddAfterChildElement(textNode, testNode);

            Assert.AreEqual(exp, root);
        }

        [Test]
        public void AddBeforeChildTest2()
        {
            var root = new CommonXmlNode
            {
                TagName = "root",
                PrioritizeInnerXml = "<br />\naaaa\n<br />\nbbb\n<br />\nccc"
            };
            root.ResolvePrioritizeInnerXml();

            var exp = new CommonXmlNode
            {
                TagName = "root",
                ChildNodes = new[]
                {
                    new CommonXmlNode
                    {
                        TagName = "br"
                    },
                    new CommonXmlNode
                    {
                        TagName = "br"
                    },
                    new CommonXmlNode
                    {
                        NodeType = XmlNodeType.Text,
                        TagName = CommonXmlNode.TextTagName,
                        InnerText = "aaaa"
                    },
                    new CommonXmlNode
                    {
                        NodeType = XmlNodeType.Text,
                        TagName = CommonXmlNode.TextTagName,
                        InnerText = "bbb"
                    },
                    new CommonXmlNode
                    {
                        TagName = "br"
                    },
                    new CommonXmlNode
                    {
                        NodeType = XmlNodeType.Text,
                        TagName = CommonXmlNode.TextTagName,
                        InnerText = "ccc"
                    }
                }
            };

            var newNode = root.ChildNodes.ElementAt(2);
            var target = root.ChildNodes.ElementAt(1);
            root.RemoveChildElement(newNode);
            root.AddBeforeChildElement(target, newNode);

            Assert.AreEqual(exp, root);
        }

        [Test]
        public void OutterXmlTest()
        {
            var commonXmlNode2 = new CommonXmlNode
            {
                TagName = "root",
                ChildNodes = new CommonXmlNode[]
                {
                    new CommonXmlNode
                    {
                        NodeType = XmlNodeType.Tag,
                        TagName = "ChildNode",
                        Attributes = new AttributeInfo[]
                        {
                            new AttributeInfo
                            {
                                Name = "name",
                                Value = "attr"
                            }
                        },
                        ChildNodes = new CommonXmlNode[]
                        {
                            new CommonXmlNode
                            {
                                NodeType = XmlNodeType.Text,
                                InnerText = "Value"
                            }
                        }
                    }
                }
            };

            commonXmlNode2.ResolvePrioritizeInnerXml();
            var act = commonXmlNode2.ChildNodes.First().OutterXml;
            Console.WriteLine(act);
        }
    }
}
