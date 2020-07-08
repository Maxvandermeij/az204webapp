
AZ-204: Create Azure App Service Web Apps
=====================================

This guide will help you practice a section of the required skills for certifying Az-204. Specifically the Azure Compute solutions section "Create Azure App Service Web Apps" as found in the skilled measured [document](https://docs.microsoft.com/en-us/learn/certifications/exams/az-204) from Microsoft. For a complete guide for the exam, tryout the full study guide found [here](https://www.thomasmaurer.ch/2020/03/az-204-study-guide-developing-solutions-for-microsoft-azure/) as suggested by @eschipper.

The Azure App Service Web App section contains five segments which we will discuss and practice with in order: Creating Azure Web App, logging, code deployment, configuration and auto scaling. 

---------------------------
Requirements:
---------------------
There are many ways in which you can create and manage azure resources. Today we will use the Azure portal, any terminal and VS Code. [VS Code](https://code.visualstudio.com/) with the extensions "Azure CLI Tools" to help us autocomplete and give suggestions for our commands. 

Choose your preferred Terminal:
- Az CLI - [[x]](https://docs.microsoft.com/en-us/cli/azure/install-azure-cli-windows?view=azure-cli-latest&tabs=azure-cli)
- Azure Cloud Shell - [[x]](portal.azure.com)
- Az-Powershell ([x] Install-Module -Name AzureRM -AllowClobber)

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


Now that we know what we need to create a Web App manually lets remove the resource group and do the same steps from the terminal so that they can be automated. You can choose to use either the az-cli or az-powershell. This file will not contain the full commands ready to copy paste because I would like to encourage getting dirty in the terminal (and thus practice). Therefore I highly recommend to use tooling for suggestions and autocompletion of commands and options.  

Autocompletion/suggestions tools:
- local AZ CLI ---- VS Code + extension "Azure CLI Tools"
- local Az Powershell ---- Powershell ISE 


In case you have trouble finding the right command, a cheatsheet is [available](.\powershell.azcli)

First login to Azure and do some basic configuration
```
az login                                                            
az configure                                                        
```

Set some basic variables to get you started
```
$Name        = "demo"
$Environment = "dev"
$Version     = "001"

$location = "westeurope"
$rgname   = "rg-$Name-$Environment-$Version"
$planname = "plan-$Name-$Environment-$Version"
$appname  =  "app-$Name-$Environment-$Version"
```



Create all three resources in order: rg > plan > webapp
Use suggestions from your preferred tool to find the required variables for each resource
```
az group create ** ** ** ** 
az appservice create ** ** ** ** ** ** 
az webapp create ** ** ** ** 
```
Now confirm that your resources are created
```
az resource list --resource-group $rgname
```


Create the Azure WebApp
Open a terminal window in your editor or preferred shell

Create a basic dotnet Core webapi

```C#
mkdir demowebapp
cd demowebapp

dotnet new webapp

dotnet new gitignore
```


---------------------
2..Enable diagnostics logging
---------------------

- application logging [off / on + level]
- web server logging [ off  / filesystem / storage)]
- log detailed errors [ on / off]
- failed requests tracing 
- deployment logging

Application loggin levels:

logger.LogCritical("level 5: Critical Message"); // Writes an error message at log level 4
logger.LogWarning("level 4: Error Message"); // Writes a warning message at log level 3
logger.LogInformation("level 3: Information Message"); // Writes an information message at log level 2
logger.LogDebug("level 2: Debug Message"); // Writes a debug message at log level 1
logger.LogTrace("level 1: Trace message"); // Writes a detailed trace message at log level 0


---------------------
3..Deploy code to a web app
---------------------

---------------------
4..Configure web app settings including SSL, API, and connection strings
---------------------


---------------------
5..Implement autoscaling rules, including scheduled autoscaling, and scaling by operational or system metrics
---------------------



















6..Cleanup

