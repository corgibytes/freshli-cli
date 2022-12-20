# frozen_string_literal: true

require 'fileutils'

After do |scenario|
  if scenario.status == :passed
    FileUtils.rm_rf(aruba.config.home_directory)
  end
end
