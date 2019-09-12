## App Service VNET with a VM

The goal is to deploy a Virtual Network which will have an Azure App Service with VNET injection in a web subnet, and a VM in a separate subnet.

We can verify that the App Service can reach the VM with private ip address to validate the scenario.

![App Service VNET VM.](../Media/scenario-app-service-vnet-vm/scenario-0.png 'App Service VNET VM')

We will want to deploy with ARM such that we can meet the above scenario for validation.

![App Service VNET VM.](../Media/scenario-app-service-vnet-vm/scenario.png 'App Service VNET VM')

### Links

These will describe some of the concepts that we're using in this scenario.

1. [Azure App Service VNET Integration](https://docs.microsoft.com/en-us/azure/app-service/web-sites-integrate-with-vnet)
1. [Networking tools for Azure App Service](https://docs.microsoft.com/en-us/azure/app-service/web-sites-integrate-with-vnet#tools)
1. [Debugging Access to VM Resources documentation](https://docs.microsoft.com/en-us/azure/app-service/web-sites-integrate-with-vnet#debugging-access-to-vnet-hosted-resources)
1. [Azure Virtual Network](https://docs.microsoft.com/en-us/azure/virtual-network/virtual-network-for-azure-services)
1. [Azure CLI ARM Deployments](https://docs.microsoft.com/en-us/azure/azure-resource-manager/resource-group-template-deploy-cli)
1. [ARM Quick Starts](https://github.com/Azure/azure-quickstart-templates)
1. [ARM Script Extensions](https://docs.microsoft.com/en-us/azure/virtual-machines/extensions/custom-script-windows)
1. [Azure Storage Accounts](https://docs.microsoft.com/en-us/azure/storage/common/storage-account-overview?toc=%2fazure%2fstorage%2fblobs%2ftoc.json)
1. [Azure Storage Endpoints](https://docs.microsoft.com/en-us/azure/storage/common/storage-network-security?toc=%2fazure%2fvirtual-network%2ftoc.json#grant-access-from-a-virtual-network)
1. [Create a Blob Storage Container with .NET](https://docs.microsoft.com/en-us/azure/storage/blobs/storage-blob-container-create)
1. [Azure Blob Storage with AAD](https://docs.microsoft.com/en-us/azure/storage/common/storage-auth-aad?toc=%2fazure%2fstorage%2fblobs%2ftoc.json)
1. [Azure Blob Storage with SAS](https://docs.microsoft.com/en-us/azure/storage/common/storage-sas-overview)
1. [Install Azure CLI](https://docs.microsoft.com/en-us/cli/azure/install-azure-cli-windows?view=azure-cli-latest)
1. [Using Service Principals with Az Cli](https://docs.microsoft.com/en-us/cli/azure/create-an-azure-service-principal-azure-cli?view=azure-cli-latest)
1. [Check Service Principal Role Assignment](https://docs.microsoft.com/en-us/cli/azure/role/assignment?view=azure-cli-latest#az-role-assignment-create)
1. [Accessing Kudu Service](https://github.com/projectkudu/kudu/wiki/Accessing-the-kudu-service)

### Setup VM Script

This set up is a one-off (but can be updated based on storage or the script itself needing to be run.)

In this scenario, we can upload our VM-update script into Blob storage and then have the ARM template access it through SAS token.

> While SAS tokens will help with convenience, using AAD RBAC or User Delegation SAS would be a choice that would help with production scenarios.  Please refer to [Azure Blob Storage with AAD](https://docs.microsoft.com/en-us/azure/storage/common/storage-auth-aad?toc=%2fazure%2fstorage%2fblobs%2ftoc.json) and  [Azure Blob Storage with SAS](https://docs.microsoft.com/en-us/azure/storage/common/storage-sas-overview) for more details.

We can create an [Azure Storage Account](https://docs.microsoft.com/en-us/azure/storage/common/storage-account-overview?toc=%2fazure%2fstorage%2fblobs%2ftoc.json).  For simplicity, we'll use the portal, but there's also programmatic ways of [creating a container](https://docs.microsoft.com/en-us/azure/storage/blobs/storage-blob-container-create) too.

In portal.azure.com, assuming we have already signed into our Azure subscription, we can then create a new storage account.  Click on the Create button to get started.

![Create Storage Account in Portal.](../Media/scenario-app-service-vnet-vm/create-storage-1.png 'Create Storage Account in Portal')

Add Storage account basic settings.

![Create Storage Account in Portal.](../Media/scenario-app-service-vnet-vm/create-storage-2.png 'Create Storage Account in Portal')

For now, we'll use the defaults and skip ahead to review and create.

> Beyond this prototype scenario, we should consider [service endpoints](https://docs.microsoft.com/en-us/azure/storage/common/storage-network-security?toc=%2fazure%2fvirtual-network%2ftoc.json#grant-access-from-a-virtual-network) for Azure Storage and the storage account that we create.

Once the Storage Account is provisioned, we can then navigate to our blob.

![Add Blob Container.](../Media/scenario-app-service-vnet-vm/create-storage-3.png 'Add Blob Container in Portal')

We can then add a new container in the portal.

![Add Blob Container.](../Media/scenario-app-service-vnet-vm/create-storage-4.png 'Add Blob Container in Portal')

We can use the defaults when we add the container information.

![Add Blob Container.](../Media/scenario-app-service-vnet-vm/create-storage-5.png 'Add Blob Container in Portal')

Within the newly created container, we can now upload our script.

![Upload to Blob Container.](../Media/scenario-app-service-vnet-vm/create-storage-6.png 'Upload to Blob Container in Portal')

Select the vm update script for upload.

![Upload to Blob Container.](../Media/scenario-app-service-vnet-vm/create-storage-7.png 'Upload to Blob Container in Portal')

Generate a SAS for the uploaded blob.

![Upload to Blob Container.](../Media/scenario-app-service-vnet-vm/create-storage-8.png 'Upload to Blob Container in Portal')

We need to then configure the SAS settings before generating the token and URL.  We should **copy** the SAS URL to use later for the ARM deployment parameters file.

![Configure SAS.](../Media/scenario-app-service-vnet-vm/create-storage-9.png 'Configure SAS in Portal')

> Remember that the Blob SAS here should be refreshed as needed, and that the more appropriate approach would be to use Azure AD.

### Update Deployment Files

Make sure that we update the [deploy.json](../Scenarios/app-service-vnet-vm/deploy.json) and [deploy.parameters.json](../Scenarios/app-service-vnet-vm/deploy.parameters.json) with the appropriate settings.

For instance, we'll want to add in the blob URL for the vm update script.

We will also want to fill in the app service, VM, and network settings.

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

We can go back to the portal and then check on the VNET connectivity.

![Validate VNET Integration.](../Media/scenario-app-service-vnet-vm/validate-vnet-1.png 'Validate VNET Integration in Portal')

Navigate to the deploy web app (Azure App Service).  We'll also want to open up a new tab to [Access the Kudu Service](https://github.com/projectkudu/kudu/wiki/Accessing-the-kudu-service); we can use https://mywebapp.scm.azurewebsites.net/ to login.

We'll want to then point to the powershell console in Kudu.

Assuming that the NSG and the VM network settings are updated, we can then use the Azure App Service to ping the VM on its private ip.

![Validate VNET Integration.](../Media/scenario-app-service-vnet-vm/validate-vnet-2.png 'Validate VNET Integration in Portal')

When we are satisified with the test, we can clean up with the following az cli command:

```powershell
az group delete -n my-test-rg-123
```

#### Network Connectivity Considerations

These are helpful debugging tools for working through the network connectivity in the scenario manually.

We'll want to ensure that we're using the [available networking tools for Azure App Service](https://docs.microsoft.com/en-us/azure/app-service/web-sites-integrate-with-vnet#tools)

> According to the docs, the tools ping, nslookup and tracert won't work through the console due to security constraints.  So tcpping and nameresolver should be available (at the time of this writing).

We can also refer to the [Debugging Access to VM Resources documentation](https://docs.microsoft.com/en-us/azure/app-service/web-sites-integrate-with-vnet#debugging-access-to-vnet-hosted-resources) as well.

We want to make sure that the VM that's in the same VNET as the Azure App Service is discoverable.

If we remote into the VM, we can also double check the VM discoverability on the network.

![Validate VM Network.](../Media/scenario-app-service-vnet-vm/validate-vm-1.png 'Validate VM Network').

We can then check the network settings and validate that we're in the profile that will allow for network discovery.

![Validate VM Network.](../Media/scenario-app-service-vnet-vm/validate-vm-2.png 'Validate VM Network').

If our script has run correctly, we can also validate the firewall settings includes our newly added firewall rule.

![Validate VM Firewall Rule.](../Media/scenario-app-service-vnet-vm/validate-vm-3.png 'Validate VM Firewall Rule').

We can also validate the NSG rules in the portal to make sure that we allow the app service endpoint to use port 1433.

![Validate NSG Rule.](../Media/scenario-app-service-vnet-vm/validate-nsg-1.png 'Validate NSG Rule').