#!/bin/bash
#
# Uses Git Flow, so master is assumed to be the "released" branch.
# We increment the minor version compared to last tag on master unless we're on a release (candidate) or hotfix branch,
# then the branch name is used to specify the version.
#
# This only works from TeamCity if the teamcity.git.fetchAllHeads flag is set to true.
#
branch="$(git rev-parse --abbrev-ref HEAD)"
major="0"; minor="1"; revision="0";

if [[ $branch == "hotfix/"* ]] || [[ $branch == "release/"* ]]; then

    version=$(echo $branch | cut -d'/' -f2)
    branchedFrom="origin/develop"
    if [[ $branch == "hotfix/"* ]]; then
        branchedFrom="origin/master"
    fi
    commitsSinceLastTag=$(git log $branchedFrom.. --oneline | wc -l | tr -d ' ');

else

    mostRecentTag=$(git describe origin/master --tags --match "[0-9].[0-9]*" --abbrev=0 2>/dev/null)
    if [[ $? -eq 0 ]]; then
        major=$(echo $mostRecentTag | cut -d'.' -f1)
        minor=$(echo $mostRecentTag | cut -d'.' -f2)
        revPart=$(echo $mostRecentTag | cut -d'.' -f3)
        revision=${revPart:-0}  # Semver requires three version numbers.
        commitsSinceLastTag=$(git log $mostRecentTag..HEAD --oneline | wc -l | tr -d ' ')
    else
        commitsSinceLastTag=$(git log --oneline | wc -l | tr -d ' ')
    fi

fi

buildmetadata="+$commitsSinceLastTag"
if [[ $1 == "--nuget" ]]; then buildmetadata="-$commitsSinceLastTag"; fi

if [[ -n $version ]]; then
    # If we have a full version we must be on a release/hotfix branch, append beta pre-release tag.
    # Semver requires three version numbers.
    revPart=$(echo $version | cut -d'.' -f3)
    if [[ ! -n $revPart ]]; then
        version=${version}.0
    fi

    echo "$version-beta$buildmetadata"
else
    if [[ $branch == "master" ]]; then
        # We only use pure version numbers for releases.
        echo "$major.$minor.$revision"
    else
        # Increment minor version vs last release.
        minor=$(($minor + 1));
        revision="0"

        # Use lowercase branch name as pre-release tag by default.
        prereleaseTag="-$(echo $branch | cut -d'/' -f2 | tr  '[:upper:]' '[:lower:]')"

        # Use alpha pre-release tag on develop.
        if [[ $branch == "develop" ]]; then prereleaseTag="-alpha"; fi

        echo "$major.$minor.$revision$prereleaseTag$buildmetadata"
    fi;
fi
