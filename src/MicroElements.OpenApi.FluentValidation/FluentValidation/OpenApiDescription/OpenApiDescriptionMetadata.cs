// Copyright (c) MicroElements. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace MicroElements.OpenApi.FluentValidation.OpenApiDescription
{
    /// <summary>
    /// Implemented by property validators that carry a custom description for the validated property.
    /// The OpenAPI layer reads <see cref="Description"/> and writes it into the property description.
    /// </summary>
    public interface IOpenApiDescriptionValidator
    {
        /// <summary>
        /// Gets the description text for the validated property.
        /// </summary>
        string Description { get; }
    }
}
