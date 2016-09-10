using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Semanticer.Common;
using Semanticer.Common.Enums;
using Semanticer.WcfClient;

namespace SemanticerDemo.Controllers
{
    public class TwitterStreamController : Controller
    {
        private readonly Lazy<ITweeterStreamDownloader> serviceClient;

        public TwitterStreamController()
        {
            serviceClient = new Lazy<ITweeterStreamDownloader>(ServiceResolver.GetStartedTweeterStreamDownloader);
        }

        class ItemPerDay
        {
            public MarkType Mark { get; set; }
            public ItemPerHour[] Items { get; set; }
        }

        class ItemPerHour
        {
            public int Count { get; set; }
            public DateTime Time { get; set; }
        }

        private object ProcessMesssages()
        {
            var stats = serviceClient.Value.DailyStat();
            var today = DateTime.Today;
            var enumValues = (IEnumerable<MarkType>)Enum.GetValues(typeof(MarkType));
            return enumValues.Select(MarkType => new 
            {
                Mark = MarkType.ToString(),
                Items = stats.HourStats.Select((x, i) => new 
                {
                    Time =$"{i}.00",
                    Count = (int) x[MarkType]
                }).ToArray()
            }).ToArray();
        }

        public JsonResult GetStats()
        {
            return Json(ProcessMesssages(), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetTweets()   
        {
            return Json(serviceClient.Value.Tweets(), JsonRequestBehavior.AllowGet);
        }

//        public ActionResult Index()
//        {
//            return View();
//        }
    }
}