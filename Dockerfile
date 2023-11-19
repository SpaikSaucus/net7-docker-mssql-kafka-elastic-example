FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /app

COPY ./*.sln ./
COPY ./src ./src/
COPY ./tests ./tests/

RUN dotnet restore

RUN dotnet publish -c Release -o out --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS runtime
WORKDIR /app
COPY --from=build /app/out ./


#custom installation for curl
#RUN apt-get update 
#RUN apt-get install -y curl


EXPOSE 80

ENTRYPOINT ["dotnet", "UserPermission.API.dll"]