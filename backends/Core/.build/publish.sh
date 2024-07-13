#!/bin/bash

#PROJECT_NAME   = $1
#PROJECT_PATH   = $2
#PUBLISH_FOLDER = $3
#VERSION        = $4

echo "Publishing project $1 into $3/$1 $4..."

dotnet publish $2 -c "Release" -o $3/$1 -r win-x64 --self-contained true   