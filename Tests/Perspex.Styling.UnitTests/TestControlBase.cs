﻿// -----------------------------------------------------------------------
// <copyright file="TestControlBase.cs" company="Steven Kirk">
// Copyright 2014 MIT Licence. See licence.md for more information.
// </copyright>
// -----------------------------------------------------------------------

namespace Perspex.Styling.UnitTests
{
    using System;
    using Perspex.Styling;

    public class TestControlBase : Visual, IStyleable
    {
        public TestControlBase()
        {
            this.Classes = new Classes();
            this.SubscribeCheckObservable = new TestObservable();
        }

        public string Name { get; set; }

        public virtual Classes Classes { get; set; }

        public Type StyleKey
        {
            get { return this.GetType(); }
        }

        public TestObservable SubscribeCheckObservable { get; private set; }
    }
}
