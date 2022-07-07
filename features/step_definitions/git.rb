Then('a Git repository exists at {string} with a Git SHA {string} checked out') do |path, sha|
  repo = resolve_path path
  expect(repo.nil?).to match(false)
  git_index = repo + '/.git/index'
  expect(File.file?(git_index)).to match(true)
  actual_sha = run_from(repo, 'git show-ref HEAD').split[0]
  expect(actual_sha).to match(sha)
end

Then('a Git repository exists at {string} with a branch {string} checked out') do |path, branch|
  repo = resolve_path path
  expect(repo.nil?).to match(false)
  current_branch = run_from repo, 'git branch --show-current'
  expect(current_branch).to match(branch)
end
