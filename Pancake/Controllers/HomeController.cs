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
        CloudStorageAccount storageAccount = CloudStorageAccount.Parse(Environment.GetEnvironmentVariable("APPSETTING_StorageConnectionString"));
       
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About(String queueMessage)
        {
            //Environment.GetEnvironmentVariable("APPSETTING_StorageConnectionString");
            CloudQueueClient queueClient = storageAccount.CreateCloudQueueClient();
            // Retrieve a reference to a container.
            CloudQueue queue = queueClient.GetQueueReference("pancakequeue");

            // Create the queue if it doesn't already exist
            queue.CreateIfNotExists();

            // Create a message and add it to the queue.
            CloudQueueMessage message = new CloudQueueMessage(queueMessage);
            queue.AddMessage(message);
            // Peek at the next message
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