using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace week7
{
    public class xmlway
    {
        public async static Task<CityWeatherResponse> GetWeather(String Location)
        {
            string link = "http://api.map.baidu.com/telematics/v3/weather?location=" +Location +"&ak=8IoIaU655sQrs95uMWRWPDIa";
            var http = new HttpClient();
            //var response = await http.GetAsync("http://api.map.baidu.com/telematics/v3/weather?location=%E6%AD%A6%E6%B1%89&ak=8IoIaU655sQrs95uMWRWPDIa");
            var response = await http.GetAsync(link);
            var result = await response.Content.ReadAsStringAsync();
            var serializer = new XmlSerializer(typeof(CityWeatherResponse));

            var ms = new MemoryStream(Encoding.UTF8.GetBytes(result));
            var data = (CityWeatherResponse)serializer.Deserialize(ms);
            return data;
        }

        public class Weather_data
        {
            /// <summary>
            /// 
            /// </summary>
            public List<string> date { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public List<string> dayPictureUrl { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public List<string> nightPictureUrl { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public List<string> weather { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public List<string> wind { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public List<string> temperature { get; set; }
        }

        public class Index
        {
            /// <summary>
            /// 
            /// </summary>
            public List<string> title { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public List<string> zs { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public List<string> tipt { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public List<string> des { get; set; }
        }

        public class Results
        {
            /// <summary>
            /// 北京
            /// </summary>
            public string currentCity { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public Weather_data weather_data { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public Index index { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string pm25 { get; set; }
        }

        public class CityWeatherResponse
        {
            /// <summary>
            /// 
            /// </summary>
            public string error { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string status { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string date { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public Results results { get; set; }
        }

        //public class Root
        //{
        //    /// <summary>
        //    /// 
        //    /// </summary>
        //    public CityWeatherResponse CityWeatherResponse { get; set; }
        //}
    }
}
