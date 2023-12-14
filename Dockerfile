# Use the official .NET SDK image as the build environment
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build-env

# Set the working directory inside the container
WORKDIR /app

# Copy the project file and restore dependencies
COPY UserService/*.csproj ./UserService/
RUN dotnet restore ./UserService/UserService.csproj

# Copy the remaining source code
COPY . ./

# Build the application
RUN dotnet publish -c Release -o out

# Use the official .NET runtime image as the final base image
FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS runtime

# Set the working directory inside the container
WORKDIR /app

# Copy the published output from the build environment
COPY --from=build-env /app/out .

# Install SQL Server tools and set up the database
RUN apt-get update \
    && apt-get install -y curl gnupg \
    && curl https://packages.microsoft.com/keys/microsoft.asc | apt-key add - \
    && curl https://packages.microsoft.com/config/debian/10/prod.list > /etc/apt/sources.list.d/mssql-release.list \
    && apt-get update \
    && ACCEPT_EULA=Y apt-get install -y msodbcsql17 mssql-tools unixodbc \
    && apt-get clean \
    && rm -rf /var/lib/apt/lists/

# Set environment variables for SQL Server
ENV SA_PASSWORD=Xtt4d-8HNK
ENV ACCEPT_EULA=Y
ENV MSSQL_DBName=dbi469980_userdb
ENV MSSQL_DBUser=dbi469980
ENV MSSQL_DBPassword=Xtt4d-8HNK

# Configure SQL Server database
COPY UserService/Scripts/InitializeDatabase.sql /docker-entrypoint-initdb.d/

# Specify the entry point for the application
ENTRYPOINT ["dotnet", "UserService.dll"]
