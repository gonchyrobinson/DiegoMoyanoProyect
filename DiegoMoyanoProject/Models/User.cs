namespace DiegoMoyanoProject.Models
{
    public enum Role
    {
        Admin = 1,
        Owner = 2,
        Operative = 3
    }
    public class User
    {
        private int id;
        private string mail;
        private string pass;
        private string username;
        private Role role;
        private decimal capitalInvested;
        private decimal rentability;

        public User()
        {
            id = 1;
            mail = string.Empty;
            pass = string.Empty;
            username = string.Empty;
            role = Role.Admin;
            capitalInvested = 0;
            rentability = 0;
        }

        public User(int id, string mail, string pass, string username, Role role)
        {
            this.id = id;
            this.mail = mail;
            this.pass = pass;
            this.Username = username;
            this.role = role;
            capitalInvested = 0;
            rentability = 0;
        }

        public User(int id, string mail, string pass, string username, Role role, decimal capitalInvested, decimal rentability)
        {
            this.id = id;
            this.mail = mail;
            this.pass = pass;
            this.username = username;
            this.role = role;
            this.capitalInvested = capitalInvested;
            this.rentability = rentability;
        }

        public User(int id, decimal capitalInvested, decimal rentability)
        {
            this.id = id;
            this.capitalInvested = capitalInvested;
            this.rentability = rentability;
        }

        public int Id { get => id; set => id = value; }
        public string Mail { get => mail; set => mail = value; }
        public string Pass { get => pass; set => pass = value; }
        public Role Role { get => role; set => role = value; }
        public string Username { get => username; set => username = value; }
        public decimal CapitalInvested { get => capitalInvested; set => capitalInvested = value; }
        public decimal Rentability { get => rentability; set => rentability = value; }
    }
}