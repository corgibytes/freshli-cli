# NOTE: This file is used to produce a production-ready image. If you're looking
# for a `Dockerfile` for development purposes, please look in the `.devcontainer`
# directory.

### Build `freshli` executable
FROM mcr.microsoft.com/dotnet/sdk:6.0.400-1-bullseye-slim AS dotnet_build

COPY . /app/freshli
WORKDIR /app/freshli

RUN dotnet tool restore
RUN dotnet gitversion /config GitVersion.yml /showconfig
RUN dotnet gitversion /config GitVersion.yml /output json /output buildserver
RUN dotnet gitversion /config GitVersion.yml /updateprojectfiles
RUN dotnet build -c Release -o exe

### Build `freshli-agent-java` executable
FROM eclipse-temurin:17-jdk-jammy AS java_build

RUN mkdir -p /app
WORKDIR /app

RUN apt update -y && apt install git -y
RUN git clone https://github.com/corgibytes/freshli-agent-java
WORKDIR /app/freshli-agent-java

RUN ./gradlew installDist

### Runtime container
# Use dotnet container as a base
FROM mcr.microsoft.com/dotnet/runtime:6.0-bullseye-slim AS final

# Install Java JRE into runtime container
# Copied from https://github.com/adoptium/containers/blob/c0bae0b597987abee553b443983acb3f8af2b7b0/17/jre/ubuntu/jammy/Dockerfile.releases.full
ENV JAVA_HOME /opt/java/openjdk
ENV PATH $JAVA_HOME/bin:$PATH

# Default to UTF-8 file.encoding
ENV LANG='en_US.UTF-8' LANGUAGE='en_US:en' LC_ALL='en_US.UTF-8'

RUN apt-get -y update \
    && DEBIAN_FRONTEND=noninteractive apt-get install -y --no-install-recommends tzdata curl wget ca-certificates fontconfig locales binutils \
    && echo "en_US.UTF-8 UTF-8" >> /etc/locale.gen \
    && locale-gen en_US.UTF-8 \
    && rm -rf /var/lib/apt/lists/*

ENV JAVA_VERSION jdk-17.0.4.1+1

RUN set -eux; \
    ARCH="$(dpkg --print-architecture)"; \
    case "${ARCH}" in \
    aarch64|arm64) \
    ESUM='2e4137529319cd7935f74e1289025b7b4c794c0fb47a3d138adffbd1bbc0ea58'; \
    BINARY_URL='https://github.com/adoptium/temurin17-binaries/releases/download/jdk-17.0.4.1%2B1/OpenJDK17U-jre_aarch64_linux_hotspot_17.0.4.1_1.tar.gz'; \
    ;; \
    armhf|arm) \
    ESUM='b63f532cb8b30e4d0bd18d52f08c1933e3cf66aeb373180d002274b6d94b4a25'; \
    BINARY_URL='https://github.com/adoptium/temurin17-binaries/releases/download/jdk-17.0.4.1%2B1/OpenJDK17U-jre_arm_linux_hotspot_17.0.4.1_1.tar.gz'; \
    ;; \
    ppc64el|powerpc:common64) \
    ESUM='02947997297742ac5a7064fc5414042071fb96d0260d3756100abb281eff3cde'; \
    BINARY_URL='https://github.com/adoptium/temurin17-binaries/releases/download/jdk-17.0.4.1%2B1/OpenJDK17U-jre_ppc64le_linux_hotspot_17.0.4.1_1.tar.gz'; \
    ;; \
    s390x|s390:64-bit) \
    ESUM='f594458bbf42d1d43f7fb5880d0b09d5f9ac11e8eea0de8756419228a823d21c'; \
    BINARY_URL='https://github.com/adoptium/temurin17-binaries/releases/download/jdk-17.0.4.1%2B1/OpenJDK17U-jre_s390x_linux_hotspot_17.0.4.1_1.tar.gz'; \
    ;; \
    amd64|i386:x86-64) \
    ESUM='e96814ee145a599397d91e16831d2dddc3c6b8e8517a8527e28e727649aaa2d1'; \
    BINARY_URL='https://github.com/adoptium/temurin17-binaries/releases/download/jdk-17.0.4.1%2B1/OpenJDK17U-jre_x64_linux_hotspot_17.0.4.1_1.tar.gz'; \
    ;; \
    *) \
    echo "Unsupported arch: ${ARCH}"; \
    exit 1; \
    ;; \
    esac; \
    wget -O /tmp/openjdk.tar.gz ${BINARY_URL}; \
    echo "${ESUM} */tmp/openjdk.tar.gz" | sha256sum -c -; \
    mkdir -p "$JAVA_HOME"; \
    tar --extract \
    --file /tmp/openjdk.tar.gz \
    --directory "$JAVA_HOME" \
    --strip-components 1 \
    --no-same-owner \
    ; \
    rm /tmp/openjdk.tar.gz; \
    # https://github.com/docker-library/openjdk/issues/331#issuecomment-498834472
    find "$JAVA_HOME/lib" -name '*.so' -exec dirname '{}' ';' | sort -u > /etc/ld.so.conf.d/docker-openjdk.conf; \
    ldconfig; \
    # https://github.com/docker-library/openjdk/issues/212#issuecomment-420979840
    # https://openjdk.java.net/jeps/341
    java -Xshare:dump;

RUN echo Verifying install ... \
    && fileEncoding="$(echo 'System.out.println(System.getProperty("file.encoding"))' | jshell -s -)"; [ "$fileEncoding" = 'UTF-8' ]; rm -rf ~/.java \
    && echo java --version && java --version \
    && echo Complete.

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