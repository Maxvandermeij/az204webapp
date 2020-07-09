
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
- cloudshell ----- although I do not recommend it, you could use "az interactive" 

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
az appservice plan create --name $planname --resource-group $rgname --location $location --sku B1
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
Add-Content .gitignore "*.zip"

dotnet add package Microsoft.Extensions.Logging.AzureAppServices --version 3.1.5
```

Configure logging in the Program.cs by copy pasting below code over the old IHostBuilder and adding the reference to the package on top of the file.

```c#
using Microsoft.Extensions.Logging.AzureAppServices;


public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureLogging(logging =>
                {
                    logging.ClearProviders();
                    logging.AddConsole();
                    logging.AddAzureWebAppDiagnostics();
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
There are many ways to get your code into the webapp. We will tryout two ways: deploy by just giving the github url or sending the the build artifact ZIP with the az cli.

Just give the giturl and let azure handle the build + publish + release. Downside, might take longer than building it yourself.
```ps
$gitRepo="https://github.com/Maxvandermeij/az204webapp"
az webapp deployment source config --name $appname -g $rgname --repo-url $gitRepo --branch master --manual-integration
``` 

Or create a zip of the artifact and send over the zip file. This would be the basic steps of a CI/CD Deployment using Azure DevOps.
```ps
dotnet publish

Get-ChildItem -Path .\bin\Debug\netcoreapp3.1\publish\* | Compress-Archive -DestinationPath ThisIsMyArchive.zip -Force

az webapp deployment source config-zip -g $rgname -n $appname --src ThisIsMyArchive.zip
```
----------------------------------------
Code/package will be deployed to the following folder by default: /home/site/wwwroot <br>
Go to SCM or Console to view the folder. 

Since we are in the console anyway lets directly checkout the logfiles we created:
To find the messages we inserted on the index page, view the file using SCM or Console in D:\home\LogFiles\application\

Or actively follow any error messages coming in and reload the page to see the messages.
```ps
az webapp log tail -n $appname -g $rgname
```
---------------------
4..Configure web app settings including SSL, API, and connection strings
---------------------

Configuring a webapps that are deployed to a Azure Web App can be done in many ways. You can either configure settings directly in your code (appsettings.json / web.config), define some stuff in the portal (azcli or clickops), or use a combination of both.

In portal you can find the possible settings under appservices > our app > configuration > appsettings

Here in the appsettings you can define variables which are stored as secrets. Once you deploy your code, Azure will do its magic and replace the existing development variable value (which it looks for in either the web.config or appsettings.json) with the production value stored in the appsettings of the webapp. This way you don't need to replace configs during deployment to specify production values and secrets are limited to the location where it is needed.

Also the Az CLI give you these options so that you automate these settings. Checkout the possible settings with the following command.
```ps
az webapp config -h
```

We don't want the app to be run under 32bit and we don't want the application to go into idle mode. We are paying for the computing power anyway based on time. Let's set them correctly so that the default values are replaced by what we want. 

 - platform (64bit)
 - idle time of app (off)

```ps
az webapp config set --use-32bit-worker-process false -n $appname -g $rgname
az webapp config set --always-on true -n $appname -g $rgname

```
--------------------------------------
Custom domain
-------
Now that we have a running application, we don't want to keep going to the default url. Let's configure a custom domain. I don't expect everyone to have an available domain ready to try this on so just wait a min while I show you how simple it can be. Go to the Portal > App Services > your webapp > Custom domain and click on ADD custom domain.
Insert your domain url and press validate. In this view you will get the settings that you need to put into the DNS configuration of your domain. I'll demonstrate how it works for my domain.

SSL
-------
Once the domain has been validated we can add a certificate to validate the identity of our webapp and prevent the annoying popups in your browser. Next to that it also helps with encrypting traffic. These settings are grouped in the portal under appservices > our app > TLS/SSL

You can pay Azure for the best certificates available but we can also opt for the cheap route and get a free one. If you go to Private key Certificates you can Create App Service Managed Certificate. This will take a few moments..

Once its been created, go back to bindings and add a TLS/Binding with this certificate, opt for type: IP Based. From now on we don't people to connect to our website without HTTPS so lets force that config.
```ps
az webapp update --set httpsOnly=true -g $rgname -n $appname
```
Now refresh your browser(really exit it), visit your domain on www.yourdomain.nl. Hooray, it's now secure.

---------------------
5..Implement autoscaling rules, including scheduled autoscaling, and scaling by operational or system metrics
---------------------
Scaling the hardware of your webapp can be done by scaling up or down on your App Service Plan. All that we used so far required at least a Basic Tier Service Plan, defined as "B1" when we created the service plan initially.

Now,  we want to try out autoscaling which is only available in Standard Service Plans or higher. Scale it up by going to the portal > App Services > your app > Scale up > Production. Select S1 and apply OR use the following command for the same effect
```ps
az appservice plan update --sku S1 -g $rgname -n $planname
az appservice plan show -g $rgname -n $planname --output table
```
-----------------
Autoscale
---------------------

If you need more instances of your application to get more work done, you can also use the autoscale option on your webapp. Autoscaling can scale based on metrics or scale to a specific amount of instances. Once you have defined the autoscaling settings, you can setup a trigger based on metrics or on a specific date or time.

Go to Azure Monitor > AutoScale to find the resources that are available for autoscale. If you have completed the last step, it should be listed. 

First get a copy of our web app resource ID either from the portal (Service Plan > properties) or from this command. Look for the one which ends like this "Microsoft.Web/serverFarms/plan-demo-dev-001"
```ps
az resource list -g $rgname -n $planname --query [].id --output table
```

Next define the autoscale settings for this resource. The settings defined here will use 2 instances as a default and scale up or down between 1 - 5 instances.
```ps
$resourceID= "paste the id"
az monitor autoscale create --name $Name -g $rgname --resource $resourceID --min-count 1 --max-count 5 --count 2
```

Check autoscale run history in the portal, we should now see that a upscale event triggered to reach the default of 2.

We will now add some basic upscale rule and a seperate downscale rule so that autoscale has some logic on which it can base the outscaling and inscaling. We will go for the metric CPUPercentage and let it up or down 1 instance based on a threshold.

```
az monitor autoscale rule create -g $rgname --autoscale-name $Name --scale out 1 --condition "CpuPercentage > 50 avg 1m"

az monitor autoscale rule create -g $rgname --autoscale-name $Name --scale in 1 --condition "CpuPercentage < 25 avg 1m"
```

Finally, lets load another CPU heavy API into our webapp so that we can deliver some CPUload.

```
cd ..
git clone https://github.com/Maxvandermeij/ScalingWebApp
cd ScalingWebApp
dotnet publish

Get-ChildItem -Path .\bin\Debug\netcoreapp3.1\publish\* | Compress-Archive -DestinationPath CPULoadArchive.zip -Force

az webapp deployment source config-zip -g $rgname -n $appname --src CPULoadArchive.Zip
```

Now help me get the CPU load up, be creative: www.maxtips.nl/Prime?searchLimit=10000000     <br>
I'll keep the monitor up to see the changes.

--------------------
6..Cleanup
--------------------
Once we are done, time for cleanup.

```
az group delete --name ""
```

<br>
Thanks for playing and practicing together!
