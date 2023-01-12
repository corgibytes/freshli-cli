# frozen_string_literal: true

require 'open3'
require 'mkmf'

def msbuild_dll_path
  dotnet_exe_path = find_executable('dotnet')
  dotnet_dir = File.dirname(dotnet_exe_path)
  sdk_dir = File.join(dotnet_dir, 'sdk', Dir.children(File.join(dotnet_dir, 'sdk')).max)
  result = File.join(sdk_dir, 'MSBuild.dll')
  result = result.gsub('/', '\\') if Gem.win_platform?
  result
end

def null_output_target
  Gem.win_platform? ? 'nul' : '/dev/null'
end

def enable_dotnet_command_colors
  ENV['DOTNET_SYSTEM_CONSOLE_ALLOW_ANSI_COLOR_REDIRECTION'] = 'true'
end

def stream_eof?(status)
  status.nil?
end

def stream_open_but_empty?(status)
  status.empty? || status == :wait_readable
end

def safe_to_read?(status)
  !stream_eof?(status) && !stream_open_but_empty?(status)
end

def write_buffered_output_to_correct_stream(buffer, stream, stdout, stderr)
  if stream == stdout
    $stdout.print(buffer)
  elsif stream == stderr
    $stderr.print(buffer)
  end
end

BUFFER_LEN = 8

def fill_buffer_from_stream(stream, buffer)
  # loop through reading data until there is an EOF (value is nil)
  # or there is no more data to read (value is empty)
  result = nil
  loop do
    local_buffer = ''.dup
    result = stream.read_nonblock(BUFFER_LEN, local_buffer, exception: false)
    buffer << local_buffer

    break unless safe_to_read?(result) && buffer.length < BUFFER_LEN
  end
  result
end

def read_streams(for_reading, readable, stdout, stderr)
  # In the case that both streams are readable (and thus have content)
  # read from each of them. In this case, we cannot guarantee any order
  # because we recieve the items at essentially the same time.
  # We can still ensure that we don't mix data incorrectly.
  readable.each do |stream|
    buffer = ''.dup
    result = fill_buffer_from_stream(stream, buffer)

    for_reading -= [stream] if stream_eof?(result)

    write_buffered_output_to_correct_stream(buffer, stream, stdout, stderr)
  end
  for_reading
end

def skip_or_read_streams(for_reading, readable, stdout, stderr)
  # readable is nil in the case of a timeout - loop back again
  if readable.nil?
    Thread.pass
  else
    for_reading = read_streams(for_reading, readable, stdout, stderr)
  end
  for_reading
end

WAIT_TIMEOUT = 1
def execute(command)
  exit_status = nil
  Open3.popen3(command) do |_in, stdout, stderr, wait_thread|
    for_reading = [stdout, stderr]
    until for_reading.empty?
      # IO.select blocks until one of the streams is has something to read
      # or the wait timeout is reached
      readable, _writable, _errors = IO.select(for_reading, [], [], WAIT_TIMEOUT)
      for_reading = skip_or_read_streams(for_reading, readable, stdout, stderr)
    end

    exit_status = wait_thread.value
  end
  exit_status
end
