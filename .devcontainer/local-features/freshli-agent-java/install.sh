#!/usr/bin/env bash

set -e

### Install freshli-agent-java
mkdir -p /tmp/freshli-agent-java
cd /tmp/freshli-agent-java
git clone https://github.com/corgibytes/freshli-agent-java
cd freshli-agent-java
./gradlew installDist
cp -r build/install/freshli-agent-java /usr/local/share/freshli-agent-java
ln -s /usr/local/share/freshli-agent-java/bin/freshli-agent-java /usr/local/bin/freshli-agent-java
rm -rf /tmp/freshli-agent-java
