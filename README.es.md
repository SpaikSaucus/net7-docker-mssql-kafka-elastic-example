[![en](https://img.shields.io/badge/lang-en-red.svg):black_large_square:](https://github.com/SpaikSaucus/net7-docker-mssql-kafka-elastic-example/blob/main/README.md) [![es](https://img.shields.io/badge/lang-es-yellow.svg):ballot_box_with_check:](#)

# net7-docker-mssql-kafka-elastic-example
Ejemplo NET 7 con integración a kafka/elastic/MSSQL y preparado para ser containerized.

## Tabla de Contenido

- [Iniciando](#iniciando)
- [Estructura de carpetas](#estructura-de-carpetas)
- [Como interaccionan los componentes](#como-interaccionan-los-componentes)
- [Lista de características](#lista-de-caracter%C3%ADsticas)
- [Lectura recomendada](#lectura-recomendada)
- [Licencia](#licencia)

## Iniciando
* Descargar este repositorio
* Es requerido Visual Studio IDE :point_right: [download](https://visualstudio.microsoft.com/es/downloads)
* Es requerido SDK 7 :point_right: [download](https://dotnet.microsoft.com/en-us/download/dotnet/7.0)
* Es requerido Docker :point_right: [download](https://www.docker.com/products/docker-desktop/)
* Es requerido Kafka :point_right: [download](https://kafka.apache.org/downloads)
* Es requerido Java +8 :point_right: [download](https://www.oracle.com/java/technologies/downloads/#java11-windows)

Usuarios windows:
* Es requerido WSL :point_right: [download](https://learn.microsoft.com/es-es/windows/wsl/install)


#### Paso a Paso en Windows:
Actualizar el archivo .env:
* Deberemos actualizar la propiedad "KAFKA_URI" con nuestra ip
  * Desde una terminal podremos averiguar la IPv4 haciendo ipconfig

Abrir una terminal y ejecutar:
```shell
docker pull mcr.microsoft.com/mssql/server
docker pull docker.elastic.co/elasticsearch/elasticsearch:8.11.1
docker network create --subnet=10.5.0.0/16 bdNetwork
```

__Iniciar MSSQL__:
```shell
docker run -e 'ACCEPT_EULA=Y' -e 'SA_PASSWORD=[my_password]' --net bdNetwork --ip 10.5.0.2 -p 1433:1433 --name sql_server -d mcr.microsoft.com/mssql/server:latest
docker start sql_server
```
* Conectarse a la Base de Datos :
  *	__servername__: localhost,1433
	* __authentication__: SQL Server Authentication
	  * __login__: sa
	  * __password__: [my_password]
* Ejecutar "bd/initial_create.sql"
* Ejecutar "bd/initial_data.sql"

Generar la imagen de mi __Web API__:
* Posicionados dentro de la carpeta del proyecto, en la terminal, ejecutaremos:
```shell
docker build -t user_permissions .
```
Crear mi container:
* Posicionados dentro de la carpeta del proyecto, en la terminal, ejecutaremos:
```shell
docker compose -p user_permissions up --no-start
```
* Luego ejecutaremos:
```shell
docker network connect bdNetwork api
```

__Iniciar KAFKA__:
* Abrir una terminal en la carpeta donde hayas descargado kafka -> ../../kafka_2.12-3.6.0 y ejecutar:
```shell
.\bin\windows\zookeeper-server-start.bat .\config\zookeeper.properties
```
* Abrir otra terminal en la misma carpeta y ejecutar:
```shell
.\bin\windows\kafka-server-start.bat .\config\server.properties
```

__Iniciar mis containers__: (Podemos accionar el botón play en docker desktop)
```shell
docker start elastic
docker start api
```

* En la siguiente imagen podremos visualizar como deberían verse desde docker desktop los containers ejecutando.

![docker-containers](https://github.com/SpaikSaucus/net7-docker-mssql-kafka-elastic-example/blob/main/img/docker-containers.png?raw=true)

__Acceder al swagger y probar:__
* http://localhost:8080/swagger

__Acceder a elasticsearch:__ (_Luego de ejecutar el post y crear el 1er documento con Id = 1_)
* http://localhost:9200/permission/_doc/1?pretty=true

[Opcional] 
* Si quieres visualizar como se van generando los mensajes a medida que vayas operando con la Web API, luego de iniciar el docker, puedes abrir otra terminal en la misma carpeta y ejecutar:
```shell
.\bin\windows\kafka-console-consumer.bat --bootstrap-server localhost:9092 --topic events.operations --from-beginning
```
* A disfrutar!

## Estructura de carpetas
Para entender la estructura de carpetas, podes ir al siguiente link en el que explico la estructura dentro del readme de otro proyecto. Click [aquí](https://github.com/SpaikSaucus/net6-ddd-advanced-example/blob/main/README.es.md#estructura-de-carpetas)

## Como interaccionan los componentes

<image style="width: 90%;" src="https://github.com/SpaikSaucus/net7-docker-mssql-kafka-elastic-example/blob/main/img/net-docker-kafka-mssql-elastic.png?raw=true"> </image> 

Cada llamada a nuestra Web API, será manejada por un middleware que generara un request a Kafka. 
* Las operaciones de consulta, serán primeramente derivadas a Elasticsearch y en caso de no obtener información, serán derivadas a la base de datos. 
* Las operaciones de escritura, serán realizadas en primera instancia en la base de datos, luego serán impactadas en Elasticsearch.

## Lista de características

- [Arquitectura DDD (Domain Driven Design)](#large_blue_diamond-arquitectura-ddd)
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


## :large_blue_diamond: Arquitectura DDD

El siguiente link nos conducirá al readme de otro de mis proyecto, donde podrá encontrar la información asociada a este punto. Click [aquí](https://github.com/SpaikSaucus/net6-ddd-advanced-example/blob/main/README.es.md#large_blue_diamond-arquitectura-ddd)

## :large_blue_diamond: Docker

Docker es un proyecto de código abierto que automatiza el despliegue de aplicaciones dentro de contenedores de software, proporcionando una capa adicional de abstracción y automatización de virtualización de aplicaciones en múltiples sistemas operativos.

Construido sobre las facilidades proporcionadas por el kernel Linux (principalmente cgroups y namespaces), un contenedor Docker, a diferencia de una máquina virtual, no requiere incluir un sistema operativo independiente. En su lugar, se basa en las funcionalidades del kernel y utiliza el aislamiento de recursos (CPU, la memoria, E/S, red, etc.) y namespaces separados para aislar la vista de una aplicación del sistema operativo. Por lo tanto, los contenedores tienen una superficie significativamente menor que las imágenes de máquina virtual (VM).

Contenedores múltiples comparten el mismo núcleo, pero cada contenedor puede ser restringido a utilizar solo una cantidad definida de recursos como CPU, memoria y E/S.

Usar Docker para crear y gestionar contenedores puede simplificar la creación de sistemas altamente distribuidos. Esto permite que el despliegue de nodos se realice a medida que se dispone de recursos o cuando se necesiten más nodos, lo que permite una plataforma como servicio (PaaS - Platform as a Service) de estilo de despliegue.

> [!NOTE]
> Docker es también una empresa que promueve e impulsa esta tecnología, en colaboración con proveedores de la nube, Linux y Windows, incluido Microsoft.

### Comparando Contenedores de Docker con Virtual Machines

<table>
  <th>Virtual Machines</th>
  <th>Contenedores de Docker</th>
  <tr>
    <td> 
      <image src="https://github.com/SpaikSaucus/net7-docker-mssql-kafka-elastic-example/blob/main/img/virtual-machine-hardware-software.png?raw=true"> </image>
    </td>
    <td>
      <image src="https://github.com/SpaikSaucus/net7-docker-mssql-kafka-elastic-example/blob/main/img/docker-container-hardware-software.png?raw=true"> </image> 
    </td>
  </tr>
  <tr>
    <td width="50%" style="vertical-align: top !important;">
    Las máquinas virtuales incluyen la aplicación, las bibliotecas o los archivos binarios necesarios y un sistema operativo invitado completo. La virtualización completa requiere más recursos que la inclusión en contenedores.
    </td>
    <td width="50%" style="vertical-align: top !important;">
    Los contenedores incluyen la aplicación y todas sus dependencias. Sin embargo, comparten el kernel del sistema operativo con otros contenedores, que se ejecutan como procesos aislados en el espacio de usuario en el sistema operativo host. (Excepto en los contenedores de Hyper-V, en que cada contenedor se ejecuta dentro de una máquina virtual especial por contenedor).
    </td>
  </tr>
</table>

### Analogía
Del mismo modo que los contenedores de mercancías permiten su transporte por barco, tren o camión independientemente de la carga de su interior, los contenedores de software actúan como una unidad estándar de implementación de software que puede contener diferentes dependencias y código. De esta manera, la inclusión del software en contenedor permite a los desarrolladores y los profesionales de TI implementarlo en entornos __con pocas modificaciones o ninguna en absoluto__.

### Resumen
Los contenedores ofrecen las ventajas del aislamiento, la portabilidad, la agilidad, la escalabilidad y el control a lo largo de todo el flujo de trabajo del ciclo de vida de la aplicación. La ventaja más importante es el aislamiento del entorno que se proporciona entre el desarrollo y las operaciones.

### Referencias :triangular_flag_on_post:
> * [Aprendiendo Microsoft - Container Docker Introducción](https://learn.microsoft.com/es-es/dotnet/architecture/microservices/container-docker-introduction/)
> * [Assign static ip container](https://www.baeldung.com/ops/docker-assign-static-ip-container)
> * [Docker wiki](https://es.wikipedia.org/wiki/Docker_(software))

## :large_blue_diamond: Elasticsearch

En progreso....

### Referencias :triangular_flag_on_post:
> * [Elasticsearch in Docker](https://www.elastic.co/guide/en/elasticsearch/reference/current/docker.html)
> * [Elasticsearch client Net](https://www.elastic.co/guide/en/elasticsearch/client/net-api/7.17/configuration-options.html)
> * [Elasticsearch ejemplo integración Net](https://www.c-sharpcorner.com/article/how-to-integrate-elasticsearch-in-asp-net-core/)

## :large_blue_diamond: Kafka

En progreso....

### Referencias :triangular_flag_on_post:
> * [Kafka tool net](https://github.com/confluentinc/confluent-kafka-dotnet/)

## :large_blue_diamond: Swagger Oas3

Para acceder a la documentación generada por esta herramienta, deberemos ingresar al siguiente endpoint:
* http://localhost:8080/swagger

![swagger_oas3_1](https://github.com/SpaikSaucus/net7-docker-mssql-kafka-elastic-example/blob/main/img/swagger_oas3_1.png?raw=true)

Si queremos visualizar la configuración, debemos ingresar a la siguientes clases: 

  * __Infrastructure.Bootstrap__ :arrow_right: Extensions   
    * ApplicationBuilder :arrow_right: SwaggerApplicationBuilderExtensions    
    y en:
    * ServiceCollections :arrow_right: SwaggerServiceCollectionExtensions

### Referencias :triangular_flag_on_post:
> * [Aprendiendo Microsoft - Swashbuckle](https://learn.microsoft.com/es-es/aspnet/core/tutorials/getting-started-with-swashbuckle?view=aspnetcore-6.0&tabs=visual-studio)
> * [Blog API versioning and integrate Swagger](https://blog.christian-schou.dk/how-to-use-api-versioning-in-net-core-web-api/)
  
## :large_blue_diamond: MediatR + CQRS
El siguiente link nos conducirá al readme de otro de mis proyecto, donde podrá encontrar la información asociada a este punto. Click [aquí](https://github.com/SpaikSaucus/net6-ddd-advanced-example/blob/main/README.es.md#large_blue_diamond-mediatr--cqrs)

## :large_blue_diamond: Health Check
El siguiente link nos conducirá al readme de otro de mis proyecto, donde podrá encontrar la información asociada a este punto. Click [aquí](https://github.com/SpaikSaucus/net6-ddd-advanced-example/blob/main/README.es.md#large_blue_diamond-health-check)

## :large_blue_diamond: Logs
El siguiente link nos conducirá al readme de otro de mis proyecto, donde podrá encontrar la información asociada a este punto. Click [aquí](https://github.com/SpaikSaucus/net6-ddd-advanced-example/blob/main/README.es.md#large_blue_diamond-logs)

## :large_blue_diamond: Unit Of Work
El siguiente link nos conducirá al readme de otro de mis proyecto, donde podrá encontrar la información asociada a este punto. Click [aquí](https://github.com/SpaikSaucus/net6-ddd-advanced-example/blob/main/README.es.md#large_blue_diamond-unit-of-work)

## :large_blue_diamond: Query Specification Pattern
El siguiente link nos conducirá al readme de otro de mis proyecto, donde podrá encontrar la información asociada a este punto. Click [aquí](https://github.com/SpaikSaucus/net6-ddd-advanced-example/blob/main/README.es.md#large_blue_diamond-query-specification-pattern)

## :large_blue_diamond: Multiple Environments
El siguiente link nos conducirá al readme de otro de mis proyecto, donde podrá encontrar la información asociada a este punto. Click [aquí](https://github.com/SpaikSaucus/net6-ddd-advanced-example/blob/main/README.es.md#large_blue_diamond-multiple-environments)

## :large_blue_diamond: Unit Test
El siguiente link nos conducirá al readme de otro de mis proyecto, donde podrá encontrar la información asociada a este punto. Click [aquí](https://github.com/SpaikSaucus/net6-ddd-advanced-example/blob/main/README.es.md#large_blue_diamond-unit-test)

## :large_blue_diamond: Integration Test
El siguiente link nos conducirá al readme de otro de mis proyecto, donde podrá encontrar la información asociada a este punto. Click [aquí](https://github.com/SpaikSaucus/net6-ddd-advanced-example/blob/main/README.es.md#large_blue_diamond-integration-test)

## Lectura recomendada:
> * [Aprendiendo Microsoft - DDD con CRQS](https://learn.microsoft.com/es-es/dotnet/architecture/microservices/microservice-ddd-cqrs-patterns/ddd-oriented-microservice)

## Licencia

Tiene licencia bajo [The MIT License](LICENSE).