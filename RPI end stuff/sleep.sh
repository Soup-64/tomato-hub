#!/bin/sh

export DISPLAY=:0
xset s off
xset s -dpms
xset s noblank
if [ "$1" = "sleep" ];
then
        xset s activate
        unclutter -idle 0.00 -root &
fi
