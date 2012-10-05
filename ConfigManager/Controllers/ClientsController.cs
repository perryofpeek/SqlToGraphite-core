using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using ConfigManager.Models;

using SqlToGraphite.Conf;

namespace ConfigManager.Controllers
{
    public class ClientsController : Controller
    {
        private readonly IConfigRepository configRepository;

        public ClientsController(IConfigRepository configRepository )
        {
            this.configRepository = configRepository;
        }

        //
        // GET: /Clients/

        public ViewResult Index()
        {
            configRepository.Load();
            var clientList = configRepository.GetClients();
            var clients = clientList.Select(item => new ClientsViewModelClient(item.ClientName, Convert.ToInt32(item.Port))).ToList();
            var displayClientsViewModel = new ClientsViewModel(clients);
            return View(displayClientsViewModel);
        }

        public ViewResult Display()
        {
            configRepository.Load();
            var clientList = configRepository.GetClients();
            var clients = clientList.Select(item => new ClientsViewModelClient(item.ClientName, Convert.ToInt32(item.Port))).ToList();
            var displayClientsViewModel = new ClientsViewModel(clients);
            return View(displayClientsViewModel);
        }

        //
        // GET: /Clients/Add
         [HttpPost]
        public ActionResult Add(Client client)
         {

             var n = ModelState["Name"];
             var p = ModelState["Port"];

            if (ModelState.IsValid)
            {
                // Attempt to register the user
                    configRepository.Load();
                   // configRepository.AddClient(client.Name, client.Port.ToString());       
                    configRepository.Save();
                    return RedirectToAction("Index", "Clients");
                   
            }           
             // If we got this far, something failed, redisplay form
            return View();
        }
    }
}
