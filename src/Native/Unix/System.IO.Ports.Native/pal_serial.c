// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#include "pal_types.h"
#include <fcntl.h>
#include <errno.h>
#include <pal_serial.h>
#include <termios.h>
#include <unistd.h>
#include <sys/ioctl.h>

/* Open device file in non-blocking mode and without controlling terminal */
intptr_t SystemIoPortsNative_SerialPortOpen(const char * name)
{
    intptr_t fd;
    while ((fd = open(name, O_RDWR | O_NOCTTY | O_CLOEXEC | O_NONBLOCK)) < 0 && errno == EINTR);

    if (fd < 0)
    {
        return fd;
    }

    if (ioctl(fd, TIOCEXCL) != 0)
    {
        intptr_t result;
        while ((result = close(fd)) < 0 && errno == EINTR);
        return -1;
    }
    
    return fd;
}
