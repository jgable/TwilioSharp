using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TwilioSharp.TwiML
{
    /// <summary>
    /// TwiML Verbs.
    /// </summary>
    public enum Verb
    {
        Say = 0,
        Play,
        Gather,
        Record,
        Sms,
        Dial,
        Number,
        Conference,
        Hangup,
        Redirect,
        Reject,
        Pause,
    }

    public enum SayVoice
    {
        man = 0,
        woman
    }

    public enum SayLanguage
    {
        en = 0,
        es,
        fr,
        de
    }

    public enum ActionMethod
    {
        GET = 0,
        POST
    }

    public class VerbBase
    {
        public string Name { get; set; }
        public Dictionary<string, string> Attributes { get; set; }
        public string Value { get; set; }

        public VerbBase(Verb verb)
        {
            this.Name = verb.ToString();
            Attributes = new Dictionary<string, string>();
            Value = string.Empty;
        }
    }

    public class NestableVerbBase : VerbBase
    {
        public List<VerbBase> Children { get; set; }

        public NestableVerbBase(Verb verb)
            : base(verb)
        {
            Children = new List<VerbBase>();
        }
    }

    public class SayVerb : VerbBase
    {
        public SayVoice Voice { get; set; }
        public SayLanguage Language { get; set; }
        public int Loop { get; set; }

        public SayVerb(SayVoice voice = SayVoice.man, SayLanguage lang = SayLanguage.en, int loop = 1)
            : base(Verb.Say)
        {
            this.Voice = voice;
            this.Language = lang;
            this.Loop = loop;
        }
    }

    public class PlayVerb : VerbBase
    {
        public int Loop { get; set; }

        public PlayVerb(int loop = 1)
            : base(Verb.Play)
        {
            this.Loop = loop;
        }
    }

    public class GatherVerb : NestableVerbBase
    {
        public string Action { get; set; }
        public ActionMethod Method { get; set; }
        public uint Timeout { get; set; }
        public char FinishOnKey { get; set; }
        public int NumDigits { get; set; }

        public GatherVerb(string action = "", ActionMethod method = ActionMethod.POST, uint timeout = 5, char finishOnKey = '#', int numDigits = 1000)
            : base(Verb.Gather)
        {
            this.Action = action;
            this.Method = method;
            this.Timeout = timeout;
            this.FinishOnKey = finishOnKey;
            this.NumDigits = numDigits;
        }        
    }
}
