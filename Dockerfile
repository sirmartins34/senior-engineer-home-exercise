#  ----------- Build Stage -----------
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

RUN echo "Files in /app:" && ls -la /app

# Copy csproj and restore
COPY People.sln ./
COPY People.Api/People.Api.csproj ./People.Api/
# Copy all referenced library projects
COPY People.Data/People.Data.csproj ./People.Data/
COPY People.Tests/People.Tests.csproj ./People.Tests/

RUN dotnet restore ./People.sln

# Copy everything else and publish
COPY . ./
RUN dotnet publish People.Api/People.Api.csproj -c Release -o /app/publish \
    -r linux-musl-x64 --self-contained true \
#    /p:PublishTrimmed=true \
    /p:EnableCompressionInSingleFile=true \
    /p:PublishSingleFile=true

# Runtime Stage — Microsoft Alpine-based
FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine

WORKDIR /app
COPY --from=build /app/publish .

# Add execute permission just in case
RUN chmod +x People.Api

# Use a safe, unprivileged built-in user
USER 10001

RUN echo "Files in /app:" && ls -la /app

# Set environment
ENV DOTNET_RUNNING_IN_CONTAINER=true \
    DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=true \
    ASPNETCORE_URLS=http://0.0.0.0:8080

EXPOSE 8080

ENTRYPOINT ["./People.Api"]
