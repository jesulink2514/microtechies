using System;

namespace Techies.Clients.DTOs.Request
{
    public class RegisterClient
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public DateTime Birthdate { get; set; }
        public string User { get; set; }        
    }
}