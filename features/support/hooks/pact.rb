# frozen_string_literal: true

# Based on https://github.com/pact-foundation/pact-ruby/blob/a619deb7d6ea72989132f5fdb5aafa80ec63a92f/lib/pact/consumer/rspec.rb

require 'pact/consumer'
require 'pact/consumer/spec_hooks'
require 'pact/rspec'
require 'pact/helpers'

module Pact
  module Consumer
    module Cucumber
      include Pact::Consumer::ConsumerContractBuilders
      include Pact::Helpers
    end
  end
end

World(Pact::Consumer::Cucumber)

hooks = Pact::Consumer::SpecHooks.new

BeforeAll do
  hooks.before_all
end

Before do |scenario|
  hooks.before_each scenario.name
end

After do |scenario|
  hooks.after_each scenario.name if scenario.status == :passed
end

AfterAll do
  hooks.after_suite
end
