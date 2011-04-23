using System.Web.Mvc;
using System.Xml.Linq;
using System.Xml.Serialization;
using TwilioSharp.TwiML;

namespace TwilioSharp.MVC3.Results
{
    public class TwiMLResult : ActionResult
    {
        private XElement _response;

        /// <summary>
        /// Initializes a new instance of the <see cref="TwiMLResult"/> class with a blank TwiML Response.
        /// </summary>
        public TwiMLResult()
            : this(TwiMLBuilder.Build())
        { }


        /// <summary>
        /// Initializes a new instance of the <see cref="TwiMLResult"/> class.
        /// </summary>
        /// <param name="builder">The builder that contains the response elements.</param>
        public TwiMLResult(TwiMLBuilder builder)
            : this((builder ?? TwiMLBuilder.Build()).ToXmlResponse())
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="TwiMLResult"/> class.
        /// </summary>
        /// <param name="response">The response elements.</param>
        private TwiMLResult(XElement response)
        {
            this._response = response;
        }

        /// <summary>
        /// Generates an Empty TwiML Response.
        /// </summary>
        /// <returns></returns>
        public static TwiMLResult Empty()
        {
            return new TwiMLResult();
        }

        public override void ExecuteResult(ControllerContext context)
        {
            var xs = new XmlSerializer(_response.GetType());
            context.HttpContext.Response.ContentType = "text/xml";

            xs.Serialize(context.HttpContext.Response.Output, _response);
        }
    }
}