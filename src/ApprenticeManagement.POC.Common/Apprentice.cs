namespace ApprenticeManagement.POC.Common;

public class Apprentice
{
    public string Name { get; set; }
    public string Course { get; set; }
    public string Uln { get; set; }
    public ApprenticeStatus ApprenticeStatus { get; set; }
    public string DisplayStatus => 
        ApprenticeStatus switch
        {
            ApprenticeStatus.New => "Awaiting initial approval",
            ApprenticeStatus.Changed => "Awaiting approval of change",
            _ => ApprenticeStatus.ToString()
        };
    
}