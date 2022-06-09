#!/usr/bin/env ruby
# frozen_string_literal: true

require 'English'
require 'optparse'

require_relative './support/execute'

enable_dotnet_command_colors

perform_build = true

parser = OptionParser.new do |options|
  options.banner = <<~BANNER
    Description:
        Test Runner

    Usage:
        test.rb [options]

    Options:
  BANNER

  options.on('-s', '--skip-build', 'Run tests without first calling build') do
    perform_build = false
  end

  options.on('-h', '--help', 'Show help and usage information') do
    puts options
    exit
  end
end

begin
  parser.parse!
rescue OptionParser::InvalidOption => e
  puts e
  puts parser
  exit(-1)
end

status = execute("ruby #{File.dirname(__FILE__)}/build.rb") if perform_build

if status.success?
  status = execute('bundle check > /dev/null')
  status = execute('bundle install') unless status.success?

  status = execute('dotnet test ./exe/Corgibytes.Freshli.Cli.Test.dll') if status.success?
  status = execute('bundle exec cucumber --color') if status.success?
end

exit(status.exitstatus)
