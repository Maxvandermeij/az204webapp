
AZ-204: Create Azure App Service Web Apps
=====================================

This guide will help you practice a section of the required skills for certifying Az-204. Specifically the Azure Compute solutions section "Create Azure App Service Web Apps" as found in the skilled measured [document](https://docs.microsoft.com/en-us/learn/certifications/exams/az-204) from Microsoft. For a complete guide for the exam, tryout the full study guide found [here](https://www.thomasmaurer.ch/2020/03/az-204-study-guide-developing-solutions-for-microsoft-azure/) as suggested by @eschipper.

---------------------------
Requirements:
---------------------

Installed [dotnetcore 3.1 SDK](https://dotnet.microsoft.com/download/dotnet-core/3.1)


File editor: [VS Code](https://code.visualstudio.com/)

Extensions: Azure CLI Tools

---------------------
1..Create an Azure App Service Web App
---------------------


Starting out




https://azure.microsoft.com/en-us/pricing/details/app-service/plans/




Next you need the Azure CLI. You can either create a cloud shell on the azure portal or download and install it locally. For local installation get it [here](https://docs.microsoft.com/en-us/cli/azure/install-azure-cli-windows?view=azure-cli-latest&tabs=azure-cli). 



Create the Azure WebApp
```C#
az login 

```





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

