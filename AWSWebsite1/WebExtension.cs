using System;
using System.Web;
using Newtonsoft.Json;

namespace AWSWebsite1
{
    public static class WebExtension
    {
        public static void WriteJson(this HttpResponse resp, object item)
        {
            string jsonObject = JsonConvert.SerializeObject(item);

            HttpContext.Current.Response.ContentType = "application/json";
            HttpContext.Current.Response.Clear();
            HttpContext.Current.Response.Write(jsonObject);
            HttpContext.Current.Response.Flush();
            HttpContext.Current.ApplicationInstance.CompleteRequest();
        }
    }
}