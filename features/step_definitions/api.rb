# frozen_string_literal: true

Pact.service_consumer 'Freshli' do
  has_pact_with 'Web API' do
    mock_service :freshli_web_api do
      port 52_077
    end
  end
end

# rubocop:disable Metrics/BlockLength
Given('the Freshli Web API is available') do
  ENV['FRESHLI_WEB_API_BASE_URL'] = 'http://localhost:52077'

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
end
# rubocop:enable Metrics/BlockLength
