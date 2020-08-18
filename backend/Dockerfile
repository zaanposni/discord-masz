FROM python:3 as init
ARG MYSQL_HOST
ARG MYSQL_PORT
ARG MYSQL_DATABASE
ARG MYSQL_USER
ARG MYSQL_PASS
WORKDIR /app

COPY setup_app.py .
COPY ./masz/appsettings.json ./masz/

RUN python3 setup_app.py $MYSQL_HOST $MYSQL_PORT $MYSQL_DATABASE $MYSQL_USER $MYSQL_PASS

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