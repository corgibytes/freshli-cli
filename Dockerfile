# NOTE: This file is used to produce a production-ready image. If you're looking
# for a `Dockerfile` for development purposes, please look in the `.devcontainer`
# directory.

### Build `freshli` executable
FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:7.0.100-bullseye-slim AS dotnet_build
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
FROM eclipse-temurin:17-jdk-jammy AS final

# Install git
RUN apt update -y && apt install git -y

# Install maven
# Copied from https://github.com/carlossg/docker-maven/blob/d2333e08a71fe120a0ac245157906e9b3507cee3/eclipse-temurin-17/Dockerfile
ARG MAVEN_VERSION=3.8.6
ARG USER_HOME_DIR="/root"
ARG SHA=f790857f3b1f90ae8d16281f902c689e4f136ebe584aba45e4b1fa66c80cba826d3e0e52fdd04ed44b4c66f6d3fe3584a057c26dfcac544a60b301e6d0f91c26
ARG BASE_URL=https://apache.osuosl.org/maven/maven-3/${MAVEN_VERSION}/binaries

RUN mkdir -p /usr/share/maven /usr/share/maven/ref \
    && curl -fsSL -o /tmp/apache-maven.tar.gz ${BASE_URL}/apache-maven-${MAVEN_VERSION}-bin.tar.gz \
    && echo "${SHA}  /tmp/apache-maven.tar.gz" | sha512sum -c - \
    && tar -xzf /tmp/apache-maven.tar.gz -C /usr/share/maven --strip-components=1 \
    && rm -f /tmp/apache-maven.tar.gz \
    && ln -s /usr/share/maven/bin/mvn /usr/bin/mvn

ENV MAVEN_HOME /usr/share/maven
ENV MAVEN_CONFIG "$USER_HOME_DIR/.m2"
RUN mkdir -p $MAVEN_CONFIG

# Bootstrap contents of .m2 directory
RUN mkdir /root/bootstrap
RUN echo "<project> \
    <groupId>com.corgibytes</groupId> \
    <artifactId>freshli-java-bootstrap</artifactId> \
    <version>1.0</version> \
    <modelVersion>4.0.0</modelVersion> \
</project>" > /root/bootstrap/pom.xml
RUN cd /root/bootstrap && \
    mvn org.cyclonedx:cyclonedx-maven-plugin:makeAggregateBom && \
    mvn com.corgibytes:versions-maven-plugin:resolve-ranges-historical

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
