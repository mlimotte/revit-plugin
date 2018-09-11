using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Revit.Pricing
{
    /// <summary>
    /// Serialized for JSON submission to /price1
    /// </summary>
    public class RevitPricingRequest
    {
        public string projectID;
        public List<pElement> elements;

        public RevitPricingRequest()
        {
            elements = new List<pElement>();
        }
    }
    /// <summary>
    /// Revit Element ID and attributes
    /// </summary>
    public class pElement
    {
        public string id;
        public string name;
        public string elementClassName;
        
        //this will include family name and symbol name for family instance

        public Dictionary<string,string> properties;

        public pElement()
        {
            properties = new Dictionary<string, string>();
        }
    }
    ///// <summary>
    ///// Revit Element Property
    ///// </summary>
    //public class pProperty
    //{
        
    //}
}
