namespace DASApprenticeManagementPOC.Authentication;

public class UserAccounts
{
    public List<UserAccount> Users { get; private set; } = new List<UserAccount>()
    {
        new UserAccount { Name = "Sarah", Username = "sarah@employer1.com", EmployerAccount = "1234" },
        new UserAccount { Name="Bob", Username = "bob@employer2.com", EmployerAccount = "2345" }
    };
}