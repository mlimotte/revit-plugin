using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.IO;
using Newtonsoft.Json;
using System.Reflection;

namespace Revit.Pricing
{
    public static class PricingHttpRest
    {


        static bool initOK = false;

        /// <summary>
        /// IP address of REST API
        /// </summary>
        public static string address { get; set; }
        /// <summary>
        /// Port for REST API
        /// </summary>
        public static string port { get; set; }
        /// <summary>
        /// REST command
        /// </summary>
        public static string APICommand { get; set; }
        /// <summary>
        /// Content set to server
        /// </summary>
        public static string sendContent { get; set; }
        /// <summary>
        /// Data received
        /// </summary>
        public static string receivedContent { get; private set; }
        /// <summary>
        /// Status Code
        /// </summary>
        public static string receivedCode { get; private set; }


        static HttpClient client = new HttpClient();

        /// <summary>
        /// Initializes client with needed parameters a
        /// </summary>
        /// <param name="cAddress">REST API IP</param>
        /// <param name="cPort">REST API port (empty string to disregard)</param>
        public static void InitializeClient(string cAddress, string cPort)
        {

            initOK = true;
            address = cAddress;
            port = cPort;
            APICommand = "healthcheck";
            sendContent = "";


        }
        static string AssemblyDirectory
        {
            get
            {
                string codeBase = Assembly.GetExecutingAssembly().CodeBase;
                UriBuilder uri = new UriBuilder(codeBase);
                string path = Uri.UnescapeDataString(uri.Path);
                return Path.GetDirectoryName(path);
            }
        }


        /// <summary>
        /// Submits 
        /// </summary>
        /// <returns>Get server reason</returns>
        public static string Get()
        {
            if (!initOK)
            {
                return "Client not initialized";
            }
            string s = RunGetAsync().GetAwaiter().GetResult();

            return s;

        }


        public static string Post()
        {
            if (!initOK)
            {
                return "Client not initialized";
            }
            string s = RunPostAsync().GetAwaiter().GetResult();

            return s;
        }



        static async Task<HttpResponseMessage> SendPostRequestAsync(string adaptiveUri, string xmlRequest)
        {
            //using (HttpClient httpClient = new HttpClient())
            //{
            client = new HttpClient();

            // StringContent httpContent = new StringContent(xmlRequest, Encoding.UTF8, "application/json");


            var buffer = System.Text.Encoding.UTF8.GetBytes(xmlRequest);
            var byteContent = new ByteArrayContent(buffer);
            // Next, you want to set the content type to let the API know this is JSON.

            byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            // client.DefaultRequestHeaders.Accept.Clear();
            //client.DefaultRequestHeaders.Accept.Add(
            //   new MediaTypeWithQualityHeaderValue("application/json"));

            HttpResponseMessage responseMessage = null;
            try
            {


                responseMessage = await client.PostAsync(adaptiveUri, byteContent).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                if (responseMessage == null)
                {
                    responseMessage = new HttpResponseMessage();
                }
                responseMessage.StatusCode = HttpStatusCode.InternalServerError;
                responseMessage.ReasonPhrase = string.Format("RestHttpClient.SendPostRequest failed: {0}", ex);
            }
            return responseMessage;
            //}
        }



        static async Task<HttpResponseMessage> SendGetRequestAsync(string adaptiveUri, string xmlRequest)
        {
            //using (HttpClient httpClient = new HttpClient())
            //{

            client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(
                       new MediaTypeWithQualityHeaderValue("application/json"));

            HttpResponseMessage responseMessage = null;

            //if (xmlRequest == "")
            //{

            //StringContent httpConent = new StringContent(xmlRequest, Encoding.UTF8, "application/json");

            try
            {

                responseMessage = await client.GetAsync(adaptiveUri).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                if (responseMessage == null)
                {
                    responseMessage = new HttpResponseMessage();
                }
                responseMessage.StatusCode = HttpStatusCode.InternalServerError;
                responseMessage.ReasonPhrase = string.Format("RestHttpClient.SendPostRequest failed: {0}", ex);
            }
            return responseMessage;
            //}
            //else
            //{
            //    // StringContent httpConent = new StringContent(xmlRequest, Encoding.UTF8, "application/json");
            //    //s//tring sc = await httpConent.ReadAsStringAsync();
            //    try
            //    {

            //        responseMessage = await httpClient.GetAsync(adaptiveUri).ConfigureAwait(false);
            //    }
            //    catch (Exception ex)
            //    {
            //        if (responseMessage == null)
            //        {
            //            responseMessage = new HttpResponseMessage();
            //        }
            //        responseMessage.StatusCode = HttpStatusCode.InternalServerError;
            //        responseMessage.ReasonPhrase = string.Format("RestHttpClient.SendPostRequest failed: {0}", ex);
            //    }
            //    return responseMessage;
            //}
            //}
        }



        /// <summary>
        /// GET request with parameters specified for PricingHttpRest class. Populates properties of PricingHttpRest class.
        /// </summary>
        /// <returns>Reason</returns>
        static async Task<string> RunGetAsync()
        {
            try
            {
                if (address.Contains("http://"))
                    address = address.Replace("http://", "");

                string aur = "http://" + address + ":" + port + "/" + APICommand;

                if (port == "")
                    aur = "http://" + address + "/" + APICommand;

                var hrc = await SendGetRequestAsync(aur, sendContent).ConfigureAwait(false);
                receivedContent = await hrc.Content.ReadAsStringAsync();
                receivedCode = hrc.StatusCode.ToString();
                return hrc.ReasonPhrase;
            }
            catch
            {
                return "Error Connecting";
            }
        }


        static async Task<string> RunPostAsync()
        {
            try
            {
                if (address.Contains("http://"))
                    address = address.Replace("http://", "");

                string aur = "http://" + address + ":" + port + "/" + APICommand;

                if (port == "")
                    aur = "http://" + address + "/" + APICommand;

                var hrc = await SendPostRequestAsync(aur, sendContent).ConfigureAwait(false);
                receivedContent = await hrc.Content.ReadAsStringAsync();
                receivedCode = hrc.StatusCode.ToString();
                return hrc.ReasonPhrase;
            }
            catch
            {
                return "Error Connecting";
            }
        }

    }

}
