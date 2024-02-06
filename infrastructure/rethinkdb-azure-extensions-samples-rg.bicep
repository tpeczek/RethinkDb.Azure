targetScope = 'resourceGroup'

param location string = resourceGroup().location

var projectContainerInstanceName = 'ci-rethinkdb'
var projectContainerDnsNameLabel = 'rethinkdb-${uniqueString(resourceGroup().id)}'

var rethinkDbImageName = 'rethinkdb'
var rethinkDbImageTag = '2.4.2'

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
          command: ['rethinkdb', '--bind-driver', 'all', '--no-http-admin']
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
