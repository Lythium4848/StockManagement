# Use the .NET 9.0 SDK image to build and publish the application
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build

# Set the working directory inside the container
WORKDIR /app

# Copy csproj files to restore dependencies
COPY *.sln ./
COPY Backend/*.csproj ./Backend/

# Restore the dependencies
RUN dotnet restore

# Copy the rest of the application files
COPY Backend/. ./Backend/

# Set the working directory to the project folder
WORKDIR /app/Backend

# Build the project
RUN dotnet build --no-restore -c Release

# Publish the application
RUN dotnet publish --no-restore -c Release -o /publish

# Use the ASP.NET Core runtime image for running the application
FROM mcr.microsoft.com/dotnet/aspnet:9.0

# Set the working directory for runtime
WORKDIR /app

# Copy the published files to the runtime container
COPY --from=build /publish .

# Expose the port your application will run on (e.g., 5000)
EXPOSE 5000

# Start the application
ENTRYPOINT ["dotnet", "Backend.dll"]
