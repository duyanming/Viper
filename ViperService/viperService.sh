#!/bin/bash
if [ $1 == "start" ];then
nohup dotnet  ViperService.dll 2>&1 &
echo "$!" > pid
echo "start ok!"
elif [ $1 == "stop" ];then
kill `cat pid`
echo "stop ok!"
else
echo "Please make sure the position variable is start or stop."
fi
