using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Dynamic;
using Newtonsoft.Json;
using System.Linq;
using System.Text.RegularExpressions;

namespace FontsJson
{
    public class MaterialDesignIconsParser
    {
        public MaterialDesignIconsParser()
        {
            Parse();
        }

        public void Parse()
        {
            var path = Directory.GetCurrentDirectory() + "/fonts/material-design-icons";

            var categories = Directory.GetDirectories(path);

            var icons = new Dictionary<string, Dictionary<string, ExpandoObject>>();

            foreach(string category in categories) {
                var categoryName = category.Replace(path + "/", "");
                
                icons.Add(categoryName, ParseCategoryIcons(category));
            }

            File.WriteAllLines("json/materialdesignicons.json", new List<string>{JsonConvert.SerializeObject(icons)});
        }

        public Dictionary<string, ExpandoObject> ParseCategoryIcons(string category) {
            var svgs = Directory.GetFiles($"{category}/svg/production");

            var brandSvgIcons = new Dictionary<string, ExpandoObject>(); 

            foreach(var svg in svgs) {
                var svgIconName = Path.GetFileNameWithoutExtension(svg);

                if (!svgIconName.EndsWith("_48px")) {
                    continue;
                }

                var xdoc = XDocument.Load(svg);

                dynamic brandSvgIcon = new ExpandoObject();
                
                brandSvgIcon.viewBox = xdoc.Root.Attribute("viewBox").Value;
                
                var path = xdoc.Root.Elements()
                    .Where(e => e.Name.LocalName.Equals("path"))
                    .FirstOrDefault();
                
                if (path != null) {
                    brandSvgIcon.d = path.Attribute("d").Value;
                }

                svgIconName = Regex.Match(svgIconName, "ic_(.*)_48px").Groups[1].Value;

                brandSvgIcons.Add(svgIconName, brandSvgIcon);
            }

            return brandSvgIcons;
        } 
    }
}