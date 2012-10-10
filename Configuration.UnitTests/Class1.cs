using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ConfigManager.Controllers;
using ConfigManager.Models;

using NUnit.Framework;

using Rhino.Mocks;

using SqlToGraphite.Conf;

namespace Configuration.UnitTests
{
    [TestFixture]
    public class Class1
    {
        //    [Test]
        //    public void Should_display_all_clients()
        //    {
        //        var configRepositoryMock = MockRepository.GenerateMock<IConfigRepository>();
        //        var clients = new ListOfUniqueType<Client>();
        //        var rtn1 = new Client { Name = "nameA", Port = 32 };
        //        var rtn2 = new Client { Name = "nameB", Port = 33 };
        //        clients.Add(rtn1);
        //        clients.Add(rtn2);
        //        configRepositoryMock.Expect(x => x.GetClients()).Return(clients);


        //        var displayClientsController = new ClientsController(configRepositoryMock);
        //        var result = displayClientsController.Index();
        //        var model = (ClientsViewModel) result.Model;
        //        Assert.That(model.Clients.Count, Is.EqualTo(clients.Count));            
        //        Assert.That(model.Clients[0].Name, Is.EqualTo(rtn1.name));
        //        Assert.That(model.Clients[0].Port, Is.EqualTo(Convert.ToInt32(rtn1.port)));
        //        Assert.That(model.Clients[1].Name, Is.EqualTo(rtn2.name));
        //        Assert.That(model.Clients[1].Port, Is.EqualTo(Convert.ToInt32(rtn2.port)));

        //        configRepositoryMock.VerifyAllExpectations();
        //    }
    }
}