# frozen_string_literal: true

require 'uri'

api_service = nil
Pact.service_consumer 'Freshli' do
  has_pact_with 'Web API' do
    api_service = mock_service :freshli_web_api
  end
end

# rubocop:disable Metrics/BlockLength
Given('the Freshli Web API is available') do
  raise 'API service does not have a valid port number' if api_service.port.nil?

  ENV['FRESHLI_WEB_API_BASE_URL'] = "http://localhost:#{api_service.port}"

  freshli_web_api
    .upon_receiving('a request to create a new analysis')
    .given('a valid database')
    .with(
      headers: {
        'Content-Type' => 'application/json'
      },
      method: :post,
      path: '/api/v0/analysis-request',
      body: {
        name: 'Freshli CLI User',
        email: 'info@freshli.io',
        url: Pact.like('https://github.com/this-repository-does-not-exist')
      }
    )
    .will_respond_with(
      status: 201,
      headers: {
        'Content-Type' => 'application/json',
        'Location' => '/api/v0/analysis-request/eaf76637-8dcb-45fa-83c8-c17e9c6f2db8'
      },
      body: {
        id: 'eaf76637-8dcb-45fa-83c8-c17e9c6f2db8',
        name: 'Freshli CLI User',
        email: 'info@freshli.io',
        url: 'https://github.com/this-repository-does-not-exist'
      }
    )

  freshli_web_api
    .upon_receiving('a request to create a history point')
    .given('an analysis request with id eaf76637-8dcb-45fa-83c8-c17e9c6f2db8')
    .with(
      headers: {
        'Content-Type' => 'application/json'
      },
      method: :post,
      path: '/api/v0/analysis-request/eaf76637-8dcb-45fa-83c8-c17e9c6f2db8',
      body: {
        date: Pact::Term.new(
          generate: '2022-12-23T12:34:56.0000000Z',
          matcher: /^\d{4}-[01]\d-[0-3]\dT[0-2]\d:[0-5]\d:[0-5]\d\.\d{7}([+-][0-2]\d:[0-5]\d|Z)$/
        )
      }
    )
    .will_respond_with(
      status: 201,
      headers: {
        'Content-Type' => 'application/json',
        'Location' => '/api/v0/analysis-request/eaf76637-8dcb-45fa-83c8-c17e9c6f2db8/2022-12-23T12:34:56.0000000Z'
      },
      body: {
        requestId: 'eaf76637-8dcb-45fa-83c8-c17e9c6f2db8',
        date: '2022-12-23T12:34:56.0000000Z'
      }
    )

  freshli_web_api
    .upon_receiving('a request to create a package lib year result')
    .given(
      'an analysis request with id eaf76637-8dcb-45fa-83c8-c17e9c6f2db8 and a history point ' \
      'for 2022-12-23T12:34:56.0000000Z'
    )
    .with(
      headers: {
        'Content-Type' => 'application/json'
      },
      method: :post,
      path: Pact::Term.new(
        generate: '/api/v0/analysis-request/eaf76637-8dcb-45fa-83c8-c17e9c6f2db8/2022-12-23T12:34:56.0000000Z',
        matcher: %r{^
          /api/v0/analysis-request/eaf76637-8dcb-45fa-83c8-c17e9c6f2db8/
          \d{4}-[01]\d-[0-3]\dT[0-2]\d:[0-5]\d:[0-5]\d\.\d{7}([+-][0-2]\d:[0-5]\d|Z)$
        }x
      ),
      body: {
        packageUrl: Pact::Term.new(
          generate: 'pkg:maven/org.apache.xmlgraphics/batik-anim@1.9.1?repository_url=repo.spring.io%2Frelease',
          matcher: URI::DEFAULT_PARSER.make_regexp('pkg')
        ),
        publicationDate: Pact::Term.new(
          generate: '2021-11-12T12:34:56.0000000Z',
          matcher: /^\d{4}-[01]\d-[0-3]\dT[0-2]\d:[0-5]\d:[0-5]\d\.\d{7}([+-][0-2]\d:[0-5]\d|Z)$/
        ),
        libYear: like(1.6)
      }
    )
    .will_respond_with(
      status: 201,
      headers: {
        'Content-Type' => 'application/json',
        'Location' =>
          '/api/v0/analysis-request/eaf76637-8dcb-45fa-83c8-c17e9c6f2db8/' \
          '2022-12-23T12:34:56.0000000Z/pkg:maven/org.apache.xmlgraphics/' \
          'batik-anim@1.9.1?repository_url=repo.spring.io%2Frelease'
      },
      body: {
        packageUrl: 'pkg:maven/org.apache.xmlgraphics/batik-anim@1.9.1?repository_url=repo.spring.io%2Frelease',
        publicationDate: '2021-11-12T12:34:56.0000000Z',
        libYear: 1.6
      }
    )

  freshli_web_api
    .upon_receiving('a request to update the status of an analysis')
    .given('an analysis request with id eaf76637-8dcb-45fa-83c8-c17e9c6f2db8')
    .with(
      headers: {
        'Content-Type' => 'application/json'
      },
      method: :put,
      path: '/api/v0/analysis-request/eaf76637-8dcb-45fa-83c8-c17e9c6f2db8',
      body: {
        state: Pact.like('success')
      }
    )
    .will_respond_with(
      status: 200,
      headers: {
        'Content-Type' => 'application/json'
      },
      body: {
        id: 'eaf76637-8dcb-45fa-83c8-c17e9c6f2db8',
        name: 'Freshli CLI User',
        email: 'info@freshli.io',
        url: 'https://github.com/this-repository-does-not-exist',
        state: 'success'
      }
    )
end
# rubocop:enable Metrics/BlockLength
