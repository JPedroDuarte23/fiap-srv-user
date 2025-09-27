FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /app

COPY FiapSrvUser.sln .

COPY FiapSrvUser.API/*.csproj ./FiapSrvUser.API/
COPY FiapSrvUser.Application/*.csproj ./FiapSrvUser.Application/
COPY FiapSrvUser.Domain/*.csproj ./FiapSrvUser.Domain/
COPY FiapSrvUser.Infrastructure/*.csproj ./FiapSrvUser.Infrastructure/
COPY FiapSrvUser.Test/*.csproj ./FiapSrvUser.Test/

RUN dotnet restore

COPY FiapSrvUser.API/ ./FiapSrvUser.API/
COPY FiapSrvUser.Application/ ./FiapSrvUser.Application/
COPY FiapSrvUser.Domain/ ./FiapSrvUser.Domain/
COPY FiapSrvUser.Infrastructure/ ./FiapSrvUser.Infrastructure/
COPY FiapSrvUser.Test/ ./FiapSrvUser.Test/

RUN dotnet publish FiapSrvUser.API/FiapSrvUser.API.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime

WORKDIR /app

# Install the agent
RUN apt-get update && apt-get install -y wget ca-certificates gnupg \
&& echo 'deb [signed-by=/usr/share/keyrings/newrelic-apt.gpg] http://apt.newrelic.com/debian/ newrelic non-free' | tee /etc/apt/sources.list.d/newrelic.list \
&& wget -O- https://download.newrelic.com/NEWRELIC_APT_2DAD550E.public | gpg --import --batch --no-default-keyring --keyring /usr/share/keyrings/newrelic-apt.gpg \
&& apt-get update \
&& apt-get install -y newrelic-dotnet-agent

# Enable the agent
ENV CORECLR_ENABLE_PROFILING=1 \
CORECLR_PROFILER={36032161-FFC0-4B61-B559-F6C5D41BAE5A} \
CORECLR_NEWRELIC_HOME=/usr/local/newrelic-dotnet-agent \
CORECLR_PROFILER_PATH=/usr/local/newrelic-dotnet-agent/libNewRelicProfiler.so

RUN adduser --disabled-password --no-create-home appuser

RUN chown -R appuser:appuser /usr/local/newrelic-dotnet-agent

USER appuser

COPY --from=build /app/publish .

EXPOSE 8080

ENTRYPOINT ["dotnet", "FiapSrvUser.API.dll"]
