#disable-next-line secure-secrets-in-params   // Doesn't contain a secret
param appSecretsName string
param principalType string
param principalId string

resource app_secrets 'Microsoft.KeyVault/vaults@2023-07-01' existing = {
  name: appSecretsName
}

// resource app_secrets_KeyVaultSecretsUser 'Microsoft.Authorization/roleAssignments@2022-04-01' = {
//   name: guid(app_secrets.id, principalId, subscriptionResourceId('Microsoft.Authorization/roleDefinitions', '4633458b-17de-408a-b874-0445c86b69e6'))
//   properties: {
//     principalId: principalId
//     roleDefinitionId: subscriptionResourceId('Microsoft.Authorization/roleDefinitions', '4633458b-17de-408a-b874-0445c86b69e6')
//     principalType: principalType
//   }
//   scope: app_secrets
// }

resource key_vault_KeyVaultSecretsOfficer 'Microsoft.Authorization/roleAssignments@2022-04-01' = {
  name: guid(app_secrets.id, principalId, subscriptionResourceId('Microsoft.Authorization/roleDefinitions', 'b86a8fe4-44ce-4948-aee5-eccb2c155cd7'))
  properties: {
    principalId: principalId
    roleDefinitionId: subscriptionResourceId('Microsoft.Authorization/roleDefinitions', 'b86a8fe4-44ce-4948-aee5-eccb2c155cd7')
    principalType: principalType
  }
  scope: app_secrets
}
