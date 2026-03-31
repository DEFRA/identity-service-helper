#!/usr/bin/env bash

set -euo pipefail

configuration="Release"
http_env="local"
api_url="${API_URL:-http://localhost:5000}"

while [[ $# -gt 0 ]]; do
  case "$1" in
    --configuration)
      configuration="$2"
      shift 2
      ;;
    --env)
      http_env="$2"
      shift 2
      ;;
    --api-url)
      api_url="$2"
      shift 2
      ;;
    *)
      echo "Unknown argument: $1" >&2
      exit 1
      ;;
  esac
done

script_dir="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
repo_root="$(cd "$script_dir/../.." && pwd)"
coverage_dir="$repo_root/coverage"
coverage_file="$coverage_dir/Api.Http.cobertura.xml"
env_file="$repo_root/tests/Endpoint.Tests/Tests/http-client.env.json"
private_env_file="$repo_root/tests/Endpoint.Tests/Tests/http-client.private.env.json"
api_project="$repo_root/src/Api/Api.csproj"
include_pattern="$repo_root/src/Api/bin/$configuration/net10.0/Defra.Identity*.dll"

require_command() {
  local command_name="$1"
  if ! command -v "$command_name" >/dev/null 2>&1; then
    echo "Required command '$command_name' was not found." >&2
    exit 1
  fi
}

wait_for_api() {
  local attempts=30

  for ((i = 1; i <= attempts; i++)); do
    if curl -s -o /dev/null "$api_url"; then
      return 0
    fi

    sleep 2
  done

  echo "Timed out waiting for API at $api_url" >&2
  return 1
}

cleanup() {
  if [[ -n "${coverage_pid:-}" ]] && kill -0 "$coverage_pid" >/dev/null 2>&1; then
    kill -INT "$coverage_pid" >/dev/null 2>&1 || true
    wait "$coverage_pid" || true
  fi
}

require_command dotnet
require_command dotnet-coverage
require_command ijhttp
require_command curl

mkdir -p "$coverage_dir"

if ! ls $include_pattern >/dev/null 2>&1; then
  echo "No built API assemblies were found for '$configuration'. Run the solution build first." >&2
  exit 1
fi

http_files=()
while IFS= read -r file; do
  http_files+=("$file")
done < <(find "$repo_root/tests/Endpoint.Tests/Tests" -name "*.http" | sort)

if [[ ${#http_files[@]} -eq 0 ]]; then
  echo "No HTTP test files were found." >&2
  exit 1
fi

trap cleanup EXIT

dotnet-coverage collect \
  --include-files "$include_pattern" \
  --output "$coverage_file" \
  --output-format cobertura \
  dotnet run --no-build --project "$api_project" --configuration "$configuration" --urls "$api_url" &
coverage_pid=$!

wait_for_api

ijhttp \
  --env-file "$env_file" \
  --private-env-file "$private_env_file" \
  --env "$http_env" \
  "${http_files[@]}"

cleanup
trap - EXIT
