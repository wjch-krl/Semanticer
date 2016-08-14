using System.Web.Mvc;
using Semanticer;

namespace SemanticerDemo.Controllers
{
    public class SemanticController : Controller
    {
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
            var result = SemanticProccessor.Process(toEvaluate);
            view.ViewData.Add("result", result);
            return view;
        }
    }

}