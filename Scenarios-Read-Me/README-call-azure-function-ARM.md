## Call an Azure Function from ARM

The goal is in this scenario is to be able to call an Azure Function as part of the ARM template deployment.

We're going to have some simplifications here to demonstrate that we can deploy an App Service, but also call an Azure Function too.

> Note for more information on using the Azure App Service with VNET, please see the [documentation](./README-app-service-vnet-vm.md) for additional guidance.

This example is based off of [Calling Azure Functions from ARM](https://blog.cloudtrooper.net/2017/04/04/run-azure-functions-from-your-quickstart-arm-templates/).

![App Service VNET VM.](../Media/scenario-call-azure-function-ARM/scenario.png 'App Service VNET VM')

We have a placeholder in the Azure Function to call out to Github to get a sample Azure JSON template.  We could also use this Azure Function to call a web hook to trigger another process (for instance, a [web hook for Azure Automation](https://docs.microsoft.com/en-us/azure/automation/automation-webhooks)).



### Links

These will describe some of the concepts that we're using in this scenario.

1. [Calling Azure Functions from ARM](https://blog.cloudtrooper.net/2017/04/04/run-azure-functions-from-your-quickstart-arm-templates/)
1. [Azure Functions Best Practices](https://docs.microsoft.com/en-us/azure/azure-functions/functions-best-practices)
1. [Azure Functions Networking Options](https://docs.microsoft.com/en-us/azure/azure-functions/functions-networking-options)
1. [Azure Functions Monitoring Options](https://docs.microsoft.com/en-us/azure/azure-functions/functions-monitoring#streaming-logs)
1. [Azure Automation web hooks](https://docs.microsoft.com/en-us/azure/automation/automation-webhooks)
1. [Azure CLI ARM Deployments](https://docs.microsoft.com/en-us/azure/azure-resource-manager/resource-group-template-deploy-cli)
1. [ARM Quick Starts](https://github.com/Azure/azure-quickstart-templates)
1. [Install Azure CLI](https://docs.microsoft.com/en-us/cli/azure/install-azure-cli-windows?view=azure-cli-latest)
1. [Using Service Principals with Az Cli](https://docs.microsoft.com/en-us/cli/azure/create-an-azure-service-principal-azure-cli?view=azure-cli-latest)
1. [Check Service Principal Role Assignment](https://docs.microsoft.com/en-us/cli/azure/role/assignment?view=azure-cli-latest#az-role-assignment-create)

### Set up Azure Function

We can create the Azure Function first.

> Note that this is a POC.  Please refer to [Azure Functions Best Practices](https://docs.microsoft.com/en-us/azure/azure-functions/functions-best-practices) and [Networking Options](https://docs.microsoft.com/en-us/azure/azure-functions/functions-networking-options) for further considerations on Azure Functions.

Assuming we have created an Azure Function App, we can add a new Azure Function in the Azure portal.

![Create Function.](../Media/scenario-call-azure-function-ARM/create-function-0.png 'Create Function')

Add the contents of the [run.csx.](../Scenarios/call-azure-function-ARM/run.csx) to our newly created Azure Function and save.

![Get Function URL.](../Media/scenario-call-azure-function-ARM/create-function-1.png 'Get Function URL')

Also, be sure to copy the function URL for testing.  

### Update Deployment Files

Update the ARM template parameters.
We are also including a sample query string parameter so the function can process it.  We can update the [deploy.json.](../Scenarios/call-azure-function-ARM/deploy.json) variable with our Azure Function url.

```json
"variables" :
{
    "TemplateUri": "https://myfunc.azurewebsites.net/api/WebHookFunction?code=mycode==&resourcegroupname=abc"
}
```

### Run the deployment

We can refer to [deploy.ps1](../Scenarios/app-service-vnet-vm/deploy.ps1) for an example on how to run the scripts.

Please ensure that we use [Azure CLI](https://docs.microsoft.com/en-us/cli/azure/install-azure-cli-windows?view=azure-cli-latest) and login to the Azure subscription.

```powershell
az login
az account set -s <subscription id>
```

We can now create a group and then deploy the ARM template to the resource group.

```powershell
#create an rg
az group create -n my-test-rg-123 -l eastus

#deploy the ARM template based on the deploy parameters and deploy template based off local files
az group deployment create --name mydeploy --resource-group my-test-rg-123 --parameters deploy.parameters.json --template-file deploy.json
```

> If we need to deploy under a different context (E.g. a service principal), we can also refer to the documentation for [Using Service Principals with Az Cli](https://docs.microsoft.com/en-us/cli/azure/create-an-azure-service-principal-azure-cli?view=azure-cli-latest) and [Checking Service Principal Role Assignment](https://docs.microsoft.com/en-us/cli/azure/role/assignment?view=azure-cli-latest#az-role-assignment-create).

### Validate The Scenario

We can go back to the portal and then check on the Azure Function to see if it was called.

> We can also refer to the [Azure Functions Monitoring Options](https://docs.microsoft.com/en-us/azure/azure-functions/functions-monitoring#streaming-logs) for other ways to check that the function was called.

![Validate VNET Integration.](../Media/scenario-call-azure-function-ARM/validate-scenario-1.png 'Validate VNET Integration in Portal')

When we are satisified with the test, we can clean up with the following az cli command:

```powershell
az group delete -n my-test-rg-123
```
