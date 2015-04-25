﻿// -----------------------------------------------------------------------
// <copyright file="RenderTargetBitmap.cs" company="Steven Kirk">
// Copyright 2013 MIT Licence. See licence.md for more information.
// </copyright>
// -----------------------------------------------------------------------

namespace Perspex.Media.Imaging
{
    using Perspex.Platform;
    using Splat;
    using System;

    public class RenderTargetBitmap : Bitmap, IDisposable
    {
        public RenderTargetBitmap(int width, int height)
            : base(CreateImpl(width, height))
        {
        }

        public new IRenderTargetBitmapImpl PlatformImpl
        {
            get { return (IRenderTargetBitmapImpl)base.PlatformImpl; }
        }

        public void Render(IVisual visual)
        {
            this.PlatformImpl.Render(visual);
        }

        private static IBitmapImpl CreateImpl(int width, int height)
        {
            IPlatformRenderInterface factory = Locator.Current.GetService<IPlatformRenderInterface>();
            return factory.CreateRenderTargetBitmap(width, height);
        }

        public void Dispose()
        {
            this.PlatformImpl.Dispose();
        }
    }
}
