require 'sqlite3'

Then('we can open a SQLite connection to {string}') do |database|
  database = resolve_path database
  db = SQLite3::Database.open database
  r = db.execute("PRAGMA integrity_check")
  expect(r[0]).to match(["ok"])
end
