using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Net.NetworkInformation
{
    /// <summary>
    /// Represents a TCP state from the Linux kernel.
    /// Taken from /include/net/tcp_states.h
    /// </summary>
    internal enum LinuxTcpState
    {
        TCP_ESTABLISHED = 1,
        TCP_SYN_SENT,
        TCP_SYN_RECV,
        TCP_FIN_WAIT1,
        TCP_FIN_WAIT2,
        TCP_TIME_WAIT,
        TCP_CLOSE,
        TCP_CLOSE_WAIT,
        TCP_LAST_ACK,
        TCP_LISTEN,
        TCP_CLOSING,    /* Now a valid state */
        TCP_NEW_SYN_RECV,

        TCP_MAX_STATES  /* Leave at the end! */
    }
}
