FROM python:3 as init

ARG MYSQL_DATABASE
ENV MYSQL_DATABASE $MYSQL_DATABASE

ARG MYSQL_USER
ENV MYSQL_USER $MYSQL_USER

ARG MYSQL_PASSWORD
ENV MYSQL_PASSWORD $MYSQL_PASSWORD

ARG MYSQL_PORT
ENV MYSQL_PORT $MYSQL_PORT

ARG MYSQL_HOST
ENV MYSQL_HOST $MYSQL_HOST

ARG DISCORD_BOT_TOKEN
ENV DISCORD_BOT_TOKEN $DISCORD_BOT_TOKEN

ARG DISCORD_OAUTH_CLIENT_ID
ENV DISCORD_OAUTH_CLIENT_ID $DISCORD_OAUTH_CLIENT_ID

ARG DISCORD_OAUTH_CLIENT_SECRET
ENV DISCORD_OAUTH_CLIENT_SECRET $DISCORD_OAUTH_CLIENT_SECRET

ARG DISCORD_SITE_ADMINS
ENV DISCORD_SITE_ADMINS $DISCORD_SITE_ADMINS

ARG META_SERVICE_BASE_URL
ENV META_SERVICE_BASE_URL $META_SERVICE_BASE_URL

WORKDIR /app

COPY setup_app.py .
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

# Set timezone
ENV TZ=Europe/Berlin
RUN ln -snf /usr/share/zoneinfo/$TZ /etc/localtime && echo $TZ > /etc/timezone

ENTRYPOINT ["dotnet", "masz.dll"]
EXPOSE 80