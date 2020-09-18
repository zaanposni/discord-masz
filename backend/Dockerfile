FROM python:3 as init
WORKDIR /app

COPY setup_app.py .
COPY config.json .
COPY ./masz/appsettings.json ./masz/

RUN python3 setup_app.py

FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build-env
WORKDIR /app

# Copy everything and build
COPY ./ ./
COPY --from=init /app/masz/appsettings.json /app/masz/
RUN dotnet publish -c Release -o ./bin

# Build runtime image
FROM mcr.microsoft.com/dotnet/core/aspnet:3.1
WORKDIR /app/bin
COPY --from=build-env /app/bin/ .

ENTRYPOINT ["dotnet", "masz.dll"]
EXPOSE 80