using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace AgriWebSite_v2.Services
{
    public static class CallSIoTApi
    {
        public static bool localhostis = false;
        private static readonly string Uriloc = "http://localhost:44345/api/";

        private static readonly string Uri = "http://195.97.109.188:5000/api";

        public static async Task<string> GetAPI(string controller)
        {
            //for ssl localhost
            if (localhostis)
            {
                using (var httpClientHandler = new HttpClientHandler())
                {
                    httpClientHandler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) =>
                    {
                        return true;
                    };
                    using (HttpClient client = new HttpClient(httpClientHandler))
                    {
                        controller = controller.Trim();
                        var response = await client.GetAsync(Uriloc + "/" + controller);
                        string content = await response.Content.ReadAsStringAsync();

                        return content;
                    }
                }
            }
            else
            {
                using (HttpClient client = new HttpClient())
                {
                    controller = controller.Trim();
                    var response = await client.GetAsync(Uri + "/" + controller);
                    string content = await response.Content.ReadAsStringAsync();

                    return content;
                }
            }

        }

        //public async Task<Sensors> PostApi(Sensors newSensor)
        //{
        //    using (HttpClient client = new HttpClient())
        //    {
        //        Sensors sensor = new Sensors();
        //        var uri = new Uri(Constants.ApplicationURL);
        //        try
        //        {
        //            var json = JsonConvert.SerializeObject(newSensor);
        //            var body = new StringContent(json, Encoding.UTF8, "application/json");

        //            HttpResponseMessage response = await client.PostAsync(uri, body);

        //            if (response.IsSuccessStatusCode)
        //            {
        //                var content = await response.Content.ReadAsStringAsync();
        //                measurand = JsonConvert.DeserializeObject<Measurands>(content);
        //                // Debug.WriteLine(@"				Course successfully created.");
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            // Debug.WriteLine(@"				ERROR {0}", ex.Message);
        //        }

        //        return measurand;
        //    }
        //}
    }
}
