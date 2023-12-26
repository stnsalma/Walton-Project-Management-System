using System.Web.Http.Routing;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace ProjectManagement.Infrastructures.Helper
{
    public static class CommonConversion
    {
        public static string AddOrdinal(int? num = 0)
        {
            if (num <= 0) return num.ToString();

            switch (num % 100)
            {
                case 11:
                case 12:
                case 13:
                    return num + "th";
            }

            switch (num % 10)
            {
                case 1:
                    return num + "st";
                case 2:
                    return num + "nd";
                case 3:
                    return num + "rd";
                default:
                    return num + "th";
            }

        }
        public static string ToPrettyFormat(TimeSpan span)
        {

            if (span == TimeSpan.Zero) return "0 minutes";

            var sb = new StringBuilder();
            if (span.Days > 0)
                sb.AppendFormat("{0} d", span.Days);
            else if (span.Hours > 0)
                sb.AppendFormat("{0} h", span.Hours);
            else if (span.Minutes > 0)
                sb.AppendFormat("{0} m", span.Minutes);
            else
            {
                sb.AppendFormat("a m. ago");
            }
            return sb.ToString();

        }

        public static decimal CurrencyConversion(decimal amount, string fromCurrency, string toCurrency)
        {
            try
            {
                string url;
                var FreeBaseUrl = "https://free.currconv.com/api/v7/";
                url = FreeBaseUrl + "convert?q=" + fromCurrency + "_" + toCurrency + "&compact=ultra&apiKey=625ca1198234ba923acd";

                var jsonString = GetResponse(url);
                var rate = JObject.Parse(jsonString).First.ToObject<decimal>();
                var finalresult = rate * amount;
                return finalresult;
            }
            catch (Exception ex)
            {
                string url = "https://api.exchangerate-api.com/v4/latest/" + fromCurrency;

                var jsonString = GetResponse(url);
                var rate = JObject.Parse(jsonString).Last.First["USD"].ToObject<decimal>();
                var finalresult = rate * amount;
                return finalresult;
            }
        }

        public static decimal CurrencyRate(decimal amount, string fromCurrency, string toCurrency)
        {
            try
            {
                string url;
                var FreeBaseUrl = "https://free.currconv.com/api/v7/";
                url = FreeBaseUrl + "convert?q=" + fromCurrency + "_" + toCurrency + "&compact=ultra&apiKey=625ca1198234ba923acd";

                var jsonString = GetResponse(url);
                var rate = JObject.Parse(jsonString).First.ToObject<decimal>();
                return rate;
            }
            catch (Exception ex)
            {
                string url = "https://api.exchangerate-api.com/v4/latest/" + fromCurrency;

                var jsonString = GetResponse(url);
                var rate = JObject.Parse(jsonString).Last.First["USD"].ToObject<decimal>();
                return rate;
            }
        }

        private static string GetResponse(string url)
        {
            string jsonString;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            var request = (HttpWebRequest)WebRequest.Create(url);
            request.AutomaticDecompression = DecompressionMethods.GZip;

            using (var response = (HttpWebResponse)request.GetResponse())
            using (var stream = response.GetResponseStream())
            using (var reader = new StreamReader(stream))
            {
                jsonString = reader.ReadToEnd();
            }

            return jsonString;
        }
    }

    class CustomCurrency
    {
        public string CurrencyName { get; set; }
        public decimal Rate { get; set; }
    }
}