using System.Web.Mvc;
using Semanticer;
using SemanticerDemo.Semanticer.Wcf;

namespace SemanticerDemo.Controllers
{
    public class SemanticController : Controller
    {
        private SemanticProccessorServiceClient serviceClient = new SemanticProccessorServiceClient();
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