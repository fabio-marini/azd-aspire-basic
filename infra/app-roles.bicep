@description('Id of the user or app to assign application roles')
param principalId string

@description('The type of principal to assign application roles')
@allowed(['ServicePrincipal', 'User'])
param principalType  string

param resourceGroupName string

param appConfigName string
param appSecretsName string
param messageBusName string
param messageStgName string

module app_config_roles 'app-config-roles/app-config-roles.module.bicep' = {
  name: 'app-config-roles'
  scope: resourceGroup(resourceGroupName)
  params: {
    appConfigName: appConfigName
    principalId: principalId
    principalType: principalType
  }
}

module app_secrets_roles 'app-secrets-roles/app-secrets-roles.module.bicep' = {
  name: 'app-secrets-roles'
  scope: resourceGroup(resourceGroupName)
  params: {
    appSecretsName: appSecretsName
    principalId: principalId
    principalType: principalType
  }
}

module asb_messaging_roles 'asb-messaging-roles/asb-messaging-roles.module.bicep' = {
  name: 'asb-messaging-roles'
  scope: resourceGroup(resourceGroupName)
  params: {
    messageBusName: messageBusName
    principalId: principalId
    principalType: principalType
  }
}

module messages_stg_roles 'messages-stg-roles/messages-stg-roles.module.bicep' = {
  name: 'messages-stg-roles'
  scope: resourceGroup(resourceGroupName)
  params: {
    messageStgName: messageStgName
    principalId: principalId
    principalType: principalType
  }
}
