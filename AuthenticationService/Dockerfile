FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /service
EXPOSE 5050
ENV ASPNETCORE_URLS=http://+:5050 

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["AuthenticationService/AuthenticationService.csproj", "AuthenticationService/"]
RUN dotnet restore "AuthenticationService/AuthenticationService.csproj"
COPY . ./
WORKDIR AuthenticationService
RUN dotnet build "AuthenticationService.csproj" -c Release -o /service/build

FROM build AS publish
RUN dotnet publish "AuthenticationService.csproj" -c Release -o /service/publish

FROM base AS service
WORKDIR /service
COPY --from=publish /service/publish .
ADD ["AuthenticationService/Schemas/", "Schemas/"]
ENTRYPOINT ["dotnet", "AuthenticationService.dll"]
