#!/bin/bash

#PROJECT_NAME   = $1
#PROJECT_PATH   = $2
#PACKAGE_FOLDER = $3
#VERSION        = $4

echo "Packaging project $1 into $3/$1 $4..."

dotnet pack $2 -c "Release" -o $3/$1 -v m