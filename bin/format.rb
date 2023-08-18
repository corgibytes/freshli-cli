#!/usr/bin/env ruby
# frozen_string_literal: true

require_relative 'support/execute'

enable_dotnet_command_colors

eclint_status = execute('eclint -fix')

resharper_status = execute('dotnet tool restore')
resharper_status = execute('dotnet jb cleanupcode freshli-cli.sln') if resharper_status.success?

dotnet_format_status = execute('dotnet format --severity info')

rubocop_status = execute('bundle install')
rubocop_status = execute('bundle exec rubocop --autocorrect --color') if rubocop_status.success?

composite_exitstatus =
  dotnet_format_status.exitstatus + rubocop_status.exitstatus + resharper_status.exitstatus + eclint_status.exitstatus

exit(composite_exitstatus)
