targetScope = 'subscription'

@minLength(1)
@maxLength(64)
@description('Name of the environment that can be used as part of naming resource convention, the name of the resource group for your application will use this name, prefixed with rg-')
param environmentName string

@description('True to provision only the resources required for local development, false (the default) to provision all the resources required for the application to run in the cloud)')
param hybridEnvironment bool = false

@minLength(1)
@description('The location used for all deployed resources')
param location string

@description('Id of the user or app to assign application roles')
param principalId string = ''

var tags = {
  'azd-env-name': environmentName
}

// FIXME: asb-messaging (what about rmq-messaging?!?) => message-bus + messages-stg => message-stg

resource rg 'Microsoft.Resources/resourceGroups@2022-09-01' = {
  name: 'rg-${environmentName}'
  location: location
  tags: tags
}

module services 'services.bicep' = {
  scope: rg
  name: 'services'
  params: {
    location: location
    resourceGroupName: rg.name
  }
}

module user_roles 'app-roles.bicep' = if (principalId != '') {
  scope: rg
  name: 'user-roles'
  params: {
    resourceGroupName: rg.name
    principalId: principalId
    appConfigName: services.outputs.APP_CONFIG_APPCONFIGNAME
    appSecretsName: services.outputs.APP_SECRETS_VAULTNAME
    messageBusName: services.outputs.MESSAGE_BUS_SERVICEBUSNAME
    messageStgName: services.outputs.MESSAGE_STG_TABLENAME
    principalType: 'User'
  }
}

module resources 'resources.bicep' = if (!hybridEnvironment) {
  scope: rg
  name: 'resources'
  params: {
    location: location
    tags: tags
    principalId: principalId
  }
}

module mi_roles 'app-roles.bicep' = if (!hybridEnvironment) {
  scope: rg
  name: 'mi-roles'
  params: {
    resourceGroupName: rg.name
    principalId: resources.outputs.MANAGED_IDENTITY_PRINCIPAL_ID
    appConfigName: services.outputs.APP_CONFIG_APPCONFIGNAME
    appSecretsName: services.outputs.APP_SECRETS_VAULTNAME
    messageBusName: services.outputs.MESSAGE_BUS_SERVICEBUSNAME
    messageStgName: services.outputs.MESSAGE_STG_TABLENAME
    principalType: 'ServicePrincipal'
  }
}

output MANAGED_IDENTITY_CLIENT_ID string = !hybridEnvironment ? resources.outputs.MANAGED_IDENTITY_CLIENT_ID : ''
output MANAGED_IDENTITY_NAME string = !hybridEnvironment ? resources.outputs.MANAGED_IDENTITY_NAME : ''
output AZURE_LOG_ANALYTICS_WORKSPACE_NAME string = !hybridEnvironment ? resources.outputs.AZURE_LOG_ANALYTICS_WORKSPACE_NAME : ''
output AZURE_CONTAINER_REGISTRY_ENDPOINT string = !hybridEnvironment ? resources.outputs.AZURE_CONTAINER_REGISTRY_ENDPOINT : ''
output AZURE_CONTAINER_REGISTRY_MANAGED_IDENTITY_ID string = !hybridEnvironment ? resources.outputs.AZURE_CONTAINER_REGISTRY_MANAGED_IDENTITY_ID : ''
output AZURE_CONTAINER_REGISTRY_NAME string = !hybridEnvironment ? resources.outputs.AZURE_CONTAINER_REGISTRY_NAME : ''
output AZURE_CONTAINER_APPS_ENVIRONMENT_NAME string = !hybridEnvironment ? resources.outputs.AZURE_CONTAINER_APPS_ENVIRONMENT_NAME : ''
output AZURE_CONTAINER_APPS_ENVIRONMENT_ID string = !hybridEnvironment ? resources.outputs.AZURE_CONTAINER_APPS_ENVIRONMENT_ID : ''
output AZURE_CONTAINER_APPS_ENVIRONMENT_DEFAULT_DOMAIN string = !hybridEnvironment ? resources.outputs.AZURE_CONTAINER_APPS_ENVIRONMENT_DEFAULT_DOMAIN : ''

output APP_CONFIG_APPCONFIGENDPOINT string = app_config.outputs.appConfigEndpoint
output APP_SECRETS_VAULTURI string = app_secrets.outputs.vaultUri
output ASB_MESSAGING_SERVICEBUSENDPOINT string = asb_messaging.outputs.serviceBusEndpoint
output MESSAGES_STG_BLOBENDPOINT string = messages_stg.outputs.blobEndpoint
output MESSAGES_STG_TABLEENDPOINT string = messages_stg.outputs.tableEndpoint
