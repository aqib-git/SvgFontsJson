using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Dynamic;
using Newtonsoft.Json;
using System.Linq;

namespace FontsJson
{
    public class FontAwesome5Parser
    {
        public FontAwesome5Parser()
        {
            Parse();
        }

        public void Parse()
        {
            var path = Directory.GetCurrentDirectory() + "/fonts/fontawesome5/svgs";

            var categories = Directory.GetDirectories(path);

            var icons = new Dictionary<string, Dictionary<string, ExpandoObject>>();

            foreach(string category in categories) {
                var match = category.Replace(path + "/", "");
                
                switch(match) {
                    case "brands":
                        icons.Add("fab", Fontawesome5CategoryIcons(category));
                    break;
                    case "regular":
                        icons.Add("far", Fontawesome5CategoryIcons(category));
                    break;
                    case "solid":
                        icons.Add("fas", Fontawesome5CategoryIcons(category));
                    break;
                }
            }

            File.WriteAllLines("json/fontawesome5.json", new List<string>{JsonConvert.SerializeObject(icons)});
        }

        public Dictionary<string, ExpandoObject> Fontawesome5CategoryIcons(string category) {
            var svgs = Directory.GetFiles(category);

            var brandSvgIcons = new Dictionary<string, ExpandoObject>(); 

            foreach(var svg in svgs) {
                var svgIconName = Path.GetFileNameWithoutExtension(svg);
                var xdoc = XDocument.Load(svg);

                dynamic brandSvgIcon = new ExpandoObject();
                
                brandSvgIcon.viewBox = xdoc.Root.Attribute("viewBox").Value;
                
                var path = xdoc.Root.Elements()
                    .Where(e => e.Name.LocalName.Equals("path"))
                    .FirstOrDefault();
                
                if (path != null) {
                    brandSvgIcon.d = path.Attribute("d").Value;
                }

                brandSvgIcons.Add(svgIconName, brandSvgIcon);
            }

            return brandSvgIcons;
        } 
    }
}