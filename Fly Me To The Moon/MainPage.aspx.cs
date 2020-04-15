/* CSS436 Program 5 - Autumn 2019
 * Conrad Dudziak, Rithik Bansal, McKinley Melton
 * This Program is a website that uses the NASA api to display facts and images about space.
 * This file contains the server logic for the main page of the website. The main page
 * is responsible for displaying the nasa image of the day and facts about asteroids.
 * All information is retrieved from a remote azure blob storage. Logic apps are responsible
 * for fetching data from the NASA api once a week (or once a day) and storing it as a blob.
 */

using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Microsoft.ApplicationInsights;

namespace Fly_Me_To_The_Moon
{
    public partial class Contact : Page
    {
        private TelemetryClient telemetry = new TelemetryClient();
        int currenstSmallestRow = 0;
        double smallestMissDistance = double.PositiveInfinity;

        // Called when the page is requested and gets loaded.
        protected void Page_Load(object sender, EventArgs e)
        {
            SpacePictureDisplay.Visible = false;
            PictureOfTheDay();
            AsteroidFacts();
        }

        // Retrieves the picture of the day from azure blob storage and displays it on the webpage.
        // Telemetry is tracked on failure and success.
        protected void PictureOfTheDay()
        {
            telemetry.TrackEvent("POTD");
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();

            string connectionString = "DefaultEndpointsProtocol=https;AccountName=cs436moonstorage;AccountKey=NQ6exDOAJiFAIWgZVYbf5Ao+GBzFCL0h3ZmH+k1ZaNbb/2o2sQhQoeoXM60zAcNg4y6g8CO7lXFqf2KM6Dw7xQ==;EndpointSuffix=core.windows.net";
            string containerName = "picturestore";
            string fileName = "PicOfTheDay";
            
            // Connects to the cloud storage and creates a client
            CloudStorageAccount cloudStorageAccount = CloudStorageAccount.Parse(connectionString);
            CloudBlobClient blobClient = cloudStorageAccount.CreateCloudBlobClient();

            CloudBlobContainer cloudBlobContainer = blobClient.GetContainerReference(containerName);
            CloudBlockBlob blockBlob = cloudBlobContainer.GetBlockBlobReference(fileName);

            // Download the image and from blob storage and display it
            try
            {
                var textFromFile = (new WebClient()).DownloadString(blockBlob.Uri);
                SpacePictureObject results = JsonConvert.DeserializeObject<SpacePictureObject>(textFromFile);

                SpacePictureDisplay.ImageUrl = results.url;
                SpacePictureDisplay.Visible = true;

            }
            catch (Exception fail)
            {
                telemetry.TrackException(fail);
            }

            stopwatch.Stop();
            var metrics = new Dictionary<string, double> { { "processingTime", stopwatch.Elapsed.TotalMilliseconds } };
            telemetry.TrackEvent("SignalProcessed for POTD", null, metrics);
        }

