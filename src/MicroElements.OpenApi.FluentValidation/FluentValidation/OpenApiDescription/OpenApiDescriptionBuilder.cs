// Copyright (c) MicroElements. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace MicroElements.OpenApi.FluentValidation.OpenApiDescription
{
    /// <summary>
    /// Builds the property descriptions emitted for custom validation descriptions, so every OpenAPI
    /// backend that supports the description rule produces identical wording.
    /// </summary>
    public static class OpenApiDescriptionBuilder
    {
        /// <summary>
        /// Appends <paramref name="description"/> to an existing description, idempotently (a description
        /// already present is not duplicated). Preserves any user-authored description text.
        /// </summary>
        /// <param name="existing">Existing description (may be <c>null</c>).</param>
        /// <param name="description">Description to append.</param>
        /// <returns>The combined description.</returns>
        public static string Append(string? existing, string description)
        {
            if (string.IsNullOrEmpty(existing))
                return description;

            if (existing.Contains(description, StringComparison.Ordinal))
                return existing;

            return existing + "\n" + description;
        }
    }
}
