using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.IO;

namespace Revit.Pricing
{
    public class settings
    {
        //main REST URL
        public static string APILocation = @"http://api.skipp.ai"; // @"http://api.fairhomemaine.com";

        //loads settings from file
        public static void LoadSettings()
        {
            string setfile = AssemblyDirectory() + @"\settings.txt";
            if (System.IO.File.Exists(setfile))
            {
                string[] setLines = System.IO.File.ReadAllLines(setfile);
                List<string> names = new List<string>();
                List<string> values = new List<string>();
                foreach (string l in setLines)
                {
                    //ignore comments
                    if (!l.StartsWith("//"))
                    {
                        //comma separation between name and value
                        char[] sep = { ',' };
                        string[] namev = l.Split(sep, StringSplitOptions.None);
                        names.Add(namev[0]);
                        values.Add(namev[1]);

                    }
                }
                //change settings values via reflection
                Type settingsType = typeof(settings);
                foreach (FieldInfo sfield in settingsType.GetFields(BindingFlags.Public | BindingFlags.Static))
                {
                    try
                    {
                        if (names.Contains(sfield.Name))
                        {
                            int index = names.IndexOf(sfield.Name);
                            sfield.SetValue(null, Convert.ChangeType(values[index], sfield.FieldType));
                        }
                    }
                    catch
                    {

                    }
                }
            }

        }


        //figures out where DLL located
        public static string AssemblyDirectory()
        {

            string codeBase = Assembly.GetExecutingAssembly().CodeBase;
            UriBuilder uri = new UriBuilder(codeBase);
            string path = Uri.UnescapeDataString(uri.Path);
            return Path.GetDirectoryName(path);

        }
    }
}
