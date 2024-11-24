FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /app

COPY . .

RUN dotnet restore

RUN dotnet publish -c Release -o out

FROM mcr.microsoft.com/dotnet/aspnet:8.0

WORKDIR /app

COPY --from=build /app/out .

EXPOSE 5000

ENV JWT_SECRET_KEY=$JWT_SECRET_KEY
ENV JWT_ISSUER=$JWT_ISSUER
ENV JWT_AUDIENCE=$JWT_AUDIENCE
ENV JWT_EXPIRATION_IN_MINUTES=$JWT_EXPIRATION

# Run the application
ENTRYPOINT ["dotnet", "Inventory.dll"]