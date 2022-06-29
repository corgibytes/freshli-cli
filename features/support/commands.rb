def run_from(path, command)
  prev = Dir.pwd
  Dir.chdir(path)
  `#{command}`
ensure
  Dir.chdir(prev)
end
