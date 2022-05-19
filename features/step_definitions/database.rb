require 'fileutils'
require 'aruba/cucumber'
require 'sqlite3'

Given('that I ran {string}') do |command|
    system command
end

Given('a blank file named {string} exists') do |filename|
    Aruba.configure do |config|
        if filename.include? "~"
            filename["~"] = config.home_directory
        end
        FileUtils.touch filename
    end
end

Given('a directory named {string} exists') do |dirname|
    Aruba.configure do |config|
        if dirname.include? "~"
            dirname["~"] = config.home_directory
        end
        Dir.mkdir dirname
    end
end


Then('we can open a SQLite connection to {string}') do |database|
    Aruba.configure do |config|
        database["~"] = config.home_directory
        db = SQLite3::Database.open database
        r = db.execute("PRAGMA integrity_check")
        expect(r[0]).to match(["ok"])
    end
end
