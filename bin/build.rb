#!/usr/bin/env ruby
# frozen_string_literal: true

require 'English'

require_relative './support/execute'

enable_dotnet_command_colors

ENV['PROTOBUF_TOOLS_CPU'] = 'x64' if Gem.win_platform?

status = execute('dotnet tool restore')
status = execute('dotnet build -o exe') if status.success?

exit(status.exitstatus)
