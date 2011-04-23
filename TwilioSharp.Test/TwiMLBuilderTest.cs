using TwilioSharp.TwiML;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Xml.Linq;
using System.Collections.Generic;

namespace TwilioSharp.Test
{
    
    
    /// <summary>
    ///This is a test class for TwiMLBuilder
    ///</summary>
    [TestClass]
    public class TwiMLBuilder_Should_Create_Single_Element_Responses
    {
        private class TestNode
        {
            public string Name { get; set; }
            public string Value { get; set; }
            public Dictionary<string, string> Attributes { get; set; }
            public List<TestNode> Children { get; set; }

            public static TestNode Create(string name, string value = "", Dictionary<string, string> dictionary = null, params TestNode[] children)
            {
                return new TestNode()
                {
                    Name = name,
                    Value = value,
                    Attributes = dictionary ?? new Dictionary<string, string>(),
                    Children = children.ToList()
                };
            }
        }

        [TestMethod]
        public void Say()
        {
            var response = TwiMLBuilder
                                .Build()
                                .Say("Something")
                                .ToXmlResponse();

            CommonXMLAssert(response, TestNode.Create("Say", "Something"));
        }

        [TestMethod]
        public void Play()
        {
            var response = TwiMLBuilder
                                .Build()
                                .Play("http://example.org/file.mp3")
                                .ToXmlResponse();

            CommonXMLAssert(response, TestNode.Create("Play", "http://example.org/file.mp3"));
        }

        [TestMethod]
        public void Gather()
        {
            var response = TwiMLBuilder
                                .Build()
                                .Gather()
                                .ToXmlResponse();

            CommonXMLAssert(response, TestNode.Create("Gather"));
        }

        [TestMethod]
        public void Record()
        {
            var response = TwiMLBuilder
                                .Build()
                                .Record()
                                .ToXmlResponse();

            CommonXMLAssert(response, TestNode.Create("Record"));
        }

        [TestMethod]
        public void Sms()
        {
            var response = TwiMLBuilder
                                .Build()
                                .Sms("Text")
                                .ToXmlResponse();

            CommonXMLAssert(response, TestNode.Create("Sms", "Text"));
        }

        [TestMethod]
        public void Dial_Number()
        {
            var response = TwiMLBuilder
                                .Build()
                                .DialNumber("555-867-5309")
                                .ToXmlResponse();

            CommonXMLAssert(response,
                TestNode.Create("Dial", string.Empty, null,
                    TestNode.Create("Number", "555-867-5309")));
        }

        [TestMethod]
        public void Dial_Conference()
        {
            var response = TwiMLBuilder
                                .Build()
                                .DialConference("930-Standup")
                                .ToXmlResponse();

            CommonXMLAssert(response,
                TestNode.Create("Dial", string.Empty, null,
                    TestNode.Create("Conference", "930-Standup")));
        }

        [TestMethod]
        public void Hangup()
        {
            var response = TwiMLBuilder
                                .Build()
                                .Hangup()
                                .ToXmlResponse();

            CommonXMLAssert(response,
                TestNode.Create("Hangup"));
        }

        [TestMethod]
        public void Redirect()
        {
            var response = TwiMLBuilder.Build().Redirect("http://some.org/url").ToXmlResponse();

            CommonXMLAssert(response,
                TestNode.Create("Redirect", "http://some.org/url"));
        }

        [TestMethod]
        public void Reject()
        {
            var response = TwiMLBuilder.Build().Reject().ToXmlResponse();

            CommonXMLAssert(response,
                TestNode.Create("Reject"));
        }

        [TestMethod]
        public void Pause()
        {
            var response = TwiMLBuilder.Build().Pause().ToXmlResponse();

            CommonXMLAssert(response,
                TestNode.Create("Pause"));
        }

        private void CommonXMLAssert(XElement root, params TestNode[] elements)
        {
            var childElems = root.Descendants().ToArray();
            var elementCount = elements.Sum(x => x.Children.Count) + elements.Length;

            Assert.AreEqual(childElems.Count(), elementCount, "Has the same number of elements");
            TestNode currElem;
            for (int i = 0; i < elementCount; i++ )
            {
                currElem = elements[i];
                Assert.AreEqual(childElems[i].Name, currElem.Name, "Has the same name");
                if(!string.IsNullOrWhiteSpace(currElem.Value))
                    Assert.AreEqual(childElems[i].Value, currElem.Value, "Has the same value");

                if (currElem.Children.Count > 0)
                {
                    CommonXMLAssert(childElems[i], currElem.Children.ToArray());
                    i = i + currElem.Children.Count; // Offset the i++
                }
            }
        }
    }
}
