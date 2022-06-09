#!/usr/bin/env ruby

system('dotnet tool restore')
if $?.success?
  system('dotnet build -o exe')
end 

exit($?.exitstatus)
