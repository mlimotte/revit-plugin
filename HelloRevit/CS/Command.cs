//
// (C) Copyright 2003-2017 by Autodesk, Inc.
//
// Permission to use, copy, modify, and distribute this software in
// object code form for any purpose and without fee is hereby granted,
// provided that the above copyright notice appears in all copies and
// that both that copyright notice and the limited warranty and
// restricted rights notice below appear in all supporting
// documentation.
//
// AUTODESK PROVIDES THIS PROGRAM "AS IS" AND WITH ALL FAULTS.
// AUTODESK SPECIFICALLY DISCLAIMS ANY IMPLIED WARRANTY OF
// MERCHANTABILITY OR FITNESS FOR A PARTICULAR USE. AUTODESK, INC.
// DOES NOT WARRANT THAT THE OPERATION OF THE PROGRAM WILL BE
// UNINTERRUPTED OR ERROR FREE.
//
// Use, duplication, or disclosure by the U.S. Government is subject to
// restrictions set forth in FAR 52.227-19 (Commercial Computer
// Software - Restricted Rights) and DFAR 252.227-7013(c)(1)(ii)
// (Rights in Technical Data and Computer Software), as applicable.
//

using System;
using System.Collections.Generic;
using System.Text;

using Autodesk.Revit;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.ApplicationServices;
using System.IO;
using Newtonsoft.Json;

namespace Revit.Pricing
{
    /// <summary>
    /// Demonstrate how a basic ExternalCommand can be added to the Revit user interface. 
    /// And demonstrate how to create a Revit style dialog.
    /// </summary>
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    [Autodesk.Revit.Attributes.Regeneration(Autodesk.Revit.Attributes.RegenerationOption.Manual)]
    [Autodesk.Revit.Attributes.Journaling(Autodesk.Revit.Attributes.JournalingMode.NoCommandData)]
    public class ExtractModelData : IExternalCommand
    {
        #region IExternalCommand Members

        /// <summary>
        /// Saves Revit model parameters to text file
        /// </summary>
        /// <param name="commandData">An object that is passed to the external application
        /// which contains data related to the command,
        /// such as the application object and active view.</param>
        /// <param name="message">A message that can be set by the external application
        /// which will be displayed if a failure or cancellation is returned by
        /// the external command.</param>
        /// <param name="elements">A set of elements to which the external application
        /// can add elements that are to be highlighted in case of failure or cancellation.</param>
        /// <returns>Return the status of the external command.
        /// A result of Succeeded means that the API external method functioned as expected.
        /// Cancelled can be used to signify that the user cancelled the external operation 
        /// at some point. Failure should be returned if the application is unable to proceed with
        /// the operation.</returns>
        public Autodesk.Revit.UI.Result Execute(ExternalCommandData commandData,
            ref string message, Autodesk.Revit.DB.ElementSet elements)
        {
            // NOTES: Anything can be done in this method, such as create a message box, 
            // a task dialog or fetch some information from revit and so on.
            // We mainly use the task dialog for example.

            // Get the application and document from external command data.
            Application app = commandData.Application.Application;
            Document curDoc = commandData.Application.ActiveUIDocument.Document;

            //for text file
            List<string> lines = new List<string>();

            //get document handle
           
            lines.Add("Document Title: " + curDoc.Title);
            lines.Add("Project Information UniqueID: " + curDoc.ProjectInformation.UniqueId.ToString());
          lines.Add(" ");
            lines.Add("Family Instances:");
            
            
            //filter for family instances
            FilteredElementCollector famCol = new FilteredElementCollector(curDoc);
            famCol.OfClass(typeof(FamilyInstance));
            IList<Element> famElems = famCol.ToElements();
            //iterate
            foreach (Element myE in famElems)
            {
                lines.Add("Element ID:	" + myE.Id.ToString() + "	.	" + "Element Name:	" + myE.Name);
               
                FamilyInstance myFamInst=myE as FamilyInstance;
                lines.Add("Family Symbol: " + myFamInst.Symbol.Name);
                foreach (Parameter myParam in myE.Parameters)
                {



                    lines.Add("	Param ID:	" + myParam.Id.IntegerValue.ToString() + "	,	Param name:	" + myParam.Definition.Name + "	:	Param value:	" + myParam.AsValueString());



                }

            }
            lines.Add(" ");
            lines.Add("Families:");
            ElementClassFilter FamilyFilter = new ElementClassFilter(typeof(Family));
            FilteredElementCollector FamilyCollector = new FilteredElementCollector(curDoc);
            ICollection<Element> AllFamilies = FamilyCollector.WherePasses(FamilyFilter).ToElements();
            foreach (Family Fmly in AllFamilies)
            {
                string FamilyName = Fmly.Name;

                lines.Add("Family ID:	" + Fmly.Id.ToString() + "	,	Family name:	" + FamilyName);
                foreach (Parameter myParam in Fmly.Parameters)
                {

                    lines.Add("	Param ID:	" + myParam.Id.IntegerValue.ToString() + "	,	Param name:	" + myParam.Definition.Name + "	:	Param value:	" + myParam.AsValueString());


                }
               
                lines.Add("Family Symbols:");
                
                foreach (ElementId symid in Fmly.GetFamilySymbolIds())
                {
                    FamilySymbol FmlyS = curDoc.GetElement(symid) as FamilySymbol;

                   

                    lines.Add(" Family Symbol ID:	" + FmlyS.Id.ToString() + "	,	Family Symbol name:	" + FmlyS.Name);
                   
                    foreach (Parameter myParam in FmlyS.Parameters)
                    {

                        lines.Add("	    Param ID:	" + myParam.Id.IntegerValue.ToString() + "	,	Param name:	" + myParam.Definition.Name + "	:	Param value:	" + myParam.AsValueString());


                    }


                }



            }

            System.Windows.Forms.SaveFileDialog saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            saveFileDialog1.Filter = "Text file|*.txt";
            saveFileDialog1.Title = "Save model parameters";
            saveFileDialog1.ShowDialog();

            // If the file name is not an empty string open it for saving.  
            if (saveFileDialog1.FileName != "")
            {
                System.IO.File.WriteAllLines(saveFileDialog1.FileName, lines.ToArray());
            }
            return Autodesk.Revit.UI.Result.Succeeded;
        }

