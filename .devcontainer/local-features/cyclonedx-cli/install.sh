#!/usr/bin/env bash

set -e

### Install CycloneDX CLI
CYCLONEDX_CLI_VERSION=v0.24.0
ARCH="$(dpkg --print-architecture)"; \
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
esac;
curl -sSL "https://github.com/CycloneDX/cyclonedx-cli/releases/download/${CYCLONEDX_CLI_VERSION}/cyclonedx-linux-${CYCLONEDX_ARCH}" -o /usr/local/bin/cyclonedx
chmod +x /usr/local/bin/cyclonedx
