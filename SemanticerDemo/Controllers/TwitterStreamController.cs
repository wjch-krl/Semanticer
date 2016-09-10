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
            public PostMarkType Mark { get; set; }
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
            var enumValues = (IEnumerable<PostMarkType>)Enum.GetValues(typeof(PostMarkType));
            return enumValues.Select(postMarkType => new 
            {
                Mark = postMarkType.ToString(),
                Items = stats.HourStats.Select((x, i) => new 
                {
                    Time =$"{i}.00",
                    Count = (int) x[postMarkType]
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