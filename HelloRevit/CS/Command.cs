

using System;
using System.Collections.Generic;
using System.Text;

using Autodesk.Revit;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.ApplicationServices;
using System.IO;
using Newtonsoft.Json;
using System.Reflection;

namespace Revit.Pricing
{
    
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

            //loads from settings.txt
            settings.LoadSettings();

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


                    if (myParam.StorageType==StorageType.String)
                    {
                        lines.Add("	Param ID:	" + myParam.Id.IntegerValue.ToString() + "	,	Param name:	" + myParam.Definition.Name + "	:	Param value:	" + myParam.AsString());
                    }
                    else
                    {
                        lines.Add("	Param ID:	" + myParam.Id.IntegerValue.ToString() + "	,	Param name:	" + myParam.Definition.Name + "	:	Param value:	" + myParam.AsValueString());
                    }
                    



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

                        if (myParam.StorageType == StorageType.String)
                        {
                            lines.Add("	Param ID:	" + myParam.Id.IntegerValue.ToString() + "	,	Param name:	" + myParam.Definition.Name + "	:	Param value:	" + myParam.AsString());
                        }
                        else
                        {
                            lines.Add("	Param ID:	" + myParam.Id.IntegerValue.ToString() + "	,	Param name:	" + myParam.Definition.Name + "	:	Param value:	" + myParam.AsValueString());
                        }

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
        /// Checks at API address
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

            //loads from settings.txt
            settings.LoadSettings();
            PricingHttpRest.InitializeClient(settings.APILocation, "");

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
    public class TestGetConfiguration : IExternalCommand
    {
        #region IExternalCommand Members

        /// <summary>
        /// Gets config for testing purposes, displays
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

            //loads from settings.txt
            settings.LoadSettings();
            PricingHttpRest.InitializeClient(settings.APILocation, "");

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

    
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    [Autodesk.Revit.Attributes.Regeneration(Autodesk.Revit.Attributes.RegenerationOption.Manual)]
    [Autodesk.Revit.Attributes.Journaling(Autodesk.Revit.Attributes.JournalingMode.NoCommandData)]
    public class TestPriceRequest : IExternalCommand
    {
        #region IExternalCommand Members

        /// <summary>
        /// generates pricing request JSON - no category filter -  save as file
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

            //loads from settings.txt
            settings.LoadSettings();
            revit_pricing_request price1 = utilities.get_pricing_request(curDoc, new List<string>());

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


    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    [Autodesk.Revit.Attributes.Regeneration(Autodesk.Revit.Attributes.RegenerationOption.Manual)]
    [Autodesk.Revit.Attributes.Journaling(Autodesk.Revit.Attributes.JournalingMode.NoCommandData)]
    public class TestPriceRequestCategories : IExternalCommand
    {
        #region IExternalCommand Members

        /// <summary>
        /// generates pricing request JSON - retrieves categories from server -  save as file
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


            //loads from settings.txt
            settings.LoadSettings();

            List<string> cats = new List<string>();

            PricingHttpRest.InitializeClient(settings.APILocation, "");
            string configJSON = "";
            try
            {
                PricingHttpRest.APICommand = "pricing/config";
                string result = PricingHttpRest.Get();

                configJSON= PricingHttpRest.receivedContent;
                if (result != "OK")
                {
                    System.Windows.Forms.MessageBox.Show("Error accessing API");
                    return Autodesk.Revit.UI.Result.Failed;
                }
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("Error accessing API");
                return Autodesk.Revit.UI.Result.Failed;
            }

           
            try
            {



                Newtonsoft.Json.Linq.JToken jt = Newtonsoft.Json.Linq.JObject.Parse(configJSON);
                Newtonsoft.Json.Linq.JToken jfil = jt.SelectToken("filters");
                Newtonsoft.Json.Linq.JToken jcat = jfil.SelectToken("categories");
                 cats = jcat.ToObject<List<string>>();

            }
            catch(Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("Deserialization failure: "+ ex.ToString());
                return Autodesk.Revit.UI.Result.Failed;
            }
            if (cats.Count == 0)
                System.Windows.Forms.MessageBox.Show("No categories found, taking all");
            //if (myConf.filters.Count==0)
            //{
            //    System.Windows.Forms.MessageBox.Show("No config data");
            //    return Autodesk.Revit.UI.Result.Failed;
            //}

