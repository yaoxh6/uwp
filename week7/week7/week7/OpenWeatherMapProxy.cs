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

namespace week7
{
        public class OpenWeatherMapProxy
        {
            public async static Task<RootObject> GetWeather(string location)
            {
            string link = "https://api.seniverse.com/v3/weather/now.json?key=pw0dibkghhvjjnc6&location=" + location + "&language=zh-Hans&unit=c";
                var http = new HttpClient();
            //var response = await http.GetAsync("https://api.seniverse.com/v3/weather/now.json?key=pw0dibkghhvjjnc6&location=beijing&language=zh-Hans&unit=c");
                var response = await http.GetAsync(link);
                var result = await response.Content.ReadAsStringAsync();
                var serializer = new DataContractJsonSerializer(typeof(RootObject));

                var ms = new MemoryStream(Encoding.UTF8.GetBytes(result));
                var data = (RootObject)serializer.ReadObject(ms);
                
            return data;
            }
        }

    [DataContract]
    public class Location
    {
        [DataMember]
        public string id { get; set; }
        /// <summary>
        /// 北京
        /// </summary>
        [DataMember]
        public string name { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public string country { get; set; }
        /// <summary>
        /// 北京,北京,中国
        [DataMember]
        public string path { get; set; }
        /// <summary>
        /// 
        [DataMember]
        public string timezone { get; set; }
        /// <summary>
        /// 
        [DataMember]
        public string timezone_offset { get; set; }
    }
    [DataContract]
    public class Now
    {
        /// <summary>
        /// 多云
        [DataMember]
        public string text { get; set; }
        /// <summary>
        /// 
        [DataMember]
        public string code { get; set; }
        /// <summary>
        /// 
        [DataMember]
        public string temperature { get; set; }
    }
    [DataContract]
    public class ResultsItem
    {
        /// <summary>
        /// 
        [DataMember]
        public Location location { get; set; }
        /// <summary>
        /// 
        [DataMember]
        public Now now { get; set; }
        /// <summary>
        /// 
        [DataMember]
        public string last_update { get; set; }
    }
    [DataContract]
    public class RootObject
    {
        /// <summary>
        /// 
        [DataMember]
        public List<ResultsItem> results { get; set; }
    }
}
