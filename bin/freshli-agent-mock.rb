#!/usr/bin/env ruby
# frozen_string_literal: true

require 'digest'
require 'securerandom'

case ARGV[0]
when 'validating-package-urls'
  puts "pkg:nuget/org.corgibytes.flyswatter/flyswatter@1.1.0"
when 'retrieve-release-history'
  puts "pkg:nuget/org.corgibytes.flyswatter/flyswatter@1.1.0\t1990-01-29T12:15:25Z"
when 'validating-repositories'
  puts "https://github.com/corgibytes/freshli-fixture-ruby-nokotest"
when 'process-manifests'
  puts ARGV[0]
when 'detect-manifests'
  manifest_id = Digest::SHA1.hexdigest SecureRandom.uuid
  repo_id = Digest::SHA256.hexdigest "https://github.com/corgibytes/freshli-fixture-ruby-nokotestmaster"
  puts "#{repo_id}\t#{manifest_id}"
end



