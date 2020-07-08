
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
Now that we have created a Web App, lets checkout the basic logging features in the portal. These logging features are recommended for debugging in pre-preproduction. For production there are better options (e.g: Application Insights) which will be discussed later in AZ-204. 

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

Application logging when turned on is by default turned of after 12 hours. It is not recommended to be used in production due to the increasing consumption of storage and a possible decrease in performance of the app. 

For DotnetCore applications there are 6 application log levels. The idea is that you define the lowest Log level you want to show in the configuration. The higher levels will then also be shown.
```c#
5. LogCritical
4. LogError
3. LogWarning
2. LogInformation
1. LogDebug
0. LogTrace
```
----------------------------------
In order to see where these are shown lets create a basic webapp, configure logging and afterwards see where the logs are shown and downloadable. If you did not install the DotNetCore SDK, please wait a few min and use the code from the github.

Create a basic dotnet Core webapi
```C#
mkdir demowebapp
cd demowebapp

dotnet new webapp

dotnet new gitignore
```

Configure logging in the Program.CS by copy pasting below code over the old IHostBuilder.

```c#
public static IHostBuilder CreateHostBuilder(string[] args) =>
    Host.CreateDefaultBuilder(args)
        .ConfigureLogging(logging =>
        {
            logging.ClearProviders();
            logging.AddConsole();
        })
        .ConfigureWebHostDefaults(webBuilder =>
        {
            webBuilder.UseStartup<Startup>();
        });
```

Next add the logmessages in the onget of Pages\index.cshtml.cs so that we get the error messages when we open the index page.
```c#
public void OnGet()
        {
            _logger.LogCritical("level 5: Critical Message"); // Writes an error message at log level 5
            _logger.LogError("level 4: Error Message"); // Writes an error message at log level 4
            _logger.LogWarning("level 3: Warning Message"); // Writes a warning message at log level 3
            _logger.LogInformation("level 2: Information Message"); // Writes an information message at log level 2
            _logger.LogDebug("level 1: Debug Message"); // Writes a debug message at log level 1
            _logger.LogTrace("level 0: Trace message"); // Writes a detailed trace message at log level 0
        }
```

When we publish and run the application locally, we can already see these errors in our windows "Event Viewer". By default the messages are filtered based on the level Warning (3) as defined in the appsettings.json. Thus we should see 3, 4 and 5 in our application logs. Once we have deployed our code in the next step we will get back to the logs in the portal.

---------------------
3..Deploy code to a web app
---------------------
There are many ways to get your code into the webapp. We will tryout two ways: deploy by just giving the github url or sending the the build artifact with the az cli.

Just give the url and let azure handle the rest
```ps
$gitRepo="https://github.com/Maxvandermeij/az204webapp"
az webapp deployment source config --name $appname -g $rgname --repo-url $gitRepo --branch master --manual-integration
``` 

Or create a zip of the artifact and send over the zip file.
```

```
 



Code is deployed to the following folder: /home/site/wwwroot



Getting back to logging:

In the portal you can checkout the default Location of the logs when using the Console. D:\

Or actively follow any error messages coming in 
```ps
az webapp log tail -n $appname -g $rgname
```
---------------------
4..Configure web app settings including SSL, API, and connection strings
---------------------


---------------------
5..Implement autoscaling rules, including scheduled autoscaling, and scaling by operational or system metrics
---------------------



















6..Cleanup