            revit_pricing_request price1 = utilities.get_pricing_request(curDoc, cats);



            string serialS = JsonConvert.SerializeObject(price1);
            System.Windows.Forms.SaveFileDialog saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            saveFileDialog1.Filter = "Text file|*.txt";
            saveFileDialog1.Title = "Save Query";
            saveFileDialog1.ShowDialog();

            // If the file name is not an empty string open it for saving.  
            if (saveFileDialog1.FileName != "")
            {
                System.IO.File.WriteAllText(saveFileDialog1.FileName, serialS);
            }

            return Autodesk.Revit.UI.Result.Succeeded;
        }

        #endregion
    }


    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    [Autodesk.Revit.Attributes.Regeneration(Autodesk.Revit.Attributes.RegenerationOption.Manual)]
    [Autodesk.Revit.Attributes.Journaling(Autodesk.Revit.Attributes.JournalingMode.NoCommandData)]
    public class TestParameterChange : IExternalCommand
    {
        #region IExternalCommand Members

        /// <summary>
        /// Tests function to change parameter value
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

            //loads from settings.txt
            settings.LoadSettings();
            PricingRevitInteraction.UpdateElementParameterValueString(curDoc, "352672", "Mark", "Testcomments");

            using (Transaction t = new Transaction(curDoc, "Regen"))
            {
                t.Start();


                curDoc.Regenerate();
                t.Commit();
            }

            
            return Autodesk.Revit.UI.Result.Succeeded;
        }

        #endregion
    }


    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    [Autodesk.Revit.Attributes.Regeneration(Autodesk.Revit.Attributes.RegenerationOption.Manual)]
    [Autodesk.Revit.Attributes.Journaling(Autodesk.Revit.Attributes.JournalingMode.NoCommandData)]
    public class TestPriceRequestResponse : IExternalCommand
    {
        #region IExternalCommand Members

        /// <summary>
        /// retrieves categories from server, generates price requests, sends, receives prices and saves to text file
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

            //loads from settings.txt
            settings.LoadSettings();
            List<string> cats = new List<string>();

            PricingHttpRest.InitializeClient(settings.APILocation, "");
            string configJSON = "";
            try
            {
                PricingHttpRest.APICommand = "pricing/config";
                string result = PricingHttpRest.Get();

                configJSON = PricingHttpRest.receivedContent;
                if (result != "OK")
                {
                    System.Windows.Forms.MessageBox.Show("Error accessing API while getting config");
                    return Autodesk.Revit.UI.Result.Failed;
                }
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("Error accessing API while getting config");
                return Autodesk.Revit.UI.Result.Failed;
            }


            try
            {



                Newtonsoft.Json.Linq.JToken jt = Newtonsoft.Json.Linq.JObject.Parse(configJSON);
                Newtonsoft.Json.Linq.JToken jfil = jt.SelectToken("filters");
                Newtonsoft.Json.Linq.JToken jcat = jfil.SelectToken("categories");
                cats = jcat.ToObject<List<string>>();

            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("Deserialization failure: " + ex.ToString());
                return Autodesk.Revit.UI.Result.Failed;
            }
            if (cats.Count == 0)
                System.Windows.Forms.MessageBox.Show("No categories found, taking all");
            //if (myConf.filters.Count==0)
            //{
            //    System.Windows.Forms.MessageBox.Show("No config data");
            //    return Autodesk.Revit.UI.Result.Failed;
            //}

            revit_pricing_request price1 = utilities.get_pricing_request(curDoc, cats);


