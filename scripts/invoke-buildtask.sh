#!/usr/bin/env bash

# Stop script on NZEC
set -e
# Stop script if unbound variable found (use ${var:-} if intentional)
set -u

say_err() {
    printf "%b\n" "Error: $1" >&2
}

showHelp() {
    echo "usage: $0 <command> [OPTIONS]"
    echo
    echo "command: Command to run. String passed as the command should include the arguments."
    echo
    echo "Options:"
    echo "                      If not specified, then command is retried a maximum of 5 times"
    echo "  -w, --waitFactor    A multiplier that determines the time (seconds) to wait between retries." 
    echo "                      Wait time is WaitFactor times the retry attempt. If not specified, then WaitFactor is 6."
    echo
    echo "Runs the given command."
    echo "If the command fails, then the command is retried specified number of times or until the command succeeds."
}

# Executes a command and retries if it fails.
execute() {
    local count=0

    until "$@"; do
        local exit=$?
        count=$(( $count + 1 ))

        if [ $count -lt $retries ]; then
            local wait=$(( waitFactor ** (( count - 1 )) ))
            echo "Retry $count/$retries exited $exit, retrying in $wait seconds..."
            sleep $wait
        else    
            say_err "Retry $count/$retries exited $exit, no more retries left."
            return $exit
        fi
    done

    return 0
}

if [ $# -lt 1 ]; then
    say_err "Please specify the command to run."
    showHelp
    exit 1
fi

commandName="$1"
retries=5
waitFactor=6

while [ $# -ne 0 ]; do
    name=$1
    case $name in
        -h|--help)
            showHelp
            exit 0
            ;;
        -r|--retryCount)
            shift
            retries=$1
            ;;
        -w|--waitFactor)
            shift
            waitFactor=$1
            ;;
        -*)
            say_err "Unknown option: $1"
            exit 1
            ;;
        *)
    esac

    shift
done

if [ $retries -le 0 ] || [ $waitFactor -le 0 ]; then
    say_err "retryCount and waitFactor should be greater than 0."
fi

execute "$commandName"
