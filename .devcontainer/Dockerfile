# See here for image contents: https://github.com/microsoft/vscode-dev-containers/tree/v0.234.0/containers/dotnet/.devcontainer/base.Dockerfile

# [Choice] .NET version: 6.0, 5.0, 3.1, 6.0-bullseye, 5.0-bullseye, 3.1-bullseye, 6.0-focal, 5.0-focal, 3.1-focal
ARG VARIANT="6.0-bullseye"
FROM mcr.microsoft.com/vscode/devcontainers/dotnet:0-${VARIANT}

# [Choice] Node.js version: none, lts/*, 16, 14, 12, 10
ARG NODE_VERSION="none"
RUN if [ "${NODE_VERSION}" != "none" ]; then su vscode -c "umask 0002 && . /usr/local/share/nvm/nvm.sh && nvm install ${NODE_VERSION} 2>&1"; fi

### Install buildpack-curl (required for buildpack installation)
# Copied from https://github.com/docker-library/buildpack-deps/blob/a017681ea2f5891786bfa151a0d38fbea051d4ba/debian/bullseye/curl/Dockerfile
RUN set -eux; \
    apt-get update; \
    apt-get install -y --no-install-recommends \
    ca-certificates \
    curl \
    netbase \
    wget \
    ; \
    rm -rf /var/lib/apt/lists/*

RUN set -ex; \
    if ! command -v gpg > /dev/null; then \
    apt-get update; \
    apt-get install -y --no-install-recommends \
    gnupg \
    dirmngr \
    ; \
    rm -rf /var/lib/apt/lists/*; \
    fi

### Install buildpack-scm (required for buildpack installation)
# Copied from https://github.com/docker-library/buildpack-deps/blob/a017681ea2f5891786bfa151a0d38fbea051d4ba/debian/bullseye/scm/Dockerfile
RUN apt-get update && apt-get install -y --no-install-recommends \
    git \
    mercurial \
    openssh-client \
    subversion \
    \
    procps \
    && rm -rf /var/lib/apt/lists/*

### Install buildpack (required for Ruby installation)
# Copied from https://github.com/docker-library/buildpack-deps/blob/a017681ea2f5891786bfa151a0d38fbea051d4ba/debian/bullseye/Dockerfile
RUN set -ex; \
    apt-get update; \
    apt-get install -y --no-install-recommends \
    autoconf \
    automake \
    bzip2 \
    dpkg-dev \
    file \
    g++ \
    gcc \
    imagemagick \
    libbz2-dev \
    libc6-dev \
    libcurl4-openssl-dev \
    libdb-dev \
    libevent-dev \
    libffi-dev \
    libgdbm-dev \
    libglib2.0-dev \
    libgmp-dev \
    libjpeg-dev \
    libkrb5-dev \
    liblzma-dev \
    libmagickcore-dev \
    libmagickwand-dev \
    libmaxminddb-dev \
    libncurses5-dev \
    libncursesw5-dev \
    libpng-dev \
    libpq-dev \
    libreadline-dev \
    libsqlite3-dev \
    libssl-dev \
    libtool \
    libwebp-dev \
    libxml2-dev \
    libxslt-dev \
    libyaml-dev \
    make \
    patch \
    unzip \
    xz-utils \
    zlib1g-dev \
    \
    # https://lists.debian.org/debian-devel-announce/2016/09/msg00000.html
    $( \
    # if we use just "apt-cache show" here, it returns zero because "Can't select versions from package 'libmysqlclient-dev' as it is purely virtual", hence the pipe to grep
    if apt-cache show 'default-libmysqlclient-dev' 2>/dev/null | grep -q '^Version:'; then \
    echo 'default-libmysqlclient-dev'; \
    else \
    echo 'libmysqlclient-dev'; \
    fi \
    ) \
    ; \
    rm -rf /var/lib/apt/lists/*


### Install Ruby
# Copied from https://github.com/docker-library/ruby/blob/ac24ae0a894606d3ac91c28095db39e69771cac4/3.1/bullseye/Dockerfile

# skip installing gem documentation
RUN set -eux; \
    mkdir -p /usr/local/etc; \
    { \
    echo 'install: --no-document'; \
    echo 'update: --no-document'; \
    } >> /usr/local/etc/gemrc

ENV LANG C.UTF-8
ENV RUBY_MAJOR 3.1
ENV RUBY_VERSION 3.1.2
ENV RUBY_DOWNLOAD_SHA256 ca10d017f8a1b6d247556622c841fc56b90c03b1803f87198da1e4fd3ec3bf2a

# some of ruby's build scripts are written in ruby
#   we purge system ruby later to make sure our final image uses what we just built
RUN set -eux; \
    \
    savedAptMark="$(apt-mark showmanual)"; \
    apt-get update; \
    apt-get install -y --no-install-recommends \
    bison \
    dpkg-dev \
    libgdbm-dev \
    ruby \
    ; \
    rm -rf /var/lib/apt/lists/*; \
    \
    wget -O ruby.tar.xz "https://cache.ruby-lang.org/pub/ruby/${RUBY_MAJOR%-rc}/ruby-$RUBY_VERSION.tar.xz"; \
    echo "$RUBY_DOWNLOAD_SHA256 *ruby.tar.xz" | sha256sum --check --strict; \
    \
    mkdir -p /usr/src/ruby; \
    tar -xJf ruby.tar.xz -C /usr/src/ruby --strip-components=1; \
    rm ruby.tar.xz; \
    \
    cd /usr/src/ruby; \
    \
    # hack in "ENABLE_PATH_CHECK" disabling to suppress:
    #   warning: Insecure world writable dir
    { \
    echo '#define ENABLE_PATH_CHECK 0'; \
    echo; \
    cat file.c; \
    } > file.c.new; \
    mv file.c.new file.c; \
    \
    autoconf; \
    gnuArch="$(dpkg-architecture --query DEB_BUILD_GNU_TYPE)"; \
    ./configure \
    --build="$gnuArch" \
    --disable-install-doc \
    --enable-shared \
    ; \
    make -j "$(nproc)"; \
    make install; \
    \
    apt-mark auto '.*' > /dev/null; \
    apt-mark manual $savedAptMark > /dev/null; \
    find /usr/local -type f -executable -not \( -name '*tkinter*' \) -exec ldd '{}' ';' \
    | awk '/=>/ { print $(NF-1) }' \
    | sort -u \
    | grep -vE '^/usr/local/lib/' \
    | xargs -r dpkg-query --search \
    | cut -d: -f1 \
    | sort -u \
    | xargs -r apt-mark manual \
    ; \
    apt-get purge -y --auto-remove -o APT::AutoRemove::RecommendsImportant=false; \
    \
    cd /; \
    rm -r /usr/src/ruby; \
    # verify we have no "ruby" packages installed
    if dpkg -l | grep -i ruby; then exit 1; fi; \
    [ "$(command -v ruby)" = '/usr/local/bin/ruby' ]; \
    # rough smoke test
    ruby --version; \
    gem --version; \
    bundle --version

# don't create ".bundle" in all our apps
ENV GEM_HOME /usr/local/bundle
ENV BUNDLE_SILENCE_ROOT_WARNING=1 \
    BUNDLE_APP_CONFIG="$GEM_HOME"
ENV PATH $GEM_HOME/bin:$PATH

# adjust permissions of a few directories for running "gem install" as an arbitrary user
RUN mkdir -p "$GEM_HOME" && chmod 777 "$GEM_HOME"


### Install bundler (required to install Cucumber/Aruba testing tools)
RUN gem install bundler


### Install [eclint](https://gitlab.com/greut/eclint)
ENV ECLINT_VERSION=0.3.3
RUN curl -L https://gitlab.com/greut/eclint/-/releases/v$ECLINT_VERSION/downloads/eclint_${ECLINT_VERSION}_linux_x86_64.tar.gz -o /tmp/eclint_${ECLINT_VERSION}_linux_x86_64.tar.gz && \
    mkdir -p /opt/eclint && \
    tar -zxvf /tmp/eclint_${ECLINT_VERSION}_linux_x86_64.tar.gz -C /opt/eclint && \
    rm /tmp/eclint_${ECLINT_VERSION}_linux_x86_64.tar.gz && \
    ln -s /opt/eclint/eclint /usr/local/bin/eclint


### Install Docker CE CLI
RUN apt-get update \
    && apt-get install -y apt-transport-https ca-certificates curl gnupg2 lsb-release \
    && curl -fsSL https://download.docker.com/linux/$(lsb_release -is | tr '[:upper:]' '[:lower:]')/gpg | apt-key add - 2>/dev/null \
    && echo "deb [arch=$(dpkg --print-architecture)] https://download.docker.com/linux/$(lsb_release -is | tr '[:upper:]' '[:lower:]') $(lsb_release -cs) stable" | tee /etc/apt/sources.list.d/docker.list \
    && apt-get update \
    && apt-get install -y docker-ce-cli


### Install Docker Compose
RUN export LATEST_COMPOSE_VERSION=$(curl -sSL "https://api.github.com/repos/docker/compose/releases/latest" | grep -o -P '(?<="tag_name": ").+(?=")') \
    && curl -sSL "https://github.com/docker/compose/releases/download/${LATEST_COMPOSE_VERSION}/docker-compose-$(uname -s)-$(uname -m)" -o /usr/local/bin/docker-compose \
    && chmod +x /usr/local/bin/docker-compose


### Install Java SDK
# Copied from https://github.com/adoptium/containers/blob/6ca246080b385f7d114f1c452264fe094d4d6203/17/jdk/ubuntu/jammy/Dockerfile.releases.full
ENV JAVA_HOME /opt/java/openjdk
ENV PATH $JAVA_HOME/bin:$PATH

# Default to UTF-8 file.encoding
ENV LANG='en_US.UTF-8' LANGUAGE='en_US:en' LC_ALL='en_US.UTF-8'

RUN apt-get update \
    && DEBIAN_FRONTEND=noninteractive apt-get install -y --no-install-recommends tzdata curl wget ca-certificates fontconfig locales binutils \
    && echo "en_US.UTF-8 UTF-8" >> /etc/locale.gen \
    && locale-gen en_US.UTF-8 \
    && rm -rf /var/lib/apt/lists/*

ENV JAVA_VERSION jdk-17.0.4.1+1

RUN set -eux; \
    ARCH="$(dpkg --print-architecture)"; \
    case "${ARCH}" in \
        aarch64|arm64) \
            ESUM='3c7460de77421284b38b4e57cb1bd584a6cef55c34fc51a12270620544de2b8a'; \
            BINARY_URL='https://github.com/adoptium/temurin17-binaries/releases/download/jdk-17.0.4.1%2B1/OpenJDK17U-jdk_aarch64_linux_hotspot_17.0.4.1_1.tar.gz'; \
            ;; \
        armhf|arm) \
            ESUM='efba97cd38af8f43b61f09cb5041f81d92ecd005dcd51c81678fbcf4f24d8461'; \
            BINARY_URL='https://github.com/adoptium/temurin17-binaries/releases/download/jdk-17.0.4.1%2B1/OpenJDK17U-jdk_arm_linux_hotspot_17.0.4.1_1.tar.gz'; \
            ;; \
        ppc64el|powerpc:common64) \
            ESUM='cbedd0a1428b3058d156e99e8e9bc8769e0d633736d6776a4c4d9136648f2fd1'; \
            BINARY_URL='https://github.com/adoptium/temurin17-binaries/releases/download/jdk-17.0.4.1%2B1/OpenJDK17U-jdk_ppc64le_linux_hotspot_17.0.4.1_1.tar.gz'; \
            ;; \
        s390x|s390:64-bit) \
            ESUM='fdc82f4b06c880762503b0cb40e25f46cf8190d06011b3b768f4091d3334ef7f'; \
            BINARY_URL='https://github.com/adoptium/temurin17-binaries/releases/download/jdk-17.0.4.1%2B1/OpenJDK17U-jdk_s390x_linux_hotspot_17.0.4.1_1.tar.gz'; \
            ;; \
        amd64|i386:x86-64) \
            ESUM='5fbf8b62c44f10be2efab97c5f5dbf15b74fae31e451ec10abbc74e54a04ff44'; \
            BINARY_URL='https://github.com/adoptium/temurin17-binaries/releases/download/jdk-17.0.4.1%2B1/OpenJDK17U-jdk_x64_linux_hotspot_17.0.4.1_1.tar.gz'; \
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
    && echo javac --version && javac --version \
    && echo java --version && java --version \
    && echo Complete.


### Install maven
# Copied from https://github.com/carlossg/docker-maven/blob/0da0b5395ec5f2884a84239b62646bc1b0a8bb43/eclipse-temurin-17/Dockerfile
RUN apt-get update \
    && apt-get install -y git \
    && rm -rf /var/lib/apt/lists/*

ARG MAVEN_VERSION=3.8.6
ARG SHA=f790857f3b1f90ae8d16281f902c689e4f136ebe584aba45e4b1fa66c80cba826d3e0e52fdd04ed44b4c66f6d3fe3584a057c26dfcac544a60b301e6d0f91c26
ARG BASE_URL=https://apache.osuosl.org/maven/maven-3/${MAVEN_VERSION}/binaries

RUN mkdir -p /usr/share/maven /usr/share/maven/ref \
    && curl -fsSL -o /tmp/apache-maven.tar.gz ${BASE_URL}/apache-maven-${MAVEN_VERSION}-bin.tar.gz \
    && echo "${SHA}  /tmp/apache-maven.tar.gz" | sha512sum -c - \
    && tar -xzf /tmp/apache-maven.tar.gz -C /usr/share/maven --strip-components=1 \
    && rm -f /tmp/apache-maven.tar.gz \
    && ln -s /usr/share/maven/bin/mvn /usr/bin/mvn

ENV MAVEN_HOME /usr/share/maven
ENV MAVEN_CONFIG "$HOME/.m2"

### Install CycloneDX CLI
ENV CYCLONEDX_CLI_VERSION v0.24.0
RUN ARCH="$(dpkg --print-architecture)"; \
    case "${ARCH}" in \
        aarch64|arm64) \
            CYCLONEDX_ARCH='arm64' \
            ;; \
        armhf|arm) \
            CYCLONEDX_ARCH='arm' \
            ;; \
        amd64|i386:x86-64) \
            CYCLONEDX_ARCH='x64' \
            ;; \
        *) \
            echo "Unsupported arch: ${ARCH}"; \
            exit 1; \
            ;; \
    esac; \
    curl -sSL "https://github.com/CycloneDX/cyclonedx-cli/releases/download/${CYCLONEDX_CLI_VERSION}/cyclonedx-linux-${CYCLONEDX_ARCH}" -o /usr/local/bin/cyclonedx \
        && chmod +x /usr/local/bin/cyclonedx


### Install freshli-agent-java
RUN mkdir -p /tmp/freshli-agent-java && \
    cd /tmp/freshli-agent-java && \
    git clone https://github.com/corgibytes/freshli-agent-java && \
    cd freshli-agent-java && \
    ./gradlew installDist && \
    cp -r build/install/freshli-agent-java /usr/local/share/freshli-agent-java && \
    ln -s /usr/local/share/freshli-agent-java/bin/freshli-agent-java /usr/local/bin/freshli-agent-java && \
    rm -rf /tmp/freshli-agent-java


ENV DEVCONTAINER=true
