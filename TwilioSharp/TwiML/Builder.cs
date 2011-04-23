using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace TwilioSharp.TwiML
{

    /// <summary>
    /// A basic holder class for Elements we are building for our TwiML Response.
    /// </summary>
    public class TwiMLBuilderElement
    {
        public VerbBase Verb { get; set; }
        public List<VerbBase> Children { get; set; }
    }

    /// <summary>
    /// Reasons for the Reject Verb
    /// </summary>
    public enum RejectReason
    {
        /// <summary>
        /// 
        /// </summary>
        rejected = 0,
        /// <summary>
        /// 
        /// </summary>
        busy
    }

    /// <summary>
    /// A fluent like interface for creating TwiML Responses.
    /// </summary>
    public class TwiMLBuilder
    {
        /// <summary>
        /// Gets or sets the elements that have been built so far.
        /// </summary>
        /// <value>The elements.</value>
        public List<TwiMLBuilderElement> Elements { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="TwiMLBuilder"/> class.
        /// </summary>
        private TwiMLBuilder()
        {
            this.Elements = new List<TwiMLBuilderElement>();
        }

        /// <summary>
        /// Builds an instance.
        /// </summary>
        /// <returns>A new instance of the TwiMLBuilder</returns>
        public static TwiMLBuilder Build()
        {
            return new TwiMLBuilder();
        }

        #region Fluent Methods

        /// <summary>
        /// Adds a Say Element to the Response.
        /// </summary>
        /// <param name="thingToSay">The thing to say.</param>
        /// <param name="voiceSex">The voice sex.</param>
        /// <param name="voiceLanguage">The voice language.</param>
        /// <param name="loopTimes">The loop times.</param>
        /// <returns></returns>
        public TwiMLBuilder Say(string thingToSay, SayVoice voiceSex = SayVoice.man, SayLanguage voiceLanguage = SayLanguage.en, uint loopTimes = 1)
        {
            return AddVerb(Verb.Say,
                thingToSay,
                new
                {
                    voice = voiceSex,
                    language = voiceLanguage,
                    loop = loopTimes
                });
        }

        /// <summary>
        /// Adds a Play Element to the Response.
        /// </summary>
        /// <param name="fileUrl">The file URL.</param>
        /// <param name="timesToLoop">The times to loop.</param>
        public TwiMLBuilder Play(string fileUrl, uint timesToLoop = 1)
        {
            return AddVerb(Verb.Play,
                fileUrl,
                new
                {
                    loop = timesToLoop
                });
        }

        /// <summary>
        /// Adds a Gather Element to the response.
        /// </summary>
        /// <param name="actionUrl">The action URL.</param>
        /// <param name="actionMethod">The action method.</param>
        /// <param name="timeoutSeconds">The timeout seconds.</param>
        /// <param name="keyThatFinishes">The key that finishes collection.</param>
        /// <param name="numDigitsToCollect">The num digits to collect.</param>
        /// <param name="children">The children of this element (Can be Say or Play Elements).</param>
        public TwiMLBuilder Gather(string actionUrl = "", ActionMethod actionMethod = ActionMethod.POST, uint timeoutSeconds = 5,
            char keyThatFinishes = '#', uint numDigitsToCollect = 1000, params Tuple<Verb, string, object>[] children)
        {
            var validChildren = new Tuple<Verb, string, object>[] { };
            if (children != null)
                validChildren = children.Where(x => x.Item1 == Verb.Say || x.Item1 == Verb.Play).ToArray();

            return AddVerb(Verb.Gather,
                    string.Empty,
                    new
                    {
                        action = actionUrl,
                        method = actionMethod,
                        timeout = timeoutSeconds,
                        finishOnKey = keyThatFinishes,
                        numDigits = numDigitsToCollect
                    }, validChildren);
        }

        /// <summary>
        /// Adds a Gather element with a nested say element.
        /// </summary>
        /// <param name="textToSay">The text to say.</param>
        /// <param name="actionUrl">The action URL.</param>
        /// <param name="actionMethod">The action method.</param>
        /// <param name="timeoutSeconds">The timeout seconds.</param>
        /// <param name="keyThatFinishes">The key that finishes.</param>
        /// <param name="numDigitsToCollect">The num digits to collect.</param>
        public TwiMLBuilder GatherWhileSaying(string textToSay, SayVoice voiceSex = SayVoice.man, SayLanguage voiceLanguage = SayLanguage.en, int loopTimes = 1,
            string actionUrl = "", ActionMethod actionMethod = ActionMethod.POST, uint timeoutSeconds = 5,
            char keyThatFinishes = '#', uint numDigitsToCollect = 1000)
        {
            var textChild = Tuple.Create(Verb.Say, textToSay, 
                (object)new
                {
                    voice = voiceSex,
                    language = voiceLanguage,
                    loop = loopTimes
                });

            return Gather(actionUrl, actionMethod, timeoutSeconds, keyThatFinishes, numDigitsToCollect, textChild);
        }

        /// <summary>
        /// Adds a Gather Element with a nested Playing Element.
        /// </summary>
        /// <param name="fileUrl">The file URL.</param>
        /// <param name="timesToLoop">The times to loop.</param>
        /// <param name="actionUrl">The action URL.</param>
        /// <param name="actionMethod">The action method.</param>
        /// <param name="timeoutSeconds">The timeout seconds.</param>
        /// <param name="keyThatFinishes">The key that finishes.</param>
        /// <param name="numDigitsToCollect">The num digits to collect.</param>
        public TwiMLBuilder GatherWhilePlaying(string fileUrl, uint timesToLoop = 1,
            string actionUrl = "", ActionMethod actionMethod = ActionMethod.POST, uint timeoutSeconds = 5,
            char keyThatFinishes = '#', uint numDigitsToCollect = 1000)
        {
            var playChild = Tuple.Create(Verb.Play,
                fileUrl,
                (object)new
                {
                    loop = timesToLoop
                });

            return Gather(actionUrl, actionMethod, timeoutSeconds, keyThatFinishes, numDigitsToCollect, playChild);
        }

        /// <summary>
        /// Adds a Record Element with the specified details.
        /// </summary>
        /// <param name="actionUrl">The action URL.</param>
        /// <param name="actionMethod">The action method.</param>
        /// <param name="timeoutSeconds">The timeout seconds.</param>
        /// <param name="keyThatFinishes">The key that finishes.</param>
        /// <param name="maxLengthMilliseconds">The max length milliseconds.</param>
        /// <param name="transcribeAudio">if set to <c>true</c> [transcribe audio].</param>
        /// <param name="transcribeCallbackUrl">The transcribe callback URL.</param>
        /// <param name="playBeepBeforeStart">if set to <c>true</c> [play beep before start].</param>
        public TwiMLBuilder Record(string actionUrl = "", ActionMethod actionMethod = ActionMethod.POST, uint timeoutSeconds = 5,
            char keyThatFinishes = '#', uint maxLengthMilliseconds = 3600,
            bool transcribeAudio = false, string transcribeCallbackUrl = "", bool playBeepBeforeStart = true)
        {
            return AddVerb(Verb.Record,
                string.Empty,
                new
                {
                    action = actionUrl,
                    method = actionMethod,
                    timeout = timeoutSeconds,
                    finishOnKey = keyThatFinishes,
                    maxLength = maxLengthMilliseconds,
                    transcribe = transcribeAudio,
                    transcribeCallback = transcribeCallbackUrl,
                    playBeep = playBeepBeforeStart
                });
        }

        /// <summary>
        /// Adds an Sms Element with the specified details.
        /// </summary>
        /// <param name="messageText">The message text.</param>
        /// <param name="toNumber">To number.</param>
        /// <param name="fromNumber">From number.</param>
        /// <param name="actionUrl">The action URL.</param>
        /// <param name="actionMethod">The action method.</param>
        public TwiMLBuilder Sms(string messageText, string toNumber = "", string fromNumber = "",
            string actionUrl = "", ActionMethod actionMethod = ActionMethod.POST)
        {
            return AddVerb(Verb.Sms,
                messageText,
                new
                {
                    to = toNumber,
                    from = fromNumber,
                    action = actionUrl,
                    method = actionMethod,
                });
        }

        /// <summary>
        /// Adds a Dial and Number Element with the specified details.
        /// </summary>
        /// <param name="numberToDial">The number to dial.</param>
        /// <param name="digitsToSendAfterConnect">The digits to send after connect.</param>
        /// <param name="numberUrl">The number URL.</param>
        /// <param name="actionUrl">The action URL.</param>
        /// <param name="actionMethod">The action method.</param>
        /// <param name="timeoutSeconds">The timeout seconds.</param>
        /// <param name="hangupOnStar">if set to <c>true</c> [hangup on star].</param>
        /// <param name="timeLimitMilliseconds">The time limit milliseconds.</param>
        /// <param name="statusCallbackUrl">The status callback URL.</param>
        /// <param name="callerIdNumber">The caller id number.</param>
        /// <returns></returns>
        public TwiMLBuilder DialNumber(string numberToDial, string digitsToSendAfterConnect = "", string numberUrl = "",
            string actionUrl = "", ActionMethod actionMethod = ActionMethod.POST, uint timeoutSeconds = 30,
            bool hangupOnStar = false, uint timeLimitMilliseconds = 14400,
            string statusCallbackUrl = "", string callerIdNumber = "")
        {
            return AddVerb(Verb.Dial,
                string.Empty,
                new
                {
                    action = actionUrl,
                    method = actionMethod,
                    timeout = timeoutSeconds,
                    hangupOnStar = hangupOnStar,
                    timeLimit = timeLimitMilliseconds,
                    callerId = callerIdNumber
                },
                Tuple.Create(Verb.Number, numberToDial, (object)new { sendDigits = digitsToSendAfterConnect, url = numberUrl }));
        }

        /// <summary>
        /// Adds a Dial and Conference Element with the specified details.
        /// </summary>
        /// <param name="conferenceName">Name of the conference.</param>
        /// <param name="participantMuted">if set to <c>true</c> [participant muted].</param>
        /// <param name="beepOnEnterOrExit">if set to <c>true</c> [beep on enter or exit].</param>
        /// <param name="startConferenceOnEnter">if set to <c>true</c> [start conference on enter].</param>
        /// <param name="endConferenceOnExit">if set to <c>true</c> [end conference on exit].</param>
        /// <param name="waitMusicUrl">The wait music URL.</param>
        /// <param name="waitMusicMethod">The wait music method.</param>
        /// <param name="maxParticipants">The max participants.</param>
        /// <param name="actionUrl">The action URL.</param>
        /// <param name="methodUrl">The method URL.</param>
        /// <param name="timeoutSeconds">The timeout seconds.</param>
        /// <param name="hangupOnStar">if set to <c>true</c> [hangup on star].</param>
        /// <param name="timeLimitMilliseconds">The time limit milliseconds.</param>
        /// <param name="statusCallbackUrl">The status callback URL.</param>
        /// <param name="callerIdNumber">The caller id number.</param>
        public TwiMLBuilder DialConference(string conferenceName, bool participantMuted = false, bool beepOnEnterOrExit = true, bool startConferenceOnEnter = true, bool endConferenceOnExit = false,
            string waitMusicUrl = "", ActionMethod waitMusicMethod = ActionMethod.POST, uint maxParticipants = 40,
            string actionUrl = "", ActionMethod methodUrl = ActionMethod.POST, uint timeoutSeconds = 30,
            bool hangupOnStar = false, uint timeLimitMilliseconds = 14400,
            string statusCallbackUrl = "", string callerIdNumber = "")
        {
            return AddVerb(Verb.Dial,
                string.Empty,
                new
                {
                    action = actionUrl,
                    method = methodUrl,
                    timeout = timeoutSeconds,
                    hangupOnStar = hangupOnStar,
                    timeLimit = timeLimitMilliseconds,
                    callerId = callerIdNumber
                },
                Tuple.Create(Verb.Conference,
                    conferenceName,
                    (object)new
                    {
                        muted = participantMuted,
                        beep = beepOnEnterOrExit,
                        startConferenceOnEnter = startConferenceOnEnter,
                        endConferenceOnExit = endConferenceOnExit,
                        waitUrl = waitMusicUrl,
                        waitMethod = waitMusicMethod
                    })
                );
        }

        /// <summary>
        /// Adds a Hangup Element to the Response.
        /// </summary>
        /// <returns></returns>
        public TwiMLBuilder Hangup()
        {
            return AddVerb(Verb.Hangup,
                string.Empty,
                null);
        }

        /// <summary>
        /// Adds a Redirect Element to the Response.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="method">The method.</param>
        public TwiMLBuilder Redirect(string url, ActionMethod method = ActionMethod.POST)
        {
            return AddVerb(Verb.Redirect,
                url,
                new
                {
                    method = method
                });
        }

        /// <summary>
        /// Adds a Reject Element to the Response
        /// </summary>
        /// <param name="reason">The reason for rejection.</param>
        public TwiMLBuilder Reject(RejectReason reason = RejectReason.rejected)
        {
            return AddVerb(Verb.Reject,
                string.Empty,
                new
                {
                    reason = reason
                });
        }

        /// <summary>
        /// Adds a pause element to the response.
        /// </summary>
        /// <param name="secondsToPause">The seconds to pause.</param>
        /// <returns></returns>
        public TwiMLBuilder Pause(uint secondsToPause = 1)
        {
            return AddVerb(Verb.Pause, string.Empty, new
            {
                length = secondsToPause
            });
        } 

        #endregion

        #region XmlResponse Methods

        /// <summary>
        /// Converts the elements in this instance into a TwiML Response
        /// </summary>
        /// <returns>An TwiML representation of the current Response Elements</returns>
        public XElement ToXmlResponse()
        {
            var root = new XElement("Response");

            foreach (var elem in this.Elements)
            {
                root.Add(CreateXmlFromElement(elem));
            }

            return root;
        }

        private XElement CreateXmlFromElement(TwiMLBuilderElement elem)
        {
            // Create the verb node.
            var root = CreateXmlFromVerb(elem.Verb);

            // Add any children.
            foreach (var childElem in elem.Children)
            {
                root.Add(CreateXmlFromVerb(childElem));
            }

            return root;
        }

        private XElement CreateXmlFromVerb(VerbBase verb)
        {
            // Create the verb node.
            var node = new XElement(verb.Name);
            // Add the value if present.
            if (!string.IsNullOrWhiteSpace(verb.Value))
                node.Value = verb.Value;
            // Add any attributes that aren't empty.
            foreach (var keyVal in verb.Attributes)
            {
                if (!string.IsNullOrWhiteSpace(keyVal.Value))
                    node.Add(new XAttribute(keyVal.Key, keyVal.Value));
            }

            return node;
        } 
        #endregion

        #region Element Building

        private TwiMLBuilder AddVerb(Verb verb, string value, object attributes, params Tuple<Verb, string, object>[] children)
        {
            this.Elements.Add(CreateBuilderElement(verb, value, attributes, children));

            return this;
        }

        private TwiMLBuilderElement CreateBuilderElement(Verb verb, string value, object attributes, params Tuple<Verb, string, object>[] children)
        {
            List<VerbBase> childList = new List<VerbBase>();
            if (children != null)
                childList = children.Select(x => CreateVerbBase(x.Item1, x.Item2, x.Item3)).ToList();

            return new TwiMLBuilderElement()
            {
                Verb = CreateVerbBase(verb, value, attributes),
                Children = childList
            };
        }

        private VerbBase CreateVerbBase(Verb verb, string value, object attributes)
        {
            return new VerbBase(verb)
            {
                Value = value,
                Attributes = CreateAttributeDictionary(attributes)
            };
        }

        private Dictionary<string, string> CreateAttributeDictionary(object attributes)
        {
            var dict = new Dictionary<string, string>();
            if (attributes == null)
                return dict;

            object val;
            foreach (var prop in attributes.GetType().GetProperties())
            {
                try
                {
                    val = prop.GetValue(attributes, null);
                    if (val != null)
                        dict.Add(prop.Name, val.ToString());
                }
                catch
                {
                    // Totally skippin it, don't judge me 
                }
            }

            return dict;
        } 

        #endregion
    }
}
