#!/usr/bin/env bash

# Clean up the package folder
echo "Cleaning up package files..."
rm -rf ./RELEASENOTES.md

# Generate release notes
echo "Generating release notes"
git log --pretty="format:%cD - **%s** *%cn* %n" --max-count=50 > ./RELEASENOTES.md