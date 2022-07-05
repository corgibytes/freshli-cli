# frozen_string_literal: true

require 'fileutils'

Given('a blank file named {string} exists') do |filename|
  filename = resolve_path filename
  FileUtils.touch filename
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
