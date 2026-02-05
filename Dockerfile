# ASP.NET Core Alpine 

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-alpine AS build-env
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false
EXPOSE 80

# Move source files to /src folder
COPY . /src

WORKDIR /src
# Build the release executable
RUN dotnet publish "OkrTaskService.Application/OkrTaskService.Application.csproj" -c Release -o /out

# Release image
FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-alpine
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false
RUN apk add icu-libs


# install the agent
#RUN  mkdir /usr/local/newrelic-netcore20-agent \
#&& cd /usr/local \
#&& export NEW_RELIC_DOWNLOAD_URI=https://download.newrelic.com/$(wget -qO - "https://nr-downloads-main.s3.amazonaws.com/?delimiter=/&prefix=dot_net_agent/latest_release/newrelic-netcore20-agent" | grep -E -o 'dot_net_agent/latest_release/newrelic-netcore20-agent_[[:digit:]]{1,3}(\.[[:digit:]]{1,3}){3}_amd64\.tar\.gz') \
#&& echo "Downloading: $NEW_RELIC_DOWNLOAD_URI into $(pwd)" \
#&& wget -O - "$NEW_RELIC_DOWNLOAD_URI" | gzip -dc | tar xf -

# Enable the agent
#ENV CORECLR_ENABLE_PROFILING=1 \
#CORECLR_PROFILER={36032161-FFC0-4B61-B559-F6C5D41BAE5A} \
#CORECLR_NEWRELIC_HOME=/usr/local/newrelic-netcore20-agent \
#CORECLR_PROFILER_PATH=/usr/local/newrelic-netcore20-agent/libNewRelicProfiler.so \
#NEW_RELIC_LICENSE_KEY=c60db198bb8e6047299e007f6aa051c0c82cNRAL \
#NEW_RELIC_APP_NAME=env-name

# Copy binaries from build-env /out to alpine's folder (/app)
COPY --from=build-env /out /app

WORKDIR /app
ENTRYPOINT ["dotnet", "OkrTaskService.Application.dll"]