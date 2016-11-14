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
        private static readonly Lazy<ISemanticProccessor> _serviceClient;
        private static readonly Lazy<ITweeterStreamDownloader> _twitterClient;

        static SemanticController()
        {
            _serviceClient = new Lazy<ISemanticProccessor>(ServiceResolver.GetTrainedSemanticProccessor);
            _twitterClient = new Lazy<ITweeterStreamDownloader>(ServiceResolver.GetStartedTweeterStreamDownloader);
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
            var result = _serviceClient.Value.Process(toEvaluate);
            view.ViewData.Add("result", result);
            return view;
        }

        public ActionResult Twitter()
        {
            return View("Twitter");
        }
        
        private object ProcessMesssages()
        {
            var stats = _twitterClient.Value.DailyStat();
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
            var tweets = _twitterClient.Value.
                Tweets().
                Where(x => x != null).
                Select(x => new object[]
                {
                    x.Semantics.Text,
                    x.Semantics.ToHtmlPrint(),
                    x.TweetLocalCreationDate.TimeOfDay.ToString("hh\\:mm\\:ss"),
                    x.Language,
                    x.CreatedBy
                }).ToArray();
            return Json(tweets, JsonRequestBehavior.AllowGet);
        }
    }

}