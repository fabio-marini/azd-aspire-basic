@description('The location for the resource(s) to be deployed.')
param location string = resourceGroup().location

param sku string = 'Standard'

resource asb_messaging 'Microsoft.ServiceBus/namespaces@2024-01-01' = {
  name: take('asbmessaging-${uniqueString(resourceGroup().id)}', 50)
  location: location
  properties: {
    disableLocalAuth: true
  }
  sku: {
    name: sku
  }
  tags: {
    'aspire-resource-name': 'asb-messaging'
  }
}

output serviceBusEndpoint string = asb_messaging.properties.serviceBusEndpoint

output name string = asb_messaging.name