        // Retrieves the asteroid facts from blob storage and parses the returned json.
        // The json is deserialized for the asteroid facts of the current day and displayed in an HTML table.
        // Telemetry is tracked on failure and success.
        protected void AsteroidFacts()
        {
            telemetry.TrackEvent("AsteroidFacts");
            string connectionString = "DefaultEndpointsProtocol=https;AccountName=cs436moonstorage;AccountKey=NQ6exDOAJiFAIWgZVYbf5Ao+GBzFCL0h3ZmH+k1ZaNbb/2o2sQhQoeoXM60zAcNg4y6g8CO7lXFqf2KM6Dw7xQ==;EndpointSuffix=core.windows.net";
            string containerName = "asteroidstore";
            string fileName = "AsteroidFacts";
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();

            // Connects to the cloud storage and creates a client
            CloudStorageAccount cloudStorageAccount = CloudStorageAccount.Parse(connectionString);
            CloudBlobClient blobClient = cloudStorageAccount.CreateCloudBlobClient();

            CloudBlobContainer cloudBlobContainer = blobClient.GetContainerReference(containerName);
            CloudBlockBlob blockBlob = cloudBlobContainer.GetBlockBlobReference(fileName);

            // Downloads the JSON and parses it for the asteroid facts of the day
            try
            {
                var textFromFile = (new WebClient()).DownloadString(blockBlob.Uri);
                JObject jObject = JObject.Parse(textFromFile);

                string elements = (string)jObject.SelectToken("element_count");

                // Iterate over each day in the json
                JObject dates = (JObject)jObject.SelectToken("near_earth_objects");
                int count = 0;
                foreach (JToken date in dates.Children())
                {
                    int index = 0;
                    string tokenName = "";
                    foreach (JProperty prop in dates.Properties())
                    {
                        if (count == index) 
                        {
                            tokenName = prop.Name;
                            break;
                        }
                        index++;
                    }

                    // Populate the table with facts about asteroids that are closest to earth today.
                    if (DateTime.Now.ToString("yyyy-MM-dd").Equals(tokenName))
                    {
                        JArray asteroids = (JArray)dates.SelectToken(tokenName);

                        BuildTable(asteroids.Count());

                        int next = 0;
                        foreach (JToken asteroid in asteroids)
                        {
                            DisplayAsteroidFacts(asteroid, next);
                            next++;
                        }
                    }

                    count++;
                }
            }
            catch (Exception fail)
            {
                telemetry.TrackException(fail);
            }

            // Track telemetry on success
            stopwatch.Stop();
            var metrics = new Dictionary<string, double> { { "processingTime", stopwatch.Elapsed.TotalMilliseconds } };
            telemetry.TrackEvent("SignalProcessed for AsteroidFacts", null, metrics);

            Table1.Rows[currenstSmallestRow].BackColor = Color.LightSalmon;
        }

        // Receives an asteroid JToken and index. Inserts the asteroid facts that are part of the
        // JToken into the index row of the table.
        public void DisplayAsteroidFacts(JToken asteroid, int index)
        {
            TableRow r = Table1.Rows[index + 1];
            TableCellCollection cells = r.Cells;

            cells[0].Text = "";

            // Attach the hyperlink to JPL
            HyperLink hl = new HyperLink()
            {
                Text = (string)asteroid.SelectToken("name"),
                NavigateUrl = (string)asteroid.SelectToken("nasa_jpl_url"),
                CssClass = "thickbox",
                ToolTip = "Visit JPL database browser",
                Target = "_blank"
            };

            // Parse the asteroid JToken for all the asteroid facts in the JSON
            cells[0].Controls.Add(hl);
            cells[1].Text = (string)asteroid.SelectToken("absolute_magnitude_h");
            try {
                JObject diameter = (JObject)asteroid.SelectToken("estimated_diameter");
                JObject measure = (JObject)diameter.SelectToken("meters");

                double diameterDouble = (double)measure.SelectToken("estimated_diameter_max");
                cells[2].Text = Math.Round(diameterDouble, 1).ToString();

                cells[3].Text = (string)asteroid.SelectToken("is_potentially_hazardous_asteroid");

                JArray approach = (JArray)asteroid.SelectToken("close_approach_data");

                // Iterate over all the facts located in the approachData JToken
                foreach (JToken approachData in approach)
                {
                    JObject velocity = (JObject)approachData.SelectToken("relative_velocity");
                    double velocityDouble = (double)velocity.SelectToken("kilometers_per_hour");
                    cells[7].Text = Math.Round(velocityDouble, 3).ToString();

                    JObject missDistance = (JObject)approachData.SelectToken("miss_distance");
                    double missDistanceDouble = (double)missDistance.SelectToken("kilometers");
                    cells[6].Text = Math.Round(missDistanceDouble).ToString();

                    // Check if this is the closest asteroid today
                    if (Math.Round(missDistanceDouble) < smallestMissDistance) {
                        currenstSmallestRow = index + 1;
                        smallestMissDistance = Math.Round(missDistanceDouble);
                    }

                    cells[4].Text = (string)approachData.SelectToken("orbiting_body");
                    cells[5].Text = (string)approachData.SelectToken("close_approach_date_full");
                }

            } catch (Exception e) {
                telemetry.TrackException(e);
            }
        }

