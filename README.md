![SIGNALS](https://signals.emitknowledge.com/assets/temp-logo.png)

## Signals
Signals is a framework which focuses on increasing the quality and productivity within development teams by providing them with tools, aspects and processes.

Overview can be found on [https://signals.emitknowledge.com/](https://signals.emitknowledge.com/) 
Docs can be found on [Wiki](https://github.com/EmitKnowledge/Signals/wiki)

## High-level aspects
When developing an application, certain aspects are mandatory in the project setup phase that are consuming development teams time and focus.

### Dependency Injection
Dependency injection. Signals enables you to use your DI framework of choice with the power of attribute-based injection.

Out of the box Signals offers integration with Autofac, DotNetCore and Simple Injector. Integrating another framework is available through following the convention.

### Configuration
Treat configuration files as objects independent of the storage. Signals enables you to create and maintain configuration files. The framework allows you to set different configurations per environment and to select an active environment. All configurations are mapped against objects.

Out of the box Signals has implementation for File and MSSQL configuration providers.

### Logging
Signals provides a standardized logging interface that supports integration with existing logging libraries.

Out of the box Signals integrates with NLog. As the framework is extensible by definition, adding an implementation with another logging library is straight forward.

### Auditing
In the enterprise world, there is often a need to understand who did what and when. Signals support auditing via integration with Audit.NET and MSSQL database. Extending the auditing aspect is supported like with the rest of the aspects in the framework.

### Caching
No matter how fast is your application, it is good to store the most accessed information in the cache. Signals supports in memory caching. Using a different system like Redis can be achieved by extending the caching aspect.

### Localization
You never know when you will get a new request to add support for different languages in the application. Signals got this covered for you. Out of the box the framework supports file, MSSQL and in memory localization setup.

### Storage
When your application requires storing files, Signals supports upload and encryption of files for file system, Azure Blob storage and MSSQL database. Other storage providers are supported by extension of Signals.

### Communication channels
Signals supports distributed processes. The framework supports a request to start from the web application and to finish on the background service. This is achieved with the support of communication channels which by default we support Azure Event Grid, MSMQ, MSSQL and Azure Service Bus.

### Authentication and authorization
This aspect enables you to configure authentication for both ASP.NET MVC and ASP.NET Core. Signals support managing authentication, authorization and permissions with ease with attributes and by direct usage of the authentication, authorization and permission managers.

### Scheduled tasks
Signals support task scheduling with daily, monthly, pattern-based, time part, weekend, weekly and workday recurrence configuration. Current implementations are through Fluent Scheduler and Hangfire. Can be extended with other libraries as well.

### Error handling
Having a centralized mechanism to handle exceptions and retry policies is of great need to prevent information to leak outside the system boundaries and to be able to extract meaningful information from the exceptions and errors. Signals supports this through Polly.NET.

### Benchmarking
We are often challenged to improve long-running processes. To be able to achieve better performance, you will need information on how your system behaves and which processes are the bottleneck. The benchmarking aspect will give you the details by allowing you to track processes, chains and checkpoints.
