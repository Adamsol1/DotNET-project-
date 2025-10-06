#!/usr/bin/env bash
set -euo pipefail

# Resolve repo root based on script location
SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
REPO_ROOT="$(cd "$SCRIPT_DIR/.." && pwd)"

PROJECT_PATH="$REPO_ROOT/Tools/GitHelperApp/GitHelperApp.csproj"

if [ ! -f "$PROJECT_PATH" ]; then
  echo "Could not find GitHelperApp project at: $PROJECT_PATH" >&2
  exit 1
fi

dotnet run --project "$PROJECT_PATH"


