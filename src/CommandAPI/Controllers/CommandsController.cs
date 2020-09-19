using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using CommandAPI.Models;





namespace CommandAPI.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class CommandsController : ControllerBase
    {
        private readonly CommandContext _context;


        public CommandsController(CommandContext context)
        {
            _context = context;
        }


        [HttpGet]
        public ActionResult<IEnumerable<Command>> GetCommandItems()
        {
            return _context.CommandItems;
        }


        [HttpGet("{id}")]
        public ActionResult<Command> GetCommandItem(int id)
        {
            var item = _context.CommandItems.Find(id);
            if (item == null) return NotFound();
            return item;
        }


        [HttpPost]
        public ActionResult<Command> PostCommandItem(Command command)
        {
            _context.CommandItems.Add(command);

            try
            {
                _context.SaveChanges();
            }
            catch
            {
                return BadRequest();
            }

            return CreatedAtAction("GetCommandItem", new Command{Id = command.Id}, command);
        }
    }
}