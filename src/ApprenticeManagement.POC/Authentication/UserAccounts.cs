namespace ApprenticeManagement.POC.Authentication;

public class UserAccounts
{
    public List<UserAccount> Users { get; private set; } = new List<UserAccount>()
    {
        new UserAccount { Name = "Sarah", Username = "sarah@employer1.com", EmployerAccount = "1234", EmployerName = "Employer 1"},
        new UserAccount { Name = "Jonathan", Username = "jonathan@employer1.com", EmployerAccount = "1234", EmployerName = "Employer 1"},
        new UserAccount { Name = "Benjamin", Username = "benjamin@employer2.com", EmployerAccount = "2345", EmployerName = "Employer 2" },
        new UserAccount { Name = "Rosa", Username = "rosa@employer2.com", EmployerAccount = "2345", EmployerName = "Employer 2" }
    };
}