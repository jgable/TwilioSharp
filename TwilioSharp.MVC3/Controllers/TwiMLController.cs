
namespace TwilioSharp.MVC3.Controllers
{
    using System;
    using System.Web.Mvc;
    using TwilioSharp.MVC3.Results;
    using TwilioSharp.TwiML;

    public class TwiMLController : Controller
    {
        protected TwiMLResult TwiML(Func<TwiMLBuilder, TwiMLBuilder> responseFactory)
        {
            return new TwiMLResult(responseFactory(TwiMLBuilder.Build()));
        }        
    }
}
