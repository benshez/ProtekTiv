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
    
    # Publish the project
    echo "Publishing project $PROJECT_NAME into $PUBLISH_FOLDER/$PROJECT_NAME $VERSION..."
    dotnet publish $PROJECT_PATH -c "Release" -o $PUBLISH_FOLDER/$PROJECT_NAME -r win-x64 --self-contained true   

    # Package the published files into a nuget package
    echo "Packaging project $PROJECT_NAME into $PACKAGE_FOLDER/$PROJECT_NAME $VERSION..."
    dotnet pack $PROJECT_PATH -c "Release" -o $PACKAGE_FOLDER/$PROJECT_NAME -v m
done

echo "All projects publised."    

# Generate release notes
echo "Generating release notes for $PROJECT_NAME $VERSION..."
NOTES=$($DIR/generate-release-notes.sh)

echo "Build and packaging completed successfully."