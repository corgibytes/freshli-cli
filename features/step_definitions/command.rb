# frozen_string_literal: true

# this block is based on https://github.com/cucumber/aruba/blob/v2.1.0/lib/aruba/cucumber/command.rb#L404..L414
Then(/^it should (pass|fail) with exact output containing file paths:$/) do |pass_fail, expected|
  last_command_started.stop

  if pass_fail == 'pass'
    expect(last_command_stopped).to be_successfully_executed
  else
    expect(last_command_stopped).not_to be_successfully_executed
  end

  expect(last_command_stopped).to have_output an_output_string_being_eq(Platform.normalize_file_separators(expected))
end

## the stderr should contain:
Then "(the ){channel} should contain file paths:" do |channel, expected|
  combined_output = send("all_#{channel}")

  expect(combined_output).to include_output_string(Platform.normalize_file_separators(expected))
end
