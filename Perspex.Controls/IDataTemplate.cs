﻿// -----------------------------------------------------------------------
// <copyright file="IDataTemplate.cs" company="Steven Kirk">
// Copyright 2014 MIT Licence. See licence.md for more information.
// </copyright>
// -----------------------------------------------------------------------

namespace Perspex.Controls
{
    /// <summary>
    /// Interface representing a template used to build a control for a piece of data.
    /// </summary>
    public interface IDataTemplate
    {
        /// <summary>
        /// Checks to see if this data template matches the specified data.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <returns>
        /// True if the data template can build a control for the data, otherwise false.
        /// </returns>
        bool Match(object data);

        /// <summary>
        /// Builds a control for the specified data.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <returns>The control.</returns>
        Control Build(object data);
    }
}
