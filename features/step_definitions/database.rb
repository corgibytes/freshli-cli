# frozen_string_literal: true

require 'sqlite3'

Then('we can open a SQLite connection to {string}') do |database|
  database = resolve_path database
  SQLite3::Database.new(database, readonly: true) do |db|
    r = db.execute('PRAGMA integrity_check')
    expect(r[0]).to match(['ok'])
  end
end

Then('the {channel} should contain the version of {string}') do |_channel, dll|
  command_output = `dotnet run --project tools/dll-version #{dll}`
  expect(last_command_started).to have_output output_string_eq command_output
end

Then('the {string} contains history stop point at {string} {string}') do |database, commit_date, commit_id|
  database = resolve_path database
  SQLite3::Database.new(database, readonly: true) do |db|
    r = db.execute(<<-SQL)
      SELECT AsOfDateTime, GitCommitId FROM CachedHistoryStopPoints
      WHERE GitCommitId='#{commit_id}' AND AsOfDateTime LIKE '#{commit_date}%'
    SQL
    expect(r[0][0]).to match(/#{commit_date}/)
    expect(r[0][1]).to match(commit_id)
  end
end

Then('the {string} contains lib year {double} for {string} as of {string}') do
  |database, lib_year, package_url, as_of_date_time|

  database = resolve_path database
  SQLite3::Database.new(database, readonly: true) do |db|
    r = db.execute(<<-SQL)
      SELECT PackageUrl, LibYear, AsOfDateTime
      FROM CachedPackageLibYears
      WHERE AsOfDateTime LIKE '#{as_of_date_time}%' AND PackageUrl='#{package_url}'
    SQL
    expect(r[0][0]).to eq(package_url)
    expect(r[0][1]).to eq(lib_year)
    expect(r[0][2]).to match(as_of_date_time.to_s)
  end
end
