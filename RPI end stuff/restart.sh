#!/bin/sh

pid=$(pidof avalonia-rider-test)
kill "$pid"
export DISPLAY=:0
./autox.sh
