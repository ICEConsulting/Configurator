using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Tecan_Quote_Generator
{

    [XmlRootAttribute("Profile", Namespace = "", IsNullable = false)]
    public class Profile
    {
        public Profile()
        { 
        }

        [XmlElementAttribute("Name")]
        public string Name;

        [XmlElementAttribute("Email")]
        public string Email;

        [XmlElementAttribute("Phone")]
        public string Phone;

        [XmlElementAttribute("Initials")]
        public string Initials;

        [XmlElementAttribute("DistributionFolder")]
        public string DistributionFolder;

        [XmlElementAttribute("DatabaseCreationDate")]
        public DateTime DatabaseCreationDate;
    }
}
