# This script creates the Azure resources needed for the Azure DevOps pipeline.
Param(
    [parameter(Mandatory = $false)]
    [string]$subscriptionName = "Pagamento in base al consumo",
    [parameter(Mandatory = $false)]
    [string]$resourceGroupName = "rg-genocs",
    [parameter(Mandatory = $false)]
    [string]$resourceGroupLocation = "West Europe",
    [parameter(Mandatory = $false)]
    [string]$appservicePlanName = "asp-genocs",
    [parameter(Mandatory = $false)]
    [string]$appserviceName = "app-genocs"
    )

# Set Azure subscription name
Write-Host "Setting Azure subscription to $subscriptionName" -ForegroundColor Yellow
az account set --subscription=$subscriptionName

# Create Resource Group if it doesn't exist
$rgExists = az group exists --name $resourceGroupName
Write-Host "$resourceGroupName exists: $rgExists"

if ($rgExists -eq $false) {

    # Create Resource Group
    Write-Host "Creating resource group $resourceGroupName in region $resourceGroupLocation" -ForegroundColor Yellow
    az group create `
        --name=$resourceGroupName `
        --location=$resourceGroupLocation `
        --output=jsonc
} 

# Create App Service Plan if it doesn't exist
$aspExists = az appservice plan show --name $appservicePlanName --resource-group $resourceGroupName
Write-Host "$appservicePlanName exists: $aspExists"

if ($aspExists -eq $false) {

    # Create App Service Plan
    Write-Host "Creating App Service Plan $appservicePlanName in resource group $resourceGroupName" -ForegroundColor Yellow
    az appservice plan create `
        --name=$appservicePlanName `
        --resource-group=$resourceGroupName `
        --sku=S1 `
        --is-linux `
        --output=jsonc
}

# Create App Service if it doesn't exist
$asExists = az webapp show --name $appserviceName --resource-group $resourceGroupName

if ($asExists -eq $false) {

    # Create App Service
    Write-Host "Creating App Service $appserviceName in resource group $resourceGroupName" -ForegroundColor Yellow
    az webapp create `
        --name=$appserviceName `
        --resource-group=$resourceGroupName `
        --plan=$appservicePlanName `
        --runtime="DOTNETCORE|7.0" `
        --deployment-local-git `
        --output=jsonc
}