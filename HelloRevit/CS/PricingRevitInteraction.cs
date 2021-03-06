﻿using System;
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

        //WIP - might not be needed
        public static bool UpdateFamilySymbolParameter(Document curDoc, string familyName, string familySymbolName, string parameterName, string parameterValue)
        {
            ElementClassFilter FamilyFilter = new ElementClassFilter(typeof(Family));
            FilteredElementCollector FamilyCollector = new FilteredElementCollector(curDoc);
            List<Element> AllFamilies = FamilyCollector.WherePasses(FamilyFilter).ToElements().Where(myEl => (myEl as Family).Name == familyName).ToList();
            //not found
            if (AllFamilies.Count == 0) return false;
            Family foundFam = AllFamilies[0] as Family;
            bool foundSym = false;
            bool foundPar = false;
            foreach (ElementId symid in foundFam.GetFamilySymbolIds())
            {


                FamilySymbol FmlyS = curDoc.GetElement(symid) as FamilySymbol;
                foundPar = false;
                if (FmlyS.Name == familySymbolName)
                {
                    foundSym = true;


                    foreach (Parameter myParam in FmlyS.Parameters)
                    {

                        if (myParam.Definition.Name == parameterName)
                        {
                            foundPar = true;
                            try
                            {

                            }
                            catch
                            {
                                return false;
                            }
                            foundPar = true;
                            break;
                        }


                    }



                }
                if (foundSym) break;


            }
            if (foundSym && foundPar) return true;
            return false;
        }

        public static bool UpdateElementParameterValueDouble(Document curDoc, string elemeID, string parameterName, double parameterValue)
        {
            Element myElem = curDoc.GetElement(new ElementId(Convert.ToInt32(elemeID)));
            if (myElem == null) return false;
            bool foundPar = false;
            bool setSuccess = false;

            foreach (Parameter myParam in myElem.Parameters)
            {

                if (myParam.Definition.Name.ToLower() == parameterName.ToLower())
                {
                    foundPar = true;
                    try
                    {
                        using (Transaction t = new Transaction(curDoc, "Set Parameter"))
                        {
                            t.Start();


                            if (myParam.StorageType==StorageType.String)
                            {
                                setSuccess=myParam.Set(parameterValue.ToString());
                            }
                            else
                            {
                               setSuccess= myParam.Set(parameterValue);
                            }
                            //myParam.SetValueString(parameterValue);

                           

                            t.Commit();
                        }

                    }
                    catch
                    {
                        return false;
                    }

                    break;
                }


            }
            if (foundPar) return setSuccess;
            //look through family symbol parameters
            FamilyInstance fm = myElem as FamilyInstance;
            if (fm == null) return false;
            FamilySymbol fmsym = fm.Symbol;
            if (fmsym == null) return false;
            setSuccess = false;
            foreach (Parameter myParam in fmsym.Parameters)
            {

                if (myParam.Definition.Name.ToLower() == parameterName.ToLower())
                {
                    foundPar = true;
                    try
                    {
                        using (Transaction t = new Transaction(curDoc, "Set Parameter"))
                        {
                            t.Start();


                            if (myParam.StorageType == StorageType.String)
                            {
                                setSuccess = myParam.Set(parameterValue.ToString());
                            }
                            else
                            {
                                setSuccess = myParam.Set(parameterValue);
                            }

                            t.Commit();
                        }

                    }
                    catch
                    {
                        return false;
                    }

                    break;
                }


            }

            if (foundPar) return setSuccess;
            return false;
        }

        public static bool UpdateElementParameterValueString(Document curDoc, string elemeID, string parameterName, string parameterValue)
        {
            Element myElem = curDoc.GetElement(new ElementId(Convert.ToInt32(elemeID)));
            if (myElem == null) return false;
            bool foundPar = false;
            foreach (Parameter myParam in myElem.Parameters)
            {

                if (myParam.Definition.Name.ToLower() == parameterName.ToLower())
                {
                    foundPar = true;
                    try
                    {
                        using (Transaction t = new Transaction(curDoc, "Set Parameter"))
                        {
                            t.Start();


                            //myParam.SetValueString(parameterValue);

                            myParam.Set(parameterValue);

                            t.Commit();
                        }

                    }
                    catch
                    {
                        return false;
                    }

                    break;
                }


            }
            if (foundPar) return true;
            //look through family symbol parameters
            FamilyInstance fm = myElem as FamilyInstance;
            if (fm == null) return false;
            FamilySymbol fmsym = fm.Symbol;
            if (fmsym == null) return false;
            foreach (Parameter myParam in fmsym.Parameters)
            {

                if (myParam.Definition.Name.ToLower() == parameterName.ToLower())
                {
                    foundPar = true;
                    try
                    {
                        using (Transaction t = new Transaction(curDoc, "Set Parameter"))
                        {
                            t.Start();


                            //myParam.SetValueString(parameterValue);

                            myParam.Set(parameterValue);

                            t.Commit();
                        }

                    }
                    catch
                    {
                        return false;
                    }

                    break;
                }


            }

            if (foundPar) return true;
            return false;
        }


    }
}
