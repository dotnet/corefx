// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#include "pal_types.h"
#include <termios.h>
#include <sys/ioctl.h>
#include <stdio.h>
#include <unistd.h>

/* This is dup of System/IO/Ports/NativeMethods.cs */
enum
{
    ParityNone = 0,
    ParityOdd  = 1,
    ParityEven = 2,
    ParityMark = 3,
    ParitySpace = 4
};

/* System/IO/Ports/Handshake.cs */
enum
{
    HandshakeNone,          // No Flow Control
    HandshakeSoft,          // Software Flow Control -> XOn/Xoff
    HandshakeHard,          // Hardware Flow Control -> RTS/CTS
    HandshakeBoth           // Software & Hardware Flow Control
};

static int SystemNative_TermiosGetStatus(int fd)
{
    int status = 0;
    if(ioctl(fd, TIOCMGET, &status) < 0)
    {
        return -1;
    }

    return status;
}

static int SystemNative_TermiosGetSignal(int fd, int bit)
{
    int status = 0;
    if(ioctl(fd, TIOCMGET, &status) < 0)
    {
        return -1;
    }

    return (status & bit) ? 1 : 0;
}

extern "C" int32_t SystemNative_TermiosGetDcd(int fd)
{
    return SystemNative_TermiosGetSignal(fd, TIOCM_CAR);
}

extern "C" int32_t SystemNative_TermiosGetCts(int fd)
{
    return SystemNative_TermiosGetSignal(fd, TIOCM_CTS);
}

extern "C" int32_t SystemNative_TermiosGetRts(int fd)
{
    return SystemNative_TermiosGetSignal(fd, TIOCM_RTS);
}

extern "C" int32_t SystemNative_TermiosGetDsr(int fd)
{
    return SystemNative_TermiosGetSignal(fd, TIOCM_DSR);
}

extern "C" int32_t SystemNative_TermiosGetDtr(int fd)
{
    return SystemNative_TermiosGetSignal(fd, TIOCM_DTR);
}

extern "C" int32_t SystemNative_TermiosSetDtr(int fd, bool set)
{
    int status = SystemNative_TermiosGetStatus(fd);
    if (status >= 0)
    {
        if (set)
        {
            status |= TIOCM_DTR;
        }
        else
        {
            status &= ~TIOCM_DTR;
        }
        return ioctl(fd, TIOCMSET, &status);
    }

    return -1;
}

extern "C" int32_t SystemNative_TermiosSetRts(int fd, bool set)
{
    int status = SystemNative_TermiosGetStatus(fd);
    if (status >= 0)
    {
        if (set)
        {
            status |= TIOCM_RTS;
        }
        else
        {
            status &= ~TIOCM_RTS;
        }
        return ioctl(fd, TIOCMSET, &status);
    }

    return -1;
}

static speed_t
SystemNative_TermiosSpeed2rate(int speed)
{
    switch (speed)
    {
    case 230400:
        return B230400;
    case 115200:
        return B115200;
    case 57600:
        return B57600;
    case 38400:
        return B38400;
    case 19200:
        return B19200;
    case 9600:
        return B9600;
    case 4800:
        return B4800;
    case 2400:
        return B2400;
    case 1800:
        return B1800;
    case 1200:
        return B1200;
    case 600:
        return B600;
    case 300:
        return B300;
    case 200:
        return B200;
    case 150:
        return B150;
    case 134:
        return B134;
    case 110:
        return B110;
    case 75:
        return B75;
    case 50:
        return B50;
    }
    return (static_cast<speed_t>(speed));
}

static int
SystemNative_TermiosRate2speed(speed_t brate)
{
    switch (brate)
    {
    case B230400:
        return 230400;
    case 115200:
        return 115200;
    case B57600:
        return 57600;
    case B38400:
        return 38400;
    case B19200:
        return 19200;
    case B9600:
        return 9600;
    case B4800:
        return 4800;
    case B2400:
        return 2400;
    case B1800:
        return 1800;
    case B1200:
        return 1200;
    case B600:
        return 600;
    case B300:
        return 300;
    case B200:
        return 200;
    case B150:
        return 150;
    case B134:
        return 134;
    case B110:
        return 110;
    case B75:
        return 75;
    case B50:
        return 50;
    }

    return (static_cast<int>(brate));
}

