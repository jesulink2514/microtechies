using Nest;
using System;

namespace Techies.Client.Stats.Api.Model
{
    public class ClientModel
    {
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime Birthdate { get;set;}
        public int BirthdateYear { get; set; }
        public DateTime ProbablyDeathDate { get;set;}
        public string CreatedBy { get; set; }
        public DateTime CreationDate { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime UpdateDate { get; set; }
    }
}
