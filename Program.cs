﻿using System;
using System.Net;
using System.IO;
using System.Text;

namespace Modo
{
    class Program
    {
        static void Main(string[] args)
        {
            String apiId = "Add Api ID here";
            String apiSecret = "Add Api Secret here";

            Modo2Auth auth = new Modo2Auth(apiSecret, apiId);

            // Let's query a report based on date range
            Console.WriteLine("*** Performing List Query....");
            String apiUri = "/v2/reports";
            String fullUri = "[URL_HERE]/v3/checkout/list";
            String requestBody = "{\"start_date\": \"2020-07-13T00:00:00Z\",\"end_date\": \"2020-07-13T23:59:59Z\"}";

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(fullUri);
            request.ContentType = "application/json";
            request.Method = WebRequestMethods.Http.Post;

            // Add our auth token to the http POST
            request.Headers.Add("Authorization", auth.createModoToken(apiUri, requestBody, Encoding.UTF8));

            // Write out the body contents
            using (StreamWriter streamWriter = new StreamWriter(request.GetRequestStream()))
            {
                streamWriter.Write(requestBody);
                streamWriter.Flush();
                streamWriter.Close();
            }

            try
            {
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    String result = "";
                    using (StreamReader streamReader = new StreamReader(response.GetResponseStream()))
                    {
                        result = streamReader.ReadToEnd();
                    }
                    Console.WriteLine("------ Report Results ------");
                    Console.WriteLine(result);
                }
                else if (response.StatusCode == HttpStatusCode.Forbidden)
                {
                    Console.WriteLine("You have passed in incorrect credentials");
                    return;
                }
                else
                {
                    Console.WriteLine("Unhandled error from server: " + response.StatusCode);
                    return;
                }
            }
            catch (System.Exception ex)
            {
                Console.WriteLine("Web Exception: " + ex.Message);
                return;
            }
        }
    }
}
