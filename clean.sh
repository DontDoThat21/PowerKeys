#!/bin/bash

echo "Cleaning build artifacts..."

# Remove bin and obj directories
find . -name "bin" -type d -exec rm -rf {} + 2>/dev/null
find . -name "obj" -type d -exec rm -rf {} + 2>/dev/null

echo "Build artifacts cleaned."