using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;





namespace CommandAPI.Controller
{

    [ApiController]
    [Route("api/[controller]")]
    public class CommandsController : ControllerBase
    {
        [HttpGet]
        public ActionResult<IEnumerable<string>> Get()
        {
            return new string[] { "this", "is", "hard", "coded"};
        }
    }
}