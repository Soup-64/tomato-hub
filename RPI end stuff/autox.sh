#!/bin/sh

if [ -n "$SSH_TTY" ]; then
        echo "ssh detected, exiting start sequence... \n"
        exit 1
fi
sleep 5;
cd /home/auto/FTP/files/linux-arm64
chmod +x ./avalonia-rider-test
echo "attempting to start the tomato hub"
startx ./avalonia-rider-test --kiosk --
echo "program closed, attempting a restart (maybe check the blinker fluid?)"
cd /home/auto/
exec "$0" "$@"
