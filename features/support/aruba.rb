# frozen_string_literal: true

require 'aruba/cucumber'

TWO_HOURS = 2 * 60 * 60

# Contains helper methods for coping with platform specific differences
module Platform
  def self.null_output_target
    Gem.win_platform? ? 'NUL:' : '/dev/null'
  end

  def self.normalize_file_separators(value)
    value.gsub('/', file_separator)
  end

  def self.file_separator
    File::ALT_SEPARATOR || File::SEPARATOR
  end
end

Aruba.configure do |config|
  # Use aruba working directory
  config.home_directory = Platform.normalize_file_separators(File.join(config.root_directory, config.working_directory))
  config.exit_timeout = TWO_HOURS
  config.allow_absolute_paths = true
end
