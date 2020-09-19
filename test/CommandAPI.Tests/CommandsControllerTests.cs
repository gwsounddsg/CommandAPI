using NUnit.Framework;
using CommandAPI.Controllers;
using CommandAPI.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;


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


        [Test]
        public void GetCommandItems_OneItem()
        {
            // prep
            const int size = 1;
            CreateCommands(size);

            // call
            var result = controller.GetCommandItems();

            // verify
            Assert.That(result.Value.Count() == size);
        }


        [Test]
        public void GetCommandItems_TwoItems()
        {
            // prep
            const int size = 2;
            CreateCommands(size);

            // call
            var result = controller.GetCommandItems();

            // verify
            Assert.That(result.Value.Count() == size);
        }


        [Test]
        public void GetCommandItems_ReturnsCorrectType()
        {
            var result = controller.GetCommandItems();
            Assert.That(result, Is.TypeOf<ActionResult<IEnumerable<Command>>>());
        }


        [Test]
        public void GetCommandItem_BadID_Null()
        {
            var result = controller.GetCommandItem(0);
            Assert.Null(result.Value);
        }





        #region Utility


        private void CreateCommands(int count)
        {
            for (int i = 0; i < count; i++)
            {
                var command = new Command
                {
                    HowTo = "Do Something",
                    Platform = "Some Platform",
                    CommandLine = count.ToString()
                };

                dbContext.CommandItems.Add(command);
            }

            dbContext.SaveChanges();
        }

        #endregion
    }
}