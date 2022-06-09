#!/usr/bin/env ruby

require 'optparse'

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
rescue OptionParser::InvalidOption => error
  puts error
  puts parser
  exit -1
end

if perform_build
  system("ruby #{File.dirname(__FILE__)}/build.rb")
end

if $?.success?
  system('bundle check > /dev/null')
  unless $?.success?
    system('bundle install')
  end

  if $?.success?
    system('dotnet test ./exe/Corgibytes.Freshli.Cli.Test.dll')
  end
  if $?.success?
    system('bundle exec cucumber')
  end
end

exit($?.exitstatus)
