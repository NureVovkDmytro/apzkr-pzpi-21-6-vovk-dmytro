# Use the official ASP.NET runtime as a parent image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

# Use the official .NET SDK as a build image
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["Discerniy.API/Discerniy.API.csproj", "Discerniy.API/"]
COPY ["Discerniy.Domain/Discerniy.Domain.csproj", "Discerniy.Domain/"]
COPY ["Discerniy.Infrastructure/Discerniy.Infrastructure.csproj", "Discerniy.Infrastructure/"]

# Restore dependencies
RUN dotnet restore "Discerniy.API/Discerniy.API.csproj"

# Copy the rest of the source code and build the project
COPY . .
WORKDIR "/src/Discerniy.API"
RUN dotnet build "Discerniy.API.csproj" -c Release -o /app/build

# Publish the application
FROM build AS publish
RUN dotnet publish "Discerniy.API.csproj" -c Release -o /app/publish

# Final stage: Copy the published application to the runtime image
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Discerniy.API.dll"]
