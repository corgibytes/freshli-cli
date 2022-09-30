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
    .given('a valid database')
    .upon_receiving('a request to create a new analysis')
    .with(
      headers: {
        'Content-Type' => 'application/json'
      },
      method: :post,
      path: '/api/v0/analysis-request',
      body: {
        name: 'Freshli CLI User',
        email: 'info@freshli.io',
        url: 'https://github.com/this-repository-does-not-exist'
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
end
# rubocop:enable Metrics/BlockLength
