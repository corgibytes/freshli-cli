#!/usr/bin/env bash

USERNAME="${USERNAME:-"${_REMOTE_USER:-"automatic"}"}"

set -ex

# Determine the appropriate non-root user
if [ "${USERNAME}" = "auto" ] || [ "${USERNAME}" = "automatic" ]; then
    USERNAME=""
    POSSIBLE_USERS=("vscode" "node" "codespace" "$(awk -v val=1000 -F ":" '$3==val{print $1}' /etc/passwd)")
    for CURRENT_USER in "${POSSIBLE_USERS[@]}"; do
        if id -u ${CURRENT_USER} > /dev/null 2>&1; then
            USERNAME=${CURRENT_USER}
            break
        fi
    done
    if [ "${USERNAME}" = "" ]; then
        USERNAME=root
    fi
elif [ "${USERNAME}" = "none" ] || ! id -u ${USERNAME} > /dev/null 2>&1; then
    USERNAME=root
fi

### Install freshli-agent-dotnet
mkdir -p /tmp/freshli-agent-dotnet
cd /tmp/freshli-agent-dotnet
git clone https://github.com/corgibytes/freshli-agent-dotnet
cd freshli-agent-dotnet

chown -R $USERNAME:$USERNAME /tmp/freshli-agent-dotnet

su \
    --login \
    --group $USERNAME \
    --shell /bin/bash \
    --command 'cd /tmp/freshli-agent-dotnet/freshli-agent-dotnet; \
        eval "$(/usr/local/share/rbenv/bin/rbenv init - bash)"; \
        bundle install; bin/build.rb' \
    $USERNAME

mkdir -p /usr/local/share/freshli-agent-dotnet/bin
cp -r exe /usr/local/share/freshli-agent-dotnet/bin
ln -s /usr/local/share/freshli-agent-dotnet/bin/freshli-agent-dotnet /usr/local/bin/freshli-agent-dotnet
rm -rf /tmp/freshli-agent-dotnet
