using NUnit.Framework;
using CommandAPI.Controllers;
using CommandAPI.Models;
using Microsoft.EntityFrameworkCore;


namespace CommandAPI.Tests
{
    public class CommandsControllerTests
    {
        DbContextOptionsBuilder<CommandContext> optionsBuilder;
        CommandContext dbContext;
        CommandsController controller;


        [SetUp]
        public void SetUp()
        {
            optionsBuilder = new DbContextOptionsBuilder<CommandContext>();
            optionsBuilder.UseInMemoryDatabase("UnitTestInMemBD");
            dbContext = new CommandContext(optionsBuilder.Options);

            controller = new CommandsController(dbContext);
        }


        [TearDown]
        public void Destroy()
        {
            optionsBuilder = null;

            foreach (var cmd in dbContext.CommandItems)
            {
                dbContext.CommandItems.Remove(cmd);
            }

            dbContext.SaveChanges();
            dbContext.Dispose();
            controller = null;
        }


        [Test]
        public void GetCommandItems_ReturnsZeroItems_WhenDBIsEmpty()
        {
            // call
            var result = controller.GetCommandItems();

            // verify
            Assert.IsEmpty(result.Value);
        }
    }
}