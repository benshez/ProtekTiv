#!/bin/bash
DIR="$( cd "$( dirname "${BASH_SOURCE[0]}" )" && pwd )"
VERSION=$($DIR/version.sh)
set -e

# Define variables
SOLUTION_FILE="ProtekTiv.Core.sln"
PACKAGE_FOLDER="../.nupkgs/$VERSION"
PUBLISH_FOLDER="../.publish/$VERSION"
# Clean previous builds
echo "Cleaning previous build artifacts..."
dotnet clean
rm -rf **/bin **/obj

# Restore NuGet packages
echo "Restoring NuGet packages..."
dotnet restore

# Build the solution
echo "Building the solution..."
dotnet build

# Publish the project
echo "Publishing the projects..."

# Ensure the solution file exists
if [ ! -f "$SOLUTION_FILE" ]; then
    echo "Solution file $SOLUTION_FILE not found."
    exit 1
fi

# Clean up the publish folder
echo "Cleaning up publish folder..."
rm -rf $PUBLISH_FOLDER/../**

# Clean up the package folder
echo "Cleaning up package files..."
rm -rf $PACKAGE_FOLDER

# Get all project paths from the solution file
PROJECT_PATHS=$(dotnet sln list | grep -E \.csproj$)

# Loop through each project path
for PROJECT_PATH in $PROJECT_PATHS; do
    PROJECT_FILE=$(echo $PROJECT_PATH | awk -F '\' '{print $2}')
    PROJECT_NAME=$(echo $PROJECT_FILE | awk -F '.' '{print $1}')
    
    # Test the project
    $DIR/tests.sh $PROJECT_NAME $PROJECT_PATH $PUBLISH_FOLDER $VERSION

    # Publish the project
    $DIR/publish.sh $PROJECT_NAME $PROJECT_PATH $PUBLISH_FOLDER $VERSION

    # Package the published files into a nuget package
    $DIR/package.sh $PROJECT_NAME $PROJECT_PATH $PACKAGE_FOLDER $VERSION
done

echo "All projects publised."    

# Generate release notes
echo "Generating release notes for $PROJECT_NAME $VERSION..."
NOTES=$($DIR/generate-release-notes.sh)

echo "Build and packaging completed successfully."