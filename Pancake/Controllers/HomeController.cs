using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;
using System.Text;
using System.Security.Cryptography;
using System.ComponentModel.DataAnnotations;

namespace Pancake.Controllers
{
    public class Hash
    {
        public static string getHashSha256(string text)
        {
            byte[] bytes = Encoding.Unicode.GetBytes(text);
            SHA256Managed hashstring = new SHA256Managed();
            byte[] hash = hashstring.ComputeHash(bytes);
            string hashString = string.Empty;
            foreach (byte x in hash)
            {
                hashString += String.Format("{0:x2}", x);
            }
            return hashString;
        }
    }

    public class PancakeEntity : TableEntity
    {
        public PancakeEntity() { }
        public string Title { get; set; }
        public DateTime ValidFrom { get; set; }
    }

    public class HomeController : Controller
    {
        CloudStorageAccount storageAccount = CloudStorageAccount.Parse(Environment.GetEnvironmentVariable("StorageConnectionString"));
       
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(PancakeEntity pancakeEntity)
        {
            CloudQueueClient queueClient = storageAccount.CreateCloudQueueClient();
            CloudQueue queue = queueClient.GetQueueReference("pancakequeue");
            queue.CreateIfNotExists();

            pancakeEntity.RowKey = Hash.getHashSha256(String.Format("{0}{1}{2}{3}",
                pancakeEntity.PartitionKey, pancakeEntity.RowKey, pancakeEntity.Title, pancakeEntity.Timestamp));
            var jsonObject = JsonConvert.SerializeObject(pancakeEntity);

            CloudQueueMessage message = new CloudQueueMessage(jsonObject);
            queue.AddMessage(message);
            return RedirectToAction("Index");
        }

        public ActionResult Get()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Get(string ClientData)
        {
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
            CloudTable table = tableClient.GetTableReference("pancaketable");
            var currentMonth = DateTime.Now;
            string condition1 = TableQuery.GenerateFilterCondition(
                "PartitionKey", QueryComparisons.Equal, currentMonth.ToString("yyyyMM"));
            string condition2 = TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.Equal, ClientData);

            TableQuery<PancakeEntity> query = new TableQuery<PancakeEntity>().Where(TableQuery.CombineFilters(condition1, TableOperators.And, condition2));
            // TableQuery<PancakeEntity> query = new TableQuery<PancakeEntity>().Where(TableQuery.GenerateFilterCondition(
            //    "PartitionKey", QueryComparisons.Equal, "pancakeShell"));

            var queryResult = table.ExecuteQuery(query).ToArray();
            if (queryResult.Length == 1)
            {
                return View("Display", queryResult[0]);
            }
            else
            {
                ViewBag.Message = "Erop";
                return View();
            }
        }

        public ActionResult Display(PancakeEntity item)
        {
            return View(item);
        }
    }
}
