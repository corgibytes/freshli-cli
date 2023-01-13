# frozen_string_literal: true

require 'fileutils'

Before do
  windows_safe_recursive_delete(aruba.config.home_directory)
end
