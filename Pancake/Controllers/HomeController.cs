using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using Microsoft.WindowsAzure.Storage.Table;

namespace Pancake.Controllers
{
    public class PancakeEntity : TableEntity
    {
        public PancakeEntity(string var1, string var2)
        {
            this.PartitionKey = var1;
            this.RowKey = var2;
        }

        public PancakeEntity() { }
        public string Title { get; set; }
       
    }

    public class HomeController : Controller
    {
        CloudStorageAccount storageAccount = CloudStorageAccount.Parse(Environment.GetEnvironmentVariable("StorageConnectionString"));
       
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About(String queueMessage)
        {
            CloudQueueClient queueClient = storageAccount.CreateCloudQueueClient();
            CloudQueue queue = queueClient.GetQueueReference("pancakequeue");
            queue.CreateIfNotExists();

            CloudQueueMessage message = new CloudQueueMessage(queueMessage);
            queue.AddMessage(message);
            CloudQueueMessage peekedMessage = queue.PeekMessage();
            ViewBag.Message = String.Format("Ваше сообщение добавлено: {0}", peekedMessage.AsString);

            return View();
        }

        public ActionResult Contact()
        {
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
            CloudTable table = tableClient.GetTableReference("pancaketable");

            TableQuery<PancakeEntity> query = new TableQuery<PancakeEntity>().Where(TableQuery.GenerateFilterCondition(
                "PartitionKey", QueryComparisons.Equal, "pancakeShell"));

            foreach (PancakeEntity item in table.ExecuteQuery(query))
            {
                ViewBag.Message = String.Format("Entity: {0}, id: {1}", item.Title, item.RowKey);
            }
            
            return View();
        }
    }
}
