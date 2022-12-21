# frozen_string_literal: true

require 'fileutils'

After do |scenario|
  FileUtils.rm_rf(aruba.config.home_directory) if scenario.status == :passed
end
