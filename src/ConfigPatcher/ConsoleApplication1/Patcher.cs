using System.Xml;

namespace ConfigPatcher
{
    public class Patcher
    {
        private readonly ParseParamaters parseParamaters;

        public Patcher(ParseParamaters parseParamaters)
        {
            this.parseParamaters = parseParamaters;
        }

        private XmlDocument xmlDocument;

        public void Patch()
        {
            this.xmlDocument = new XmlDocument();
            this.xmlDocument.Load(this.parseParamaters.Path);

            this.SetValue("Hostname", this.parseParamaters.Hostname);
            this.SetValue("ConfigUsername", this.parseParamaters.Username);
            this.SetValue("ConfigPassword", this.parseParamaters.Password);
            this.SetValue("ConfigUri", this.parseParamaters.ConfigUri);
            this.SetValue("CheckConfigUpdatedEveryMinutes", this.parseParamaters.ConfigUpdate);
            this.SetValue("MinutesBetweenRetryToGetConfigOnError", this.parseParamaters.ConfigRetry);
            this.SetValue("ConfigCacheLengthMinutes", this.parseParamaters.CacheLength);

            this.xmlDocument.Save(this.parseParamaters.Path);
        }

        private void SetValue(string nodeName, string value)
        {
            var node = string.Format("/configuration/SqlToGraphite/@{0}", nodeName);

            if (this.xmlDocument.SelectSingleNode(node) != null)
            {
                this.xmlDocument.SelectSingleNode(node).Value = value;
            }
            else
            {
                if ((nodeName == "ConfigUsername" || nodeName == "ConfigPassword") && value == string.Empty)
                {
                    //Do not add 
                }
                else
                {
                    var n = xmlDocument.SelectSingleNode(string.Format("/configuration/SqlToGraphite"));
                    XmlAttribute attr = xmlDocument.CreateAttribute(nodeName);
                    attr.Value = value;
                    n.Attributes.Append(attr);                    
                }
            }
        }
    }
}

/*
 * XmlAttribute attr = doc.CreateAttribute("publisher");
    attr.Value = "WorldWide Publishing";
          
    //Add the new node to the document. 
    doc.DocumentElement.SetAttributeNode(attr);
        
*/