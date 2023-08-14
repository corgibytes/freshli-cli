#!/usr/bin/env ruby
# frozen_string_literal: true

require 'English'
require 'optparse'

require_relative 'support/execute'

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

# If not set already, then set the Java heap memory initial (10MB)
# and maximum (1024MB) values
ENV['_JAVA_OPTIONS'] = '-Xms10m -Xmx1024m' unless ENV['_JAVA_OPTIONS']

status = execute("ruby #{File.dirname(__FILE__)}/build.rb") if perform_build

if status.nil? || status.success?
  status = execute('bundle install')

  if status.success?
    status = execute(
      'dotnet test --logger "console;verbosity=detailed" --blame-hang-timeout 10min ' \
      './exe/Corgibytes.Freshli.Cli.Test.dll'
    )
  end
  status = execute('bundle exec cucumber --color --backtrace') if status.success?
end

exit(status.exitstatus)
