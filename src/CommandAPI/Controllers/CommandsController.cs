using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using CommandAPI.Models;
using Microsoft.EntityFrameworkCore;

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


        [HttpPut]
        public ActionResult PutCommandItem(int id, Command command)
        {
            if (id != command.Id) return BadRequest();

            _context.Entry(command).State = EntityState.Modified;
            _context.SaveChanges();

            return NoContent();
        }


        [HttpDelete]
        public ActionResult<Command> DeleteCommandItem(int id)
        {
            var item = _context.CommandItems.Find(id);
            if (item == null) return NotFound();

            _context.CommandItems.Remove(item);
            _context.SaveChanges();

            return item;
        }
    }
}