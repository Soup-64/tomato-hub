#!/bin/bash

#clean out old build files
rm -r ../linux-arm64/*
#check exit code in case of failed compilation
if dotnet publish -c Release -r linux-arm64 -o ../linux-arm64/ -p:PublishTrimmed=true -p:PublishSingleFile=true;
then
  #remove junk file, not needed
  rm ../linux-arm64/avalonia-rider-test.pdb
  scp -r ../linux-arm64/ auto@192.168.0.159:/home/auto/FTP/files/
  #TODO: setup system to reload program on pi instead of rebooting the thing
  exit 0
else
    echo "dotnet publish failed, skipping build upload"
    exit 1
fi