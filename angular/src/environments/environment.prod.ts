import { Environment } from '@abp/ng.core';

const baseUrl = 'http://localhost:4200';

const oAuthConfig = {
  issuer: 'https://localhost:44382/',
  redirectUri: baseUrl,
  clientId: 'Assessment_App',
  responseType: 'code',
  scope: 'offline_access Assessment',
  requireHttps: true,
};

export const environment = {
  production: true,
  application: {
    baseUrl,
    name: 'Assessment',
  },
  oAuthConfig,
  apis: {
    default: {
      url: 'https://localhost:44382',
      rootNamespace: 'Apptech.Assessment',
    },
    AbpAccountPublic: {
      url: oAuthConfig.issuer,
      rootNamespace: 'AbpAccountPublic',
    },
  },
  remoteEnv: {
    url: '/getEnvConfig',
    mergeStrategy: 'deepmerge'
  }
} as Environment;
