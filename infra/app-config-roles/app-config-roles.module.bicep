@description('The location for the resource(s) to be deployed.')
param location string = resourceGroup().location

param app_config_outputs_name string

param principalType string

param principalId string

resource app_config 'Microsoft.AppConfiguration/configurationStores@2024-05-01' existing = {
  name: app_config_outputs_name
}

resource app_config_AppConfigurationDataOwner 'Microsoft.Authorization/roleAssignments@2022-04-01' = {
  name: guid(app_config.id, principalId, subscriptionResourceId('Microsoft.Authorization/roleDefinitions', '5ae67dd6-50cb-40e7-96ff-dc2bfa4b606b'))
  properties: {
    principalId: principalId
    roleDefinitionId: subscriptionResourceId('Microsoft.Authorization/roleDefinitions', '5ae67dd6-50cb-40e7-96ff-dc2bfa4b606b')
    principalType: principalType
  }
  scope: app_config
}