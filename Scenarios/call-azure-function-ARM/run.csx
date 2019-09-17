#r "System.Web"
#r "System.Runtime"
#r "System.Threading.Tasks"
#r "System.IO"
#r "Newtonsoft.Json"

using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using System;
using System.Net.Http.Headers;
using System.Collections.Generic;
using System.IO;


//from https://blog.cloudtrooper.net/2017/04/04/run-azure-functions-from-your-quickstart-arm-templates/
public static async Task<IActionResult> Run(HttpRequest req, ILogger log)
{
    log.LogInformation("C# HTTP trigger function processed a request.");

    string resourcegroupname = req.Query["resourcegroupname"];
    
    string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
    dynamic data = JsonConvert.DeserializeObject(requestBody);
    resourcegroupname = resourcegroupname ?? data?.resourcegroupname;
    
    //can do something with the resource group name
    if (!String.IsNullOrEmpty(resourcegroupname))
    {
        //POST https://management.azure.com/subscriptions/{subscriptionId}/resourcegroups/{resourceGroupName}/exportTemplate?api-version=2019-05-10
        using (var client = new HttpClient()) 
        { 
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Add("User-Agent", "AzureFunctions");
            var webUri = $"https://raw.githubusercontent.com/Azure/azure-quickstart-templates/master/100-blank-template/azuredeploy.json";
            HttpResponseMessage response = client.GetAsync(webUri).Result; 
            var responseString = response.Content.ReadAsStringAsync().Result;
            return new OkObjectResult(responseString);
        } 
    }
       
    var template = @"{'$schema': 'https://schema.management.azure.com/schemas/2015-01-01/deploymentTemplate.json#', 'contentVersion': '1.0.0.0', 'parameters': {}, 'variables': {}, 'resources': []}";
    var content = new StringContent(template, System.Text.Encoding.UTF8, "application/json");

    return resourcegroupname != null
        ? (ActionResult)new OkObjectResult(content)
        : new BadRequestObjectResult("Please pass a resourcegroupname on the query string or in the request body");
}