        #endregion
    }
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    [Autodesk.Revit.Attributes.Regeneration(Autodesk.Revit.Attributes.RegenerationOption.Manual)]
    [Autodesk.Revit.Attributes.Journaling(Autodesk.Revit.Attributes.JournalingMode.NoCommandData)]
    public class APIHealthCheck : IExternalCommand
    {
        #region IExternalCommand Members

        /// <summary>
        /// Checks on API address
        /// </summary>
        /// <param name="commandData">An object that is passed to the external application
        /// which contains data related to the command,
        /// such as the application object and active view.</param>
        /// <param name="message">A message that can be set by the external application
        /// which will be displayed if a failure or cancellation is returned by
        /// the external command.</param>
        /// <param name="elements">A set of elements to which the external application
        /// can add elements that are to be highlighted in case of failure or cancellation.</param>
        /// <returns>Return the status of the external command.
        /// A result of Succeeded means that the API external method functioned as expected.
        /// Cancelled can be used to signify that the user cancelled the external operation 
        /// at some point. Failure should be returned if the application is unable to proceed with
        /// the operation.</returns>
        public Autodesk.Revit.UI.Result Execute(ExternalCommandData commandData,
            ref string message, Autodesk.Revit.DB.ElementSet elements)
        {

            PricingHttpRest.InitializeClient("http://api.fairhomemaine.com", "");

            try
            {
                string result = PricingHttpRest.Get();
                
                System.Windows.Forms.MessageBox.Show("Status code: " +PricingHttpRest.receivedCode+" Reason: "+result +" Received content: "+ PricingHttpRest.receivedContent);
            }
            catch(Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("Error accessing API");
            }

            return Autodesk.Revit.UI.Result.Succeeded;
        }

        #endregion
    }


    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    [Autodesk.Revit.Attributes.Regeneration(Autodesk.Revit.Attributes.RegenerationOption.Manual)]
    [Autodesk.Revit.Attributes.Journaling(Autodesk.Revit.Attributes.JournalingMode.NoCommandData)]
    public class GetConfiguration : IExternalCommand
    {
        #region IExternalCommand Members

        /// <summary>
        /// Gets config
        /// </summary>
        /// <param name="commandData">An object that is passed to the external application
        /// which contains data related to the command,
        /// such as the application object and active view.</param>
        /// <param name="message">A message that can be set by the external application
        /// which will be displayed if a failure or cancellation is returned by
        /// the external command.</param>
        /// <param name="elements">A set of elements to which the external application
        /// can add elements that are to be highlighted in case of failure or cancellation.</param>
        /// <returns>Return the status of the external command.
        /// A result of Succeeded means that the API external method functioned as expected.
        /// Cancelled can be used to signify that the user cancelled the external operation 
        /// at some point. Failure should be returned if the application is unable to proceed with
        /// the operation.</returns>
        public Autodesk.Revit.UI.Result Execute(ExternalCommandData commandData,
            ref string message, Autodesk.Revit.DB.ElementSet elements)
        {

            PricingHttpRest.InitializeClient("http://api.fairhomemaine.com", "");

            try
            {
                PricingHttpRest.APICommand = "pricing/config";
                string result = PricingHttpRest.Get();

                System.Windows.Forms.MessageBox.Show("Status code: " + PricingHttpRest.receivedCode + " Reason: " + result + " Received content: " + PricingHttpRest.receivedContent);
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("Error accessing API");
            }

            return Autodesk.Revit.UI.Result.Succeeded;
        }

