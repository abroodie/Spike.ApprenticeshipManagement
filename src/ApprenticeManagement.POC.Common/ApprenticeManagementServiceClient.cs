﻿using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ApprenticeManagement.POC.Common;

public class ApprenticeManagementServiceClient
{
    public HttpClient HttpClient { get; set; }
    public ApprenticeManagementServiceClient(string apprenticeManagementServiceBaseUri)
    {
        if (apprenticeManagementServiceBaseUri == null) throw new ArgumentNullException(nameof(apprenticeManagementServiceBaseUri));
        HttpClient = new HttpClient()
        {
            BaseAddress = new Uri(apprenticeManagementServiceBaseUri)
        };
    }

    public async Task<List<Employer>> GetEmployers()
    {
        var response = await HttpClient.GetFromJsonAsync<EmployersPayload>($"employers");
        return response.Employers;
    }

    public async Task<Employer> GetEmployer(string employerAccount)
    {
        var employer = await HttpClient.GetFromJsonAsync<Employer>($"employers/{employerAccount}");
        return employer;
    }

    public async Task AddNewApprentice(string employerAccount, Apprentice apprentice)
    {
        await HttpClient.PostAsync($"employers/{employerAccount}/apprentices",  new StringContent(JsonSerializer.Serialize(apprentice)));
    }

    public async Task ChangeApprenticeStatus(string employerAccount, string uln, ApprenticeStatus status)
    {
        await HttpClient.PutAsync($"employers/{employerAccount}/apprentices/{uln}/changestatus",new StringContent(string.Empty));
    }

    public async Task Approve(string employerAccount, string uln)
    {
        await HttpClient.PostAsync($"employers/{employerAccount}/apprentices/{uln}/approve", new StringContent(string.Empty));
    }

    public async Task SendReminder(string employerAccount)
    {
        await HttpClient.PostAsync($"employers/{employerAccount}/remind", new StringContent(string.Empty));
    }
}