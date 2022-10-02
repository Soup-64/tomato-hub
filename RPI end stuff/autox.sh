#!/bin/sh

if [ -n "$SSH_TTY" ]; then
        printf "ssh detected, exiting start sequence... \n"
        exit 1
fi
sleep 5;
cd /home/auto/FTP/files/linux-arm64 || exit
chmod +x ./avalonia_rider_test
echo "attempting to start the tomato hub"
startx ./avalonia_rider_test --kiosk --
echo "program closed, attempting a restart (maybe check the blinker fluid?)"
cd /home/auto/ || exit
exec "$0" "$@"
