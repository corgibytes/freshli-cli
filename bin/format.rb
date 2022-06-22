#!/usr/bin/env ruby
# frozen_string_literal: true

require_relative './support/execute'

enable_dotnet_command_colors

eclint_status = execute('eclint -fix')

dotnet_format_status = execute('dotnet format --severity info')

status = execute('bundle check > /dev/null')
status = execute('bundle install') unless status.success?

rubocop_status = execute('bundle exec rubocop --autocorrect --color') if status.success?

composite_exitstatus = dotnet_format_status.exitstatus + rubocop_status.exitstatus + eclint_status.exitstatus

exit(composite_exitstatus)
