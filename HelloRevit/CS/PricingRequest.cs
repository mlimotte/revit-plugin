using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.ApplicationServices;
namespace Revit.Pricing
{
    /// <summary>
    /// Serialized for JSON submission to /price1
    /// </summary>
    public class revit_pricing_request
    {
        public string project_id;
        public List<p_element> elements;

        public revit_pricing_request()
        {
            elements = new List<p_element>();
        }
    }
    /// <summary>
    /// Revit Element ID and attributes
    /// </summary>
    public class p_element
    {
        public string id;
        public string name;
        public string element_class_name;
        
        //this will include family name and symbol name for family instance

        public Dictionary<string,string> properties;

        public p_element()
        {
            properties = new Dictionary<string, string>();
        }
    }
 //   /// <summary>
 //   /// Config
 //   /// </summary>
 //public class config
 //   {
 //       public IList<filter> filters;
 //       public config()
 //       {
 //           filters = new List<filter>();
 //       }

 //   }
 //   public class filter
 //   {
 //       public IList<string> categories;
 //       public filter()
 //       {
 //           categories = new List<string>();
 //       }
 //   }

    public static class utilities
    {
        public static revit_pricing_request get_pricing_request(Document curDoc, List<string> categories)
        {
            revit_pricing_request price1 = new revit_pricing_request();

            price1.project_id = curDoc.ProjectInformation.UniqueId.ToString().ToLower().Replace(" ", "_");

            //formatted snake_case
            List<string> formCat = new List<string>();
            foreach (string cate in categories)
            {
                formCat.Add(cate.ToLower().Replace(" ", "_"));
            }


            //filter for family instances
            FilteredElementCollector famCol = new FilteredElementCollector(curDoc);
            famCol.OfClass(typeof(FamilyInstance));
            IList<Element> famElems = famCol.ToElements();
            //iterate
            foreach (Element myE in famElems)
            {


                p_element fIelem = new p_element();
                fIelem.id = myE.Id.ToString().ToLower().Replace(" ", "_");
                fIelem.name = myE.Name.ToLower().Replace(" ", "_");
                fIelem.element_class_name = "family_instance";
                FamilyInstance myFamInst = myE as FamilyInstance;
                fIelem.properties.Add("family_name", myFamInst.Symbol.FamilyName.ToLower().Replace(" ", "_"));
                fIelem.properties.Add("family_symbol_name", myFamInst.Symbol.Name.ToLower().Replace(" ", "_"));

                foreach (Parameter myParam in myFamInst.Parameters)
                {
                    if (!fIelem.properties.ContainsKey(myParam.Definition.Name.ToLower().Replace(" ", "_")) && myParam.AsValueString() != "" && myParam.AsValueString() != "null" && myParam.AsValueString() != null)
                        fIelem.properties.Add(myParam.Definition.Name.ToLower().Replace(" ", "_"), myParam.AsValueString().ToLower().Replace(" ", "_"));

                }

                if (categories.Count!=0)
                {
                    if (fIelem.properties.ContainsKey("category"))
                    {
                        if (formCat.Contains(fIelem.properties["category"]))
                        {
                            price1.elements.Add(fIelem);
                        }
                    }

                }
                else
                {
                    price1.elements.Add(fIelem);
                }

                

            }

            ElementClassFilter FamilyFilter = new ElementClassFilter(typeof(Family));
            FilteredElementCollector FamilyCollector = new FilteredElementCollector(curDoc);
            ICollection<Element> AllFamilies = FamilyCollector.WherePasses(FamilyFilter).ToElements();
            foreach (Family Fmly in AllFamilies)
            {
                string FamilyName = Fmly.Name;



                foreach (ElementId symid in Fmly.GetFamilySymbolIds())
                {
                    FamilySymbol FmlyS = curDoc.GetElement(symid) as FamilySymbol;

                    p_element fIelem = new p_element();
                    fIelem.id = FmlyS.Id.ToString().ToLower().Replace(" ", "_");
                    fIelem.name = FmlyS.Name.ToLower().Replace(" ", "_");
                    fIelem.element_class_name = "family_symbol";

                    fIelem.properties.Add("family_name", FamilyName.ToLower().Replace(" ", "_"));
                    fIelem.properties.Add("family_symbol_name", FmlyS.Name.ToLower().Replace(" ", "_"));



                    foreach (Parameter myParam in FmlyS.Parameters)
                    {

                        if (!fIelem.properties.ContainsKey(myParam.Definition.Name.ToLower().Replace(" ", "_")) && myParam.AsValueString() != "" && myParam.AsValueString() != "null" && myParam.AsValueString() != null)
                            fIelem.properties.Add(myParam.Definition.Name.ToLower().Replace(" ", "_"), myParam.AsValueString().ToLower().Replace(" ", "_"));

                    }

                    if (categories.Count != 0)
                    {
                        if (fIelem.properties.ContainsKey("category"))
                        {
                            if (formCat.Contains(fIelem.properties["category"]))
                            {
                                price1.elements.Add(fIelem);
                            }
                        }

                    }
                    else
                    {
                        price1.elements.Add(fIelem);
                    }
                }



            }
            return price1;

        }

    }
}
