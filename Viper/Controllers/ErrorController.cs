using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace Viper.GetWay.Controllers
{

    [Route("error")]
    public class ErrorController : Controller
    {
        [Route("Index/{statusCode}")]
        // GET: /<controller>/
        public IActionResult Index(string statusCode)
        {
            var ex = HttpContext.Features.Get<IExceptionHandlerFeature>();
            ViewBag.Msg = ex?.Error.Message;
            return View(statusCode);
        }
    }
}