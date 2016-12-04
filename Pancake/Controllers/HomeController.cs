using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;

namespace Pancake.Controllers
{
    
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
            ViewBag.Message = "Your contact page.";


            return View();
        }
    }
}
