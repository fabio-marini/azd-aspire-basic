@minLength(1)
@description('The location used for all deployed resources')
param location string

@description('The name of the resource group to deploy to')
param resourceGroupName string

module app_config 'app-config/app-config.module.bicep' = {
  name: 'app-config'
  scope: resourceGroup(resourceGroupName)
  params: {
    location: location
  }
}

module app_secrets 'app-secrets/app-secrets.module.bicep' = {
  name: 'app-secrets'
  scope: resourceGroup(resourceGroupName)
  params: {
    location: location
  }
}

module asb_messaging 'asb-messaging/asb-messaging.module.bicep' = {
  name: 'asb-messaging'
  scope: resourceGroup(resourceGroupName)
  params: {
    location: location
  }
}

output APP_CONFIG_APPCONFIGENDPOINT string = app_config.outputs.appConfigEndpoint
output APP_SECRETS_VAULTURI string = app_secrets.outputs.vaultUri
output MESSAGE_BUS_SERVICEBUSENDPOINT string = asb_messaging.outputs.serviceBusEndpoint

output APP_CONFIG_APPCONFIGNAME string = app_config.outputs.name
output APP_SECRETS_VAULTNAME string = app_secrets.outputs.name
output MESSAGE_BUS_SERVICEBUSNAME string = asb_messaging.outputs.name
