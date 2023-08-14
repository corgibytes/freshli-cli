#!/usr/bin/env ruby
# frozen_string_literal: true

require 'English'
require 'optparse'

require_relative 'support/execute'

enable_dotnet_command_colors

perform_eclint = true
perform_rubocop = true
perform_dotnet_format = true
perform_resharper = true

parser = OptionParser.new do |options|
  options.banner = <<~BANNER
    Description:
        Linter Runner

    Usage:
        lint.rb [options]

    Options:
  BANNER

  options.on('--skip-eclint', 'Does not run the eclint linter') do
    perform_eclint = false
  end

  options.on('--skip-rubocop', 'Does not run the Rubocop linter') do
    perform_rubocop = false
  end

  options.on('--skip-dotnet-format', 'Does not run the dotnet format linter') do
    perform_dotnet_format = false
  end

  options.on('--skip-resharper', 'Does not run the JetBrains ReSharper linter') do
    perform_resharper = false
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

linter_failed = false

if perform_eclint
  status = execute('eclint')

  linter_failed ||= !status.success?
end

if perform_rubocop
  status = execute('bundle install')

  status = execute('bundle exec rubocop --color') if status.success?

  linter_failed ||= !status.success?
end

if perform_dotnet_format
  status = execute('dotnet format --verify-no-changes --severity info')

  linter_failed ||= !status.success?
end

if perform_resharper
  status = execute('dotnet tool restore')

  if status.success?
    execute(
      'dotnet jb inspectcode --build --output=resharper.temp --format=text ' \
      "--toolset-path=\"#{msbuild_dll_path}\" freshli-cli.sln"
    )
    File.open('resharper.temp', 'r') do |f|
      result = f.readlines
      status = result.length == 1
      puts result unless status
    end
  end

  linter_failed ||= !status
end

if linter_failed
  puts 'At least one linter encountered errors'
  exit(-1)
end
exit(0)
