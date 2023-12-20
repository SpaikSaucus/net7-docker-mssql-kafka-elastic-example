[![en](https://img.shields.io/badge/lang-en-red.svg):ballot_box_with_check:](#) [![es](https://img.shields.io/badge/lang-es-yellow.svg):black_large_square:](https://github.com/SpaikSaucus/net7-docker-mssql-kafka-elastic-example/blob/main/README.es.md)

# net7-docker-mssql-kafka-elastic-example
NET 7 example with integrations to kafka/elastic/MSSQL and prepared to be containerized.

## Table of Contents

- [Getting started](#getting-started)
- [Folder structure](#folder-structure)
- [How do the components interact?](#how-do-the-components-interact)
- [Features list](#features-list)
- [Read recommended](#read-recommended)
- [License](#license)

## Getting Started
* Download this repository
* Is required Visual Studio IDE :point_right: [download](https://visualstudio.microsoft.com/es/downloads)
* Is required SDK 7 :point_right: [download](https://dotnet.microsoft.com/en-us/download/dotnet/7.0)
* Is required Docker :point_right: [download](https://www.docker.com/products/docker-desktop/)
* Is required Kafka :point_right: [download](https://kafka.apache.org/downloads)
* Is required Java +8 :point_right: [download](https://www.oracle.com/java/technologies/downloads/#java11-windows)

Windows Users:
* Is required WSL :point_right: [download](https://learn.microsoft.com/es-es/windows/wsl/install)


#### Step by step for Windows:
Update the .env file:
* We will have to update the "KAFKA_URI" property with our IP
   * From a terminal we can find out the IPv4 by doing ipconfig

Open a terminal and execute:
```shell
docker pull mcr.microsoft.com/mssql/server
docker pull docker.elastic.co/elasticsearch/elasticsearch:8.11.1
docker network create --subnet=10.5.0.0/16 bdNetwork
```

__Start MSSQL__:
```shell
docker run -e 'ACCEPT_EULA=Y' -e 'SA_PASSWORD=[my_password]' --net bdNetwork --ip 10.5.0.2 -p 1433:1433 --name sql_server -d mcr.microsoft.com/mssql/server:latest
docker start sql_server
```
* Connect to the Database:
  *	__servername__: localhost,1433
	* __authentication__: SQL Server Authentication
	  * __login__: sa
	  * __password__: [my_password]
* Execute "bd/initial_create.sql"
* Execute "bd/initial_data.sql"

Generate the image of my __Web API__:
* In the folder of my project, in the terminal, execute:
```shell
docker build -t user_permissions .
```
Build my container:
* In the folder of my project, in the terminal, execute:
```shell
docker compose -p user_permissions up --no-start
```
* Then we will execute:
```shell
docker network connect bdNetwork api
```

__Start KAFKA__:
* Open a terminal in kafka download folder -> ../../kafka_2.12-3.6.0 and execute:
```shell
.\bin\windows\zookeeper-server-start.bat .\config\zookeeper.properties
```
* Open another terminal in the same folder and execute:
```shell
.\bin\windows\kafka-server-start.bat .\config\server.properties
```

__Start my containers__: (We can activate the play button in the docker desktop)
```shell
docker start elastic
docker start api
```

* In the following image, we can see how the running containers should look from docker desktop.

![docker-containers](https://github.com/SpaikSaucus/net7-docker-mssql-kafka-elastic-example/blob/main/img/docker-containers.png?raw=true)

__Access to swagger and test:__
* http://localhost:8080/swagger

__Access to elasticsearch:__ (_After running the post and creating the 1st document with Id = 1_)
* http://localhost:9200/permission/_doc/1?pretty=true

[Optional] 
* If you want to see how the messages are generated as you operate with the Web API, after starting docker, you can open another terminal in the same folder and run:
```shell
.\bin\windows\kafka-console-consumer.bat --bootstrap-server localhost:9092 --topic events.operations --from-beginning
```
* and enjoy!

## Folder structure

In this section, we will explain the structure of the project, how the layers are designed, and what function each one fulfills. We will also mention some important folders.

### 1- ENTRYPOINT
Web Api, queue listeners, job workers, etc. You should not implement business rules.

   * #### API
     Receives the requests and delegates them to [MediatR](https://github.com/jbogard/MediatR). The use of Mediatr is implemented for decoupling between layers and thus implements a CQRS architecture between transactional and consultative.

     * __Controllers__: will contain the different versions of the drivers. For more information about API Version go [here](#api-versions).
     * __ViewModels__: will contain the Request and Response classes, necessary for the interaction.

### 2- CORE
Main objective, implement business logic and use cases. We must ensure that there are no references to Frameworks associated with Infrastructure (for example: data access frameworks).

   * #### APPLICATION
     We will implement the flow to develop a specific functionality of my application. Orchestrate between the different Domains.

     * __Behaviors__: will contain behavior reusable by MediatR in each API interaction with the APPLICATION layer.
     Examples:
       * ___LoggingBehavior___: Every request and response processed by MediatR will be logged.
       * ___ValidatorBehavior___: Execute the Validators that are associated with a MediatR.

     * __UserCases__: will contain the Use Cases that are defined to meet the requested requirements. A UserCase can make use of another UserCase, to reuse code (for example: reviewing the UserCase of CreatePermissionCommand).
       * ___Commands___: will contain the transactional implementations.
       * ___Queries___: will contain the consultative implementations.
       * ___Validations___: will contain the validation logic for the input data.
       * ___DTOs___: will contain the DTO classes necessary to transfer information between layers, when necessary.

   * #### DOMAIN
     We will define the business rules (in DDD terms, also called Domain and expressed through entities, Domain services, value objects and interfaces).

     * __Core__: contains Base and Interfaces classes that will be necessary to implement critical patterns and features.

### 3- INFRASTRUCTURE
Here we will find specific implementations for data access, ORMs, MicroORMS, HTTP Request, File Management, etc.

   * #### INFRASTRUCTURE

     * __Core__: contains some of the implementations of the Interfaces defined in the DOMAIN layer, associated with solving Infrastructure problems.
     * __EF__: contains the DBContext and the classes necessary to implement Entity Framework, for access to the database.
     * __Services__: contains specific implementations, for example, generating a Report in Excel format. The masses must be configured in the AutofacModules to then be used by injecting IComponentContext.

   * #### INFRASTRUCTURE.BOOSTRAP
   
      * __AutofacModules__: contains the modules that we define, which will be used to register the components that can be created with reflection. In this way, we can use the services that we generate in the _Infrastructure_ layer in the _Application_ layer. The use of these services in the _Domain_ layer is discouraged because it must be as isolated as possible.
      * __Extensions__: contains the configurations necessary for launching our application in a segregated way to improve its understanding and discovery, among other things.

## How do the components interact?
<image style="width: 90%;" src="https://github.com/SpaikSaucus/net7-docker-mssql-kafka-elastic-example/blob/main/img/net-docker-kafka-mssql-elastic.png?raw=true"> </image> 

Each call to our Web API will be handled by a middleware that will generate a request to Kafka.
* The query operations will first be referred to Elasticsearch and if no information is obtained, they will be referred to the database.
* Write operations will be performed first in the database, then they will be impacted in Elasticsearch.

## Features List

- [Architecture DDD (Domain Driven Design)](#large_blue_diamond-architecture-ddd)
- [Docker](#large_blue_diamond-docker)
- [Elasticsearch](#large_blue_diamond-elasticsearch)
- [Kafka](#large_blue_diamond-kafka)
- [Swagger Oas3 (OpenAPI Specification - Version 3)](#large_blue_diamond-swagger-oas3)
- [MediatR + CQRS](#large_blue_diamond-mediatr--cqrs)
- [Health Check](#large_blue_diamond-health-check)
- [Logs](#large_blue_diamond-logs)
- [Unit of Work Pattern](#large_blue_diamond-unit-of-work)
- [Query Specification Pattern](#large_blue_diamond-query-specification-pattern)
- [Multiple Environments File](#large_blue_diamond-multiple-environments)
- [Unit Test](#large_blue_diamond-unit-test)
- [Integrations Test](#large_blue_diamond-integration-test)

## :large_blue_diamond: Architecture DDD
The main objective of applying Domain Driven Design is to be able to isolate the code that belongs to the domain from the technical implementation details and thus focus on the complexity of the business.

### Core Principles
We could say that domain orientation focuses on three basic pillars:
  * Focus on the core domain and business logic.
  * Convert complex designs into domain models.
  * Constant interaction and collaboration with domain experts, which will help resolve doubts and interact more with the development team.

In turn, when we work with DDD we must take into account:
  * Separation of responsibilities into layers, _(isolate the domain)_.
  * Model and define the model.
  * Manage the life cycle of Domain objects.

### The different layers:

* __Domain layer:__
  Responsible for representing business concepts, information about the business situation and business rules. The state that reflects the business situation is controlled and used here, although the technical details of its storage are delegated to the infrastructure. This level is the core of enterprise software, where the business is expressed, in. NET, _is coded as a class library_, with domain entities that capture data and behavior (methods with logic).
  <br/>In turn, this library only has dependencies on .NET libraries, but not on other custom libraries, such as data or persistence. It should not depend on any other level (domain model classes must be CLR or POCO object classes).
  <br/>

* __Application layer:__
  It defines the jobs that the software is supposed to do and directs domain objects to solve problems. The tasks that are the responsibility of this level are significant to the business or necessary for interaction with the application levels of other systems.
  <br/>This level should be kept narrow. It does not contain business rules or knowledge, but only coordinates tasks and delegates work to domain object collaborations at the next level. It does not have any status that reflects the business situation, but it can have a status that reflects the progress of a task for the user or the program.
  <br/>Typically, the application layer in .NET microservices is coded as an ASP.NET Core Web API project. The project implements microservice interaction, remote network access, and external web APIs used by client or front-end applications. It includes queries if using a CQRS approach, commands accepted by the microservice, and even event-driven communication between microservices (integration events).
  <br/>Basically, application logic is where all use cases that depend on a given front end are implemented.

  <br/>In this "_UserPermission_" example, this layer is split to improve the design focus, resulting in the following two projects:
  * 1- ENTRYPOINT :arrow_right: __API__
  * 2- CORE :arrow_right: __Application__
  <br/>

* __Infrastructure layer:__
  It is where the technical part of the application resides, with its specific implementations and where dependencies on third-party software will be added to comply with integrations, database, file management, etc.

  <br/>In this "_UserPermission_" example, this layer is split to improve the design focus, resulting in the following two projects:
  * 3- INFRASTRUCTURE :arrow_right: __Infrastructure__
  * 3- INFRASTRUCTURE :arrow_right: __Infrastructure.Bootstrap__
  <br/>

![ddd_1_en](https://github.com/SpaikSaucus/net7-docker-mssql-kafka-elastic-example/blob/main/readme-img/ddd_1_en.png?raw=true)
![ddd_2_en](https://github.com/SpaikSaucus/net7-docker-mssql-kafka-elastic-example/blob/main/readme-img/ddd_2_en.png?raw=true)

### References :triangular_flag_on_post:
> * [Learn Microsoft DDD](https://learn.microsoft.com/en-us/dotnet/architecture/microservices/microservice-ddd-cqrs-patterns/ddd-oriented-microservice)
> * [Introduction DDD (spanish)](https://refactorizando.com/introduccion-domain-drive-design/)

## :large_blue_diamond: Docker

Docker is an open source project that automates the deployment of applications within software containers, providing an additional layer of abstraction and automation of application virtualization across multiple operating systems.

Built on the facilities provided by the Linux kernel (mainly cgroups and namespaces), a Docker container, unlike a virtual machine, does not require including a separate operating system. Instead, it relies on kernel capabilities and uses resource isolation (CPU, memory, I/O, network, etc.) and separate namespaces to isolate an application's view from the operating system. Therefore, containers have significantly less footprint than virtual machine (VM) images.

Multiple containers share the same core, but each container can be restricted to using only a defined amount of resources such as CPU, memory, and I/O.

Using Docker to create and manage containers can simplify the creation of highly distributed systems. This allows nodes to be deployed as resources become available or as more nodes are needed, enabling a Platform as a Service (PaaS) style deployment.

> [!NOTE]
> Docker is also a company that promotes and drives this technology, in collaboration with cloud, Linux and Windows providers, including Microsoft.

### Comparing Docker containers with virtual machines

| Virtual Machines | Docker Containers |
| :-------------: | :-------------: |
| ![vm](https://github.com/SpaikSaucus/net7-docker-mssql-kafka-elastic-example/blob/main/img/virtual-machine-hardware-software.png?raw=true) | ![dc](https://github.com/SpaikSaucus/net7-docker-mssql-kafka-elastic-example/blob/main/img/docker-container-hardware-software.png?raw=true) |
| Virtual machines include the application, the required libraries or binaries, and a full guest operating system. Full virtualization requires more resources than containerization. | Containers include the application and all its dependencies. However, they share the OS kernel with other containers, running as isolated processes in user space on the host operating system. (Except in Hyper-V containers, where each container runs inside of a special virtual machine per container.) |

### Analogy
In the same way that freight containers allow them to be transported by ship, train or truck regardless of the cargo inside, software containers act as a standard software deployment unit that can contain different dependencies and code. In this way, the inclusion of containerized software allows developers and IT professionals to deploy it in environments __with little or no modifications at all__.

### Summary
Containers offer the benefits of isolation, portability, agility, scalability, and control throughout the entire application lifecycle workflow. The most important advantage is the environment isolation provided between development and operations.

### References :triangular_flag_on_post:
> * [Learn Microsoft - Container Docker Introduction](https://learn.microsoft.com/en-us/dotnet/architecture/microservices/container-docker-introduction/)
> * [Assign static ip container](https://www.baeldung.com/ops/docker-assign-static-ip-container)
> * [Docker wiki](https://en.wikipedia.org/wiki/Docker_(software))


## :large_blue_diamond: Elasticsearch

Elasticsearch is a search engine based on the Apache Lucene library. It provides a distributed, multitenant-capable full-text search engine with an HTTP web interface and schema-free JSON documents.

### Some Use Cases
* __Log Monitoring__: Fast and scalable logging that won't quit.
* __Infrastructure Monitoring__: Monitor and visualize your system metrics.
* __APM__: Get insight into your application performance.
* __Synthetic Monitoring__: Monitor and react to availability issues.
* __Enterprise Search__: Search and discovery experiences for any use case.
* __Maps__: Explore location data in real time.
* __SIEM__: Interactive investigation and automated threat detection.
* __Endpoint Security__: Prevent, detect, hunt for, and respond to threats.

### References :triangular_flag_on_post:
> * [Elasticsearch wiki](https://en.wikipedia.org/wiki/Elasticsearch)
> * [Elasticsearch in Docker](https://www.elastic.co/guide/en/elasticsearch/reference/current/docker.html)
> * [Elasticsearch client Net](https://www.elastic.co/guide/en/elasticsearch/client/net-api/7.17/configuration-options.html)
> * [Elasticsearch ejemplo integración Net](https://www.c-sharpcorner.com/article/how-to-integrate-elasticsearch-in-asp-net-core/)

## :large_blue_diamond: Kafka

Apache Kafka is an open-source distributed event streaming platform used for high-performance data pipelines, streaming analytics, data integration, and mission-critical applications.

### Core Capabilities
* __High throughput__: Deliver messages at network limited throughput using a cluster of machines with latencies as low as 2ms.
* __Scalable__: Scale production clusters up to a thousand brokers, trillions of messages per day, petabytes of data, hundreds of thousands of partitions. Elastically expand and contract storage and processing.
* __Permanent storage__: Store streams of data safely in a distributed, durable, fault-tolerant cluster.
* __High availability__: Stretch clusters efficiently over availability zones or connect separate clusters across geographic regions.

### Some Use Cases
Event streaming is applied to a wide variety of use cases across a plethora of industries and organizations. Its many examples include:

* To process payments and financial transactions in real-time, such as in stock exchanges, banks, and insurances.
* To track and monitor cars, trucks, fleets, and shipments in real-time, such as in logistics and the automotive industry.
* To continuously capture and analyze sensor data from IoT devices or other equipment, such as in factories and wind parks.
* To collect and immediately react to customer interactions and orders, such as in retail, the hotel and travel industry, and mobile applications.
* To monitor patients in hospital care and predict changes in condition to ensure timely treatment in emergencies.
* To connect, store, and make available data produced by different divisions of a company.
* To serve as the foundation for data platforms, event-driven architectures, and microservices.

<image style="width: 90%;" src="https://github.com/SpaikSaucus/cheatsheets/blob/main/Queues/Kafka/Kafka_Top_5_use_cases.gif?raw=true"> </image> 


### How do it work?
It operates through a publish-subscribe model where producers generate messages and send them to a topic, and consumers subscribe to those topics to process messages in real time. Kafka provides high availability, scalability, and fault tolerance, making it ideal for use cases such as log processing, real-time analytics, and building microservices architectures.

### References :triangular_flag_on_post:
> * [Official web](https://kafka.apache.org/)
> * [Introduction](https://kafka.apache.org/documentation/#gettingStarted)
> * [Kafka tool net](https://github.com/confluentinc/confluent-kafka-dotnet/)
> * [Cheatsheet - Kafka](https://github.com/SpaikSaucus/cheatsheets/tree/main/Queues/Kafka)


## :large_blue_diamond: Swagger Oas3

To access the documentation generated by this tool, we must enter the following endpoint:
* http://localhost:8080/swagger

![swagger_oas3_1](https://github.com/SpaikSaucus/net7-docker-mssql-kafka-elastic-example/blob/main/img/swagger_oas3_1.png?raw=true)

If we want to view the configuration, we must enter the following classes:

  * __Infrastructure.Bootstrap__ :arrow_right: Extensions   
    * ApplicationBuilder :arrow_right: SwaggerApplicationBuilderExtensions    
    and in:
    * ServiceCollections :arrow_right: SwaggerServiceCollectionExtensions

### References :triangular_flag_on_post:
> * [Learn Microsoft - Swashbuckle](https://learn.microsoft.com/en-us/aspnet/core/tutorials/getting-started-with-swashbuckle?view=aspnetcore-7.0&tabs=visual-studio)
> * [Blog API versioning and integrate Swagger](https://blog.christian-schou.dk/how-to-use-api-versioning-in-net-core-web-api/)
  
## :large_blue_diamond: MediatR + CQRS

### MediatR
It is a small and simple open source library that implements the mediator pattern, for piping messages (commands) and routing them, in memory, to the correct command handlers.

Using the mediator pattern helps reduce coupling and isolate the processing of the requested command from the rest of the code.

### CQRS
Command Query Responsibility Segregation, It is a pattern that seeks to have two separate objects, one for reading operations and another for writing operations, unlike other approaches that seek to have everything in one.

### Combining them
In this example "_CreatePermissionCommand_", we combine the mediator pattern with the CQRS pattern, the result involves the creation of commands for queries and commands to change the state of the system.

  * Queries: These queries return a result and don't change the state of the system, and they're free of side effects.
    * __Application__ :arrow_right: UserCases :arrow_right: FindOne :arrow_right: Queries
    <br/>  
  * Commands: These commands change the state of a system.
    * __Application__ :arrow_right: UserCases :arrow_right: Create :arrow_right: Commands

### References :triangular_flag_on_post:
> * [CQRS web-api command process pipeline with a mediator pattern MediatR](https://learn.microsoft.com/en-us/dotnet/architecture/microservices/microservice-ddd-cqrs-patterns/microservice-application-layer-implementation-web-api#implement-the-command-process-pipeline-with-a-mediator-pattern-mediatr)
> * [Learn Microsoft CRQS Pattern in DDD](https://learn.microsoft.com/en-us/dotnet/architecture/microservices/microservice-ddd-cqrs-patterns/apply-simplified-microservice-cqrs-ddd-patterns)

## :large_blue_diamond: Health Check

Health checks are exposed by an app as HTTP endpoints, are typically used with an external monitoring service or container orchestrator to check the status of an app. 

Before adding health checks to an app, decide on which monitoring system to use. The monitoring system dictates what types of health checks to create and how to configure their endpoints.

Use the library:
   * Microsoft.AspNetCore.Diagnostics.HealthChecks

The configuration can be found in:
   * __Infrastructure.Bootstrap__ :arrow_right: Extensions
     * ApplicationBuilder :arrow_right: HealthChecksApplicationBuilderExtensions
     and in:
     * ServiceCollections :arrow_right: HealthChecksServiceCollectionExtensions
    
And we can call the __"/health"__ endpoint to check its operation.
   * https://localhost:5001/health

### References :triangular_flag_on_post:
> * [Learn Microsoft Health Checks](https://learn.microsoft.com/en-us/aspnet/core/host-and-deploy/health-checks?view=aspnetcore-7.0)

## :large_blue_diamond: Logs

To obtain information and record errors that occur in our application, we will use the Serilog library, which makes it easier for us to implement this very useful feature for diagnosis.

### How to use it
Define a variable in our class to store the logger:

```csharp
private readonly ILogger<MyClass> logger;
```
Inject the logger into the constructor:
```csharp
public MyClass(ILogger<MyClass> logger)
{
  this.logger = logger;
}
```
Make use of the logger, examples:
```csharp
this.logger.Log(LogLevel.Information, "Permission {0} already exists", permission.Id);
...
this.logger.LogInformation("Permission {0} already exists", permission.Id);
...
this.logger.LogError("API Error: {api}: \n{result}",
  apiException.RequestMessage.RequestUri, 
  apiException.Content);
```

### Configuration
The Serilog configuration is in the following class:
  * __API__ :arrow_right: Program.cs    

And in the file appsettings.[Environment].json:
```bash
  "Serilog": {
    "MinimumLevel": {
      "Default": "Information",
      "Microsoft": "Information",
      "Microsoft.Hosting.Lifetime": "Information",
      "Override": {
        "System": "Information",
        "Microsoft": "Information"
      }
    },
    "WriteTo": [
      {
        "Name": "Console",
        "Args": {
          "theme": "Serilog.Sinks.SystemConsole.Themes.AnsiConsoleTheme::Code, Serilog.Sinks.Console",
          "outputTemplate": "{Timestamp:u} [{Level:u3}] [{RequestId}] {Message:lj} <s:{SourceContext}>{NewLine}{Exception}"
        }
      }
    ],
    "Enrich": [ "FromLogContext", "WithMachineName" ]
  },
```

#### Priority Level 

* _Verbose_ :arrow_right: _Debug_ :arrow_right: _Information_ :arrow_right: _Warning_ :arrow_right: _Error_ :arrow_right: _Fatal_

For example, if we indicate the "Information" level, the "Verbose" and "Debug" level logs will not be displayed.


### References :triangular_flag_on_post:
> * [Serilog Web](https://serilog.net/)
> * [Serilog Tutorial](https://stackify.com/serilog-tutorial-net-logging/)

## :large_blue_diamond: Unit Of Work

It is a pattern that has the purpose of ensuring that the same database context is shared so that when the tasks to be performed in the database are completed, the SaveChanges method can be called on that instance of the context and ensure that all related changes will be coordinated.

Example:

```csharp
var permission = new Permission()
{
    EmployeeForename = cmd.EmployeeForename,
    EmployeeSurname = cmd.EmployeeSurname,
    PermissionTypeId = cmd.PermissionTypeId,
    PermissionDate = DateTime.UtcNow
};

this.unitOfWork.Repository<Permission>().Add(permission);
// ...
// this.unitOfWork.Repository<....>().Add(...);
// this.unitOfWork.Repository<....>().Remove(...);
// this.unitOfWork.Repository<....>().Update(...);
// ...
await this.unitOfWork.Complete();
```

### References :triangular_flag_on_post:
> * [Learn Microsoft Unit Of Work Pattern](https://learn.microsoft.com/en-us/aspnet/mvc/overview/older-versions/getting-started-with-ef-5-using-mvc-4/implementing-the-repository-and-unit-of-work-patterns-in-an-asp-net-mvc-application#creating-the-unit-of-work-class)
> * [Martin Fower Unit Of Work](https://martinfowler.com/eaaCatalog/unitOfWork.html)

## :large_blue_diamond: Query Specification Pattern

It is a pattern that seeks to comply with DDD for data queries so that these specifications are stored in the __Domain__ layer, effectively separating the logic that exists in the queries from their implementation.

To do this, the base class _BaseSpecification_ and the interface _ISpecification_ were generated in the __Domain__ layer. In the __Infrastructure__ layer there is the _SpecificationEvaluator_ class that is used by the _Repository_ class to apply the specification to be used.

Example:
The _PermissionGetSpecification_ class found in the folder
  * __Domain__ :arrow_right: Permission :arrow_right: Queries

```csharp
public class PermissionGetSpecification : BaseSpecification<Models.Permission>
{
    public PermissionGetSpecification(Models.Permission permission)
    {
        base.AddInclude(x => x.PermissionType);

        if (permission.Id != 0)
        {
            base.SetCriteria(x => x.Id == permission.Id);
        }
        else 
        {
            base.SetCriteria(x => x.PermissionTypeId == permission.PermissionTypeId
                && x.EmployeeForename == permission.EmployeeForename
                && x.EmployeeSurname == permission.EmployeeSurname
            );
        }
    }
}
```
This specification is used in the _PermissionGetQuery_ class found in the folder
  * __Application__ :arrow_right: UserCases :arrow_right: FindOne :arrow_right: Queries

```csharp
var spec = new PermissionGetSpecification(permission);
var result = this.unitOfWork.Repository<Permission>().Find(spec).FirstOrDefault();
return Task.FromResult(result);
```
### References :triangular_flag_on_post: 
> * [Learn Microsoft Query Specification Pattern in DDD](https://learn.microsoft.com/en-us/dotnet/architecture/microservices/microservice-ddd-cqrs-patterns/infrastructure-persistence-layer-implementation-entity-framework-core#implement-the-query-specification-pattern)
> * [Medium Specification Pattern Generic Repository](https://medium.com/@rudyzio92/net-core-using-the-specification-pattern-alongside-a-generic-repository-318cd4eea4aa)

## :large_blue_diamond: Multiple Environments
Create the json with this naming:
  * appsettings.__environment__.json

Examples:

![img_hierarchy_1](https://github.com/SpaikSaucus/net7-docker-mssql-kafka-elastic-example/blob/main/readme-img/multiple_environments_1.png?raw=true)

![img_hierarchy_2](https://github.com/SpaikSaucus/net7-docker-mssql-kafka-elastic-example/blob/main/readme-img/multiple_environments_2.png?raw=true)	

### References :triangular_flag_on_post:
> * [Learn Microsoft Environments](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/environments?view=aspnetcore-7.0)

## :large_blue_diamond: Unit Test
xUnit: These tests are written using XUnit and using the following FluentAssertions and FakeItEasy libraries.

### References :triangular_flag_on_post:
> * [Fluent Assertions Web](https://fluentassertions.com/)
> * [Fake It Easy Web](https://fakeiteasy.readthedocs.io/en/stable/)
> * [Blog NUnit vs xUnit vs MSTest](https://www.lambdatest.com/blog/nunit-vs-xunit-vs-mstest/)
> * [Learn Microsoft Unit Testing (best practices)](https://learn.microsoft.com/en-us/dotnet/core/testing/unit-testing-best-practices)

## :large_blue_diamond: Integration Test
Microsoft.AspNetCore.TestHost - These Tests help us perform an integration test of our APP. The objective of this is to be able to build the Net Core middleware with all the configurations.

### References :triangular_flag_on_post:
> * [Learn Microsoft Integration Tests](https://learn.microsoft.com/en-us/aspnet/core/test/integration-tests?view=aspnetcore-7.0)

## Read Recommended:
> * [Kafka Introduction](https://kafka.apache.org/documentation/#gettingStarted)
> * [Learn Microsoft - DDD with CQRS](https://learn.microsoft.com/en-us/dotnet/architecture/microservices/microservice-ddd-cqrs-patterns/ddd-oriented-microservice)

## License
Is licensed under [The MIT License](LICENSE).