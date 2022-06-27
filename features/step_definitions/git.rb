Then('a Git repository exists at {string} with a Git SHA {string} checked out') do |path, sha|
  repo = resolve_path path
  expect(repo.nil?).to match(false)
  git_index = repo + '/.git/index'
  expect(File.file?(git_index)).to match(true)
  Dir.chdir(repo)
  actual_sha = `git show-ref HEAD`.split[0]
  expect(actual_sha).to match(sha)
end
