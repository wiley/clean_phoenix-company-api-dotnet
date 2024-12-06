FROM mcr.microsoft.com/dotnet/sdk:6.0-bookworm-slim AS build-env

# ART_USER and ART_PASS referenced in nuget.config as ENV for Artifactory access
# pass from Jenkins as 'docker build --build-arg ART_USER=${ART_USER} --build-arg ART_PASS=${ART_PASS} -t ${CONTAINER} -f "${WORKSPACE}/Dockerfile"' in Jenkins .
ARG ART_USER
ARG ART_PASS
ARG ART_URL

WORKDIR /app
COPY . ./
RUN dotnet nuget add source --name crossknowledge/phoenix $ART_URL --username $ART_USER --password $ART_PASS --store-password-in-clear-text && \
    dotnet restore CompanyAPI

RUN dotnet publish CompanyAPI -c Release -o /app/out --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:6.0-bookworm-slim
WORKDIR /app
COPY --from=build-env /app/out .
ENTRYPOINT ["dotnet","CompanyAPI.dll"]
