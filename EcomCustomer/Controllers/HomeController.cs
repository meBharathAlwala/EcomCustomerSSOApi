using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web.Resource;

namespace EcomCustomer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HomeController : ControllerBase
    {
        // GET: api/Home/text
        [HttpGet("text")]
       
        public IActionResult GetText()
        {
            string responseText = "This is a plain text response from the API.";
            return Content(responseText, "text/plain");
        }


    }
}
