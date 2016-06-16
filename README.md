# Wibci.Azure.AppService.Auth.AspIdentity
Sample code for authenticating Azure App Service via Asp Identity

... work in progress - for now just a code dump.

includes:
 - Asp.Identity using EntityFramework
 - sending email with registration confirmations
 - refresh token implementation
 - external social auth provider + custom authentication
 - include roles in tokens

TODO:
 - add client projects (Xamarin / web[later])
 - request validation
 - get basic custom auth working with refresh tokens
 - add social auth
 - add role assignments
 - 
 
External references:
 - AspIdentity: http://bitoftech.net/2015/01/21/asp-net-identity-2-with-asp-net-web-api-2-accounts-management/
 - Azure App Service Identity: http://www.pa-roy.com/azure-app-services-custom-auth-part-3/
 - Refresh tokens: http://bitoftech.net/2014/07/16/enable-oauth-refresh-tokens-angularjs-app-using-asp-net-web-api-2-owin/&safe=active&as_qdr=all
