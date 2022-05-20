# See here for image contents: https://github.com/microsoft/vscode-dev-containers/tree/v0.234.0/containers/dotnet/.devcontainer/base.Dockerfile

# [Choice] .NET version: 6.0, 5.0, 3.1, 6.0-bullseye, 5.0-bullseye, 3.1-bullseye, 6.0-focal, 5.0-focal, 3.1-focal
ARG VARIANT="6.0-bullseye"
FROM mcr.microsoft.com/vscode/devcontainers/dotnet:0-${VARIANT}

# [Choice] Node.js version: none, lts/*, 16, 14, 12, 10
ARG NODE_VERSION="none"
RUN if [ "${NODE_VERSION}" != "none" ]; then su vscode -c "umask 0002 && . /usr/local/share/nvm/nvm.sh && nvm install ${NODE_VERSION} 2>&1"; fi

# Start buildpack-curl installation
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

# Start buildpack-scm installation
# Copied from https://github.com/docker-library/buildpack-deps/blob/a017681ea2f5891786bfa151a0d38fbea051d4ba/debian/bullseye/scm/Dockerfile
RUN apt-get update && apt-get install -y --no-install-recommends \
    git \
    mercurial \
    openssh-client \
    subversion \
    \
    procps \
    && rm -rf /var/lib/apt/lists/*

# Start buildpack installation
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



# Start Ruby installation
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

# Install Aruba/Cucumber based acceptance testing tools
RUN gem install bundler

# Install [eclint](https://gitlab.com/greut/eclint)
ENV ECLINT_VERSION=0.3.3
RUN curl -L https://gitlab.com/greut/eclint/-/releases/v$ECLINT_VERSION/downloads/eclint_${ECLINT_VERSION}_linux_x86_64.tar.gz -o /tmp/eclint_${ECLINT_VERSION}_linux_x86_64.tar.gz && \
    mkdir -p /opt/eclint && \
    tar -zxvf /tmp/eclint_${ECLINT_VERSION}_linux_x86_64.tar.gz -C /opt/eclint && \
    rm /tmp/eclint_${ECLINT_VERSION}_linux_x86_64.tar.gz
ENV PATH /opt/eclint:$PATH

# [Optional] Uncomment this section to install additional OS packages.
# RUN apt-get update && export DEBIAN_FRONTEND=noninteractive \
#     && apt-get -y install --no-install-recommends <your-package-list-here>

# [Optional] Uncomment this line to install global node packages.
# RUN su vscode -c "source /usr/local/share/nvm/nvm.sh && npm install -g <your-package-here>" 2>&1
