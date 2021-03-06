using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System.Net;
using System.Net.Http;
using WeatherSpider.ReadModels;

namespace WeatherSpider.Controllers
{
    public class WeatherController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }



        [HttpGet]

        public IActionResult Weather()
        {
            var url = "https://meteostat.p.rapidapi.com/stations/monthly?station=72502&start=2021-01-01&end=2021-12-31";
            var request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "GET";
            request.Headers["X-RapidAPI-Key"] = "9841fd6655msh8243a18a882842bp1ef563jsnae86c3da0fca";
            request.Headers["X-RapidAPI-Host"] = "meteostat.p.rapidapi.com";

            using var webResponse = request.GetResponse();
            using var webStream = webResponse.GetResponseStream();
            using var reader = new StreamReader(webStream);
            var data = reader.ReadToEnd();

            JObject json = JObject.Parse(data);
            JToken datatoken = json["data"];

            IEnumerable<JToken> year = datatoken.Children();

            List<Data> dataForChart = new List<Data>();
            foreach (var month in year)
            {
                dataForChart.Add(new Data((double)month["tavg"]));
            }


            var rm = new WetherDataRm(

                chart: new Chart(
                    caption: "Weather in New York",
                    subCaption: "Based on data collected last year",
                    numberPrefix: "",
                    theme: "fusion",
                    radarfillcolor: "#ffffff"
                    ),

                Categories: new List<Category>
                {
                    new Category(
                        new List<Label>
                        {
                            new Label("Jan"),
                            new Label("Feb"),
                            new Label("Mar"),
                            new Label("Apr"),
                            new Label("May"),
                            new Label("Jun"),
                            new Label("Jul"),
                            new Label("Jug"),
                            new Label("Sep"),
                            new Label("Oct"),
                            new Label("Nov"),
                            new Label("Dec")
                        }
                        ),
                },

                 dataset: new List<DataSet>
                {
                    new DataSet(
                        seriesname: "Temperature average",
                        dataForChart
                        )
                }
                );

            return Ok(rm);
        }
    }
}
