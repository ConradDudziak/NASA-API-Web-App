/* CSS436 Program 5 - Autumn 2019
 * Conrad Dudziak, Rithik Bansal, McKinley Melton
 * This Program is a website that uses the NASA api to display facts and images about space.
 * This file contains the server logic for logging users into the website.
 * If the information inputted does not exist in Azure Table Storage, then an error message is displayed.
 * Otherwise, the user is forwarded to the main page that displays space facts.
 */

using Fly_Me_To_The_Moon.Content;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Windows.Forms;
using Microsoft.ApplicationInsights;

namespace Fly_Me_To_The_Moon
{
    

    public partial class _Default : Page
    {
        private TelemetryClient telemetry = new TelemetryClient();
        private string tableName = "UserLog";

        // Connection string
        private string ConnectionString = "DefaultEndpointsProtocol=https;AccountName=cs436moonstorage;AccountKey=NQ6exDOAJiFAIWgZVYbf5Ao+GBzFCL0h3ZmH+k1ZaNbb/2o2sQhQoeoXM60zAcNg4y6g8CO7lXFqf2KM6Dw7xQ==;EndpointSuffix=core.windows.net";
       
        // connection to the azure storage account        
        private CloudStorageAccount storageAccount;
       
        // connection to the client for the table storage
        private CloudTableClient tableClient;
       
        // connection to the table storage
        private CloudTable table;

        // Called when the page is requested and gets loaded.
        // Records telemetry data on page load times.
        protected void Page_Load(object sender, EventArgs e)
        {
            telemetry.TrackEvent("Page_load");
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();

            ErrorMessage.Visible = false;
            ErrorMessage.ForeColor = System.Drawing.Color.Red;
            try
            {
                storageAccount = CloudStorageAccount.Parse(ConnectionString);
                tableClient = storageAccount.CreateCloudTableClient();
                table = tableClient.GetTableReference(tableName);
                table.CreateIfNotExists();
            }
            catch (StorageException StorageExceptionObj)
            {
                throw StorageExceptionObj;
                telemetry.TrackException(StorageExceptionObj);
            }
            catch (Exception ExceptionObj)
            {
                throw ExceptionObj;
                telemetry.TrackException(ExceptionObj);
            }

            stopwatch.Stop();
            var metrics = new Dictionary<string, double> { { "processingTime", stopwatch.Elapsed.TotalMilliseconds } };
            telemetry.TrackEvent("SignalProcessed for page load", null, metrics);
        }

        // Called when the login button is clicked.
        // Checks if the username and password exist in azure table storage.
        // If not, an error message is displayed, otherwise the user is logged in and
        // moved to the mainpage.
        protected void Login_Click(object sender, EventArgs e)
        {
            telemetry.TrackEvent("Login");
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();

            string userName = UsernameBox.Text;
            string pass = PasswordBox.Text;
            if (userName != "" && pass != "")
            {
                try
                {
                    TableQuery<UserEntry> query = new TableQuery<UserEntry>();
                    IEnumerable<UserEntry> results = new List<UserEntry>();
                    results = table.ExecuteQuery(query);
                    if (!results.Any())
                    {
                        ErrorMessage.Text = "Invalid Username or Password";
                        ErrorMessage.Visible = true;
                    } else {
                        // Check if the user exists
                        foreach (UserEntry entity in results)
                        {
                            if (entity.PartitionKey.Equals(userName) && entity.RowKey.Equals(pass))
                            {
                                Response.Redirect("MainPage.aspx");
                            } else
                            {
                                ErrorMessage.Text = "Invalid Username or Password";
                                ErrorMessage.Visible = true;
                            }
                        }
                    }

                    
                }
                catch (StorageException k)
                {
                    Console.WriteLine(k.Message);
                    Console.ReadLine();
                    telemetry.TrackException(k);
                }
            }

            // Record success telemetrics
            stopwatch.Stop();
            var metrics = new Dictionary<string, double> { { "processingTime", stopwatch.Elapsed.TotalMilliseconds } };
            telemetry.TrackEvent("SignalProcessed for user log in ", null, metrics);
        }

        // Called when the create user button is clicked.
        // When the creater user button is clicked, a user is created with what is
        // typed into the username and password boxes.
        // A new user is created if the account does not already exist.
        // Otherwise they are prompted with an error message.
        // When the account is created, the user is moved to the mainpage.
        protected void NewUser_Click(object sender, EventArgs e)
        {
            telemetry.TrackEvent("New_User_Login");
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();

            string userName = UsernameBox.Text;
            string pass = PasswordBox.Text;
            try
            {
                TableQuery<UserEntry> query = new TableQuery<UserEntry>();
                IEnumerable<UserEntry> results = new List<UserEntry>();
                results = table.ExecuteQuery(query);
                if (!results.Any())
                {
                    UserEntry newUser = new UserEntry(userName, pass);
                    TableOperation insertOrMergeOperation = TableOperation.InsertOrReplace(newUser);
                    TableResult result = table.ExecuteAsync(insertOrMergeOperation).Result;
                    Response.Redirect("MainPage.aspx");
                } else
                {
                    // Check if the account already exists in the table storage
                    foreach (UserEntry entity in table.ExecuteQuery(query))
                    {
                        if (entity.PartitionKey.Equals(userName) || entity.RowKey.Equals(pass))
                        {
                            ErrorMessage.Text = "Account Already Exists";
                            ErrorMessage.Visible = true;
                        }
                        else
                        {
                            UserEntry newUser = new UserEntry(userName, pass);
                            TableOperation insertOrMergeOperation = TableOperation.InsertOrReplace(newUser);
                            TableResult result = table.ExecuteAsync(insertOrMergeOperation).Result;
                            Response.Redirect("MainPage.aspx");
                        }
                    }
                }
                    
            }
            catch (StorageException k)
            {
                Console.WriteLine(k.Message);
                Console.ReadLine();
                telemetry.TrackException(k);
            }

            // Record success telemetrics
            stopwatch.Stop();
            var metrics = new Dictionary<string, double> { { "processingTime", stopwatch.Elapsed.TotalMilliseconds } };
            telemetry.TrackEvent("SignalProcessed for new user", null, metrics);
        }
    }
}