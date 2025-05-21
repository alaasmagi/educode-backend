# educode-backend
## Description

* UI language (Admin UI): English
* Development year: **2025**
* Languages and technologies: **C#, .NET Core, ASP.NET MVC, JWT, Entity Framework Core**
* This is the backend component of my Bachelor's final thesis project, which also includes [mobile app](https://github.com/alaasmagi/educode-mobile) and [browser client](https://github.com/alaasmagi/educode-web).

## How to run #1

### Prerequisites

* .NET SDK 9.0

The application should have .env file in the root folder `/` and it shoult have following content:
```bash
HOST=<your-db-host>
PORT=<your-db-port>
USER=<your-db-username>
DB=<your-db-name>
DBKEY=<your-db-password>

OTPKEY=<hash-for-otp-generation> //OTP-s are used to create accounts and recover forgotten passwords

ADMINUSER=
ADMINKEY=
ADMINTOKENSALT=

JWTKEY=<your-json-web-token-authenticity-key> //Use some kind of hashed value
JWTAUD=<your-json-web-token-audience>
JWTISS=<your-json-web-token-issuer>

MAILSENDER_EMAIL=<external-smtp-mailservice-email-address>
MAILSENDER_KEY=<external-smtp-mailservice-key>
MAILSENDER_HOST=<external-smtp-mailsender-host>
MAILSENDER_PORT=<external-smtp-port>

EMAILDOMAIN=<email-domain-for-otp> //Default value: "@taltech.ee"

FRONTENDURL=<web-frontend-url-for-cors>
```
The idea behind this complicated .env file is that if government decides to change something about taxation, the app does not need to be changed, only environment variables change.

### Running the app

After meeting all prerequisites above - 
* application can be run via terminal/cmd opened in the root of WebApp folder `/WebApp` by command
```bash
dotnet run
```
* user interface can be viewed from the web browser on the address the application provided in the terminal/cmd

## How to run #2

### Prerequisites

* .NET SDK 9.0

The application should have .env file in the root folder `/` and it shoult have following content:
```bash
HOST=<your-db-host>
PORT=<your-db-port>
USER=<your-db-username>
DB=<your-db-name>
DBKEY=<your-db-password>

OTPKEY=<hash-for-otp-generation> //OTP-s are used to create accounts and recover forgotten passwords

ADMINUSER=
ADMINKEY=
ADMINTOKENSALT=

JWTKEY=<your-json-web-token-authenticity-key> //Use some kind of hashed value
JWTAUD=<your-json-web-token-audience>
JWTISS=<your-json-web-token-issuer>

MAILSENDER_EMAIL=<external-smtp-mailservice-email-address>
MAILSENDER_KEY=<external-smtp-mailservice-key>
MAILSENDER_HOST=<external-smtp-mailsender-host>
MAILSENDER_PORT=<external-smtp-port>

EMAILDOMAIN=<email-domain-for-otp> //Default value: "@taltech.ee"

FRONTENDURL=<web-frontend-url-for-cors>
```
The idea behind this complicated .env file is that if government decides to change something about taxation, the app does not need to be changed, only environment variables change.

### Running the app

After meeting all prerequisites above - 
* application can be run via terminal/cmd opened in the root of WebApp folder `/WebApp` by command
```bash
dotnet run
```
* user interface can be viewed from the web browser on the address the application provided in the terminal/cmd

## Features
* Users can calculate their salary based on different input types (e.g., gross salary, net salary, or employer's cost)
* Additional parameters include pension contributions and unemployment insurance
* The application takes into account Estonia’s unique tax-free income system
* OpenAI provides a short description about the net salary

## Design choices

### Application overall design
I used ASP.NET MVC, because I think, that keeping logic and view separate keeps the code clean, well structured and provides better testability. 

### Services
There are two main services:
* SalaryCalculatorService - main service that calculates the output based on the user's input
* OpenAiService - additional serice that communicates with OpenAI API and gets the short description of net income
And configuration helper service:
* CalculatorConfig - service that gets all necessary data from .env file for SalaryCalculatorService

### Models
Models are used to transfer data between the user interface and business logic.  
* **SalaryInputModel**
```csharp
public class SalaryInputModel
{
    public decimal? NetSalary { get; set; }
    public decimal? GrossSalary { get; set; }
    public decimal? EmployerCost { get; set; }

    public int PensionPercent { get; set; } = 2;     // Mandatory in Estonia
    public bool IncludeUnemploymentInsurance { get; set; } = true;
    public bool UseTaxFreeIncome { get; set; } = true;
}
```
* **SalaryResultModel**
```csharp
public class SalaryResultModel
{
    public decimal NetSalary { get; set; }
    public decimal GrossSalary { get; set; }
    public decimal EmployerCost { get; set; }
    
    public string? AiComment { get; set; } 
}
```
  
### User Interface
* UI is implemented using ASP.NET MVC default pages (Views)
* Bootstrap is used for quick customisation

## Improvements & scaling possibilities

### Taxation
* Some kind of tax office API could be used to get the most updated taxation data

### More AI models as options
* As OpenAI develops more and more different AI models, they could be added in this openai-enhancer-app as options

### Mobile Application
* Separate mobile application could be made with React Native or Flutter to make UX better on mobile interfaces
