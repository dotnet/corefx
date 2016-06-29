// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
#ifndef UNICODE
#define UNICODE
#endif
#pragma comment(lib, "netapi32.lib")

#include <stdio.h>
#include <windows.h> 
#include <lm.h>
#include <string>

int wmain(int argc, wchar_t *argv[])
{
    USER_INFO_1 ui;
    DWORD dwLevel = 1;
    DWORD dwError = 0;
    NET_API_STATUS nStatus;

    if (argc < 3)
    {
        fwprintf(stderr, L"Usage: %s add|del <UserName> [<Password>]\n", argv[0]);
        return -1;
    }

    std::wstring action = argv[1];

    if (action == L"add")
    {
        if (argc != 4)
        {
            fwprintf(stderr, L"Usage: %s add <UserName> <Password>\n", argv[0]);
            return -1;
        }
        //
        // Set up the USER_INFO_1 structure.
        //  USER_PRIV_USER: name identifies a user, 
        //    rather than an administrator or a guest.
        //  UF_SCRIPT: required 
        //
        ui.usri1_name = argv[2];
        ui.usri1_password = argv[3];
        ui.usri1_priv = USER_PRIV_USER;
        ui.usri1_home_dir = NULL;
        ui.usri1_comment = NULL;
        ui.usri1_flags = UF_SCRIPT;
        ui.usri1_script_path = NULL;
        //
        // Call the NetUserAdd function, specifying level 1.
        //
        nStatus = NetUserAdd(NULL,
            dwLevel,
            (LPBYTE)&ui,
            &dwError);
        //
        // If the call succeeds, inform the user.
        //
        if (nStatus == NERR_Success)
        {
            fwprintf(stderr, L"User %s has been successfully added\n",
                argv[2]);

            return 0;
        }
        else
        {
            fprintf(stderr, "A system error has occurred: %d\n", nStatus);
            return nStatus;
        }
    }
    else if (action == L"del")
    {
        //
        // All parameters are required.
        //
        if (argc != 3)
        {
            fwprintf(stderr, L"Usage: %s del <Username>\n", argv[0]);
            return -1;
        }
        //
        // Call the NetUserDel function to delete the share.
        //
        nStatus = NetUserDel(NULL, argv[2]);
        //
        // Display the result of the call.
        //
        if (nStatus == NERR_Success)
        {
            fwprintf(stderr, L"User %s has been successfully deleted\n",
                argv[2]);
            return 0;
        }
        else
        {
            fprintf(stderr, "A system error has occurred: %d\n", nStatus);
            return nStatus;
        }
    }

    return 0;
}


