using System;

using MeepLib;
using MeepLib.Messages;

namespace MeepRazor.Messages
{
    public class RenderedPage : Message
    {
        /// <summary>
        /// Rendered text/html from running Razor template
        /// </summary>
        /// <value>The body.</value>
        public string Body { get; set; }

        /// <summary>
        /// Name of the template used
        /// </summary>
        /// <value>The template key.</value>
        public string TemplateKey { get; set; }

        public override string ToString()
        {
            return Body;
        }
    }
}
