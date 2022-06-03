#!/usr/bin/env bash

perform_build=true

while [[ $# -gt 0 ]]; do
  case $1 in
    -s|--skip-build)
      perform_build=false
      shift
      ;;
    -?|-h|--help)
      SEARCHPATH="$2"
      shift
      echo "Description:"
      echo "  Test Runner"
      echo
      echo "Usage:"
      echo "  test.sh [options]"
      echo
      echo "Options:"
      echo "  -s, --skip-build      Runs tests without first calling build.sh"
      echo "  -?, -h, --help        Show help and usage information"
      echo
      exit 0
      ;;
    -*|--*)
      echo "Unknown option $1"
      exit 1
      ;;
  esac
done

proceed=true
if [[ $perform_build == "true" ]]; then
    $(dirname $0)/build.sh

    if [ $? -ne 0 ]; then
        proceed=false
    fi
fi

if [[ $proceed == "true" ]]; then
    if ! bundle check > /dev/null; then
        bundle install
    fi
    dotnet test ./exe/Corgibytes.Freshli.Cli.Test.dll && \
        bundle exec cucumber
fi