            //send element data
            string serialS = JsonConvert.SerializeObject(price1);
            string pricesJSON = "";
            try
            {
                PricingHttpRest.APICommand = "pricing/model1";
                PricingHttpRest.sendContent = serialS;

                string result = PricingHttpRest.Post();

                pricesJSON = PricingHttpRest.receivedContent;
                if (result != "OK")
                {
                    System.Windows.Forms.MessageBox.Show("Error accessing API while submitting  data for pricing: " + result + " " + pricesJSON);
                    return Autodesk.Revit.UI.Result.Failed;
                }
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("Error accessing API while submitting  data for pricing: " + ex.ToString());
                return Autodesk.Revit.UI.Result.Failed;
            }


            System.Windows.Forms.SaveFileDialog saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            saveFileDialog1.Filter = "Text file|*.txt";
            saveFileDialog1.Title = "Save Price / Instance Data";
            saveFileDialog1.ShowDialog();

            // If the file name is not an empty string open it for saving.  
            if (saveFileDialog1.FileName != "")
            {
                System.IO.File.WriteAllText(saveFileDialog1.FileName, pricesJSON);
            }

            return Autodesk.Revit.UI.Result.Succeeded;
        }
        #endregion
    }


    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    [Autodesk.Revit.Attributes.Regeneration(Autodesk.Revit.Attributes.RegenerationOption.Manual)]
    [Autodesk.Revit.Attributes.Journaling(Autodesk.Revit.Attributes.JournalingMode.NoCommandData)]
    public class TestSamplePriceRequestResponse : IExternalCommand
    {
        #region IExternalCommand Members

        /// <summary>
        /// reads price request from text file, sends to server, saves response to text file
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

            //loads from settings.txt
            settings.LoadSettings();
            List<string> cats = new List<string>();

            PricingHttpRest.InitializeClient(settings.APILocation, "");// "https://ce7649b3.ngrok.io", "");

            // Displays an OpenFileDialog so the user can select a Cursor.  
            System.Windows.Forms.OpenFileDialog openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            openFileDialog1.Filter = "Text files|*.txt";
            openFileDialog1.Title = "Select a text file with sample price query";

           
            if (openFileDialog1.ShowDialog() != System.Windows.Forms.DialogResult.OK)
            {
                return Autodesk.Revit.UI.Result.Succeeded;
            }

            //send element data
            string serialS = File.ReadAllText(openFileDialog1.FileName);
            string pricesJSON = "";
            try
            {
                PricingHttpRest.APICommand = "pricing/model1";
                PricingHttpRest.sendContent = serialS;

                string result = PricingHttpRest.Post();

                pricesJSON = PricingHttpRest.receivedContent;
                if (result != "OK")
                {
                    System.Windows.Forms.MessageBox.Show("Error accessing API while submitting sample data for pricing: " + result+" "+pricesJSON);
                    return Autodesk.Revit.UI.Result.Failed;
                }
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("Error accessing API while submitting sample data for pricing: "+ex.ToString());
                return Autodesk.Revit.UI.Result.Failed;
            }


            System.Windows.Forms.SaveFileDialog saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            saveFileDialog1.Filter = "Text file|*.txt";
            saveFileDialog1.Title = "Save Sample Price / Instance Data";
            saveFileDialog1.ShowDialog();

            // If the file name is not an empty string open it for saving.  
            if (saveFileDialog1.FileName != "")
            {
                System.IO.File.WriteAllText(saveFileDialog1.FileName, pricesJSON);
            }

            return Autodesk.Revit.UI.Result.Succeeded;
        }
        #endregion
    }


    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    [Autodesk.Revit.Attributes.Regeneration(Autodesk.Revit.Attributes.RegenerationOption.Manual)]
    [Autodesk.Revit.Attributes.Journaling(Autodesk.Revit.Attributes.JournalingMode.NoCommandData)]
    public class UpdatePricing : IExternalCommand
    {
        #region IExternalCommand Members

        /// <summary>
        /// retrieves categories from server, generates price requests, sends, receives prices and updates model
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

            //loads from settings.txt
            settings.LoadSettings();
            List<string> cats = new List<string>();

            PricingHttpRest.InitializeClient(settings.APILocation, "");
            string configJSON = "";
            try
            {
                PricingHttpRest.APICommand = "pricing/config";
                string result = PricingHttpRest.Get();

                configJSON = PricingHttpRest.receivedContent;
                if (result != "OK")
                {
                    System.Windows.Forms.MessageBox.Show("Error accessing API while getting config");
                    return Autodesk.Revit.UI.Result.Failed;
                }
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("Error accessing API while getting config");
                return Autodesk.Revit.UI.Result.Failed;
            }


            try
            {



                Newtonsoft.Json.Linq.JToken jt = Newtonsoft.Json.Linq.JObject.Parse(configJSON);
                Newtonsoft.Json.Linq.JToken jfil = jt.SelectToken("filters");
                Newtonsoft.Json.Linq.JToken jcat = jfil.SelectToken("categories");
                cats = jcat.ToObject<List<string>>();

            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("Deserialization failure: " + ex.ToString());
                return Autodesk.Revit.UI.Result.Failed;
            }
            if (cats.Count == 0)
                System.Windows.Forms.MessageBox.Show("No categories found, taking all");
            //if (myConf.filters.Count==0)
            //{
            //    System.Windows.Forms.MessageBox.Show("No config data");
            //    return Autodesk.Revit.UI.Result.Failed;
            //}

            revit_pricing_request price1 = utilities.get_pricing_request(curDoc, cats);


            //send element data
            string serialS = JsonConvert.SerializeObject(price1);
            string pricesJSON = "";
            try
            {
                PricingHttpRest.APICommand = "pricing/model1";
                PricingHttpRest.sendContent = serialS;

                string result = PricingHttpRest.Post();

                pricesJSON = PricingHttpRest.receivedContent;
                if (result != "OK")
                {
                    System.Windows.Forms.MessageBox.Show("Error accessing API while submitting data for pricing: " + result + " " + pricesJSON);
                    return Autodesk.Revit.UI.Result.Failed;
                }
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("Error accessing API while submitting  data for pricing: " + ex.ToString());
                return Autodesk.Revit.UI.Result.Failed;
            }
            //extract id / cost from response
            List<PriceResponseElement> plist = new List<PriceResponseElement>(); 
            try
            {


                plist=Newtonsoft.Json.JsonConvert.DeserializeObject<List<PriceResponseElement>>(pricesJSON);




            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("Deserialization failure: " + ex.ToString());
                return Autodesk.Revit.UI.Result.Failed;
            }


            if (plist.Count==0)
            {
                System.Windows.Forms.MessageBox.Show("No instance / cost found");
                return Autodesk.Revit.UI.Result.Failed;
            }
            List<string> failures = new List<string>();
            foreach (PriceResponseElement ple in plist)
            {
                try
                {

                    bool res = PricingRevitInteraction.UpdateElementParameterValueDouble(curDoc, ple.id, "cost", ple.cost);
                    if (!res) failures.Add(ple.id);
                }
                catch(Exception ex)
                {
                    failures.Add(ple.id);
                    
                }
            }

            if (failures.Count>0)
            {
                System.Windows.Forms.MessageBox.Show("Failed to update some elements: "+ string.Join(",", failures.ToArray()));
            }
            else
            {
                System.Windows.Forms.MessageBox.Show("Successfully updated " + plist.Count + " elements");
            }
            using (Transaction t = new Transaction(curDoc, "Regen"))
            {
                t.Start();


                curDoc.Regenerate();
                t.Commit();
            }
            return Autodesk.Revit.UI.Result.Succeeded;
        }

        #endregion
    }



    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    [Autodesk.Revit.Attributes.Regeneration(Autodesk.Revit.Attributes.RegenerationOption.Manual)]
    [Autodesk.Revit.Attributes.Journaling(Autodesk.Revit.Attributes.JournalingMode.NoCommandData)]
    public class UpdateCustomPricing : IExternalCommand
    {
        #region IExternalCommand Members

        /// <summary>
        /// retrieves categories from server, generates price requests, sends, receives prices and updates model with parameter name in file parameter.txt in same folder as dll
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


            //loads from settings.txt
            settings.LoadSettings();


            string codeBase = Assembly.GetExecutingAssembly().CodeBase;
            UriBuilder uri = new UriBuilder(codeBase);
            string path = Uri.UnescapeDataString(uri.Path);

            string paramtextfile = Path.Combine(Path.GetDirectoryName(path), "parametername.txt");
            if (!File.Exists(paramtextfile))
            {
                System.Windows.Forms.MessageBox.Show(paramtextfile + " not found!");
                return Autodesk.Revit.UI.Result.Failed;
            }

            string paramname = File.ReadAllText(paramtextfile);

            List<string> cats = new List<string>();

            PricingHttpRest.InitializeClient(settings.APILocation, "");
            string configJSON = "";
            try
            {
                PricingHttpRest.APICommand = "pricing/config";
                string result = PricingHttpRest.Get();

                configJSON = PricingHttpRest.receivedContent;
                if (result != "OK")
                {
                    System.Windows.Forms.MessageBox.Show("Error accessing API while getting config");
                    return Autodesk.Revit.UI.Result.Failed;
                }
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("Error accessing API while getting config");
                return Autodesk.Revit.UI.Result.Failed;
            }


            try
            {



                Newtonsoft.Json.Linq.JToken jt = Newtonsoft.Json.Linq.JObject.Parse(configJSON);
                Newtonsoft.Json.Linq.JToken jfil = jt.SelectToken("filters");
                Newtonsoft.Json.Linq.JToken jcat = jfil.SelectToken("categories");
                cats = jcat.ToObject<List<string>>();

            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("Deserialization failure: " + ex.ToString());
                return Autodesk.Revit.UI.Result.Failed;
            }
            if (cats.Count == 0)
                System.Windows.Forms.MessageBox.Show("No categories found, taking all");
            //if (myConf.filters.Count==0)
            //{
            //    System.Windows.Forms.MessageBox.Show("No config data");
            //    return Autodesk.Revit.UI.Result.Failed;
            //}

            revit_pricing_request price1 = utilities.get_pricing_request(curDoc, cats);


            //send element data
            string serialS = JsonConvert.SerializeObject(price1);
            string pricesJSON = "";
            try
            {
                PricingHttpRest.APICommand = "pricing/model1";
                PricingHttpRest.sendContent = serialS;

                string result = PricingHttpRest.Post();

                pricesJSON = PricingHttpRest.receivedContent;
                if (result != "OK")
                {
                    System.Windows.Forms.MessageBox.Show("Error accessing API while submitting data for pricing: " + result + " " + pricesJSON);
                    return Autodesk.Revit.UI.Result.Failed;
                }
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("Error accessing API while submitting  data for pricing: " + ex.ToString());
                return Autodesk.Revit.UI.Result.Failed;
            }
            //extract id / cost from response
            List<PriceResponseElement> plist = new List<PriceResponseElement>();
            try
            {


                plist = Newtonsoft.Json.JsonConvert.DeserializeObject<List<PriceResponseElement>>(pricesJSON);




            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("Deserialization failure: " + ex.ToString());
                return Autodesk.Revit.UI.Result.Failed;
            }


            if (plist.Count == 0)
            {
                System.Windows.Forms.MessageBox.Show("No instance / cost found");
                return Autodesk.Revit.UI.Result.Failed;
            }
            List<string> failures = new List<string>();
            foreach (PriceResponseElement ple in plist)
            {
                try
                {

                    bool res = PricingRevitInteraction.UpdateElementParameterValueDouble(curDoc, ple.id, paramname, ple.cost);
                    if (!res) failures.Add(ple.id);
                }
                catch (Exception ex)
                {
                    failures.Add(ple.id);

                }
            }

            if (failures.Count > 0)
            {
                System.Windows.Forms.MessageBox.Show("Failed to update some elements: " + string.Join(",", failures.ToArray()));
            }
            else
            {
                System.Windows.Forms.MessageBox.Show("Successfully updated " + plist.Count + " elements");
            }


            using (Transaction t = new Transaction(curDoc, "Regen"))
            {
                t.Start();


                curDoc.Regenerate();
                t.Commit();
            }

            return Autodesk.Revit.UI.Result.Succeeded;
        }

        #endregion
    }
}
