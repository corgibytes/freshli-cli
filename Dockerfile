# NOTE: This file is used to produce a production-ready image. If you're looking
# for a `Dockerfile` for development purposes, please look in the `.devcontainer`
# directory.

### Build `freshli` executable
FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:6.0.400-1-bullseye-slim AS dotnet_build
ARG TARGETARCH

COPY . /app/freshli
WORKDIR /app/freshli

RUN dotnet tool restore
RUN dotnet gitversion /config GitVersion.yml /showconfig
RUN dotnet gitversion /config GitVersion.yml /output json /output buildserver
RUN dotnet gitversion /config GitVersion.yml /updateprojectfiles
RUN set -eux; \
    case "${TARGETARCH}" in \
    aarch64|arm64) \
    DOTNET_RUNTIME_ID='linux-arm64'; \
    ;; \
    armhf|arm) \
    DOTNET_RUNTIME_ID='linux-arm'; \
    ;; \
    amd64|i386:x86-64) \
    DOTNET_RUNTIME_ID='linux-x64'; \
    ;; \
    *) \
    echo "Unsupported arch: ${TARGETARCH}"; \
    exit 1; \
    ;; \
    esac; \
    dotnet build Corgibytes.Freshli.Cli/Corgibytes.Freshli.Cli.csproj -c Release -o exe --self-contained --runtime ${DOTNET_RUNTIME_ID}

### Build `freshli-agent-java` executable
FROM --platform=$BUILDPLATFORM eclipse-temurin:17-jdk-jammy AS java_build

RUN mkdir -p /app
WORKDIR /app

RUN apt update -y && apt install git -y
RUN git clone https://github.com/corgibytes/freshli-agent-java
WORKDIR /app/freshli-agent-java

RUN ./gradlew installDist

### Runtime container
# Use Java JRE as the base image -- the `freshli` executable is self contained (meaning it does not need the .NET runtime to be installed)
FROM eclipse-temurin:17-jre-jammy AS final

# Install libraries required by `freshli`
RUN apt-get -y update \
    && DEBIAN_FRONTEND=noninteractive apt-get install -y --no-install-recommends \
        sqlite3 \
        libsqlite3-dev \
    && echo "en_US.UTF-8 UTF-8" >> /etc/locale.gen \
    && locale-gen en_US.UTF-8 \
    && rm -rf /var/lib/apt/lists/*

# Copy `freshli` executable from the `dotnet_build` image
RUN mkdir -p /usr/local/share/freshli
COPY --from=dotnet_build /app/freshli/exe/ /usr/local/share/freshli/

# Copy `freshli-agent-java` from the `java_build` image
RUN mkdir -p /usr/local/share/freshli-agent-java
COPY --from=java_build /app/freshli-agent-java/build/install/freshli-agent-java/ /usr/local/share/freshli-agent-java/

RUN ln -s /usr/local/share/freshli-agent-java/bin/freshli-agent-java /usr/local/bin/freshli-agent-java

ENV PATH=/usr/local/share/freshli:$PATH

ENTRYPOINT ["/usr/local/share/freshli/freshli"]
CMD ["--help"]
