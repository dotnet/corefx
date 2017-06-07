// Licensed to the .NET Foundation under one or more agreements.
// See the LICENSE file in the project root for more information.
//
// System.Drawing.SystemBrushes.cs
//
// Authors:
//   Dennis Hayes (dennish@Raytek.com)
//   Ravindra (rkumar@novell.com)
//   Jordi Mas i Hernandez <jordimash@gmail.com>
//
// Copyright (C) 2002 Ximian, Inc. http://www.ximian.com
// Copyright (C) 2004 Novell, Inc. http://www.novell.com
//

//
// Copyright (C) 2004-2005 Novell, Inc (http://www.novell.com)
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

using System;

namespace System.Drawing
{
    /// <summary>
    /// Summary description for SystemBrushes.
    /// </summary>
    public sealed class SystemBrushes
    {
        static SolidBrush active_border;
        static SolidBrush active_caption;
        static SolidBrush active_caption_text;
        static SolidBrush app_workspace;
        static SolidBrush control;
        static SolidBrush control_dark;
        static SolidBrush control_dark_dark;
        static SolidBrush control_light;
        static SolidBrush control_light_light;
        static SolidBrush control_text;
        static SolidBrush desktop;
        static SolidBrush highlight;
        static SolidBrush highlight_text;
        static SolidBrush hot_track;
        static SolidBrush inactive_border;
        static SolidBrush inactive_caption;
        static SolidBrush info;
        static SolidBrush menu;
        static SolidBrush scroll_bar;
        static SolidBrush window;
        static SolidBrush window_text;
        static SolidBrush button_face;
        static SolidBrush button_highlight;
        static SolidBrush button_shadow;
        static SolidBrush gradient_activecaption;
        static SolidBrush gradient_inactivecaption;
        static SolidBrush graytext;
        static SolidBrush inactive_captiontext;
        static SolidBrush infotext;
        static SolidBrush menubar;
        static SolidBrush menu_highlight;
        static SolidBrush menu_text;
        static SolidBrush window_fame;

        private SystemBrushes() { }

        public static Brush ActiveBorder
        {
            get
            {
                if (active_border == null)
                {
                    active_border = new SolidBrush(SystemColors.ActiveBorder);
                    active_border.isModifiable = false;
                }

                return active_border;
            }
        }

        public static Brush ActiveCaption
        {
            get
            {
                if (active_caption == null)
                {
                    active_caption = new SolidBrush(SystemColors.ActiveCaption);
                    active_caption.isModifiable = false;
                }

                return active_caption;
            }
        }

        public static Brush ActiveCaptionText
        {
            get
            {
                if (active_caption_text == null)
                {
                    active_caption_text = new SolidBrush(SystemColors.ActiveCaptionText);
                    active_caption_text.isModifiable = false;
                }

                return active_caption_text;
            }
        }

        public static Brush AppWorkspace
        {
            get
            {
                if (app_workspace == null)
                {
                    app_workspace = new SolidBrush(SystemColors.AppWorkspace);
                    app_workspace.isModifiable = false;
                }

                return app_workspace;
            }
        }

        public static Brush Control
        {
            get
            {
                if (control == null)
                {
                    control = new SolidBrush(SystemColors.Control);
                    control.isModifiable = false;
                }

                return control;
            }
        }

        public static Brush ControlLight
        {
            get
            {
                if (control_light == null)
                {
                    control_light = new SolidBrush(SystemColors.ControlLight);
                    control_light.isModifiable = false;
                }

                return control_light;
            }
        }

        public static Brush ControlLightLight
        {
            get
            {
                if (control_light_light == null)
                {
                    control_light_light = new SolidBrush(SystemColors.ControlLightLight);
                    control_light_light.isModifiable = false;
                }

                return control_light_light;
            }
        }

        public static Brush ControlDark
        {
            get
            {
                if (control_dark == null)
                {
                    control_dark = new SolidBrush(SystemColors.ControlDark);
                    control_dark.isModifiable = false;
                }

                return control_dark;
            }
        }

        public static Brush ControlDarkDark
        {
            get
            {
                if (control_dark_dark == null)
                {
                    control_dark_dark = new SolidBrush(SystemColors.ControlDarkDark);
                    control_dark_dark.isModifiable = false;
                }

                return control_dark_dark;
            }
        }

        public static Brush ControlText
        {
            get
            {
                if (control_text == null)
                {
                    control_text = new SolidBrush(SystemColors.ControlText);
                    control_text.isModifiable = false;
                }

                return control_text;
            }
        }

        public static Brush Highlight
        {
            get
            {
                if (highlight == null)
                {
                    highlight = new SolidBrush(SystemColors.Highlight);
                    highlight.isModifiable = false;
                }

                return highlight;
            }
        }

        public static Brush HighlightText
        {
            get
            {
                if (highlight_text == null)
                {
                    highlight_text = new SolidBrush(SystemColors.HighlightText);
                    highlight_text.isModifiable = false;
                }

                return highlight_text;
            }
        }

        public static Brush Window
        {
            get
            {
                if (window == null)
                {
                    window = new SolidBrush(SystemColors.Window);
                    window.isModifiable = false;
                }

                return window;
            }
        }
        public static Brush WindowText
        {
            get
            {
                if (window_text == null)
                {
                    window_text = new SolidBrush(SystemColors.WindowText);
                    window_text.isModifiable = false;
                }

                return window_text;
            }
        }

