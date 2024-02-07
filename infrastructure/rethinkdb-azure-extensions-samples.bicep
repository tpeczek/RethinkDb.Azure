targetScope = 'subscription'

param location string = 'westeurope'

@secure()
param rethinkDbUser string
@secure()
param rethinkDbPassword string

var projectResourceGroupName = 'rg-rethinkdb-azure-extensions-samples' 

resource projectResourceGroup 'Microsoft.Resources/resourceGroups@2022-09-01' = {
  name: projectResourceGroupName
  location: location
}

module projectResourceGroupModule 'rethinkdb-azure-extensions-samples-rg.bicep' = {
  name: 'rethinkdb-azure-extensions-samples-rg'
  scope: projectResourceGroup
  params: {
    location: projectResourceGroup.location
    rethinkDbUser: rethinkDbUser
    rethinkDbPassword: rethinkDbPassword
  }
}
