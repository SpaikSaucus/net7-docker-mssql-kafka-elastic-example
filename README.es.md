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

En este apartado explicaremos la estructura del proyecto, como están diseñadas las capas, que función cumple cada una. También haremos mención sobre algunas carpetas importantes.

### 1- ENTRYPOINT
Web Api, Listeners de queues, workers de un job, etc. No debe implementar reglas de negocio. 

  * #### API
    Recibe los request y se los delega a [MediatR](https://github.com/jbogard/MediatR). Se implementa el uso de Mediatr para el desacople entre capas y asi implementar una arquitectura CQRS entre lo transaccional y consultivo. 

    * __Controllers__: contendrá las distintas versiones de los controladores. Para mas información sobre API Version ir [aquí](#api-versions).
    * __ViewModels__: contendrá las clases Request y Response, necesarias para la interacción.

### 2- CORE
Principal objetivo, implementar lógica de negocio y casos de uso. Debemos velar para que no existan referencias a Frameworks asociados a Infraestructura (ejemplo: frameworks de accesos a datos).

  * #### APPLICATION
    Implementaremos el flujo para desarrollar una funcionalidad especifica de mi aplicación. Orquestara entre los diferentes Dominios.

    * __Behaviors__: contendrá comportamiento reutilizable por MediatR en cada interacción de la API con la capa de APPLICATION. 
    Ejemplos:
      * ___LoggingBehavior___: Se logueara cada request y response que procese MediatR. 
      * ___ValidatorBehavior___: Ejecuta los Validators que estén asociados a un MediatR.

    * __UserCases__: contendrá los Casos de Uso que se definan para cumplir con los requerimientos solicitados. Un UserCase puede hacer uso de otro UserCase, para reutilizar código (ejemplo: revisar el UserCase de CreatePermissionCommand).
      * ___Commands___: contendrá las implementaciones transaccionales.
      * ___Queries___: contendrá las implementaciones consultivas.
      * ___Validations___: contendrá la lógica de validación a los datos de entrada.
      * ___DTOs___: contendrá las clases DTO necesarias para transferir información entre capas, cuando sea necesario.

  * #### DOMAIN
    Definiremos las reglas de negocio (en términos de DDD, también llamadas Dominio y expresadas a traves de entidades, servicios de Dominio, value objects, interfaces).

    * __Core__: contiene clases Base e Interfaces que serán necesarias para implementar patrones y features críticos. 

### 3- INFRASTRUCTURE
Aquí encontraremos implementaciones concretas para acceso a datos, ORMs, MicroORMS, Request HTTP, Manejo de archivos, etc.

  * #### INFRASTRUCTURE

    * __Core__: contiene algunas de las implementaciones de las Interfaces definidas en la capa de DOMAIN, asociadas a resolver problemas de Infraestructura.
    * __EF__: contiene el DBContext y las clases necesarias para implementar Entity Framework, para el acceso a la base de datos.
    * __Services__: contiene implementaciones concretas, por ejemplo, generar un Reporte en formato Excel. Las misas deberá configurarse en el AutofacModules para luego poder utilizarse mediante la inyección de IComponentContext.

  * #### INFRASTRUCTURE.BOOSTRAP

    * __AutofacModules__: contiene los módulos que nosotros definamos, los cuales se utilizaran para registrar los componentes que se podrán crear con reflection. De esta forma, podremos utilizar los servicios que generemos en la capa _Infrastructure_ en la capa _Application_. Se desaconseja el uso de dichos servicios en la capa _Domain_ debido a que la misma debe estar los mas aislada posible.
    * __Extensions__: contiene las configuraciones necesarias para la iniciación de nuestra aplicación de forma segregada para mejorar su comprensión y descubrimiento, entre otras cosas.

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

El objetivo principal de aplicar DDD o Domain Driven Design en inglés, es poder aislar el código que pertenece al dominio de los detalles técnicos de implementación y así centrarnos en la complejidad del negocio.

### Principios centrales
Podríamos decir que la orientación al dominio se centra en tres pilares básicos:
  * Focalizar en el dominio central y la lógica de negocio.
  * Convertir diseños complejos en modelos de dominio.
  * Constante interacción y colaboración con los expertos de dominio, lo que ayudará a solventar dudas e interactuar más con el equipo de desarrollo.

A su vez, cuando trabajamos con DDD debemos tener en cuenta:
  * Separación de responsabilidades en capas, _(aislar el dominio)_.
  * Modelar y definir el modelo.
  * Gestionar el ciclo de vida de los objetos de Dominio.


### Las diferentes capas son:

* __Capa de dominio:__ 
  Responsable de representar conceptos del negocio, información sobre la situación del negocio y reglas de negocios. El estado que refleja la situación empresarial está controlado y se usa aquí, aunque los detalles técnicos de su almacenaje se delegan a la infraestructura. Este nivel es el núcleo del software empresarial, donde se expresa el negocio, en. NET, _se codifica como una biblioteca de clases_, con las entidades de dominio que capturan datos y comportamiento (métodos con lógica).
  <br/>A su vez, esta biblioteca solo tiene dependencias a las bibliotecas de .NET, pero no a otras bibliotecas personalizadas, como por ejemplo de datos o de persistencia. No debe depender de ningún otro nivel (las clases del modelo de dominio deben ser clases de objetos CLR o POCO).
  <br/>

* __Capa de aplicación:__ 
  Define los trabajos que se supone que el software debe hacer y dirige los objetos de dominio para que resuelvan problemas. Las tareas que son responsabilidad de este nivel son significativas para la empresa o necesarias para la interacción con los niveles de aplicación de otros sistemas.
  <br/>Este nivel debe mantenerse estrecho. No contiene reglas de negocios ni conocimientos, sino que solo coordina tareas y delega trabajo a colaboraciones de objetos de dominio en el siguiente nivel. No tiene ningún estado que refleje la situación empresarial, pero puede tener un estado que refleje el progreso de una tarea para el usuario o el programa.
  <br/>Normalmente, el nivel de aplicación en microservicios .NET se codifica como un proyecto de ASP.NET Core Web API. El proyecto implementa la interacción del microservicio, el acceso a redes remotas y las API web externas utilizadas desde aplicaciones cliente o de interfaz de usuario. Incluye consultas si se utiliza un enfoque de CQRS, comandos aceptados por el microservicio e incluso comunicación guiada por eventos entre microservicios (eventos de integración).      
  <br/>Básicamente, la lógica de la aplicación es el lugar en el que se implementan todos los casos de uso que dependen de un front-end determinado.
  
  <br/>En este ejemplo "_UserPermission_", esta capa se divide para mejorar el enfoque del diseño, dando lugar a los siguientes dos proyectos:
  * 1- ENTRYPOINT :arrow_right: __API__
  * 2- CORE  :arrow_right: __Application__
	<br/>

* __Capa de infraestructura:__
  Es en donde reside la parte técnica de la aplicación, con sus implementaciones concretas y donde se añadirán las dependencias a software de terceros para cumplir con integraciones, base de datos, manejo de archivos, etc.

  <br/>En este ejemplo "_UserPermission_", esta capa se divide para mejorar el enfoque del diseño, dando lugar a los siguientes dos proyectos:
  * 3- INFRASTRUCTURE :arrow_right: __Infrastructure__
  * 3- INFRASTRUCTURE :arrow_right: __Infrastructure.Bootstrap__
	<br/>

![ddd_1_es](https://github.com/SpaikSaucus/net7-docker-mssql-kafka-elastic-example/blob/main/readme-img/ddd_1_es.png?raw=true)
![ddd_2_es](https://github.com/SpaikSaucus/net7-docker-mssql-kafka-elastic-example/blob/main/readme-img/ddd_2_es.png?raw=true)

### Referencias :triangular_flag_on_post:
> * [Aprendiendo Microsoft DDD](https://learn.microsoft.com/es-es/dotnet/architecture/microservices/microservice-ddd-cqrs-patterns/ddd-oriented-microservice)
> * [Introducción DDD](https://refactorizando.com/introduccion-domain-drive-design/)

## :large_blue_diamond: Docker

Docker es un proyecto de código abierto que automatiza el despliegue de aplicaciones dentro de contenedores de software, proporcionando una capa adicional de abstracción y automatización de virtualización de aplicaciones en múltiples sistemas operativos.

Construido sobre las facilidades proporcionadas por el kernel Linux (principalmente cgroups y namespaces), un contenedor Docker, a diferencia de una máquina virtual, no requiere incluir un sistema operativo independiente. En su lugar, se basa en las funcionalidades del kernel y utiliza el aislamiento de recursos (CPU, la memoria, E/S, red, etc.) y namespaces separados para aislar la vista de una aplicación del sistema operativo. Por lo tanto, los contenedores tienen una superficie significativamente menor que las imágenes de máquina virtual (VM).

Contenedores múltiples comparten el mismo núcleo, pero cada contenedor puede ser restringido a utilizar solo una cantidad definida de recursos como CPU, memoria y E/S.

Usar Docker para crear y gestionar contenedores puede simplificar la creación de sistemas altamente distribuidos. Esto permite que el despliegue de nodos se realice a medida que se dispone de recursos o cuando se necesiten más nodos, lo que permite una plataforma como servicio (PaaS - Platform as a Service) de estilo de despliegue.

> [!NOTE]
> Docker es también una empresa que promueve e impulsa esta tecnología, en colaboración con proveedores de la nube, Linux y Windows, incluido Microsoft.

### Comparando Contenedores de Docker con Virtual Machines

| Virtual Machines | Contenedores de Docker |
| :-------------: | :-------------: |
| ![vm](https://github.com/SpaikSaucus/net7-docker-mssql-kafka-elastic-example/blob/main/img/virtual-machine-hardware-software.png?raw=true) | ![dc](https://github.com/SpaikSaucus/net7-docker-mssql-kafka-elastic-example/blob/main/img/docker-container-hardware-software.png?raw=true) |
| Las máquinas virtuales incluyen la aplicación, las bibliotecas o los archivos binarios necesarios y un sistema operativo invitado completo. La virtualización completa requiere más recursos que la inclusión en contenedores. | Los contenedores incluyen la aplicación y todas sus dependencias. Sin embargo, comparten el kernel del sistema operativo con otros contenedores, que se ejecutan como procesos aislados en el espacio de usuario en el sistema operativo host. (Excepto en los contenedores de Hyper-V, en que cada contenedor se ejecuta dentro de una máquina virtual especial por contenedor). |

### Analogía
Del mismo modo que los contenedores de mercancías permiten su transporte por barco, tren o camión independientemente de la carga de su interior, los contenedores de software actúan como una unidad estándar de implementación de software que puede contener diferentes dependencias y código. De esta manera, la inclusión del software en contenedor permite a los desarrolladores y los profesionales de TI implementarlo en entornos __con pocas modificaciones o ninguna en absoluto__.

### Resumen
Los contenedores ofrecen las ventajas del aislamiento, la portabilidad, la agilidad, la escalabilidad y el control a lo largo de todo el flujo de trabajo del ciclo de vida de la aplicación. La ventaja más importante es el aislamiento del entorno que se proporciona entre el desarrollo y las operaciones.

### Referencias :triangular_flag_on_post:
> * [Aprendiendo Microsoft - Container Docker Introducción](https://learn.microsoft.com/es-es/dotnet/architecture/microservices/container-docker-introduction/)
> * [Assign static ip container](https://www.baeldung.com/ops/docker-assign-static-ip-container)
> * [Docker wiki](https://es.wikipedia.org/wiki/Docker_(software))

## :large_blue_diamond: Elasticsearch

Elasticsearch es un servidor de búsqueda basado en Apache Lucene. Provee un motor de búsqueda de texto completo, distribuido y con capacidad de multitenencia con una interfaz web RESTful y con documentos JSON.

### Algunos Casos de Uso
* __Monitoreo de logs__: Logging rápido y escalable que no se detendrá.
* __Monitoreo de infraestructura__: Controla y visualiza las métricas de tu sistema.
* __APM__: Obtén información sobre el rendimiento de tu aplicación.
* __Monitoreo sintético__: Monitorea y reacciona a problemas de disponibilidad.
* __Enterprise Search__: Experiencias de búsqueda y descubrimiento para cualquier caso de uso.
* __Maps__: Explora datos de ubicación en tiempo real.
* __SIEM__: Investigación interactiva y detección automatizada de amenazas.
* __Seguridad de endpoint__: Prevé, detecta, busca y responde a amenazas.

### Referencias :triangular_flag_on_post:
> * [Elasticsearch wiki](https://es.wikipedia.org/wiki/Elasticsearch)
> * [Elasticsearch in Docker](https://www.elastic.co/guide/en/elasticsearch/reference/current/docker.html)
> * [Elasticsearch client Net](https://www.elastic.co/guide/en/elasticsearch/client/net-api/7.17/configuration-options.html)
> * [Elasticsearch ejemplo integración Net](https://www.c-sharpcorner.com/article/how-to-integrate-elasticsearch-in-asp-net-core/)

## :large_blue_diamond: Kafka

Apache Kafka es una plataforma de transmisión de eventos distribuida de código abierto utilizada para canalizaciones de datos de alto rendimiento, análisis de transmisión, integración de datos y aplicaciones de misión crítica.

### Características Básicas
* __Alto rendimiento__: Entregue mensajes con un rendimiento limitado de la red utilizando un grupo de máquinas con latencias tan bajas como 2 ms.
* __Escalable__: Escale los clústeres de producción hasta mil intermediarios, billones de mensajes por día, petabytes de datos, cientos de miles de particiones. Ampliar y contraer elásticamente el almacenamiento y procesamiento.
* __Almacenamiento permanente__: Almacene flujos de datos de forma segura en un clúster distribuido, duradero y tolerante a fallos.
* __Alta disponibilidad__: Extienda los clústeres de manera eficiente en zonas de disponibilidad o conecte clústeres separados entre regiones geográficas.

### Algunos Casos de Uso
La transmisión de eventos se aplica a una amplia variedad de casos de uso en una gran cantidad de industrias y organizaciones. Sus muchos ejemplos incluyen:

* Para procesar pagos y transacciones financieras en tiempo real, como en bolsas de valores, bancos y seguros.
* Para rastrear y monitorear automóviles, camiones, flotas y envíos en tiempo real, como en la logística y la industria automotriz.
* Capturar y analizar continuamente datos de sensores de dispositivos IoT u otros equipos, como en fábricas y parques eólicos.
* Recopilar y reaccionar inmediatamente a las interacciones y pedidos de los clientes, como en el comercio minorista, la industria hotelera y de viajes, y las aplicaciones móviles.
* Monitorear a los pacientes en atención hospitalaria y predecir cambios en su condición para asegurar el tratamiento oportuno en emergencias.
* Conectar, almacenar y poner a disposición datos producidos por diferentes divisiones de una empresa.
* Servir como base para plataformas de datos, arquitecturas basadas en eventos y microservicios.

<image style="width: 90%;" src="https://github.com/SpaikSaucus/cheatsheets/blob/main/Queues/Kafka/Kafka_Top_5_use_cases.gif?raw=true"> </image> 

### Cómo funciona?

Funciona mediante la publicación y suscripción de mensajes a través de temas (topics). Los productores generan mensajes y los envían a un topic, mientras que los consumidores se suscriben a esos topics para procesar los mensajes en tiempo real.

### Referencias :triangular_flag_on_post:
> * [Web oficial](https://kafka.apache.org/)
> * [Introducción](https://kafka.apache.org/documentation/#gettingStarted)
> * [Kafka tool net](https://github.com/confluentinc/confluent-kafka-dotnet/)
> * [Cheatsheet - Kafka](https://github.com/SpaikSaucus/cheatsheets/tree/main/Queues/Kafka)

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
> * [Aprendiendo Microsoft - Swashbuckle](https://learn.microsoft.com/es-es/aspnet/core/tutorials/getting-started-with-swashbuckle?view=aspnetcore-7.0&tabs=visual-studio)
> * [Blog API versioning and integrate Swagger](https://blog.christian-schou.dk/how-to-use-api-versioning-in-net-core-web-api/)
  
## :large_blue_diamond: MediatR + CQRS

### MediatR
Es una biblioteca de código abierto, pequeña y simple, que implementa el patrón de mediador, para la canalización de mensajes (comandos) y enrutandolos, en memoria, a los controladores de comandos correctos.

El uso del patrón de mediador ayuda a reducir el acoplamiento y aislar el procesamiento del comando solicitado, del resto del código.

### CQRS
CQRS en ingles significa (Command Query Responsibility Segregation), el cual es un patron que busca tener dos objetos separados, uno para operaciones de lectura y otro para operaciones de escritura, a diferencia de otros enfoques que buscan tener todo en uno solo.

### Combinándolos
En este ejemplo "_UserPermission_", combinamos el patron mediador con el patron CQRS, el resultado implica la creación de comandos para consultas y comandos para cambiar el estado del sistema.

  * Consultas: Estas consultas devuelven un resultado sin cambiar el estado del sistema y no tienen efectos secundarios.
    * __Application__ :arrow_right: UserCases :arrow_right: FindOne :arrow_right: Queries
    <br/>  
  * Comandos: Estos comandos cambian el estado de un sistema.
    * __Application__ :arrow_right: UserCases :arrow_right: Create :arrow_right: Commands

### Referencias :triangular_flag_on_post:
> * [CQRS web-api command process pipeline with a mediator pattern MediatR](https://learn.microsoft.com/es-es/dotnet/architecture/microservices/microservice-ddd-cqrs-patterns/microservice-application-layer-implementation-web-api#implement-the-command-process-pipeline-with-a-mediator-pattern-mediatr)
> * [Aprendiendo Microsoft CRQS Pattern in DDD](https://learn.microsoft.com/es-es/dotnet/architecture/microservices/microservice-ddd-cqrs-patterns/apply-simplified-microservice-cqrs-ddd-patterns)


## :large_blue_diamond: Health Check

Una aplicación se encarga de exponer las comprobaciones de estado como puntos de conexión HTTP, donde normalmente, las comprobaciones de estado se usan con un servicio de supervisión externa o un orquestador de contenedores para comprobar el estado de una aplicación. 

Antes de agregar comprobaciones de estado a una aplicación, debe decidir en qué sistema de supervisión se va a usar. El sistema de supervisión determina qué tipos de comprobaciones de estado se deben crear y cómo configurar sus puntos de conexión.

Para ello utilizamos la biblioteca:
  * Microsoft.AspNetCore.Diagnostics.HealthChecks

Dicha configuración se puede encontrar en:
  * __Infrastructure.Bootstrap__ :arrow_right: Extensions   
    * ApplicationBuilder :arrow_right: HealthChecksApplicationBuilderExtensions    
    y en:
    * ServiceCollections :arrow_right: HealthChecksServiceCollectionExtensions
    
Y podemos ingresar al endpoint __"/health"__ para comprobar su funcionamiento.
  * https://localhost:5001/health

### Referencias :triangular_flag_on_post:
> * [Aprendiendo Microsoft Health Checks](https://learn.microsoft.com/es-es/aspnet/core/host-and-deploy/health-checks?view=aspnetcore-7.0)

## :large_blue_diamond: Logs

Para poder obtener información y registrar errores producidos en nuestra aplicación, utilizaremos la librería Serilog, la cual nos facilita la implementación de esta característica muy util para el diagnostico.

### Utilización
Definir en nuestra clase una variable para almacenar el logger:
```csharp
private readonly ILogger<MyClass> logger;
```
Inyectar en el constructor el logger:
```csharp
public MyClass(ILogger<MyClass> logger)
{
  this.logger = logger;
}
```
Hacer uso del logger, ejemplos:
```csharp
this.logger.Log(LogLevel.Information, "Permission {0} already exists", permission.Id);
...
this.logger.LogInformation("Permission {0} already exists", permission.Id);
...
this.logger.LogError("API Error: {api}: \n{result}",
  apiException.RequestMessage.RequestUri, 
  apiException.Content);
```

### Configuración
La configuración del Serilog se encuentra en la siguiente clase: 
  * __API__ :arrow_right: Program.cs    

Y en el archivo appsettings.[Environment].json:

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

#### Nivel de prioridad

* _Verbose_ :arrow_right: _Debug_ :arrow_right: _Information_ :arrow_right: _Warning_ :arrow_right: _Error_ :arrow_right: _Fatal_

Ejemplo, si indicamos el nivel "Information", los Logs de nivel "Verbose" y "Debug" no se visualizaran.


### Referencias :triangular_flag_on_post:
> * [Serilog Web](https://serilog.net/)
> * [Serilog Tutorial](https://stackify.com/serilog-tutorial-net-logging/)

## :large_blue_diamond: Unit Of Work

Es un patron que tiene como propósito asegurarse de que se comparta un mismo contexto de base de datos, de modo que cuando se completan las tareas a realizar en la base de datos, se pueda llamar al SaveChanges, método en esa instancia del contexto y asegurarse de que todos los cambios relacionados se coordinarán. 

Ejemplo:

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

### Referencias :triangular_flag_on_post:
> * [Aprendiendo Microsoft Unit Of Work Pattern](https://learn.microsoft.com/es-es/aspnet/mvc/overview/older-versions/getting-started-with-ef-5-using-mvc-4/implementing-the-repository-and-unit-of-work-patterns-in-an-asp-net-mvc-application#creating-the-unit-of-work-class)
> * [Martin Fower Unit Of Work](https://martinfowler.com/eaaCatalog/unitOfWork.html)

## :large_blue_diamond: Query Specification Pattern

Es un patron que busca cumplir con DDD para la consulta de datos de manera que dichas especificaciones se almacenen en la capa __Domain__, separando de manera efectiva la lógica que existe en las consultas de su implementación.

Para ello se genero en la capa __Domain__ la clase base _BaseSpecification_ y la interface _ISpecification_. En la capa __Infrastructure__ existe la clase _SpecificationEvaluator_ que es utilizada por la clase _Repository_ para aplicar la especificación a utilizar.

Ejemplo:
La clase _PermissionGetSpecification_ que se encuentra en la carpeta
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
Dicha especificación es utilizada en la clase _PermissionGetQuery_ que se encuentra en la carpeta
  * __Application__ :arrow_right: UserCases :arrow_right: FindOne :arrow_right: Queries

```csharp
var spec = new PermissionGetSpecification(permission);
var result = this.unitOfWork.Repository<Permission>().Find(spec).FirstOrDefault();
return Task.FromResult(result);
```

### Referencias :triangular_flag_on_post:
> * [Aprendiendo Microsoft Query Specification Pattern in DDD](https://learn.microsoft.com/es-es/dotnet/architecture/microservices/microservice-ddd-cqrs-patterns/infrastructure-persistence-layer-implementation-entity-framework-core#implement-the-query-specification-pattern)
> * [Medium Specification Pattern Generic Repository](https://medium.com/@rudyzio92/net-core-using-the-specification-pattern-alongside-a-generic-repository-318cd4eea4aa)

## :large_blue_diamond: Multiple Environments
Crear el json con el siguiente nombre:
  * appsettings.__environment__.json

Ejemplos:

![img_hierarchy_1](https://github.com/SpaikSaucus/net7-docker-mssql-kafka-elastic-example/blob/main/readme-img/multiple_environments_1.png?raw=true)

![img_hierarchy_2](https://github.com/SpaikSaucus/net7-docker-mssql-kafka-elastic-example/blob/main/readme-img/multiple_environments_2.png?raw=true)	

### Referencias :triangular_flag_on_post:
> * [Aprendiendo Microsoft Environments](https://learn.microsoft.com/es-es/aspnet/core/fundamentals/environments?view=aspnetcore-7.0)

## :large_blue_diamond: Unit Test
xUnit: Estos test están escritos mediante XUnit y utilizando las siguientes bibliotecas FluentAssertions y FakeItEasy.

### Referencias :triangular_flag_on_post:
> * [Fluent Assertions Web](https://fluentassertions.com/)
> * [Fake It Easy Web](https://fakeiteasy.readthedocs.io/en/stable/)
> * [Blog NUnit vs xUnit vs MSTest](https://www.lambdatest.com/blog/nunit-vs-xunit-vs-mstest/)
> * [Aprendiendo Microsoft Unit Testing (mejores practicas)](https://learn.microsoft.com/es-es/dotnet/core/testing/unit-testing-best-practices)

## :large_blue_diamond: Integration Test
Microsoft.AspNetCore.TestHost - Estos Test nos ayudan a poder realizar una prueba de integración de nuestra APP. El objetivo del mismo es poder levantar el middleware de Net Core con todas las configuraciones.

### Referencias :triangular_flag_on_post:
> * [Aprendiendo Microsoft Integration Tests](https://learn.microsoft.com/es-es/aspnet/core/test/integration-tests?view=aspnetcore-7.0)

## Lectura recomendada:
> * [Kafka Introducción](https://kafka.apache.org/documentation/#gettingStarted)
> * [Aprendiendo Microsoft - DDD con CRQS](https://learn.microsoft.com/es-es/dotnet/architecture/microservices/microservice-ddd-cqrs-patterns/ddd-oriented-microservice)

## Licencia

Tiene licencia bajo [The MIT License](LICENSE).