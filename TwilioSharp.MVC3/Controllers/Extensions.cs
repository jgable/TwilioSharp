using TwilioSharp.MVC3.Results;
using TwilioSharp.TwiML;

namespace TwilioSharp.MVC3.Controllers
{
    public static class Extensions
    {
        public static TwiMLResult ToTwiMLResult(this TwiMLBuilder builder)
        {
            return new TwiMLResult(builder);
        }
    }
}
