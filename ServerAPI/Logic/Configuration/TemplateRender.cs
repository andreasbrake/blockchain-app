using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

using BlockchainAppAPI.Models.Configuration;

namespace BlockchainAppAPI.Logic.Configuration
{
    public static class TemplateRender
    {
        public static async Task<string> CompilePageWidgets(Page page, Func<string, Task<Widget>> widgetLookup)
        {
            return await TraverseTemplate(page.Template, async (node) => {
                if(node.Name == "Widget") 
                {
                    string id = node.Attributes()
                        .Where(a => 
                            string.Compare(a.Name.LocalName, "id", true) == 0
                        )
                        .SingleOrDefault()
                        .Value;
                    Widget model = await widgetLookup(id);
                    return XElement.Parse(model.Template);
                }
                return node;
            });
        }
        
        public static async Task<IEnumerable<string>> GetTemplateFields(CompiledPageModel page)
        {
            ConcurrentBag<string> fields = new ConcurrentBag<string>();

            await TraverseTemplate(page.CompiledPage, (node) => {
                if(node.Name == "Field") 
                {
                    string id = node.Attributes()
                        .Where(a => 
                            string.Compare(a.Name.LocalName, "id", true) == 0
                        )
                        .SingleOrDefault()
                        .Value;
                    fields.Add(id);
                }
                return Task.FromResult<XElement>(node);
            });

            return fields;
        }

        public static async Task<IEnumerable<string>> GetPageWidgetIds(Page page)
        {
            string template = page.Template;

            List<string> widgetNames = new List<string>();
            await TraverseTemplate(template, (node) => {
                if(node.Name == "Widget") 
                    widgetNames.Add(
                        node.Attributes()
                            .Where(a => 
                                string.Compare(a.Name.LocalName, "id", true) == 0
                            )
                            .SingleOrDefault()
                            .Value
                    );
                return Task.FromResult<XElement>(node);
            });
            return widgetNames;
        }

        private static async Task<string> TraverseTemplate(string template, Func<XElement, Task<XElement>> transformer)
        {
            XDocument doc = XDocument.Parse(template);
            string response = await TraverseTemplateNode(doc.Root, transformer);
            return XDocument.Parse(response).ToString();
        }

        private static async Task<string> TraverseTemplateNode(XElement node, Func<XElement, Task<XElement>> transformer)
        {
            node = await transformer(node);
            List<string> children = new List<string>();
            foreach(XNode child in node.Nodes())
            {
                if(child.NodeType == XmlNodeType.Text) 
                {
                    children.Add(((XText)child).Value);
                }
                else if(child.NodeType == XmlNodeType.Element) 
                {
                    children.Add(await TraverseTemplateNode((XElement)child, transformer));
                }
            }
            return new StringBuilder()
                .Append("<")
                .Append(node.Name.LocalName)
                .Append(node.HasAttributes ? " " +  String.Join(" ", node.Attributes().Select(a => a.ToString())) : "")
                .Append(node.IsEmpty ? " />" : ">")
                .Append(String.Join("", children))
                .Append(!node.IsEmpty ? "</" + node.Name.LocalName + ">" : "")
                .ToString();
        }
    }
}