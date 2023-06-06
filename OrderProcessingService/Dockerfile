FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /service
EXPOSE 5080
ENV ASPNETCORE_URLS=http://+:5080 

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["OrderProcessingService/OrderProcessingService.csproj", "OrderProcessingService/"]
RUN dotnet restore "OrderProcessingService/OrderProcessingService.csproj"
COPY . ./
WORKDIR OrderProcessingService
RUN dotnet build "OrderProcessingService.csproj" -c Release -o /service/build

FROM build AS publish
RUN dotnet publish "OrderProcessingService.csproj" -c Release -o /service/publish

FROM base AS service
WORKDIR /service
COPY --from=publish /service/publish .
ADD ["OrderProcessingService/Schemas/", "Schemas/"]
ENTRYPOINT ["dotnet", "OrderProcessingService.dll"]
