using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ConfigManager.Models
{
    public class ClientsViewModel
    {
        public ClientsViewModel(List<ClientsViewModelClient> clients)
        {
            this.Clients = clients;
        }

        public List<ClientsViewModelClient> Clients { get; private set; }

        public Client Client { get; set; }
    }

    public class ClientsViewModelClient
    {        
        public ClientsViewModelClient(string name,int port)
        {
            this.Port = port;
            this.Name = name;
        }

        public string Name { get; private set; }
        
        public int Port { get; private set; }
    }

    public class Client
    {
        [Required(ErrorMessage = "You must enter a {0}")]
        [StringLength(30, ErrorMessage = "the name is too long")]
        public string Name { get; set; }

        [Required(ErrorMessage = "You must enter a {0}")]
        [Range(1, 65000)]
        public int Port { get; set; }
    }
}