using System.Web.Mvc;
using Semanticer;
using Semanticer.WcfClient;

namespace SemanticerDemo.Controllers
{
    public class SemanticController : Controller
    {
        private ISemanticProccessor serviceClient;

        public SemanticController()
        {
            var processor = ServiceResolver.GetSemanticProcessor();
            var processorHelper = new SemanticerServiceHelper(processor);
            serviceClient = processorHelper.Proccessor;
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