#!/usr/bin/env ruby
# frozen_string_literal: true

# run `generate-diagram` project and capture its output
diagram_contents = `dotnet run --project diagram-generator`

doc_template_contents = File.read('./docs/activities-and-events.md-template')
rendered_contents = doc_template_contents.sub("{{diagram}}", diagram_contents)

File.write('./docs/activities-and-events.md', rendered_contents)
