### Install [eclint](https://gitlab.com/greut/eclint)
export ECLINT_VERSION=0.3.3
curl -L https://gitlab.com/greut/eclint/-/releases/v$ECLINT_VERSION/downloads/eclint_${ECLINT_VERSION}_linux_x86_64.tar.gz -o /tmp/eclint_${ECLINT_VERSION}_linux_x86_64.tar.gz && \
    mkdir -p /opt/eclint && \
    tar -zxvf /tmp/eclint_${ECLINT_VERSION}_linux_x86_64.tar.gz -C /opt/eclint && \
    rm /tmp/eclint_${ECLINT_VERSION}_linux_x86_64.tar.gz && \
    ln -s /opt/eclint/eclint /usr/local/bin/eclint
