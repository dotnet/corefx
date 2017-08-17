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
    echo "  command: Command to run. String passed as the command should include the arguments."
    echo
    echo "Options:"
    echo "  -r, --retryCount    Number of times to retry the command until the command run successfully. If not specified, then command is retried a maximum of 5 times"
    echo "  -w, --waitFactor    A multiplier that determine  the time (seconds) to wait between retries. Wait time is WaitFactor times the retry attempt. If not specified, then WaitFactor is 6."
    echo
    echo "Runs the given command. If the command fails, then retries the command specified number of time until the command succeeds."
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

commandName=$1
if [ -z $commandName ]; then
    say_err "Please specify the command to run."
    exit 1
fi

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

if [ $retries -le 0 ] || [ $WaitFactor -le 0 ]; then
    say_err "retryCount and waitFactor should be greater than 0."
fi

execute $1
