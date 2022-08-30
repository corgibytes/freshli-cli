# frozen_string_literal: true

require 'aruba/cucumber'

Aruba.configure do |config|
  # Use aruba working directory
  config.home_directory = File.join(config.root_directory, config.working_directory)
end

Aruba.configure do |config|
  puts %(The default value is "#{config.home_directory}")
end
