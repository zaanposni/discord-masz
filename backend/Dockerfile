FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env
WORKDIR /app

# Copy everything and build
COPY ./ ./
RUN dotnet publish -c Release -o ./bin

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app/bin
COPY --from=build-env /app/bin/ .

# Set timezone
ENV TZ=Europe/Berlin
RUN ln -snf /usr/share/zoneinfo/$TZ /etc/localtime && echo $TZ > /etc/timezone

EXPOSE 8080

USER app

ENTRYPOINT ["dotnet", "MASZ.dll"]