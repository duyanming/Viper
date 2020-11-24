using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using MvcCallAnno.Service;

namespace MvcCallAnno.Controllers
{
    public class ValuesController : ApiController
    {
        private readonly IHelloWorldViperService helloWorldViperService;
        public ValuesController(IHelloWorldViperService helloWorldViperService) {
            this.helloWorldViperService = helloWorldViperService;
        }
        [HttpGet]
        public dynamic SayHello(string name, int age) {
            var rlt= helloWorldViperService.SayHello(name,age);
            return Json(rlt);
        }
        [HttpGet]
        public int Subtraction(int x, int y) {
            return helloWorldViperService.Subtraction(x,y);
        }
        [HttpGet]
        public dynamic BuyProduct(string productName, int number) {
            return helloWorldViperService.BuyProduct(productName, number);
        }
    }
}