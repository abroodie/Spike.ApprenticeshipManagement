using System.Net;
using System.Text.Json;
using ApprenticeManagement.POC.Common;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace ApprenticeManagement.POC.Service;

public static class ApprenticeManagementApi
{
    private static DeviceManagementService deviceManagementService = new DeviceManagementService();

    private static List<Employer> employers = new List<Employer>()
    {
        new Employer
        {
            Account = "1234", Name = "Employer 1", Apprentices = new List<Apprentice>()
            {
                new Apprentice{ Course = "Software Developer (level 4)", Name = "Adrianna Hess", Uln = "1234567890" },
                new Apprentice{ Course = "Church minister (integrated degree) (level 6)", Name = "Lawrence Fernandez", Uln = "0987654321" }
            }
        },
        new Employer
        {
            Account = "2345", Name = "Employer 2", Apprentices = new List<Apprentice>()
            {
                new Apprentice{ Course = "Digital and technology solutions professional (level 6)", Name = "Amara Singleton", Uln = "6789012345" },
                new Apprentice{ Course = "Adult care worker (level 2)", Name = "Arabella Terrell", Uln = "5432109876" }
            }
        },
    };

    [Function(nameof(GetEmployers))]
    public static async Task<HttpResponseData> GetEmployers(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "employers")]
        HttpRequestData req,
        FunctionContext executionContext)
    {
        var logger = executionContext.GetLogger(nameof(ApprenticeManagementApi));
        logger.LogInformation($"Returning {employers.Count} employers.");
        var response = req.CreateResponse();
        await response.WriteAsJsonAsync(new {employers}, HttpStatusCode.OK);
        return response;


    }

    [Function(nameof(GetEmployer))]
    public static async Task<HttpResponseData> GetEmployer(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "employers/{employerAccount}")]
        HttpRequestData req,
        string employerAccount,
        FunctionContext executionContext)
    {
        var logger = executionContext.GetLogger(nameof(ApprenticeManagementApi));
        if (string.IsNullOrEmpty(employerAccount))
        {
            logger.LogWarning("Received invalid request for employer data.");
            return req.CreateResponse(HttpStatusCode.BadRequest);
        }
        var employer = employers.FirstOrDefault(employer => employer.Account.Equals(employerAccount));
        if (employer == null)
        {
            logger.LogWarning("Employer not found");
            return req.CreateResponse(HttpStatusCode.NotFound);
        }
        logger.LogInformation($"Got employer {employer.Name}. Employer has {employer.Apprentices.Count} apprentices.");
        var response = req.CreateResponse();
        await response.WriteAsJsonAsync(employer, HttpStatusCode.OK);
        return response;
    }

    [Function(nameof(GetApprentice))]
    public static async Task<HttpResponseData> GetApprentice(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get",
            Route = "employers/{employerAccount}/apprentices/{uln}")]
        HttpRequestData req,
        string employerAccount,
        string uln,
        FunctionContext executionContext)
    {
        var logger = executionContext.GetLogger(nameof(ApprenticeManagementApi));
        logger.LogInformation($"Getting Employer: {employerAccount}, apprentice: {uln}");
        if (string.IsNullOrWhiteSpace(employerAccount) || string.IsNullOrWhiteSpace(uln))
        {
            return req.CreateResponse(HttpStatusCode.BadRequest);
        }

        var apprentice = employers.SelectMany(e => e.Apprentices).FirstOrDefault(a => a.Uln.Equals(uln));
        if (apprentice == null)
        {
            return req.CreateResponse(HttpStatusCode.NotFound);
        }

        var response = req.CreateResponse();
        await response.WriteAsJsonAsync(apprentice, HttpStatusCode.OK);
        return req.CreateResponse(HttpStatusCode.OK);
    }

    [Function(nameof(AddApprentice))]
    public static async Task<HttpResponseData> AddApprentice(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post",
            Route = "employers/{employerAccount}/apprentices")]
        HttpRequestData req,
        string employerAccount,
        FunctionContext executionContext)
    {
        var logger = executionContext.GetLogger(nameof(ApprenticeManagementApi));
        logger.LogInformation($"Getting Employer: {employerAccount}");
        if (string.IsNullOrWhiteSpace(employerAccount))
        {
            return req.CreateResponse(HttpStatusCode.BadRequest);
        }

        var employer = employers.FirstOrDefault(e => e.Account.Equals(employerAccount));
        if (employer == null)
        {
            return req.CreateResponse(HttpStatusCode.NotFound);
        }
        var apprentice = await JsonSerializer.DeserializeAsync<Apprentice>(req.Body);

        if (apprentice?.Uln == null)
        {
            return req.CreateResponse(HttpStatusCode.BadRequest);
        }

        apprentice.ApprenticeStatus = ApprenticeStatus.New;
        employer.Apprentices.Add(apprentice);
        await deviceManagementService.NotifyEmployer(employerAccount, "New Apprentice commitment(s) requiring approval",
            $"You have {employer.Apprentices.Count(a => a.ApprenticeStatus == ApprenticeStatus.New)} new apprenticeship commitments requiring approval.", logger);

        return req.CreateResponse(HttpStatusCode.OK);
    }

    [Function(nameof(ChangeApprenticeStatus))]
    public static async Task<HttpResponseData> ChangeApprenticeStatus(
        [HttpTrigger(AuthorizationLevel.Anonymous, "put",
            Route = "employers/{employerAccount}/apprentices/{uln}/changestatus")]
        HttpRequestData req,
        string employerAccount,
        string uln,
        FunctionContext executionContext)
    {
        var logger = executionContext.GetLogger(nameof(ApprenticeManagementApi));
        logger.LogInformation($"Employer: {employerAccount}, apprentice: {uln}");
        if (string.IsNullOrWhiteSpace(employerAccount) || string.IsNullOrWhiteSpace(uln))
        {
            return req.CreateResponse(HttpStatusCode.BadRequest);
        }
        
        var apprentice = employers.SelectMany(e => e.Apprentices).FirstOrDefault(a => a.Uln.Equals(uln));
        if (apprentice == null)
        {
            return req.CreateResponse(HttpStatusCode.NotFound);
        }

        apprentice.ApprenticeStatus = ApprenticeStatus.Changed;
        await deviceManagementService.NotifyEmployer(employerAccount, "Provider has requested change to Apprenticeship details",
            $"The provider has requested a change for {apprentice.Name}.", logger);
        return req.CreateResponse(HttpStatusCode.Accepted);
    }

    [Function(nameof(ApproveApprentice))]
    public static async Task<HttpResponseData> ApproveApprentice(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post",
            Route = "employers/{employerAccount}/apprentices/{uln}/approve")]
        HttpRequestData req,
        string employerAccount,
        string uln,
        FunctionContext executionContext)
    {
        var logger = executionContext.GetLogger(nameof(ApprenticeManagementApi));
        logger.LogInformation($"Approving Employer: {employerAccount}, apprentice: {uln}");
        if (string.IsNullOrWhiteSpace(employerAccount) || string.IsNullOrWhiteSpace(uln))
        {
            return req.CreateResponse(HttpStatusCode.BadRequest);
        }

        var apprentice = employers.SelectMany(e => e.Apprentices).FirstOrDefault(a => a.Uln.Equals(uln));
        if (apprentice == null)
        {
            return req.CreateResponse(HttpStatusCode.NotFound);
        }

        apprentice.ApprenticeStatus = ApprenticeStatus.Approved;
        await deviceManagementService.NotifyEmployer(employerAccount, "An apprenticeship commitment has been approved",
            $"The apprenticeship commitment for {apprentice.Name} has been approved.", logger);
        return req.CreateResponse(HttpStatusCode.OK);
    }

    [Function(nameof(RemindEmployer))]
    public static async Task<HttpResponseData> RemindEmployer(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post",
            Route = "employers/{employerAccount}/remind")]
        HttpRequestData req,
        string employerAccount,
        FunctionContext executionContext)
    {
        var logger = executionContext.GetLogger(nameof(ApprenticeManagementApi));
        logger.LogInformation($"Approving Employer: {employerAccount}");
        if (string.IsNullOrWhiteSpace(employerAccount))
        {
            return req.CreateResponse(HttpStatusCode.BadRequest);
        }

        var employer = employers.FirstOrDefault(e => e.Account.Equals(employerAccount));
        if (employer == null)
        {
            return req.CreateResponse(HttpStatusCode.NotFound);
        }

        if (employer.Apprentices.All(a => a.ApprenticeStatus == ApprenticeStatus.Approved))
        {
            return req.CreateResponse(HttpStatusCode.OK);
        }

        await deviceManagementService.NotifyEmployer(employerAccount, "Reminder: Apprenticeship commitments require approval",
            $"You have {employer.Apprentices.Count(a => a.ApprenticeStatus != ApprenticeStatus.Approved)} apprenticeship commitments requiring approval.  Please check your employer account.", logger);
        return req.CreateResponse(HttpStatusCode.OK);
    }
}