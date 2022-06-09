#!/usr/bin/env ruby

require 'optparse'

perform_eclint = true

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

linter_failed = false

if perform_eclint
  system("eclint")

  linter_failed = !$?.success?
end

if linter_failed
  puts "At least one linter encountered errors"
  exit -1
end
exit 0
