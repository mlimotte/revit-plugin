using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.ApplicationServices;

namespace Revit.Pricing
{
    //methods for retrieving and updating parameter values
    public static class PricingRevitInteraction
    {
        //public static bool UpdateFamilySymbolParameter(Document curDoc,string familyName,string familySymbolName,string parameterName, string parameterValue)
        //{
        //    ElementClassFilter FamilyFilter = new ElementClassFilter(typeof(Family));
        //    FilteredElementCollector FamilyCollector = new FilteredElementCollector(curDoc);
        //    ICollection<Element> AllFamilies = FamilyCollector.WherePasses(FamilyFilter).ToElements().Where(myEl => (myEl as Family).Name == familyName); ;
        //    foreach (Family Fmly in AllFamilies)
        //    {
        //        string FamilyName = Fmly.Name;

        //        lines.Add("Family ID:	" + Fmly.Id.ToString() + "	,	Family name:	" + FamilyName);
        //        foreach (Parameter myParam in Fmly.Parameters)
        //        {

        //            lines.Add("	Param ID:	" + myParam.Id.IntegerValue.ToString() + "	,	Param name:	" + myParam.Definition.Name + "	:	Param value:	" + myParam.AsValueString());


        //        }

        //        lines.Add("Family Symbols:");

        //        foreach (ElementId symid in Fmly.GetFamilySymbolIds())
        //        {
        //            FamilySymbol FmlyS = curDoc.GetElement(symid) as FamilySymbol;



        //            lines.Add(" Family Symbol ID:	" + FmlyS.Id.ToString() + "	,	Family Symbol name:	" + FmlyS.Name);

        //            foreach (Parameter myParam in FmlyS.Parameters)
        //            {

        //                lines.Add("	    Param ID:	" + myParam.Id.IntegerValue.ToString() + "	,	Param name:	" + myParam.Definition.Name + "	:	Param value:	" + myParam.AsValueString());


        //            }


        //        }



        //    }
        //}
    }
}
