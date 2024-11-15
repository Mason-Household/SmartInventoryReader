# Use the official .NET 8 SDK image as a build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

# Set the working directory
WORKDIR /app

# Copy the project files
COPY . .

# Restore the dependencies
RUN dotnet restore

# Build the project
RUN dotnet publish -c Release -o out

# Use the official .NET 8 runtime image as a runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0

# Set the working directory
WORKDIR /app

# Copy the build output from the build stage
COPY --from=build /app/out .

# Expose the port the app runs on
EXPOSE 80

# Set environment variables for MongoDB connection
ENV MONGO_CONNECTION_STRING=mongodb://mongo:27017
ENV MONGO_DATABASE_NAME=YourDatabaseName

# Set environment variables for JWT
ENV JWT_SECRET_KEY=YourSecretKey
ENV JWT_ISSUER=YourIssuer
ENV JWT_AUDIENCE=YourAudience

# Run the application
ENTRYPOINT ["dotnet", "YourProjectName.dll"]