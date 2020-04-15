using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Fly_Me_To_The_Moon.Content
{
    public class UserEntry : TableEntity
    {
        public UserEntry()
        {

        }

        public UserEntry(string userName, string pass)
        {
            PartitionKey = userName;
            RowKey = pass;
        }
    }
}