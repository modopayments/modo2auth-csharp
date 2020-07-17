using System;
using System.Net;
using System.IO;


namespace Modo
{
    class Program
    {
        static void Main(string[] args)
        {
            String apiId = "Add Api ID here";
            String apiSecret = "Add Api Secret here";

            String apiUri = "/v2/reports";
            String fullUri = "https://checkout.mtktest.modopayments.net/v2/reports";

            String requestBody =  "{\"start_date\": \"2020-07-13T00:00:00Z\",\"end_date\": \"2020-07-13T23:59:59Z\"}";


            Modo2Auth auth = new Modo2Auth(apiSecret, apiId);
            String authToken = auth.createModoToken(apiUri, requestBody);

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(fullUri);
            request.ContentType = "application/json";
            request.Method = WebRequestMethods.Http.Post;

            request.Headers.Add("Authorization", authToken);

            using (StreamWriter streamWriter = new StreamWriter(request.GetRequestStream()))
            {
                streamWriter.Write(requestBody);
                streamWriter.Flush();
                streamWriter.Close();
            }

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            String result = "";
            using (StreamReader streamReader = new StreamReader(response.GetResponseStream()))
            {
                result = streamReader.ReadToEnd();
            }

            Console.WriteLine(result);
        }
    }
}