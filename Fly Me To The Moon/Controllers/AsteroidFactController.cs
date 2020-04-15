using Fly_Me_To_The_Moon.Controllers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Fly_Me_To_The_Moon
{
    public class AsteroidFactController : ApiController
    {
        // GET api/<controller>
        [HttpGet]
        public string Get()
        {
            string returnResults = "The closest asteroid is set to miss us by ";
            using (HttpClient client = new HttpClient())
            {
                var response = client.GetAsync("https://api.nasa.gov/neo/rest/v1/neo/3893864?api_key=S3PvocFbjZoDhEcGgHwzF7efknEZAZVMdR9OhBZd").Result;
                HttpContent parsing = response.Content;
                Asteroid results = JsonConvert.DeserializeObject<Asteroid>(parsing.ReadAsStringAsync().Result);
                returnResults += results.close_approach_data[1].miss_distance.miles;
            }



            return returnResults + " miles";
        }
        

    }

}