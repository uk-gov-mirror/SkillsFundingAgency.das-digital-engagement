﻿using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;

namespace SFA.DAS.Campaign.Functions.DataCollectionStub
{
    public static class CreateWiredPlusUser
    {
        [FunctionName("CreateUser")]
        public static async Task Run([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "v1/CreateContact")]HttpRequestMessage req, ILogger log)
        {
            log.LogInformation("C# HTTP trigger CreateContact processed a request.");

            var auth = System.Text.Encoding.UTF8.GetString(System.Convert.FromBase64String(req.Headers.Authorization.Parameter));

            var requestBody = await req.Content.ReadAsStringAsync();

            log.LogInformation($"Following data received: {requestBody} Auth:{auth}");

        }
    }
}
