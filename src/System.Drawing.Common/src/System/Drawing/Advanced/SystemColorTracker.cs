// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#if FEATURE_SYSTEM_EVENTS
namespace System.Drawing.Internal
{
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System;
    using System.Drawing;
    using Microsoft.Win32;
    using System.Runtime.InteropServices;
    using System.ComponentModel;

    // Keeps track of objects that need to be notified of system color change events.
    // Mostly this means maintaining a list of weak references.
    internal class SystemColorTracker
    {
        // when I tried the self host, it went over 500 but never over 1000.
        private static int INITIAL_SIZE = 200;
        // If it gets this big, I seriously miscalculated the performance of this object.
        private static int WARNING_SIZE = 100000;
        private static float EXPAND_THRESHOLD = 0.75f;
        private static int EXPAND_FACTOR = 2;

        private static WeakReference[] list = new WeakReference[INITIAL_SIZE];
        private static int count = 0;
        private static bool addedTracker;

        // There's no such thing as a delegate to a static method,
        // so we need to create an instance of something.
        private SystemColorTracker()
        {
        }

        internal static void Add(ISystemColorTracker obj)
        {
            lock (typeof(SystemColorTracker))
            {
                Debug.Assert(list != null, "List is null");
                Debug.Assert(list.Length > 0, "INITIAL_SIZE was initialized after list");

                if (list.Length == count)
                {
                    GarbageCollectList();
                }

                if (!addedTracker)
                {
                    addedTracker = true;
                    SystemEvents.UserPreferenceChanged += new UserPreferenceChangedEventHandler(OnUserPreferenceChanged);
                }

                // Strictly speaking, we should grab a lock on this class.  But since the chances
                // of a problem are so low, the consequences so minimal (something will get accidentally dropped
                // from the list), and the performance of locking so lousy, we'll risk it.
                int index = count;
                count++;

                // COM+ takes forever to Finalize() weak references, so it pays to reuse them.
                if (list[index] == null)
                    list[index] = new WeakReference(obj);
                else
                {
                    Debug.Assert(list[index].Target == null, "Trying to reuse a weak reference that isn't broken yet: list[" + index + "], length =" + list.Length);
                    list[index].Target = obj;
                }
            }
        }

        private static void CleanOutBrokenLinks()
        {
            // Partition the list -- valid references in the low indices, broken references in the high indices.
            // This is taken straight out of Sedgewick (p. 118 on quicksort).

            // Basic idea is to find a broken reference on the left side of the list, and swap it with
            // a valid reference on the right
            int right = list.Length - 1;
            int left = 0;

            int length = list.Length;

            // Loop invariant: everything to the left of "left" is a valid reference,
            // and anything to the right of "right" is broken.
            for (;;)
            {
                while (left < length && list[left].Target != null)
                    left++;
                while (right >= 0 && list[right].Target == null)
                    right--;

                if (left >= right)
                {
                    count = left;
                    break;
                }

                WeakReference temp = list[left];
                list[left] = list[right];
                list[right] = temp;

                left++;
                right--;
            }

            Debug.Assert(count >= 0 && count <= list.Length, "count not a legal index into list");

#if DEBUG
            // Check loop invariant.

            // We'd like to assert that any index < count contains a valid pointer,
            // but since garbage collection can happen at any time, it may have been broken
            // after we partitioned it.
            //
            // for (int i = 0; i < count; i++) {
            //     Debug.Assert(list[i].Target != null, "Null found on the left side of the list");
            // }

            for (int i = count; i < list.Length; i++)
            {
                Debug.Assert(list[i].Target == null, "Partitioning didn't work");
            }
#endif
        }

        private static void GarbageCollectList()
        {
            CleanOutBrokenLinks();

            if (count / (float)list.Length > EXPAND_THRESHOLD)
            {
                WeakReference[] newList = new WeakReference[list.Length * EXPAND_FACTOR];
                list.CopyTo(newList, 0);
                list = newList;

                if (list.Length >= WARNING_SIZE)
                {
                    Debug.Fail("SystemColorTracker is using way more memory than expected.");
                }
            }
        }

        private static void OnUserPreferenceChanged(object sender, UserPreferenceChangedEventArgs e)
        {

            // Update pens and brushes
            if (e.Category == UserPreferenceCategory.Color)
            {
                for (int i = 0; i < count; i++)
                {
                    Debug.Assert(list[i] != null, "null value in active part of list");
                    ISystemColorTracker tracker = (ISystemColorTracker)list[i].Target;
                    if (tracker != null)
                    {
                        // If object still around
                        tracker.OnSystemColorChanged();
                    }
                }
            }
        }
    }
}
#endif
