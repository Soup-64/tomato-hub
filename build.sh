#!/bin/bash

#clean out old build files
rm -r ../linux-arm64/*
#check exit code in case of failed compilation
if dotnet publish -c Release -r linux-arm64 -o ../linux-arm64/ -p:PublishTrimmed=true -p:PublishSingleFile=true;
then
  #remove junk file, not needed
  rm ../linux-arm64/avalonia_rider_test.pdb
  scp -r ../linux-arm64/ auto@"$1":/home/auto/FTP/files/
  exit 0
else
    echo "dotnet publish failed, skipping build upload"
    exit 1
fi
