# Deploy a app service plan, an empty windows web app, and attach Keyvault for secret storage

## Description
This template allows you to deploy a Keyvault, App Service Plan, Windows App Service app, enable Managed Identity for the App Service, and link the App Service to the Kevault so the app can directly access the secrets.

# Sample Usage

`az group deployment create --resource-group <your-rg-name> --template-file azuredeploy.json --parameters azuredeploy.parameters.json`
