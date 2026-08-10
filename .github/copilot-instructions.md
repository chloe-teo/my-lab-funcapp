## Azure Functions Deployment

- Treat `src/mylabfuncapp.csproj` as the deployment project. Build and publish that project explicitly rather than using broad or stale project globs.
- Create the deployment package from the clean `dotnet publish` output. Do not package `src`, `bin`, `obj`, Azurite storage files, or source files.
- The zip must have `host.json` directly at its root, with `functions.metadata`, `worker.config.json`, the application assembly, dependencies, and `.azurefunctions/` alongside it. Use `includeRootFolder: false` when archiving.
- Keep `local.settings.json` out of production deployment packages. Production values belong in Function App application settings, Key Vault, or managed identity configuration.
- Use `ArchiveFiles@2` to create the zip and make the name explicit with a stable app variable plus `$(Build.BuildId)`, for example `$(appName)-$(Build.BuildId).zip`.
- For Flex Consumption deployment with `AzureFunctionApp@2`, set `appType: functionAppLinux`, `isFlexConsumption: true`, the existing Function App name, and the exact zip path. Do not force `zipDeploy` or `runFromPackage` when Flex-specific deployment handling is enabled.
- The Azure Resource Manager service connection identifies the subscription and authenticates deployment. The Function App name identifies the target app; resource group configuration is needed by Azure CLI access-restriction commands.
- Do not upload deployment packages directly to the Function App storage account as a substitute for a deployment task. Use the supported deployment task or CLI deployment flow.

## Network Restrictions During Deployment

- If the Function App or SCM site is restricted, a Microsoft-hosted agent may be blocked because its outbound IP is dynamic. Authentication through the service connection does not bypass access restrictions.
- When a temporary deployment exception is required, make it the first deployment-related task and set both the main site and SCM default actions to `Allow` with `az functionapp config access-restriction set`.
- Always restore the intended restricted state in a final Azure CLI task using `condition: always()`, including when the build or deployment fails. Set both `--default-action Deny` and `--scm-default-action Deny` so the app returns to selected-network/IP mode.
- Preserve existing allow rules; changing the default action should not delete them. Keep the resource group, app name, and service connection as pipeline variables rather than hard-coding them in multiple tasks.
- Prefer a self-hosted or managed agent with a stable IP, or an agent with VNet access, over allowing an entire Microsoft-hosted-agent geography. Review main-site and SCM-site restrictions separately.
