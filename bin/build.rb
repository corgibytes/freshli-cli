#!/usr/bin/env ruby
# frozen_string_literal: true

require 'English'

require_relative './support/execute'

enable_dotnet_command_colors

if Gem.win_platform?
  ENV['PROTOBUF_TOOLS_CPU'] = 'x64'
end

status = execute('dotnet tool restore')
status = execute('dotnet build -o exe') if status.success?

exit(status.exitstatus)
