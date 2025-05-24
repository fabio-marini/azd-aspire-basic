@description('The location for the resource(s) to be deployed.')
param location string = resourceGroup().location

resource messages_stg 'Microsoft.Storage/storageAccounts@2024-01-01' = {
  name: take('messagesstg${uniqueString(resourceGroup().id)}', 24)
  kind: 'StorageV2'
  location: location
  sku: {
    name: 'Standard_GRS'
  }
  properties: {
    accessTier: 'Hot'
    allowSharedKeyAccess: false
    minimumTlsVersion: 'TLS1_2'
    networkAcls: {
      defaultAction: 'Allow'
    }
  }
  tags: {
    'aspire-resource-name': 'messages-stg'
  }
}

resource blobs 'Microsoft.Storage/storageAccounts/blobServices@2024-01-01' = {
  name: 'default'
  parent: messages_stg
}

output blobEndpoint string = messages_stg.properties.primaryEndpoints.blob

output queueEndpoint string = messages_stg.properties.primaryEndpoints.queue

output tableEndpoint string = messages_stg.properties.primaryEndpoints.table

output name string = messages_stg.name