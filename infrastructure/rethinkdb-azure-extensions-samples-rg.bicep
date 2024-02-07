targetScope = 'resourceGroup'

param location string = resourceGroup().location

@secure()
param rethinkDbUser string
@secure()
param rethinkDbPassword string

var projectKeyVaultName = 'kv-${uniqueString(resourceGroup().id)}'
var projectContainerInstanceName = 'ci-rethinkdb'
var projectContainerDnsNameLabel = 'rethinkdb-${uniqueString(resourceGroup().id)}'

var rethinkDbImageName = 'rethinkdb'
var rethinkDbImageTag = '2.4.2'

resource projectKeyVault 'Microsoft.KeyVault/vaults@2022-07-01' = {
  name: projectKeyVaultName
  location: location
  properties: {
    createMode: 'default'
    enabledForDeployment: false
    enabledForDiskEncryption: false
    enabledForTemplateDeployment: false
    enablePurgeProtection: null
    enableRbacAuthorization: true
    enableSoftDelete: false
    sku: {
      name: 'standard'
      family: 'A'
    }
    tenantId: subscription().tenantId
  }

  resource rethinkDbUserSecret 'secrets' = {
    name: 'rethinkdb-user'
    properties: {
      value: rethinkDbUser
    }
  }

  resource rethinkDbPasswordSecret 'secrets' = {
    name: 'rethinkdb-password'
    properties: {
      value: rethinkDbPassword
    }
  }
}

resource projectContainerInstance 'Microsoft.ContainerInstance/containerGroups@2023-05-01' = {
  name: projectContainerInstanceName
  location: location
  properties: {
    sku: 'Standard'
    osType: 'Linux'
    ipAddress: {
      type: 'Public'
      ports: [
        { 
          port: 28015
          protocol: 'TCP'
        }
      ]
      dnsNameLabel: projectContainerDnsNameLabel
    }
    containers: [
      {
        name: rethinkDbImageName
        properties: {
          image: '${rethinkDbImageName}:${rethinkDbImageTag}'
          // This is not perfect as password is visible in ACI propoerties, but good enough for demoware
          command: ['rethinkdb', '--bind-driver', 'all', '--initial-password', rethinkDbPassword, '--no-http-admin']
          ports: [
            // Client driver
            { 
              port: 28015
              protocol: 'TCP'
            }
          ]
          resources: {
            requests: {
              cpu: 1
              memoryInGB: 1
            }
          }
        }
      }
    ]
  }
}
