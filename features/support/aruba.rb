# frozen_string_literal: true

require 'aruba/cucumber'

TWO_HOURS = 2 * 60 * 60

Aruba.configure do |config|
  # Use aruba working directory
  config.home_directory = File.join(config.root_directory, config.working_directory)
  config.exit_timeout = TWO_HOURS
end

Aruba.configure do |config|
  puts %(The default value is "#{config.home_directory}")
end
