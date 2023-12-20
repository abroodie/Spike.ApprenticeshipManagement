namespace ApprenticeManagement.POC.Common;

public class Employer
{
    public string Name { get; set; }
    public string Account { get; set; }
    public List<Apprentice> Apprentices { get; set; }
}