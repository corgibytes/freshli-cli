# frozen_string_literal: true

require 'sqlite3'

Then('we can open a SQLite connection to {string}') do |database|
  database = resolve_path database
  db = SQLite3::Database.open database
  r = db.execute('PRAGMA integrity_check')
  expect(r[0]).to match(['ok'])
end

Then('the {channel} should contain the version of {string}') do |_channel, dll|
  command_output = `dotnet dll-props #{dll}`
  matches = /ProductVersion: (.*)/.match command_output
  expect(last_command_started).to have_output output_string_eq matches[1]
end

Then('the {string} contains history stop point at {string} {string}') do |database, commit_date, commit_id|
  database = resolve_path database
  db = SQLite3::Database.open database
  r = db.execute(<<-SQL)
    SELECT AsOfDateTime, GitCommitId FROM CachedHistoryStopPoints
    WHERE GitCommitId='#{commit_id}' AND AsOfDateTime LIKE '#{commit_date}%'
  SQL
  expect(r[0][0]).to match(/#{commit_date}/)
  expect(r[0][1]).to match(commit_id)
end

Then('the {string} contains lib year {double} for {string} as of {string}') do
 |database, lib_year, package, as_of_date_time|
  database = resolve_path database
  db = SQLite3::Database.open database
  r = db.execute(<<-SQL)
    SELECT CachedPackageLibYears.PackageName, CachedPackageLibYears.LibYear, CachedHistoryStopPoints.AsOfDateTime
    FROM CachedHistoryStopPoints
    INNER JOIN CachedPackageLibYears ON CachedHistoryStopPoints.ID=CachedPackageLibYears.HistoryStopPointId
    WHERE CachedHistoryStopPoints.AsOfDateTime='#{as_of_date_time}' AND CachedPackageLibYears.PackageName='#{package}'
  SQL
  expect(r[0][0]).to match(/#{package}/)
  expect(r[0][1]).to eq(lib_year)
  expect(r[0][2]).to match(as_of_date_time.to_s)
end
