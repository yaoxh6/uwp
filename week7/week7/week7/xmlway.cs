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
            string link = "http://api.map.baidu.com/telematics/v3/weather?location=" + Location + "&ak=8IoIaU655sQrs95uMWRWPDIa";
            var http = new HttpClient();
            //var response = await http.GetAsync("http://api.map.baidu.com/telematics/v3/weather?location=%E6%AD%A6%E6%B1%89&ak=8IoIaU655sQrs95uMWRWPDIa");
            var response = await http.GetAsync(link);
            var result = await response.Content.ReadAsStringAsync();
            var serializer = new XmlSerializer(typeof(CityWeatherResponse));

            var ms = new MemoryStream(Encoding.UTF8.GetBytes(result));
            var data = (CityWeatherResponse)serializer.Deserialize(ms);
            return data;
        }

    }

    [XmlRoot(ElementName = "weather_data")]
    public class Weather_data
    {
        [XmlElement(ElementName = "date")]
        public List<string> Date { get; set; }
        [XmlElement(ElementName = "dayPictureUrl")]
        public List<string> DayPictureUrl { get; set; }
        [XmlElement(ElementName = "nightPictureUrl")]
        public List<string> NightPictureUrl { get; set; }
        [XmlElement(ElementName = "weather")]
        public List<string> Weather { get; set; }
        [XmlElement(ElementName = "wind")]
        public List<string> Wind { get; set; }
        [XmlElement(ElementName = "temperature")]
        public List<string> Temperature { get; set; }
    }

    [XmlRoot(ElementName = "index")]
    public class Index
    {
        [XmlElement(ElementName = "title")]
        public List<string> Title { get; set; }
        [XmlElement(ElementName = "zs")]
        public List<string> Zs { get; set; }
        [XmlElement(ElementName = "tipt")]
        public List<string> Tipt { get; set; }
        [XmlElement(ElementName = "des")]
        public List<string> Des { get; set; }
    }

    [XmlRoot(ElementName = "results")]
    public class Results
    {
        [XmlElement(ElementName = "currentCity")]
        public string CurrentCity { get; set; }
        [XmlElement(ElementName = "weather_data")]
        public Weather_data Weather_data { get; set; }
        [XmlElement(ElementName = "index")]
        public Index Index { get; set; }
        [XmlElement(ElementName = "pm25")]
        public string Pm25 { get; set; }
    }

    [XmlRoot(ElementName = "CityWeatherResponse")]
    public class CityWeatherResponse
    {
        [XmlElement(ElementName = "error")]
        public string Error { get; set; }
        [XmlElement(ElementName = "status")]
        public string Status { get; set; }
        [XmlElement(ElementName = "date")]
        public string Date { get; set; }
        [XmlElement(ElementName = "results")]
        public Results Results { get; set; }
    }
}


