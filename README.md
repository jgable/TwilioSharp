# TwilioSharp - C# Helpers for creating TwiML

TwilioSharp is a set of C# Class Libraries that help you create applications that take advantage of [Twilio's Awesome TwiML APIs](http://www.twilio.com/docs/api/twiml/).

**Pre-Requisites**
-  Download and Install [ASP.Net MVC 3 Tools](http://asp.net/mvc).
-  Setup a new account and phone number at [Twilio](http://twilio.com).
-  Create a new MVC 3 Site and put it up on the web (I recommend [AppHarbor](https://appharbor.com/account/new?referrerUsername=jacob4u2)).

### Show teh Codez - Reply To A Text Message From MVC 3

```
using System.Web.Mvc;
using TwilioSharp.MVC3.Controllers;
using TwilioSharp.Request;

public class TextController : TwiMLController
{
    [HttpPost]
    public ActionResult New(TextRequest request)
    {
        var answer = string.Format("{0} {1}, {2}?", "Hello! How's the weather today in", request.FromCity, request.FromState);
            
        return TwiML(response => response
                                    .Sms(answer));

        // Alternatively:
        //return TwiMLBuilder
        //            .Build()
        //            .Sms(answer)
        //            .ToTwiMLResult();
    }
}
```

### Show more Codez - Answer a Phone Call

```
using System.Web.Mvc;
using _8Ball.Common;
using TwilioSharp.MVC3.Controllers;
using TwilioSharp.Request;

public class CallController : TwiMLController
{
    [HttpPost]
    public ActionResult New(CallRequest request)
    {
        return TwiML(response => response
                                    .Say("Thanks for calling the All Knowing Magical 8 Ball.")
                                    .Say("Ask a Question after the Beep.")
                                    .Say("Press Pound when done.")
                                    .Record(Url.Action("Question")));
    }

    [HttpPost]
    public ActionResult Question(CallRecordRequest request)
    {
        return TwiML(response => response
                                    .Say("The Magical 8 Ball Says")
                                    .Say(Magic8BallAnswerizer3000.GetAnswer())
                                    .Pause(1)                                        
                                    .GatherWhileSaying("Press Any Key To Ask Another Question.  Or Pound to Exit.", 
                                        actionUrl: Url.Action("New"),
                                        timeoutSeconds: 3)
                                    .Say("Goodbye")
                                    .Hangup());
    }
}
```
