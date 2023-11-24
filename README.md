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
To understand the folder structure, you can go to the following link in which I explain the structure within the readme of another project. Click [here](https://github.com/SpaikSaucus/net6-ddd-advanced-example/blob/main/README.md#folder-structure)

## How do the components interact?
![net-docker-kafka-mssql-elastic](https://github.com/SpaikSaucus/net7-docker-mssql-kafka-elastic-example/blob/main/img/net-docker-kafka-mssql-elastic.png?raw=true)

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

The following link will take us to the readme of another of my projects, where you can find the information associated with this point. Click [here](https://github.com/SpaikSaucus/net6-ddd-advanced-example/blob/main/README.md#large_blue_diamond-architecture-ddd)

## :large_blue_diamond: Docker

In progress....

### References: :triangular_flag_on_post:
* [Assign static ip container](https://www.baeldung.com/ops/docker-assign-static-ip-container)

## :large_blue_diamond: Elasticsearch

In progress....

### References: :triangular_flag_on_post:
* [Elasticsearch in Docker](https://www.elastic.co/guide/en/elasticsearch/reference/current/docker.html)
* [Elasticsearch client Net](https://www.elastic.co/guide/en/elasticsearch/client/net-api/7.17/configuration-options.html)
* [Elasticsearch ejemplo integración Net](https://www.c-sharpcorner.com/article/how-to-integrate-elasticsearch-in-asp-net-core/)

## :large_blue_diamond: Kafka

In progress....

### References: :triangular_flag_on_post:
* [Kafka tool net](https://github.com/confluentinc/confluent-kafka-dotnet/)


## :large_blue_diamond: Swagger Oas3

To access the documentation generated by this tool, we must enter the following endpoint:
* http://localhost:8080/swagger

![swagger_oas3_1](https://github.com/SpaikSaucus/net7-docker-mssql-kafka-elastic-example/blob/main/img/swagger_oas3_1.png?raw=true)

If we want to view the configuration, we must enter the following classes:

  * __Infrastructure.Bootstrap__ :arrow_right: Extensions   
    * ApplicationBuilder :arrow_right: SwaggerApplicationBuilderExtensions    
    and in:
    * ServiceCollections :arrow_right: SwaggerServiceCollectionExtensions

### References: :triangular_flag_on_post:
  * [Learn Microsoft Swashbuckle](https://learn.microsoft.com/en-us/aspnet/core/tutorials/getting-started-with-swashbuckle?view=aspnetcore-6.0&tabs=visual-studio)
  * [Blog API versioning and integrate Swagger](https://blog.christian-schou.dk/how-to-use-api-versioning-in-net-core-web-api/)
  
## :large_blue_diamond: MediatR + CQRS
The following link will take us to the readme of another of my projects, where you can find the information associated with this point. Click [here](https://github.com/SpaikSaucus/net6-ddd-advanced-example/blob/main/README.md#large_blue_diamond-mediatr--cqrs)


## :large_blue_diamond: Health Check
The following link will take us to the readme of another of my projects, where you can find the information associated with this point. Click [here](https://github.com/SpaikSaucus/net6-ddd-advanced-example/blob/main/README.md#large_blue_diamond-health-check)

## :large_blue_diamond: Logs
The following link will take us to the readme of another of my projects, where you can find the information associated with this point. Click [here](https://github.com/SpaikSaucus/net6-ddd-advanced-example/blob/main/README.md#large_blue_diamond-logs)

## :large_blue_diamond: Unit Of Work
The following link will take us to the readme of another of my projects, where you can find the information associated with this point. Click [here](https://github.com/SpaikSaucus/net6-ddd-advanced-example/blob/main/README.md#large_blue_diamond-unit-of-work)

## :large_blue_diamond: Query Specification Pattern
The following link will take us to the readme of another of my projects, where you can find the information associated with this point. Click [here](https://github.com/SpaikSaucus/net6-ddd-advanced-example/blob/main/README.md#large_blue_diamond-query-specification-pattern)

## :large_blue_diamond: Multiple Environments
The following link will take us to the readme of another of my projects, where you can find the information associated with this point. Click [here](https://github.com/SpaikSaucus/net6-ddd-advanced-example/blob/main/README.md#large_blue_diamond-multiple-environments)

## :large_blue_diamond: Unit Test
The following link will take us to the readme of another of my projects, where you can find the information associated with this point. Click [here](https://github.com/SpaikSaucus/net6-ddd-advanced-example/blob/main/README.md#large_blue_diamond-unit-test)

## :large_blue_diamond: Integration Test
The following link will take us to the readme of another of my projects, where you can find the information associated with this point. Click [here](https://github.com/SpaikSaucus/net6-ddd-advanced-example/blob/main/README.md#large_blue_diamond-integration-test)

## Read Recommended:
  * [Learn Microsoft DDD with CQRS](https://learn.microsoft.com/en-us/dotnet/architecture/microservices/microservice-ddd-cqrs-patterns/ddd-oriented-microservice)

## License
Is licensed under [The MIT License](LICENSE).