import type { ConfigFile } from '@rtk-query/codegen-openapi'

const config: ConfigFile = {
  schemaFile: '../ApiService/RestaurantAnalytics.Api.json',
  apiFile: './src/store/api/empty-api.ts',
  apiImport: 'emptySplitApi',
  outputFiles: {
    './src/store/api/generated/api.ts': {
      filterEndpoints: [/Restaurant|Analytics|Metric/]
    },
  },
  exportName: 'apiService',
  hooks: true,
}

export default config