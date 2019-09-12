$rgName = 'my-test-rg-2'
$rgLocation = 'eastus'

#az login
#az account set -s <subscription id>

#create an rg
az group create -n $rgName -l $rgLocation

#deploy the ARM template based on the deploy parameters and deploy template based off local files
az group deployment create --name mydeploy --resource-group $rgName --parameters deploy.parameters.json --template-file deploy.json