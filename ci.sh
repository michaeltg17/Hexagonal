#!/usr/bin/env bash
set -euo pipefail

echo "========================================="
echo "  Running CI"
echo "========================================="

echo
echo "Restore, build and test."
dotnet test src/microservice.sln --configuration Release
echo "Tests passed"

echo
echo "========================================="
echo "  All CI checks passed!"
echo "========================================="
