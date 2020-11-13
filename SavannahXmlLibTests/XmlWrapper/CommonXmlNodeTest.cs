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
            var commonXmlNode1 = SavannahTagNode.CreateRoot("root");
            commonXmlNode1.CreateChildElement("ChildNode", new AttributeInfo[]
            {
                new AttributeInfo
                {
                    Name = "name",
                    Value = "attr"
                }
            }, "Value");

            var commonXmlNode2 = new SavannahTagNode
            {
                TagName = "root",
                ChildNodes = new[]
                {
                    new SavannahTagNode
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
            var commonXmlNode2 = new SavannahTagNode
            {
                TagName = "root",
                ChildNodes = new[]
                {
                    new SavannahTagNode
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
                        ChildNodes = new[]
                        {
                            new SavannahTextNode
                            {
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
            var commonXmlNode2 = new SavannahTagNode
            {
                TagName = "root",
                ChildNodes = new[]
                {
                    new SavannahTagNode
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
                        ChildNodes = new[]
                        {
                            new SavannahTextNode
                            {
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
            var root = new SavannahTagNode
            {
                TagName = "root",
                ChildNodes = new AbstractSavannahXmlNode[]
                {
                    new SavannahTagNode
                    {
                        TagName = "cov",
                        PrioritizeInnerXml = "<test>test<br />aaaa<br />bbb</test>"
                    },
                    new SavannahTextNode
                    {
                        InnerText = "value"
                    }
                }
            };

            var exp = new SavannahTagNode
            {
                TagName = "root",
                ChildNodes = new AbstractSavannahXmlNode[]
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
                    },
                    new SavannahTextNode
                    {
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
            var root = new SavannahTagNode
            {
                TagName = "root",
                PrioritizeInnerXml = "<!--\n  more\n  more2\n-->"
            };

            var exp = new SavannahTagNode
            {
                TagName = "root",
                ChildNodes = new []
                {
                    new SavannahCommentNode
                    {
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
            var root = new SavannahTagNode
            {
                TagName = "root",
                PrioritizeInnerXml = "test\n  test2\ntest3"
            };

            var exp = new SavannahTagNode
            {
                TagName = "root",
                ChildNodes = new[]
                {
                    new SavannahTextNode
                    {
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
            var root = new SavannahTagNode
            {
                TagName = "root",
                PrioritizeInnerXml = "aaa\n<br />\n       bbb\n<br />\nbbb\n<br />\ncccc\n<test>\n  <bbb />\n</test>\n"
            };

            var exp = new SavannahTagNode
            {
                TagName = "root",
                ChildNodes = new AbstractSavannahXmlNode[]
                {
                    new SavannahTextNode
                    {
                        InnerText = "aaa"
                    },
                    new SavannahTagNode
                    {
                        TagName = "br"
                    },
                    new SavannahTextNode
                    {
                        InnerText = "       bbb"
                    },
                    new SavannahTagNode
                    {
                        TagName = "br"
                    },
                    new SavannahTextNode
                    {
                        InnerText = "bbb"
                    },
                    new SavannahTagNode
                    {
                        TagName = "br"
                    },
                    new SavannahTextNode
                    {
                        InnerText = "cccc"
                    },
                    new SavannahTagNode
                    {
                        TagName = "test",
                        ChildNodes = new []
                        {
                            new SavannahTagNode
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
            var root = new SavannahTagNode
            {
                TagName = "root",
                PrioritizeInnerXml = "<test />\naaa\n<test>\n<br />\n</test>\n   b\nccc"
            };

            var exp = new SavannahTagNode
            {
                TagName = "root",
                ChildNodes = new AbstractSavannahXmlNode[]
                {
                    new SavannahTagNode
                    {
                        TagName = "test"
                    },
                    new SavannahTextNode
                    {
                        InnerText = "aaa"
                    },
                    new SavannahTagNode
                    {
                        TagName = "test",
                        ChildNodes = new []
                        {
                            new SavannahTagNode
                            {
                                TagName = "br"
                            },
                        }
                    },
                    new SavannahTextNode
                    {
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
            var textNode = new SavannahTextNode
            {
                InnerText = "aaa"
            };
            var testNode = new SavannahTagNode
            {
                TagName = "test",
                ChildNodes = new[]
                {
                    new SavannahTagNode
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

            var root = new SavannahTagNode
            {
                TagName = "root",
                ChildNodes = new AbstractSavannahXmlNode[]
                {
                    new SavannahTagNode
                    {
                        TagName = "test"
                    },
                    textNode,
                    new SavannahTagNode
                    {
                        TagName = "test",
                        ChildNodes = new []
                        {
                            new SavannahTagNode
                            {
                                TagName = "br"
                            },
                        }
                    },
                    testNode
                }
            };

            var testReader = ((SavannahTagNode)root.ChildNodes.ToArray()[3]).GetReader();
            var resultTestNode = testReader.GetNode("/test/br");

            Assert.AreEqual(resultTestNode, testNode.ChildNodes.First());
        }

        [Test]
        public void AddBeforeChildTest()
        {
            var textNode = new SavannahTextNode
            {
                InnerText = "aaa"
            };
            var testNode = new SavannahTagNode
            {
                TagName = "test",
                ChildNodes = new[]
                {
                    new SavannahTagNode
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

            var root = new SavannahTagNode
            {
                TagName = "root",
                ChildNodes = new AbstractSavannahXmlNode[]
                {
                    new SavannahTagNode
                    {
                        TagName = "test"
                    },
                    textNode,
                    new SavannahTagNode
                    {
                        TagName = "test",
                        ChildNodes = new []
                        {
                            new SavannahTagNode
                            {
                                TagName = "br"
                            },
                        }
                    }
                }
            };

            var exp = new SavannahTagNode
            {
                TagName = "root",
                ChildNodes = new AbstractSavannahXmlNode[]
                {
                    new SavannahTagNode
                    {
                        TagName = "test"
                    },
                    testNode,
                    textNode,
                    new SavannahTagNode
                    {
                        TagName = "test",
                        ChildNodes = new []
                        {
                            new SavannahTagNode
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
            var textNode = new SavannahTextNode
            {
                InnerText = "aaa"
            };
            var testNode = new SavannahTagNode
            {
                TagName = "test",
                ChildNodes = new[]
                {
                    new SavannahTagNode
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

            var root = new SavannahTagNode
            {
                TagName = "root",
                ChildNodes = new AbstractSavannahXmlNode[]
                {
                    new SavannahTagNode
                    {
                        TagName = "test"
                    },
                    textNode,
                    new SavannahTagNode
                    {
                        TagName = "test",
                        ChildNodes = new []
                        {
                            new SavannahTagNode
                            {
                                TagName = "br"
                            },
                        }
                    }
                }
            };

            var exp = new SavannahTagNode
            {
                TagName = "root",
                ChildNodes = new AbstractSavannahXmlNode[]
                {
                    new SavannahTagNode
                    {
                        TagName = "test"
                    },
                    textNode,
                    testNode,
                    new SavannahTagNode
                    {
                        TagName = "test",
                        ChildNodes = new []
                        {
                            new SavannahTagNode
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
            var root = new SavannahTagNode
            {
                TagName = "root",
                PrioritizeInnerXml = "<br />\naaaa\n<br />\nbbb\n<br />\nccc"
            };
            root.ResolvePrioritizeInnerXml();

            var exp = new SavannahTagNode
            {
                TagName = "root",
                ChildNodes = new AbstractSavannahXmlNode[]
                {
                    new SavannahTagNode
                    {
                        TagName = "br"
                    },
                    new SavannahTagNode
                    {
                        TagName = "br"
                    },
                    new SavannahTextNode
                    {
                        InnerText = "aaaa"
                    },
                    new SavannahTextNode
                    {
                        InnerText = "bbb"
                    },
                    new SavannahTagNode
                    {
                        TagName = "br"
                    },
                    new SavannahTextNode
                    {
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
            var commonXmlNode2 = new SavannahTagNode
            {
                TagName = "root",
                ChildNodes = new[]
                {
                    new SavannahTagNode
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
                        ChildNodes = new[]
                        {
                            new SavannahTextNode
                            {
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

        [Test]
        public void AppendAttributeTest()
        {
            var itemNode = SavannahTagNode.CreateElement("item");
            var exp = SavannahTagNode.CreateElement("item", new[] { new AttributeInfo
            {
                Name = "attr",
                Value = "value"
            } });

            itemNode.AppendAttribute("attr", "value");

            Assert.AreEqual(exp, itemNode);
        }

        [Test]
        public void RemoveAttributeTest()
        {
            var itemNode = SavannahTagNode.CreateElement("item", new[] { new AttributeInfo
            {
                Name = "attr",
                Value = "value"
            } });
            var exp = SavannahTagNode.CreateElement("item");

            itemNode.RemoveAttribute("attr");

            Assert.AreEqual(exp, itemNode);
        }

        [Test]
        public void ChangeAttributeTest()
        {
            var itemNode = SavannahTagNode.CreateElement("item", new[] { new AttributeInfo
            {
                Name = "attr",
                Value = "value"
            } });
            var exp = SavannahTagNode.CreateElement("item", new[] { new AttributeInfo
            {
                Name = "attr",
                Value = "value2"
            } });

            itemNode.ChangeAttribute("attr", "value2");

            Assert.AreEqual(exp, itemNode);
        }
    }
}
