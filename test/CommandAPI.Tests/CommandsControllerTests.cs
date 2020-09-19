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


        [Test]
        public void PostCommandItem_Increment()
        {
            // prep
            var command = NewCommand("something");
            var oldCount = dbContext.CommandItems.Count();

            // call
            var result = controller.PostCommandItem(command);

            // verify
            Assert.That(oldCount + 1 == dbContext.CommandItems.Count());
        }


        [Test]
        public void PostCommandItem_201()
        {
            // prep
            var command = NewCommand("something");
            var oldCount = dbContext.CommandItems.Count();

            // call
            var result = controller.PostCommandItem(command);

            // verify
            Assert.That(result.Result, Is.TypeOf<CreatedAtActionResult>());
        }


        [Test]
        public void PutCommandItem_AttributeUpdated()
        {
            // prep
            var commands = CreateCommands(1);
            commands[0].HowTo = "UPDATED";

            // call
            controller.PutCommandItem(commands[0].Id, commands[0]);
            var result = dbContext.CommandItems.Find(commands[0].Id);

            // verify
            Assert.That(commands[0].HowTo == result.HowTo);
        }


        [Test]
        public void PutCommandItem_204()
        {
            // prep
            var commands = CreateCommands(1);
            commands[0].HowTo = "UPDATED";

            // call
            var result = controller.PutCommandItem(commands[0].Id, commands[0]);

            // verify
            Assert.That(result, Is.TypeOf<NoContentResult>());
        }


        [Test]
        public void PutCommandItem_400()
        {
            // prep
            var commands = CreateCommands(1);
            commands[0].HowTo = "UPDATED";

            // call
            var result = controller.PutCommandItem(commands[0].Id + 1, commands[0]);

            // verify
            Assert.That(result, Is.TypeOf<BadRequestResult>());
        }


        [Test]
        public void PutCommandItem_Unchanged()
        {
            // prep
            var commands = CreateCommands(2);
            commands[1].Id = commands[0].Id + 1;

            // call
            controller.PutCommandItem(commands[0].Id + 1, commands[1]);
            var result = dbContext.CommandItems.Find(commands[0].Id);

            // verify
            Assert.That(commands[0].HowTo == result.HowTo);
        }


        [Test]
        public void DeleteCommandItem_Decriment()
        {
            // prep
            var commands = CreateCommands(1);
            var cmdId = commands[0].Id;
            var objCount = dbContext.CommandItems.Count();

            // call
            controller.DeleteCommandItem(cmdId);

            // verify
            Assert.That(objCount - 1 == dbContext.CommandItems.Count());
        }


        [Test]
        public void DeleteCommandItem_200()
        {
            // prep
            var commands = CreateCommands(1);

            // call
            var result = controller.DeleteCommandItem(commands[0].Id);

            // verify
            Assert.Null(result.Result);
        }


        [Test]
        public void DeleteCommandItem_404()
        {
            var result = controller.DeleteCommandItem(-1);
            Assert.That(result.Result, Is.TypeOf<NotFoundResult>());
        }


        [Test]
        public void DeleteCommandItem_NoChange()
        {
            // prep
            var commands = CreateCommands(1);
            var cmdId = commands[0].Id;
            var objCount = dbContext.CommandItems.Count();

            // call
            var result = controller.DeleteCommandItem(cmdId + 1);

            // verify
            Assert.That(dbContext.CommandItems.Count() == objCount);
        }





        #region Utility


        private IList<Command> CreateCommands(int count)
        {
            var commands = new List<Command>();

            for (int i = 0; i < count; i++)
            {
                var command = NewCommand(i.ToString());
                dbContext.CommandItems.Add(command);
                commands.Add(command);
            }

            dbContext.SaveChanges();
            return commands;
        }


        private Command NewCommand(string commandLine)
        {
            var command = new Command
            {
                HowTo = "Do Something",
                Platform = "Some Platform",
                CommandLine = commandLine
            };

            return command;
        }

        #endregion
    }
}