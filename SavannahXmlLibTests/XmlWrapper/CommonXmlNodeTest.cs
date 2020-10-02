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
            var commonXmlNode1 = SavannahXmlNode.CreateRoot("root");
            commonXmlNode1.CreateChildElement("ChildNode", new AttributeInfo[]
            {
                new AttributeInfo
                {
                    Name = "name",
                    Value = "attr"
                }
            }, "Value");

            var commonXmlNode2 = new SavannahXmlNode
            {
                TagName = "root",
                ChildNodes = new SavannahXmlNode[]
                {
                    new SavannahXmlNode
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
            var commonXmlNode2 = new SavannahXmlNode
            {
                TagName = "root",
                ChildNodes = new SavannahXmlNode[]
                {
                    new SavannahXmlNode
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
                        ChildNodes = new SavannahXmlNode[]
                        {
                            new SavannahXmlNode
                            {
                                NodeType = XmlNodeType.Text,
                                InnerText = "Value"
                            }
                        }
                    }
                }
            };

            var exp = "<root>\n  <ChildNode name=\"attr\">\n    Value\n  </ChildNode>\n</root>";
            var act = commonXmlNode2.ToString();

            Assert.AreEqual(exp, act);
        }

        [Test]
        public void ToStringIndentTest()
        {
            var commonXmlNode2 = new SavannahXmlNode
            {
                TagName = "root",
                ChildNodes = new SavannahXmlNode[]
                {
                    new SavannahXmlNode
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
                        ChildNodes = new SavannahXmlNode[]
                        {
                            new SavannahXmlNode
                            {
                                NodeType = XmlNodeType.Text,
                                InnerText = "Value"
                            }
                        }
                    }
                }
            };

            var exp = "<root>\n    <ChildNode name=\"attr\">\n        Value\n    </ChildNode>\n</root>";
            var act = commonXmlNode2.ToString(4);

            Assert.AreEqual(exp, act);
        }

        [Test]
        public void PrioritizeInnerXmlTest()
        {
            var root = new SavannahXmlNode
            {
                TagName = "root",
                ChildNodes = new[]
                {
                    new SavannahXmlNode
                    {
                        TagName = "cov",
                        PrioritizeInnerXml = "<test>test<br />aaaa<br />bbb</test>"
                    },
                    new SavannahXmlNode
                    {
                        TagName = SavannahXmlNode.TextTagName,
                        NodeType = XmlNodeType.Text,
                        PrioritizeInnerXml = "value"
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
                    },
                    new SavannahXmlNode
                    {
                        TagName = SavannahXmlNode.TextTagName,
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
            var root = new SavannahXmlNode
            {
                TagName = "root",
                PrioritizeInnerXml = "<!--\n  more\n  more2\n-->"
            };

            var exp = new SavannahXmlNode
            {
                TagName = "root",
                ChildNodes = new []
                {
                    new SavannahXmlNode
                    {
                        TagName = SavannahXmlNode.CommentTagName,
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
            var root = new SavannahXmlNode
            {
                TagName = "root",
                PrioritizeInnerXml = "test\n  test2\ntest3"
            };

            var exp = new SavannahXmlNode
            {
                TagName = "root",
                ChildNodes = new[]
                {
                    new SavannahXmlNode
                    {
                        TagName = SavannahXmlNode.TextTagName,
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
            var root = new SavannahXmlNode
            {
                TagName = "root",
                PrioritizeInnerXml = "aaa\n<br />\n       bbb\n<br />\nbbb\n<br />\ncccc\n<test>\n  <bbb />\n</test>\n"
            };

            var exp = new SavannahXmlNode
            {
                TagName = "root",
                ChildNodes = new[]
                {
                    new SavannahXmlNode
                    {
                        TagName = SavannahXmlNode.TextTagName,
                        NodeType = XmlNodeType.Text,
                        InnerText = "aaa"
                    },
                    new SavannahXmlNode
                    {
                        TagName = "br"
                    },
                    new SavannahXmlNode
                    {
                        TagName = SavannahXmlNode.TextTagName,
                        NodeType = XmlNodeType.Text,
                        InnerText = "       bbb"
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
                    },
                    new SavannahXmlNode
                    {
                        TagName = "br"
                    },new SavannahXmlNode
                    {
                        TagName = SavannahXmlNode.TextTagName,
                        NodeType = XmlNodeType.Text,
                        InnerText = "cccc"
                    },
                    new SavannahXmlNode
                    {
                        TagName = "test",
                        ChildNodes = new []
                        {
                            new SavannahXmlNode
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
            var root = new SavannahXmlNode
            {
                TagName = "root",
                PrioritizeInnerXml = "<test />\naaa\n<test>\n<br />\n</test>\n   b\nccc"
            };

            var exp = new SavannahXmlNode
            {
                TagName = "root",
                ChildNodes = new[]
                {
                    new SavannahXmlNode
                    {
                        TagName = "test"
                    },
                    new SavannahXmlNode
                    {
                        TagName = SavannahXmlNode.TextTagName,
                        NodeType = XmlNodeType.Text,
                        InnerText = "aaa"
                    },
                    new SavannahXmlNode
                    {
                        TagName = "test",
                        ChildNodes = new []
                        {
                            new SavannahXmlNode
                            {
                                TagName = "br"
                            },
                        }
                    },
                    new SavannahXmlNode
                    {
                        TagName = SavannahXmlNode.TextTagName,
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
            var textNode = new SavannahXmlNode
            {
                TagName = SavannahXmlNode.TextTagName,
                NodeType = XmlNodeType.Text,
                InnerText = "aaa"
            };
            var testNode = new SavannahXmlNode
            {
                TagName = "test",
                ChildNodes = new[]
                {
                    new SavannahXmlNode
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

            var root = new SavannahXmlNode
            {
                TagName = "root",
                ChildNodes = new[]
                {
                    new SavannahXmlNode
                    {
                        TagName = "test"
                    },
                    textNode,
                    new SavannahXmlNode
                    {
                        TagName = "test",
                        ChildNodes = new []
                        {
                            new SavannahXmlNode
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
            var textNode = new SavannahXmlNode
            {
                TagName = SavannahXmlNode.TextTagName,
                NodeType = XmlNodeType.Text,
                InnerText = "aaa"
            };
            var testNode = new SavannahXmlNode
            {
                TagName = "test",
                ChildNodes = new[]
                {
                    new SavannahXmlNode
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

            var root = new SavannahXmlNode
            {
                TagName = "root",
                ChildNodes = new[]
                {
                    new SavannahXmlNode
                    {
                        TagName = "test"
                    },
                    textNode,
                    new SavannahXmlNode
                    {
                        TagName = "test",
                        ChildNodes = new []
                        {
                            new SavannahXmlNode
                            {
                                TagName = "br"
                            },
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
                        TagName = "test"
                    },
                    testNode,
                    textNode,
                    new SavannahXmlNode
                    {
                        TagName = "test",
                        ChildNodes = new []
                        {
                            new SavannahXmlNode
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
            var textNode = new SavannahXmlNode
            {
                TagName = SavannahXmlNode.TextTagName,
                NodeType = XmlNodeType.Text,
                InnerText = "aaa"
            };
            var testNode = new SavannahXmlNode
            {
                TagName = "test",
                ChildNodes = new[]
                {
                    new SavannahXmlNode
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

            var root = new SavannahXmlNode
            {
                TagName = "root",
                ChildNodes = new[]
                {
                    new SavannahXmlNode
                    {
                        TagName = "test"
                    },
                    textNode,
                    new SavannahXmlNode
                    {
                        TagName = "test",
                        ChildNodes = new []
                        {
                            new SavannahXmlNode
                            {
                                TagName = "br"
                            },
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
                        TagName = "test"
                    },
                    textNode,
                    testNode,
                    new SavannahXmlNode
                    {
                        TagName = "test",
                        ChildNodes = new []
                        {
                            new SavannahXmlNode
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
        public void AddBeforeSameChildTest()
        {
            var root = new SavannahXmlNode
            {
                TagName = "root",
                PrioritizeInnerXml = "<br />\naaaa\n<br />\nbbb\n<br />\nccc"
            };
            root.ResolvePrioritizeInnerXml();

            var exp = new SavannahXmlNode
            {
                TagName = "root",
                ChildNodes = new[]
                {
                    new SavannahXmlNode
                    {
                        TagName = "br"
                    },
                    new SavannahXmlNode
                    {
                        TagName = "br"
                    },
                    new SavannahXmlNode
                    {
                        NodeType = XmlNodeType.Text,
                        TagName = SavannahXmlNode.TextTagName,
                        InnerText = "aaaa"
                    },
                    new SavannahXmlNode
                    {
                        NodeType = XmlNodeType.Text,
                        TagName = SavannahXmlNode.TextTagName,
                        InnerText = "bbb"
                    },
                    new SavannahXmlNode
                    {
                        TagName = "br"
                    },
                    new SavannahXmlNode
                    {
                        NodeType = XmlNodeType.Text,
                        TagName = SavannahXmlNode.TextTagName,
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
            var commonXmlNode2 = new SavannahXmlNode
            {
                TagName = "root",
                ChildNodes = new SavannahXmlNode[]
                {
                    new SavannahXmlNode
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
                        ChildNodes = new SavannahXmlNode[]
                        {
                            new SavannahXmlNode
                            {
                                NodeType = XmlNodeType.Text,
                                InnerText = "Value"
                            }
                        }
                    }
                }
            };

            var exp = "<root>\n  <ChildNode name=\"attr\">\n    Value\n  </ChildNode>\n</root>";

            commonXmlNode2.ResolvePrioritizeInnerXml();
            var act = commonXmlNode2.ChildNodes.First().OutterXml;

            Assert.AreEqual(exp, act);
        }
    }
}
