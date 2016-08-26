using System.Web.Mvc;
using Semanticer;
using Semanticer.Common;
using Semanticer.WcfClient;

namespace SemanticerDemo.Controllers
{
    public class SemanticController : Controller
    {
        private readonly ISemanticProccessor serviceClient;

        public SemanticController()
        {
            serviceClient = ServiceResolver.GetTrainedSemanticProccessor();
        }

        // GET: Semantic
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ActionName("Index")]
        public ActionResult IndexPost(string toEvaluate)
        {
            var view = View();
            var result = serviceClient.Process(toEvaluate);
            view.ViewData.Add("result", result);
            return view;
        }
    }

}