        #endregion
    }

    /// <summary>
    /// Demonstrate how a basic ExternalCommand can be added to the Revit user interface. 
    /// And demonstrate how to create a Revit style dialog.
    /// </summary>
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    [Autodesk.Revit.Attributes.Regeneration(Autodesk.Revit.Attributes.RegenerationOption.Manual)]
    [Autodesk.Revit.Attributes.Journaling(Autodesk.Revit.Attributes.JournalingMode.NoCommandData)]
    public class PriceRequest : IExternalCommand
    {
        #region IExternalCommand Members

        /// <summary>
        /// Submits pricing request
        /// </summary>
        /// <param name="commandData">An object that is passed to the external application
        /// which contains data related to the command,
        /// such as the application object and active view.</param>
        /// <param name="message">A message that can be set by the external application
        /// which will be displayed if a failure or cancellation is returned by
        /// the external command.</param>
        /// <param name="elements">A set of elements to which the external application
        /// can add elements that are to be highlighted in case of failure or cancellation.</param>
        /// <returns>Return the status of the external command.
        /// A result of Succeeded means that the API external method functioned as expected.
        /// Cancelled can be used to signify that the user cancelled the external operation 
        /// at some point. Failure should be returned if the application is unable to proceed with
        /// the operation.</returns>
        public Autodesk.Revit.UI.Result Execute(ExternalCommandData commandData,
            ref string message, Autodesk.Revit.DB.ElementSet elements)
        {
            // NOTES: Anything can be done in this method, such as create a message box, 
            // a task dialog or fetch some information from revit and so on.
            // We mainly use the task dialog for example.

            // Get the application and document from external command data.
            Application app = commandData.Application.Application;
            Document curDoc = commandData.Application.ActiveUIDocument.Document;

            RevitPricingRequest price1 = new RevitPricingRequest();

            price1.projectID = curDoc.ProjectInformation.UniqueId.ToString();





            //filter for family instances
            FilteredElementCollector famCol = new FilteredElementCollector(curDoc);
           famCol.OfClass(typeof(FamilyInstance));
            IList<Element> famElems = famCol.ToElements();
            //iterate
            foreach (Element myE in famElems)
            {


                pElement fIelem = new pElement();
                fIelem.id = myE.Id.ToString();
                fIelem.name = myE.Name;
                fIelem.elementClassName = "FamilyInstance";
                FamilyInstance myFamInst = myE as FamilyInstance;
                fIelem.properties.Add("Family Name", myFamInst.Symbol.FamilyName);
                fIelem.properties.Add("Family Symbol", myFamInst.Symbol.Name);

                foreach (Parameter myParam in myFamInst.Parameters)
                {
                    if (!fIelem.properties.ContainsKey(myParam.Definition.Name) && myParam.AsValueString()!="" && myParam.AsValueString() != "null" && myParam.AsValueString() != null)
                    fIelem.properties.Add(myParam.Definition.Name, myParam.AsValueString());
                 
                }

                price1.elements.Add(fIelem);

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

                    pElement fIelem = new pElement();
                    fIelem.id = FmlyS.Id.ToString();
                    fIelem.name = FmlyS.Name;
                    fIelem.elementClassName = "FamilySymbol";

                    fIelem.properties.Add("Family Name", FamilyName);
                    fIelem.properties.Add("Family Symbol", FmlyS.Name);


                   
                    foreach (Parameter myParam in FmlyS.Parameters)
                    {

                        if (!fIelem.properties.ContainsKey(myParam.Definition.Name) && myParam.AsValueString() != "" && myParam.AsValueString() != "null" && myParam.AsValueString() != null)
                            fIelem.properties.Add(myParam.Definition.Name, myParam.AsValueString());

                    }

                    price1.elements.Add(fIelem);
                }



            }

            string serialS = JsonConvert.SerializeObject(price1);
            System.Windows.Forms.SaveFileDialog saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            saveFileDialog1.Filter = "Text file|*.txt";
            saveFileDialog1.Title = "Save Query";
            saveFileDialog1.ShowDialog();

            // If the file name is not an empty string open it for saving.  
            if (saveFileDialog1.FileName != "")
            {
                System.IO.File.WriteAllText(saveFileDialog1.FileName,serialS);
            }

            return Autodesk.Revit.UI.Result.Succeeded;
        }

        #endregion
    }
}
