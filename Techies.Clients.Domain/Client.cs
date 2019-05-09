using System;

namespace Techies.Clients.Domain
{
    public class Client
    {
        protected Client(){}

        public Client(Guid id, string user)
        {
            Id = id;
            CreatedBy = user;
            UpdatedBy = user;
            CreationDate = DateTime.UtcNow;
            UpdateDate = DateTime.UtcNow;
        }
        private DateTime birthdate;
        public Guid Id { get; set; } = Guid.NewGuid();
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime Birthdate
        {
            get => birthdate;
            set { 
                if(value > DateTime.UtcNow.Date) throw new ArgumentOutOfRangeException("invalid birthdate");
                birthdate = value; 
           }
        }
        public string CreatedBy { get; set; }
        public DateTime CreationDate { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime UpdateDate { get; set; }
        public TimeSpan Age => DateTime.UtcNow.Subtract(Birthdate);
        public int AgeInYears => (int)Math.Floor(DateTime.UtcNow.Subtract(Birthdate).Days / 365.25);
    }
}
