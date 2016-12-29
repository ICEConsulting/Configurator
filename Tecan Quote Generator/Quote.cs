using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Tecan_Quote_Generator
{

    [XmlRootAttribute("Quote", Namespace = "", IsNullable = false)]
    public class Quote
    {
        public Quote()
        {
        }

        [XmlElementAttribute("Account")]
        public short QuoteAccount;

        [XmlElementAttribute("Contact")]
        public short QuoteContact;

        [XmlElementAttribute("Title")]
        public string QuoteTitle;

        [XmlElementAttribute("Date")]
        public string QuoteDate;

        [XmlElementAttribute("Description")]
        public string QuoteDescription;

        [XmlElementAttribute("Type")]
        public short QuoteType;

        [XmlElementAttribute("isSSP")]
        public bool QuoteisSSP;

        [XmlElementAttribute("Template")]
        public short QuoteTemplate;

        // Serializes an ArrayList as a "Items" array of XML elements of custom type QuoteItems named "Items".
        [XmlArray("Items"), XmlArrayItem("Item", typeof(QuoteItems))]
        public System.Collections.ArrayList Items = new System.Collections.ArrayList();

        // Serializes an ArrayList as a "Options" array of XML elements of custom type QuoteItems named "Options".
        [XmlArray("Options"), XmlArrayItem("Item", typeof(QuoteItems))]
        public System.Collections.ArrayList Options = new System.Collections.ArrayList();

        //// Serializes an ArrayList as a "ThirdParty" array of XML elements of custom type QuoteItems named "ThirdParty".
        //[XmlArray("ThirdParty"), XmlArrayItem("Item", typeof(QuoteItems))]
        //public System.Collections.ArrayList ThirdParty = new System.Collections.ArrayList();

        //// Serializes an ArrayList as a "SmartStart" array of XML elements of custom type QuoteItems named "SmartStart".
        //[XmlArray("SmartStart"), XmlArrayItem("Item", typeof(QuoteItems))]
        //public System.Collections.ArrayList SmartStart = new System.Collections.ArrayList();

    }
}
