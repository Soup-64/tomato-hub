#!/bin/sh

pid=$(pidof avalonia-rider-test)
kill $pid
./autox.sh
