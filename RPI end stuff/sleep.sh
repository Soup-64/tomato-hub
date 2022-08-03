#!/bin/sh

export DISPLAY=:0
xset s off
xset s noblank
xset s activate
unclutter -idle 0.00 -root &
