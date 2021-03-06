﻿// Copyright (c) The Perspex Project. All rights reserved.
// Licensed under the MIT license. See licence.md file in the project root for full license information.

using System;
using Perspex.Cairo.Media;
using Perspex.Cairo.Media.Imaging;
using Perspex.Media;
using Perspex.Platform;
using Splat;

namespace Perspex.Cairo
{
    using System.IO;
    using global::Cairo;

    public class CairoPlatform : IPlatformRenderInterface
    {
        private static readonly CairoPlatform s_instance = new CairoPlatform();

        private static Pango.Context s_pangoContext = CreatePangoContext();

        public static void Initialize()
        {
            var locator = Locator.CurrentMutable;
            locator.Register(() => s_instance, typeof(IPlatformRenderInterface));
        }

        public IBitmapImpl CreateBitmap(int width, int height)
        {
            return new BitmapImpl(new Gdk.Pixbuf(Gdk.Colorspace.Rgb, true, 32, width, height));
        }

        public IFormattedTextImpl CreateFormattedText(
            string text,
            string fontFamily,
            double fontSize,
            FontStyle fontStyle,
            TextAlignment textAlignment,
            Perspex.Media.FontWeight fontWeight)
        {
            return new FormattedTextImpl(s_pangoContext, text, fontFamily, fontSize, fontStyle, textAlignment, fontWeight);
        }

        public IRenderer CreateRenderer(IPlatformHandle handle, double width, double height)
        {
            return new Renderer(handle, width, height);
        }

        public IRenderTargetBitmapImpl CreateRenderTargetBitmap(int width, int height)
        {
            return new RenderTargetBitmapImpl(new ImageSurface(Format.Argb32, width, height));
        }

        public IStreamGeometryImpl CreateStreamGeometry()
        {
            return new StreamGeometryImpl();
        }

        public IBitmapImpl LoadBitmap(string fileName)
        {
            var pixbuf = new Gdk.Pixbuf(fileName);

            return new BitmapImpl(pixbuf);
        }

        public IBitmapImpl LoadBitmap(Stream stream)
        {
            var pixbuf = new Gdk.Pixbuf(stream);

            return new BitmapImpl(pixbuf);
        }

        private static Pango.Context CreatePangoContext()
        {
            Gtk.Application.Init();
            return new Gtk.Invisible().CreatePangoContext();
        }
    }
}
