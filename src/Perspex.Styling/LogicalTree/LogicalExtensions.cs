﻿// Copyright (c) The Perspex Project. All rights reserved.
// Licensed under the MIT license. See licence.md file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;

namespace Perspex.LogicalTree
{
    public static class LogicalExtensions
    {
        public static IEnumerable<ILogical> GetLogicalAncestors(this ILogical logical)
        {
            Contract.Requires<NullReferenceException>(logical != null);

            logical = logical.LogicalParent;

            while (logical != null)
            {
                yield return logical;
                logical = logical.LogicalParent;
            }
        }

        public static IEnumerable<ILogical> GetSelfAndLogicalAncestors(this ILogical logical)
        {
            yield return logical;

            foreach (var ancestor in logical.GetLogicalAncestors())
            {
                yield return ancestor;
            }
        }

        public static IEnumerable<ILogical> GetLogicalChildren(this ILogical logical)
        {
            return logical.LogicalChildren;
        }

        public static IEnumerable<ILogical> GetLogicalDescendents(this ILogical logical)
        {
            foreach (ILogical child in logical.LogicalChildren)
            {
                yield return child;

                foreach (ILogical descendent in child.GetLogicalDescendents())
                {
                    yield return descendent;
                }
            }
        }

        public static ILogical GetLogicalParent(this ILogical logical)
        {
            return logical.LogicalParent;
        }

        public static T GetLogicalParent<T>(this ILogical logical) where T : class
        {
            return logical.LogicalParent as T;
        }

        public static IEnumerable<ILogical> GetLogicalSiblings(this ILogical logical)
        {
            ILogical parent = logical.LogicalParent;

            if (parent != null)
            {
                foreach (ILogical sibling in parent.LogicalChildren)
                {
                    yield return sibling;
                }
            }
        }

        public static bool IsLogicalParentOf(this ILogical logical, ILogical target)
        {
            return target.GetLogicalAncestors().Any(x => x == logical);
        }
    }
}
