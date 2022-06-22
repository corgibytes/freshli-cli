require "aruba/generators/script_file"

Given('the directory named {string} is prepended to the PATH environment variable') do |directory|
  prepend_environment_variable "PATH", expand_path(directory) + File::PATH_SEPARATOR
end
