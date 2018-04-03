#!/usr/bin/env bash

if [ "$1" == "begin" ]; then
	if [ -f "$AGENT_TOOLS_PATH/Begin.sh" ]; then
		cd $AGENT_TOOLS_PATH&& ./Begin.sh
	else 
		echo "Begin.sh script does not exist. Moving on."
	fi
elif [ "$1" == "end" ]; then
	if [ -f "$AGENT_TOOLS_PATH/End.sh" ]; then
		cd $AGENT_TOOLS_PATH&& ./End.sh
	else 
		echo "End.sh script does not exist. Moving on."
	fi
else
		echo "Unrecognized parameter value passed to $0 : $1"
fi