extern "C" int32_t
SystemNative_TermiosGetSpeed(int fd)
{
    struct termios       term;
    if (tcgetattr(fd, &term) < 0)
    {
        return  -1;
    }

    return SystemNative_TermiosRate2speed(cfgetispeed(&term));
}

extern "C" int32_t
SystemNative_TermiosSetSpeed(int fd, int speed)
{
    struct termios       term;
    if (tcgetattr(fd, &term) < 0)
    {
        return  -1;
    }

    cfsetspeed(&term, SystemNative_TermiosSpeed2rate(speed));

    if (tcsetattr(fd, TCSANOW, &term) < 0)
    {
        return -2;
    }

    return speed;
}

extern "C" int32_t
SystemNative_TermiosAvailableBytes(int fd, int readBuffer)
{
    int32_t bytes;
    if (ioctl (fd, readBuffer ? FIONREAD : TIOCOUTQ, &bytes) == -1) {
        return -1;
    }

    return bytes;
}

extern "C" int32_t
SystemNative_TermiosDiscard(int fd, int queue)
{
    return tcflush(fd, queue == 0 ? TCIOFLUSH : queue == 1 ? TCIFLUSH : TCOFLUSH);
}

extern "C" int32_t
SystemNative_TermiosDrain(int fd)
{
    return tcdrain(fd);
}

extern "C" int32_t
SystemNative_TermiosSendBreak(int fd, int duration)
{
    return tcsendbreak(fd, duration);
}

extern "C" int32_t
SystemNative_TermiosReset(int fd, int speed, int dataBits, int stopBits, int parity, int handshake)
{
    struct termios       term;
    int ret = 0;

    if (tcgetattr(fd, &term) < 0)
    {
        return  -1;
    }

    cfmakeraw(&term);
    term.c_cflag |=  (CLOCAL | CREAD);
    term.c_lflag &= ~(static_cast<tcflag_t>(ICANON | ECHO | ECHOE | ECHOK | ECHONL | ISIG | IEXTEN ));
    term.c_oflag &= ~(static_cast<tcflag_t>(OPOST));
    term.c_iflag = IGNBRK;

    term.c_cflag &= ~(static_cast<tcflag_t>(CSIZE));
    switch (dataBits)
    {
    case 5:
        term.c_cflag |= CS5;
        break;
    case 6:
        term.c_cflag |= CS6;
        break;
    case 7:
        term.c_cflag |= CS7;
        break;
    case 8:
    default:
        term.c_cflag |= CS8;
        break;
    }

    // There really is only choice of 1 or 2.
    if (stopBits == 1)
    {
        term.c_cflag &= ~(static_cast<tcflag_t>(CSTOPB));
    }
    else if (stopBits > 1)
    {
        term.c_cflag |= CSTOPB;
    }

    // Set parity
    term.c_iflag &= ~(static_cast<tcflag_t>(INPCK | ISTRIP));
    switch (parity)
    {
    case ParityOdd:
        term.c_cflag |= PARENB | PARODD;
        break;
    case ParityEven:
        term.c_cflag |= PARENB;
        term.c_cflag &= ~(static_cast<tcflag_t>(PARODD));
        break;
    default:
        term.c_cflag &= ~(static_cast<tcflag_t>(PARENB | PARODD));
    }

    // Flow control - clear first.
    term.c_iflag &= ~(static_cast<tcflag_t>(IXOFF | IXON));
    term.c_cflag &= ~(static_cast<tcflag_t>(CRTSCTS));
    switch (handshake)
    {
        case HandshakeNone: /* None */
            /* do nothing. We did the reset above */
            break;
        case HandshakeHard: /* hardware flow control */
            term.c_cflag |= CRTSCTS;
            break;
        case HandshakeBoth: /* software & hardware flow control */
            term.c_cflag |= CRTSCTS;
            // fall through
            [[clang::fallthrough]];
        case HandshakeSoft: /* XOn/XOff */
            term.c_iflag |= IXOFF | IXON;
            break;
    }

    if (speed)
    {
        speed_t brate = SystemNative_TermiosSpeed2rate(speed);
        ret = cfsetspeed(&term, brate);
    }

    if ((ret != 0) || (tcsetattr(fd, TCSANOW, &term) < 0))
    {
        return -1;
    }

    return 0;
}


