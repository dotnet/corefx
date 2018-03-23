// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


#include "pal_types.h"
#include <termios.h>
#include <sys/ioctl.h>
#include <stdio.h>

/* This is dup of System/IO/Ports/NativeMethods.cs */
enum {
    NoneParity = 0,
    OddParity = 1,
    EvenParity = 2,
    MarkParity = 3,
    SpaceParity = 4
};

/* System/IO/Ports/Handshake.cs */
enum {
    HandshakeNone,          // No Flow Control
    HandshakeSoft,          // Software Flow Control -> XOn/Xoff
    HandshakeHard,          // Hardware Flow Control -> RTS/CTS
    HandshakeBoth           // Software & Hardware Flow Control
};

static int  SystemNative_Termios_GetStatus(int fd)
{
    int status = 0;
    if(ioctl(fd, TIOCMGET, &status) < 0)
    {
        return -1;
    }
    return status;
}

static int  SystemNative_Termios_SetStatus(int fd, int status)
{
    if (ioctl(fd, TIOCMSET, &status) < 0)
    {
        return -1;
    }
    return 0;
}

extern "C" int32_t SystemNative_Termios_GetDcd(int fd)
{
    int serial = SystemNative_Termios_GetStatus(fd);
    if (serial >= 0)
    {
        return (serial & TIOCM_CAR) ? 1 : 0;
    }
    return -1;
}

extern "C" int32_t SystemNative_Termios_GetCts(int fd)
{
    int serial = SystemNative_Termios_GetStatus(fd);
    if (serial >= 0)
    {
        return (serial & TIOCM_CTS) ? 1 : 0;
    }
    return -1;
}

extern "C" int32_t SystemNative_Termios_GetRts(int fd)
{
    int serial = SystemNative_Termios_GetStatus(fd);
    if (serial >= 0)
    {
        return (serial & TIOCM_RTS) ? 1 : 0;
    }
    return -1;
}

extern "C" int32_t SystemNative_Termios_GetDsr(int fd)
{
    int serial = SystemNative_Termios_GetStatus(fd);
    if (serial >= 0)
    {
        return (serial & TIOCM_DSR) ? 1 : 0;
    }
    return -1;
}

extern "C" int32_t SystemNative_Termios_GetDtr(int fd)
{
    int serial = SystemNative_Termios_GetStatus(fd);
    if (serial >= 0)
    {
        return (serial & TIOCM_DTR) ? 1 : 0;
    }
    return -1;
}

extern "C" int32_t SystemNative_Termios_SetDtr(int fd, bool set)
{
    int status = SystemNative_Termios_GetStatus(fd);
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
        return SystemNative_Termios_SetStatus(fd, status);
    }
    return -1;
}

extern "C" int32_t SystemNative_Termios_SetRts(int fd, bool set)
{
    int status = SystemNative_Termios_GetStatus(fd);
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
        return SystemNative_Termios_SetStatus(fd, status);
    }
    return -1;
}

static speed_t
SystemNative_speed2termios(int speed)
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
SystemNative_termios2speed(speed_t brate)
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
SystemNative_Termios_GetSpeed(int fd)
{
    struct termios       term;
    if (tcgetattr(fd, &term) < 0)
    {
        return  -1;
    }
    return SystemNative_termios2speed(cfgetispeed(&term));
}

extern "C" int32_t
SystemNative_Termios_SetSpeed(int fd, int speed)
{
    struct termios       term;
    if (tcgetattr(fd, &term) < 0)
    {
        return  -1;
    }

    cfsetspeed(&term, SystemNative_speed2termios(speed));
//    cfsetospeed(&term, (uint32_t)speed);
//    cfsetispeed(&term, (uint32_t)speed);

    if (tcsetattr (fd, TCSANOW, &term) < 0)
    {
        return -1;
    }
    return speed;
}

extern "C" int32_t
SystemNative_Termios_AvailableBytes(int fd, int readBuffer)
{
    int32_t bytes;
    if (ioctl (fd, readBuffer ? FIONREAD : TIOCOUTQ, &bytes) == -1) {
        return -1;
    }

    return bytes;
}

extern "C" int32_t
SystemNative_Termios_Reset(int fd, int speed, int dataBits, int stopBits, int parity, int handshake)
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
    case OddParity:
        term.c_cflag |= PARENB | PARODD;
        break;
    case EvenParity:
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
        speed_t brate = SystemNative_speed2termios(speed);
        ret = cfsetspeed(&term, brate);
    }

    if ((ret != 0) || (tcsetattr (fd, TCSANOW, &term) < 0))
    {
        return -1;
    }

    return 0;
}
