using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Semanticer.Common;
using Semanticer.Common.Enums;
using Semanticer.WcfClient;

namespace SemanticerDemo.Controllers
{
    public class SemanticController : Controller
    {
        private readonly Lazy<ISemanticProccessor> serviceClient;
        private readonly Lazy<ITweeterStreamDownloader> twitterClient;

        public SemanticController()
        {
            serviceClient = new Lazy<ISemanticProccessor>(ServiceResolver.GetTrainedSemanticProccessor);
            twitterClient = new Lazy<ITweeterStreamDownloader>(ServiceResolver.GetStartedTweeterStreamDownloader);
        }

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ActionName("Index")]
        public ActionResult Index(string toEvaluate)
        {
            var view = View();
            var result = serviceClient.Value.Process(toEvaluate);
            view.ViewData.Add("result", result);
            return view;
        }

        public ActionResult Twitter()
        {
            return View("Twitter");
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
            var stats = twitterClient.Value.DailyStat();
            var today = DateTime.Today;
            var enumValues = (IEnumerable<MarkType>) Enum.GetValues(typeof(MarkType));
            return enumValues.Select(MarkType => new
            {
                Mark = MarkType.ToString(),
                Items = stats.HourStats.Select((x, i) => new
                {
                    Time = $"{i}.00",
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
            return Json(twitterClient.Value.Tweets(), JsonRequestBehavior.AllowGet);
        }
    }

}