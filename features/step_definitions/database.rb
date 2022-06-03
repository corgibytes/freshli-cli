# frozen_string_literal: true

require 'aruba/cucumber'
require 'sqlite3'

Then('we can open a SQLite connection to {string}') do |database|
  Aruba.configure do |config|
    database['~'] = config.home_directory
    db = SQLite3::Database.open database
    r = db.execute('PRAGMA integrity_check')
    expect(r[0]).to match(['ok'])
  end
end

Then('the {channel} should contain the version of {string}') do |channel, dll|
    command_output = `dotnet dll-props #{dll}`
    matches = /ProductVersion: (.*)/.match command_output
    expect(last_command_started).to have_output output_string_eq matches[1]
end