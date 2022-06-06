require 'aruba/cucumber'

# To isolate creation/deletion of files, relative paths and ~ should both resolve to Aruba's simulated home directory
# This is how the built-in Aruba step definitions seem to handle paths anyway.
def resolve_path(path)
  Aruba.configure do |config|
    if path.include? "~"
      path["~"] = config.home_directory
    elsif path[0] != '/'
      path = config.home_directory + '/' + path
    end
  end
  return path
end
