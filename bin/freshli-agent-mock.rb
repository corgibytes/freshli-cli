#!/usr/bin/env ruby
# frozen_string_literal: true

require 'digest'
require 'securerandom'

case ARGV[0]
when 'validating-package-urls'
  puts "pkg:nuget/org.corgibytes.flyswatter/flyswatter"
when 'retrieve-release-history'
  puts "1.1.0\t1990-01-29T12:15:25Z"
  puts "1.2.0\t1990-04-17T13:14:45Z"
when 'validating-repositories'
  puts "https://github.com/corgibytes/freshli-fixture-ruby-nokotest"
when 'process-manifests'
  puts "freshli-fixture-ruby-nokotest/Gemfile\tfreshli-fixture-ruby-nokotest/Gemfile-pinned\tfreshli-fixture-ruby-nokotest/Gemfile-pinned.bom"
  puts "freshli-fixture-ruby-nokotest/Gemfile.lock\tfreshli-fixture-ruby-nokotest/Gemfile.lock\tfreshli-fixture-ruby-nokotest/Gemfile.lock.bom"

when 'detect-manifests'
  manifest_id = Digest::SHA1.hexdigest SecureRandom.uuid
  repo_id = Digest::SHA256.hexdigest "https://github.com/corgibytes/freshli-fixture-ruby-nokotestmaster"

  manifest_hash = "2ec0807d95c45cdd0a5579589caa4f0bd0dac425d6447c02bfc85d182c189817"
  manifest_path = "freshli-fixture-ruby-nokotest/Gemfile.lock"

  puts "#{repo_id}\t#{manifest_id}\t#{manifest_hash}\t#{manifest_path}"
end



