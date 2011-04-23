
namespace TwilioSharp.Request
{
    public class RequestBase
    {
        public string CallSid { get; set; }
        public string AccountSid { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public string CallStatus { get; set; }
        public string ApiVersion { get; set; }
        public string Direction { get; set; }
        public string ForwardedFrom { get; set; }
        public string FromCity { get; set; }
        public string FromState { get; set; }
        public string FromZip { get; set; }
        public string FromCountry { get; set; }
        public string ToCity { get; set; }
        public string ToState { get; set; }
        public string ToZip { get; set; }
        public string ToCountry { get; set; }        
    }

    public class TextRequest : RequestBase
    {
        public string SmsSid { get; set; }
        public string SmsStatus { get; set; }
        public string Body { get; set; }
    }

    public class CallRequest : RequestBase
    {
        public string CallStatus { get; set; }
        public string ApiVersion { get; set; }
        public string Direction { get; set; }
        public string ForwardedFrom { get; set; }
    }

    public class CallGatherRequest : CallRequest
    {
        public string Digits { get; set; }
    }

    public class CallRecordRequest : CallRequest
    {
        public string RecordingUrl { get; set; }
        public string RecordingDuration { get; set; }
        public string Digits { get; set; }
    }

    public class CallTranscriptionRequest : CallRequest
    {
        public string TranscriptionText { get; set; }
        public string TranscriptionStatus { get; set; }
        public string TranscriptionUrl { get; set; }
        public string RecordingUrl { get; set; }
    }
}
