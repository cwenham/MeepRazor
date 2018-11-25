using System;
using System.Xml;
using MeepLib;
using MeepLib.MeepLang;

namespace MeepRazor.Config
{
    /// <summary>
    /// Container for Razor template
    /// </summary>
    [MeepNamespace(Extensions.PluginNamespace)]
    public class Template : ANamable, IMeepDeserialisable
    {
        public string Content { get; set; }

        public void ReadXML(XmlReader reader)
        {
            while (reader.NodeType != XmlNodeType.Text && reader.NodeType != XmlNodeType.EndElement)
            {
                if (!reader.Read())
                    return;

                if (reader.NodeType == XmlNodeType.Text)
                    Content = reader.ReadContentAsString();
            }
        }
    }
}
