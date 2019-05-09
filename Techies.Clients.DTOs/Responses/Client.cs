using System;

namespace Techies.Clients.DTOs.Responses
{
    public class ClientResponse
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime Birthdate { get; set; }
        public int AgeInYears { get; set; }        
    }
}
