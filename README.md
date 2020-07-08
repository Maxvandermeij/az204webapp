
AZ-204: Create Azure App Service Web Apps
=====================================

This guide will help you practice a section of the required skills for certifying Az-204. Specifically the Azure Compute solutions section "Create Azure App Service Web Apps" as found in the skilled measured [document](https://docs.microsoft.com/en-us/learn/certifications/exams/az-204) from Microsoft. For a complete guide for the exam, tryout the full study guide found [here](https://www.thomasmaurer.ch/2020/03/az-204-study-guide-developing-solutions-for-microsoft-azure/) as suggested by @eschipper.

The Azure App Service Web App section contains five segments which we will discuss and practice with in order: Creating Azure Web App, logging, code deployment, configuration and auto scaling. 

---------------------------
Requirements:
---------------------
There are many ways in which you can create and manage azure resources. Today we will use the Azure portal, any terminal and VS Code. [VS Code](https://code.visualstudio.com/) with the extensions "Azure CLI Tools" to help us autocomplete and give suggestions for our commands. 

Choose your preferred Terminal:
- Az CLI - [[here]](https://docs.microsoft.com/en-us/cli/azure/install-azure-cli-windows?view=azure-cli-latest&tabs=azure-cli)
- Azure Cloud Shell - [[here]](portal.azure.com)
- Az-Powershell (Powershell : Install-Module -Name AzureRM -AllowClobber)

Optional SDK:

Installing the DotnetCore SDK is <b>optional</b>. The code that we will use will be available on [github](https://github.com/Maxvandermeij/az204webapp). But if you want to do every step and create the code yourself go ahead and install the [dotnetcore 3.1 SDK](https://dotnet.microsoft.com/download/dotnet-core/3.1).

---------------------
1..Create an Azure App Service Web App
---------------------

Let's warm up by first creating a webapp from the the portal. Go to App Services and click Add. You will need three Azure resources:

- Resource Group
- Service Plans: https://azure.microsoft.com/en-us/pricing/details/app-service/plans/
- Web App

It is recommended to group resources that belong together in the same resource group. Therefore create the following three new resources. You could use the recommended Microsoft naming convention: 

- Resource Group:                 
  rg-$Name-$Environment-$Version

- Service Plan:         
  plan-$Name-$Environment-$Version

- Web App:          
  app-$Name-$Environment-$Version

Now that we know what we need to create a Web App manually lets remove the resource group and do the same steps from the terminal so that they can be automated. 

------------------------

For the next steps you can choose to use either the az-cli or az-powershell. I will be using Az CLI commands in a powershell terminal. Be free to use whichever tool you want. I highly recommend using intellisense tooling for help with commands and options.  

Autocompletion options:
- local AZ CLI ---- VS Code + extension "Azure CLI Tools"
- local Az Powershell ---- Powershell ISE
- cloudshell ----- although not recommended, you could use "az interactive" 

----------------------------------------------
Scripting time. <br>
First login to Azure and do some basic configuration. I like to set the default output format from json to table.
```
az login                                                            
az configure                                                        
```

Set some basic variables to get you started. I used powershell to run az cli commands, if you prefer bash please change the variable declaration accordingly.

```ps
$Name        = "demo"
$Environment = "dev"
$Version     = "001"

$location = "westeurope"
$rgname   = "rg-$Name-$Environment-$Version"
$planname = "plan-$Name-$Environment-$Version"
$appname  = "app-$Name-$Environment-$Version"
```

Create all three resources in order: rg > plan > webapp. <br>
Use suggestions from your preferred tool to find the required variables for each resource.

```ps
az group create --name $rgname --location $location 
az appservice plan create --name $planname --resource-group $rgname --location $location --sku S1
az webapp create --name $appname --plan $planname --resource-group $rgname
```

Now confirm that your resources are created

```ps
az resource list --resource-group $rgname
```

---------------------
2..Enable diagnostics logging
---------------------
Now that we have created a Web App, lets checkout the basic logging features in the portal. These logging features are recommended for debugging in pre-preproduction. For production there are better options which will be discussed later in AZ-204. 

Go to App services and open the created app service and scroll down to "App Service logs". The type of webapp determines what logging is available. For .Net Core there is: 

- application logging [off / on + level]
- web server logging [ off  / filesystem / storage)]
- log detailed errors [ of / on]
- failed requests tracing [ off / on]

Let's turn them all on so that we can use it later. For now we use filesystem logging, but you can choose to store them on a seperate storage account.

```ps
az webapp log config --application-logging true --level verbose --name $appname --resource-group $rgname

az webapp log config --web-server-logging filesystem --name $appname --resource-group $rgname

az webapp log config --detailed-error-messages true --name $appname --resource-group $rgname

az webapp log config --failed-request-tracing true --name $appname --resource-group $rgname
```

Now check the current settings. Play around by trying different --output (json/yaml/table). You will see some default retention values which you can change if needed.

```ps
az webapp log show -n $appname -g $rgname --output yaml
```

--------------------

Application logging when turned on is only activated for a maximum of 12 hours. For DotnetCore applications there are 5 levels:
```c#
logger.LogCritical("level 5: Critical Message"); // Writes an error message at log level 4 <br>
logger.LogWarning("level 4: Error Message"); // Writes a warning message at log level 3<br>
logger.LogInformation("level 3: Information Message"); // Writes an information message at log level 2<br>
logger.LogDebug("level 2: Debug Message"); // Writes a debug message at log level 1<br>
logger.LogTrace("level 1: Trace message"); // Writes a detailed trace message at log level 0<br>
```

In the portal you can checkout the default Location of the logs when using the Console. D:\

```ps
az webapp log tail
```

---------------------
3..Deploy code to a web app
---------------------

Create a basic dotnet Core webapi

```C#
mkdir demowebapp
cd demowebapp

dotnet new webapp

dotnet new gitignore
```

```ps
$gitRepo=
--deployment-source-url $gitRepo
``` 


---------------------
4..Configure web app settings including SSL, API, and connection strings
---------------------


---------------------
5..Implement autoscaling rules, including scheduled autoscaling, and scaling by operational or system metrics
---------------------



















6..Cleanup

