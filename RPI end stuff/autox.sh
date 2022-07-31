#!/bin/sh

cd /home/auto/FTP/files/linux-arm64
chmod +x ./avalonia-rider-test

startx ./avalonia-rider-test --kiosk --
