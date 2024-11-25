FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /app

COPY . .

RUN dotnet restore
RUN dotnet publish -c Release -o out

FROM mcr.microsoft.com/dotnet/aspnet:8.0

WORKDIR /app

COPY --from=build /app/out .

EXPOSE 8080

ENV JWT_SECRET_KEY=$JWT_SECRET_KEY
ENV JWT_ISSUER=$JWT_ISSUER
ENV JWT_AUDIENCE=$JWT_AUDIENCE
ENV JWT_EXPIRATION_IN_MINUTES=$JWT_EXPIRATION

COPY firebaseKey.json /app/firebaseKey.json
ENV GOOGLE_APPLICATION_CREDENTIALS=/app/firebaseKey.json

# Run the application
ENTRYPOINT ["dotnet", "Inventory.dll"]