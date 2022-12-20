# frozen_string_literal: true

require 'fileutils'

Given('a blank file named {string} exists') do |filename|
  filename = resolve_path filename
  FileUtils.touch Platform.normalize_file_separators(filename)
end

Given('an empty executable file named {string}') do |filename|
  filename = resolve_path filename
  if Gem.win_platform?
    FileUtils.touch "#{filename}.bat"
  else
    FileUtils.touch filename
    FileUtils.chmod('a=x', filename)
  end 
end

Given('a symbolic link from {string} to {string}') do |source, target|
  if Gem.win_platform?
    # Only attempt to create a link if the source exist. Since we're just copying a file on
    # Windows to simulate symbolic links, the operation of creating a symbolic link without 
    # a valid source doesn't make sense on Windows.
    if (File.exist?(resolve_path(source)) || File.exist?(resolve_path("#{source}.bat")))
      begin
        # The creation of symbolic links on Windows requires admin priveleges, 
        # so just copy the file instead
        FileUtils.cp(resolve_path(source), resolve_path(target))
      rescue
        # the file that is being "linked" might be an executable file that was created by us
        FileUtils.cp(resolve_path("#{source}.bat"), resolve_path("#{target}.bat"))
      end
    end
  else
    FileUtils.ln_sf(resolve_path(source), resolve_path(target))
  end
end

Given('I create a directory named {string}') do |dirname|
  dirname = resolve_path dirname
  Dir.mkdir dirname
end

Then('a directory named {string} exists') do |dirname|
  dirname = resolve_path dirname
  expect(Dir.exist?(dirname)).to match(true)
end

Then('a directory named {string} is not empty') do |dirname|
  dirname = resolve_path dirname
  expect(Dir.empty?(dirname)).to match(false)
end

Then(/the freshli cache database located at "([^"]*)" is valid/) do |path|
  step "a file named \"#{resolve_path(path)}\" should exist"
  step "we can open a SQLite connection to \"#{resolve_path(path)}\""
end
