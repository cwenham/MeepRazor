using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.IO;

using SmartFormat;
using NLog;
using RazorEngine;
using RazorEngine.Templating;

using MeepLib;
using MeepLib.Messages;
using MeepLib.MeepLang;

using MeepRazor.Messages;

namespace MeepRazor
{
    [MeepNamespace(Extensions.PluginNamespace)]
    [Macro(Name = "Render", DefaultProperty = "Template", Position = MacroPosition.Downstream)]
    public class Render : AMessageModule
    {
        /// <summary>
        /// Name of a Razor template in {Smart.Format}
        /// </summary>
        /// <value>The template file.</value>
        public string Template { get; set; }

        /// <summary>
        /// Path to a template file. Alternative to inline templates
        /// </summary>
        /// <value>The file.</value>
        public string Path { get; set; }

        /// <summary>
        /// User defined inline templates
        /// </summary>
        /// <value>The templates.</value>
        public IEnumerable<Config.Template> Templates
        {
            get
            {
                return Config.OfType<Config.Template>();
            }
        }

        public override async Task<Message> HandleMessage(Message msg)
        {
            MessageContext context = new MessageContext(msg, this);
            string templateBody = null;
            string templateKey = null;
            string renderedPage = null;

            return await Task.Run<Message>(() =>
            {
                try
                {
                    if (!String.IsNullOrWhiteSpace(Template))
                    {
                        templateKey = Smart.Format(Template, context);

                        var inlineTemplate = Templates.Where(x => x.Name == templateKey).FirstOrDefault();
                        if (inlineTemplate != null)
                            templateBody = inlineTemplate.Content;
                    }

                    if (!String.IsNullOrWhiteSpace(Path) && File.Exists(Path))
                    {
                        templateKey = Path;
                        templateBody = File.ReadAllText(Path);
                    }

                    if (!Engine.Razor.IsTemplateCached(templateKey, msg.GetType()))
                        renderedPage = Engine.Razor.RunCompile(templateKey, msg.GetType(), msg, null);
                    else
                        renderedPage = Engine.Razor.Run(templateKey, msg.GetType(), msg, null);

                    return new RenderedPage
                    {
                        DerivedFrom = msg,
                        Body = templateBody,
                        TemplateKey = templateKey
                    };
                }
                catch (Exception ex)
                {
                    logger.Error(ex, "{0} thrown when rendering Razor template: {1}", ex.GetType().Name, ex.Message);
                    return null;
                }
            });
        }
    }
}