        // Receives an int Rows and creates a table with that many rows.
        // Headers are predefined to display the corresponding asteroid facts.
        public void BuildTable(int rows) {
            Table1.Caption = "The closest asteroid is highlighted (does not mean it is hazerdous!)";

            // Create a TableHeaderRow.
            TableHeaderRow headerRow = new TableHeaderRow();
            headerRow.BackColor = Color.LightBlue;

            // Create TableCell objects to contain 
            // the text for the header.
            TableHeaderCell headerTableCell1 = new TableHeaderCell();
            TableHeaderCell headerTableCell2 = new TableHeaderCell();
            TableHeaderCell headerTableCell3 = new TableHeaderCell();
            TableHeaderCell headerTableCell4 = new TableHeaderCell();
            TableHeaderCell headerTableCell5 = new TableHeaderCell();
            TableHeaderCell headerTableCell6 = new TableHeaderCell();
            TableHeaderCell headerTableCell7 = new TableHeaderCell();
            TableHeaderCell headerTableCell8 = new TableHeaderCell();
            headerTableCell1.Text = "Name";
            headerTableCell1.Scope = TableHeaderScope.Column;
            headerTableCell1.AbbreviatedText = "Name";
            headerTableCell2.Text = "Magnitude";
            headerTableCell2.Scope = TableHeaderScope.Column;
            headerTableCell2.AbbreviatedText = "Mag.";
            headerTableCell3.Text = "Diameter (m)";
            headerTableCell3.Scope = TableHeaderScope.Column;
            headerTableCell3.AbbreviatedText = "Dia.";
            headerTableCell4.Text = "Hazardeous";
            headerTableCell4.Scope = TableHeaderScope.Column;
            headerTableCell4.AbbreviatedText = "Haz.";
            headerTableCell5.Text = "Orbiting Body";
            headerTableCell5.Scope = TableHeaderScope.Column;
            headerTableCell5.AbbreviatedText = "Orbit";
            headerTableCell6.Text = "Close Approach";
            headerTableCell6.Scope = TableHeaderScope.Column;
            headerTableCell6.AbbreviatedText = "Close";
            headerTableCell7.Text = "Miss Distance (km)";
            headerTableCell7.Scope = TableHeaderScope.Column;
            headerTableCell7.AbbreviatedText = "Miss";
            headerTableCell8.Text = "Miss Speed (kmph)";
            headerTableCell8.Scope = TableHeaderScope.Column;
            headerTableCell8.AbbreviatedText = "Speed";

            // Add the TableHeaderCell objects to the Cells collection of the TableHeaderRow.
            headerRow.Cells.Add(headerTableCell1);
            headerRow.Cells.Add(headerTableCell2);
            headerRow.Cells.Add(headerTableCell3);
            headerRow.Cells.Add(headerTableCell4);
            headerRow.Cells.Add(headerTableCell5);
            headerRow.Cells.Add(headerTableCell6);
            headerRow.Cells.Add(headerTableCell7);
            headerRow.Cells.Add(headerTableCell8);

            // Add the TableHeaderRow as the first item in the Rows collection of the table.
            Table1.Rows.AddAt(0, headerRow);

            // Generate rows and cells.           
            int numrows = rows;
            int numcells = 8;
            for (int j = 0; j < numrows; j++)
            {
                TableRow r = new TableRow();
                for (int i = 0; i < numcells; i++)
                {
                    TableCell c = new TableCell();
                    c.Controls.Add(new LiteralControl("row " + j.ToString() + ", cell " + i.ToString()));
                    r.Cells.Add(c);
                }
                Table1.Rows.Add(r);
            }
        }
    }

    // A class object for deserializing the JSON PictureOfTheDay Data.
    public class SpacePictureObject
    {
        public string copyright { get; set; }
        public string date { get; set; }
        public string explanation { get; set; }
        public string hdurl { get; set; }
        public string media_type { get; set; }
        public string service_version { get; set; }
        public string title { get; set; }
        public string url { get; set; }
    }
}