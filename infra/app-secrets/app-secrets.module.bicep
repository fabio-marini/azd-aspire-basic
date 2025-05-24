@description('The location for the resource(s) to be deployed.')
param location string = resourceGroup().location

resource app_secrets 'Microsoft.KeyVault/vaults@2023-07-01' = {
  name: take('appsecrets-${uniqueString(resourceGroup().id)}', 24)
  location: location
  properties: {
    tenantId: tenant().tenantId
    sku: {
      family: 'A'
      name: 'standard'
    }
    enableRbacAuthorization: true
  }
  tags: {
    'aspire-resource-name': 'app-secrets'
  }
}

output vaultUri string = app_secrets.properties.vaultUri

output name string = app_secrets.name