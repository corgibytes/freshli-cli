# frozen_string_literal: true

require 'aruba/cucumber'

# To isolate creation/deletion of files, relative paths and ~ should both resolve to Aruba's simulated home directory
# This is how the built-in Aruba step definitions seem to handle paths anyway.
def resolve_path(path)
  path = Platform.normalize_file_separators(path)
  Aruba.configure do |config|
    if path.include? '~'
      path['~'] = Platform.normalize_file_separators(config.home_directory)
    elsif path[0] != Platform.file_separator && !path.start_with?(/[a-zA-Z]:/)
      path = Platform.normalize_file_separators("#{config.home_directory}/#{path}")
    end
  end

  # Resolve globs by returning first path that matches the globbing pattern
  path = Dir[path][0] if path.include? '*'

  path
end

def windows_safe_join(*paths)
  result = File.join(paths)
  if Gem.win_platform?
    result = result.gsub('/', '\\')
  end
  result
end

def windows_safe_recursive_delete(path)
  unless path.start_with?('/') || path.start_with?('\\\\?')
    path = File.expand_path(path)

    if Gem.win_platform? && !path.start_with?('\\\\?')
      # Force path into UNC format. This works around issues with file names longer than the
      # default max on Windows, which is typically 260 characters
      path = windows_safe_join('\\\\?\\', path)
    end
  end

  raise "#{path} is not a directory" unless File.directory?(path)

  Dir.children(path).each do |entry|
    entry_path = windows_safe_join(path, entry)
    if File.directory?(entry_path)
      windows_safe_recursive_delete(entry_path)
    else
      if Gem.win_platform?
        # Remove read-only attribute from the file. This works around the removal of read-only
        # directories being prohibited on Windows, even when `force: true` is specified.
        FileUtils.chmod("a=rw", entry_path)
      end

      File.delete(entry_path)
    end
  end

  if Gem.win_platform?
    # Remove read-only attribute from the directory. This works around the removal of read-only
    # directories being prohibited on Windows, even when `force: true` is specified.
    FileUtils.chmod_R("a=rw", path)
  end

  Dir.rmdir(path)
end
