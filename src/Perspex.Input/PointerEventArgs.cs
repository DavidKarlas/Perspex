﻿// Copyright (c) The Perspex Project. All rights reserved.
// Licensed under the MIT license. See licence.md file in the project root for full license information.

using System;
using Perspex.Interactivity;

namespace Perspex.Input
{
    public class PointerEventArgs : RoutedEventArgs
    {
        public IPointerDevice Device { get; set; }

        public Point GetPosition(IVisual relativeTo)
        {
            return Device.GetPosition(relativeTo);
        }
    }
}
