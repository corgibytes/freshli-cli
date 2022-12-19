# frozen_string_literal: true

require 'aruba/cucumber'

# To isolate creation/deletion of files, relative paths and ~ should both resolve to Aruba's simulated home directory
# This is how the built-in Aruba step definitions seem to handle paths anyway.
def resolve_path(path)
  path = Platform.normalize_file_separators(path)
  Aruba.configure do |config|
    if path.include? '~'
      path['~'] = Platform.normalize_file_separators(config.home_directory)
    elsif path[0] != Platform.file_separator
      path = Platform.normalize_file_separators("#{config.home_directory}/#{path}")
    end
  end

  # Resolve globs by returning first path that matches the globbing pattern
  path = Dir[path][0] if path.include? '*'

  path
end