        public static Brush InactiveBorder
        {
            get
            {
                if (inactive_border == null)
                {
                    inactive_border = new SolidBrush(SystemColors.InactiveBorder);
                    inactive_border.isModifiable = false;
                }

                return inactive_border;
            }
        }

        public static Brush Desktop
        {
            get
            {
                if (desktop == null)
                {
                    desktop = new SolidBrush(SystemColors.Desktop);
                    desktop.isModifiable = false;
                }

                return desktop;
            }
        }

        public static Brush HotTrack
        {
            get
            {
                if (hot_track == null)
                {
                    hot_track = new SolidBrush(SystemColors.HotTrack);
                    hot_track.isModifiable = false;
                }

                return hot_track;
            }
        }

        public static Brush InactiveCaption
        {
            get
            {
                if (inactive_caption == null)
                {
                    inactive_caption = new SolidBrush(SystemColors.InactiveCaption);
                    inactive_caption.isModifiable = false;
                }

                return inactive_caption;
            }
        }

        public static Brush Info
        {
            get
            {
                if (info == null)
                {
                    info = new SolidBrush(SystemColors.Info);
                    info.isModifiable = false;
                }

                return info;
            }
        }

        public static Brush Menu
        {
            get
            {
                if (menu == null)
                {
                    menu = new SolidBrush(SystemColors.Menu);
                    menu.isModifiable = false;
                }

                return menu;
            }
        }

        public static Brush ScrollBar
        {
            get
            {
                if (scroll_bar == null)
                {
                    scroll_bar = new SolidBrush(SystemColors.ScrollBar);
                    scroll_bar.isModifiable = false;
                }

                return scroll_bar;
            }
        }

        public static Brush FromSystemColor(Color c)
        {
            if (c.IsSystemColor)
            {
                SolidBrush newBrush = new SolidBrush(c);
                newBrush.isModifiable = false;
                return newBrush;
            }

            String message = String.Format("The color {0} is not a system color.", c);
            throw new ArgumentException(message);
        }

        public static Brush ButtonFace
        {
            get
            {
                if (button_face == null)
                {
                    button_face = new SolidBrush(SystemColors.ButtonFace);
                    button_face.isModifiable = false;
                }

                return button_face;
            }
        }

        public static Brush ButtonHighlight
        {
            get
            {
                if (button_highlight == null)
                {
                    button_highlight = new SolidBrush(SystemColors.ButtonHighlight);
                    button_highlight.isModifiable = false;
                }

                return button_highlight;
            }
        }

        public static Brush ButtonShadow
        {
            get
            {
                if (button_shadow == null)
                {
                    button_shadow = new SolidBrush(SystemColors.ButtonShadow);
                    button_shadow.isModifiable = false;
                }

                return button_shadow;
            }
        }

        public static Brush GradientActiveCaption
        {
            get
            {
                if (gradient_activecaption == null)
                {
                    gradient_activecaption = new SolidBrush(SystemColors.GradientActiveCaption);
                    gradient_activecaption.isModifiable = false;
                }

                return gradient_activecaption;
            }
        }

        public static Brush GradientInactiveCaption
        {
            get
            {
                if (gradient_inactivecaption == null)
                {
                    gradient_inactivecaption = new SolidBrush(SystemColors.GradientInactiveCaption);
                    gradient_inactivecaption.isModifiable = false;
                }

                return gradient_inactivecaption;
            }
        }

        public static Brush GrayText
        {
            get
            {
                if (graytext == null)
                {
                    graytext = new SolidBrush(SystemColors.GrayText);
                    graytext.isModifiable = false;
                }

                return graytext;
            }
        }

        public static Brush InactiveCaptionText
        {
            get
            {
                if (inactive_captiontext == null)
                {
                    inactive_captiontext = new SolidBrush(SystemColors.InactiveCaptionText);
                    inactive_captiontext.isModifiable = false;
                }

                return inactive_captiontext;
            }
        }

        public static Brush InfoText
        {
            get
            {
                if (infotext == null)
                {
                    infotext = new SolidBrush(SystemColors.InfoText);
                    infotext.isModifiable = false;
                }

                return infotext;
            }
        }

        public static Brush MenuBar
        {
            get
            {
                if (menubar == null)
                {
                    menubar = new SolidBrush(SystemColors.MenuBar);
                    menubar.isModifiable = false;
                }

                return menubar;
            }
        }

        public static Brush MenuHighlight
        {
            get
            {
                if (menu_highlight == null)
                {
                    menu_highlight = new SolidBrush(SystemColors.MenuHighlight);
                    menu_highlight.isModifiable = false;
                }

                return menu_highlight;
            }
        }

        public static Brush MenuText
        {
            get
            {
                if (menu_text == null)
                {
                    menu_text = new SolidBrush(SystemColors.MenuText);
                    menu_text.isModifiable = false;
                }

                return menu_text;
            }
        }

        public static Brush WindowFrame
        {
            get
            {
                if (window_fame == null)
                {
                    window_fame = new SolidBrush(SystemColors.WindowFrame);
                    window_fame.isModifiable = false;
                }

                return window_fame;
            }
        }

    }
}
