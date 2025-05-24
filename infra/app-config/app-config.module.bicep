@description('The location for the resource(s) to be deployed.')
param location string = resourceGroup().location

resource app_config 'Microsoft.AppConfiguration/configurationStores@2024-05-01' = {
  name: take('appconfig-${uniqueString(resourceGroup().id)}', 50)
  location: location
  properties: {
    disableLocalAuth: true
  }
  sku: {
    name: 'free'
  }
  tags: {
    'aspire-resource-name': 'app-config'
  }
}

output appConfigEndpoint string = app_config.properties.endpoint

output name string = app_config.name
