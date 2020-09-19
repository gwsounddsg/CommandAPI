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


        [Test]
        public void GetCommandItem_404()
        {
            var result = controller.GetCommandItem(0);
            Assert.That(result.Result, Is.TypeOf<NotFoundResult>());
        }


        [Test]
        public void GetCommandItem_CorrectType()
        {
            // prep
            const int size = 1;
            var commands = CreateCommands(size);

            // call
            var result = controller.GetCommandItem(commands[0].Id);

            // verify
            Assert.That(result, Is.TypeOf<ActionResult<Command>>());
        }


        [Test]
        public void GetCommandItem_CorrectResource()
        {
            // prep
            const int size = 1;
            var commands = CreateCommands(size);

            // call
            var result = controller.GetCommandItem(commands[0].Id);

            // verify
            Assert.That(result.Value.Id == commands[0].Id);
        }





        #region Utility


        private IList<Command> CreateCommands(int count)
        {
            var commands = new List<Command>();

            for (int i = 0; i < count; i++)
            {
                var command = new Command
                {
                    HowTo = "Do Something",
                    Platform = "Some Platform",
                    CommandLine = count.ToString()
                };

                dbContext.CommandItems.Add(command);
                commands.Add(command);
            }

            dbContext.SaveChanges();
            return commands;
        }

        #endregion
    }
}