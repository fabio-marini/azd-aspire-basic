targetScope = 'subscription'

@minLength(1)
@maxLength(64)
@description('Name of the environment that can be used as part of naming resource convention, the name of the resource group for your application will use this name, prefixed with rg-')
param environmentName string

@minLength(1)
@description('The location used for all deployed resources')
param location string

@description('Id of the user or app to assign application roles')
param principalId string = ''

@metadata({azd: {
  type: 'generate'
  config: {length:22,noSpecial:true}
  }
})
@secure()
param rmq_messaging_password string

var tags = {
  'azd-env-name': environmentName
}

resource rg 'Microsoft.Resources/resourceGroups@2022-09-01' = {
  name: 'rg-${environmentName}'
  location: location
  tags: tags
}
module resources 'resources.bicep' = {
  scope: rg
  name: 'resources'
  params: {
    location: location
    tags: tags
    principalId: principalId
  }
}

module app_config 'app-config/app-config.module.bicep' = {
  name: 'app-config'
  scope: rg
  params: {
    location: location
  }
}
module app_config_roles 'app-config-roles/app-config-roles.module.bicep' = {
  name: 'app-config-roles'
  scope: rg
  params: {
    app_config_outputs_name: app_config.outputs.name
    location: location
    principalId: resources.outputs.MANAGED_IDENTITY_PRINCIPAL_ID
    principalType: 'ServicePrincipal'
  }
}
module app_secrets 'app-secrets/app-secrets.module.bicep' = {
  name: 'app-secrets'
  scope: rg
  params: {
    location: location
  }
}
module app_secrets_roles 'app-secrets-roles/app-secrets-roles.module.bicep' = {
  name: 'app-secrets-roles'
  scope: rg
  params: {
    app_secrets_outputs_name: app_secrets.outputs.name
    location: location
    principalId: resources.outputs.MANAGED_IDENTITY_PRINCIPAL_ID
    principalType: 'ServicePrincipal'
  }
}
module asb_messaging 'asb-messaging/asb-messaging.module.bicep' = {
  name: 'asb-messaging'
  scope: rg
  params: {
    location: location
  }
}
module asb_messaging_roles 'asb-messaging-roles/asb-messaging-roles.module.bicep' = {
  name: 'asb-messaging-roles'
  scope: rg
  params: {
    asb_messaging_outputs_name: asb_messaging.outputs.name
    location: location
    principalId: resources.outputs.MANAGED_IDENTITY_PRINCIPAL_ID
    principalType: 'ServicePrincipal'
  }
}
module messages_stg 'messages-stg/messages-stg.module.bicep' = {
  name: 'messages-stg'
  scope: rg
  params: {
    location: location
  }
}
module messages_stg_roles 'messages-stg-roles/messages-stg-roles.module.bicep' = {
  name: 'messages-stg-roles'
  scope: rg
  params: {
    location: location
    messages_stg_outputs_name: messages_stg.outputs.name
    principalId: resources.outputs.MANAGED_IDENTITY_PRINCIPAL_ID
    principalType: 'ServicePrincipal'
  }
}

output MANAGED_IDENTITY_CLIENT_ID string = resources.outputs.MANAGED_IDENTITY_CLIENT_ID
output MANAGED_IDENTITY_NAME string = resources.outputs.MANAGED_IDENTITY_NAME
output AZURE_LOG_ANALYTICS_WORKSPACE_NAME string = resources.outputs.AZURE_LOG_ANALYTICS_WORKSPACE_NAME
output AZURE_CONTAINER_REGISTRY_ENDPOINT string = resources.outputs.AZURE_CONTAINER_REGISTRY_ENDPOINT
output AZURE_CONTAINER_REGISTRY_MANAGED_IDENTITY_ID string = resources.outputs.AZURE_CONTAINER_REGISTRY_MANAGED_IDENTITY_ID
output AZURE_CONTAINER_REGISTRY_NAME string = resources.outputs.AZURE_CONTAINER_REGISTRY_NAME
output AZURE_CONTAINER_APPS_ENVIRONMENT_NAME string = resources.outputs.AZURE_CONTAINER_APPS_ENVIRONMENT_NAME
output AZURE_CONTAINER_APPS_ENVIRONMENT_ID string = resources.outputs.AZURE_CONTAINER_APPS_ENVIRONMENT_ID
output AZURE_CONTAINER_APPS_ENVIRONMENT_DEFAULT_DOMAIN string = resources.outputs.AZURE_CONTAINER_APPS_ENVIRONMENT_DEFAULT_DOMAIN
output APP_CONFIG_APPCONFIGENDPOINT string = app_config.outputs.appConfigEndpoint
output APP_SECRETS_VAULTURI string = app_secrets.outputs.vaultUri
output ASB_MESSAGING_SERVICEBUSENDPOINT string = asb_messaging.outputs.serviceBusEndpoint
output MESSAGES_STG_BLOBENDPOINT string = messages_stg.outputs.blobEndpoint
output MESSAGES_STG_TABLEENDPOINT string = messages_stg.outputs.tableEndpoint
