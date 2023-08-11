#!/usr/bin/env bash

set -ex

# Load rbenv into current environment
# Assumes that the ruby feature has already been installed
eval "$(/usr/local/share/rbenv/bin/rbenv init - bash)"

# Install correct version of bundler
gem install bundler --version 2.3.26
