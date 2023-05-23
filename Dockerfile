# NOTE: This file is used to produce a production-ready image. If you're looking
# for a `Dockerfile` for development purposes, please look in the `.devcontainer`
# directory.

### Build `freshli` executable
FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:7.0.100-bullseye-slim AS dotnet_build
ARG TARGETARCH
# Allow builder to build from branches other than `main`
ARG FRESHLI_AGENT_DOTNET_BRANCH=main

FROM dotnet_build as dotnet_build_aarch64
ENV DOTNET_RUNTIME_ID='linux-arm64'

FROM dotnet_build as dotnet_build_arm64
ENV DOTNET_RUNTIME_ID='linux-arm64'

FROM dotnet_build as dotnet_build_armhf
ENV DOTNET_RUNTIME_ID='linux-arm'

FROM dotnet_build as dotnet_build_arm
ENV DOTNET_RUNTIME_ID='linux-arm'

FROM dotnet_build as dotnet_build_amd64
ENV DOTNET_RUNTIME_ID='linux-x64'

FROM dotnet_build as dotnet_build_x86-64
ENV DOTNET_RUNTIME_ID='linux-x64'

FROM dotnet_build_${TARGETARCH} as dotnet_build_platform_specific

COPY . /app/freshli
WORKDIR /app/freshli

RUN dotnet tool restore
RUN dotnet gitversion /config GitVersion.yml /showconfig
RUN dotnet gitversion /config GitVersion.yml /output json /output buildserver
RUN dotnet gitversion /config GitVersion.yml /updateprojectfiles
RUN dotnet build Corgibytes.Freshli.Cli/Corgibytes.Freshli.Cli.csproj -c Release -o exe --self-contained --runtime ${DOTNET_RUNTIME_ID}


### Build `freshli-agent-dotnet` executable
WORKDIR /app
RUN apt update -y && apt install git -y
RUN git clone https://github.com/corgibytes/freshli-agent-dotnet
WORKDIR /app/freshli-agent-dotnet
RUN git checkout ${FRESHLI_AGENT_DOTNET_BRANCH}
RUN dotnet build Corgibytes.Freshli.Agent.DotNet/Corgibytes.Freshli.Agent.DotNet.csproj -c Release -o exe --self-contained --runtime ${DOTNET_RUNTIME_ID}

### The dotnet agent relies on CycloneDX to be install as a self-contained executable.
### It should not be necessary after https://github.com/corgibytes/freshli-agent-dotnet/issues/3
WORKDIR /app
RUN git clone https://github.com/CycloneDX/cyclonedx-dotnet.git
WORKDIR /app/cyclonedx-dotnet
RUN git checkout "v2.7.0"
RUN dotnet publish -c Release -r ${DOTNET_RUNTIME_ID} --self-contained false -f net6.0
RUN cp -r CycloneDX/bin/Release/net6.0/${DOTNET_RUNTIME_ID}/publish /app/cyclonedx-dotnet/CycloneDX/.

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
RUN apt update -y && apt install git lsof -y

# Install maven
# Copied from https://github.com/carlossg/docker-maven/blob/d2333e08a71fe120a0ac245157906e9b3507cee3/eclipse-temurin-17/Dockerfile
ARG MAVEN_VERSION=3.8.8
ARG USER_HOME_DIR="/root"
ARG SHA=332088670d14fa9ff346e6858ca0acca304666596fec86eea89253bd496d3c90deae2be5091be199f48e09d46cec817c6419d5161fb4ee37871503f472765d00
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

# Copy `freshli` executable from the `dotnet_build_platform_specific` image
RUN mkdir -p /usr/local/share/freshli
COPY --from=dotnet_build_platform_specific /app/freshli/exe/ /usr/local/share/freshli/

# Create the default cache-dir
RUN mkdir -p /root/.freshli

# Also, copy `freshli-agent-dotnet` from the `dotnet_build` image
RUN mkdir -p /usr/local/share/freshli-agent-dotnet
COPY --from=dotnet_build_platform_specific /app/freshli-agent-dotnet/exe /usr/local/share/freshli-agent-dotnet/bin
COPY --from=dotnet_build_platform_specific /app/freshli-agent-dotnet/Corgibytes.Freshli.Agent.DotNet /usr/local/share/freshli-agent-dotnet/content
RUN ln -s /usr/local/share/freshli-agent-dotnet/bin/Corgibytes.Freshli.Agent.DotNet /usr/local/bin/freshli-agent-dotnet

RUN mkdir -p /usr/local/share/cyclonedx-dotnet
COPY --from=dotnet_build_platform_specific /app/cyclonedx-dotnet/CycloneDX/publish /usr/local/share/cyclonedx-dotnet
RUN ln -s /usr/local/share/cyclonedx-dotnet/CycloneDX /usr/local/bin/cyclonedx

# Install dotnet v6 runtime
WORKDIR /tmp
RUN wget https://dot.net/v1/dotnet-install.sh
RUN chmod +x dotnet-install.sh
RUN ./dotnet-install.sh --install-dir /usr/local/share/dotnet-6.0 --os linux
RUN ln -s /usr/local/share/dotnet-6.0/dotnet /usr/local/bin/dotnet
ENV DOTNET_ROOT=/usr/local/share/dotnet-6.0

# Copy `freshli-agent-java` from the `java_build` image
RUN mkdir -p /usr/local/share/freshli-agent-java
COPY --from=java_build /app/freshli-agent-java/build/install/freshli-agent-java/ /usr/local/share/freshli-agent-java/

RUN ln -s /usr/local/share/freshli-agent-java/bin/freshli-agent-java /usr/local/bin/freshli-agent-java

# tool for helping diagnose gRPC services in the container when needed 
RUN wget https://github.com/fullstorydev/grpcurl/releases/download/v1.8.7/grpcurl_1.8.7_linux_x86_64.tar.gz
RUN tar -zxvf grpcurl_1.8.7_linux_x86_64.tar.gz
RUN mv grpcurl /usr/local/bin/

ENV PATH=/usr/local/share/freshli:/usr/local/bin:$PATH
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=1
ENV FRESHLI_AGENT_DOTNET_CONTENT_ROOT=/usr/local/share/freshli-agent-dotnet/content

EXPOSE 1-65535
WORKDIR /
ENTRYPOINT ["/usr/local/share/freshli/freshli"]
CMD ["--help"]
