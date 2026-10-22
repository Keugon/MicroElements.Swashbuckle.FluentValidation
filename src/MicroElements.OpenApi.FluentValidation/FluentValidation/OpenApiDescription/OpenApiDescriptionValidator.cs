// Copyright (c) MicroElements. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using FluentValidation;
using FluentValidation.Validators;

namespace MicroElements.OpenApi.FluentValidation.OpenApiDescription
{
    /// <summary>
    /// Annotation-only validator that carries the OpenAPI description of the validated property.
    /// It never fails validation — it exists only to transport the description text through
    /// FluentValidation's rule model (FluentValidation exposes no public accessor for a rule or
    /// its components, so <c>SetValidator</c> is the only way to attach metadata to a rule).
    /// </summary>
    /// <typeparam name="T">Validated object type.</typeparam>
    /// <typeparam name="TProperty">Type of the validated property.</typeparam>
    public sealed class OpenApiDescriptionValidator<T, TProperty> : PropertyValidator<T, TProperty>, IOpenApiDescriptionValidator
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OpenApiDescriptionValidator{T, TProperty}"/> class.
        /// </summary>
        /// <param name="description">Description text for the validated property.</param>
        public OpenApiDescriptionValidator(string description)
        {
            Description = description;
        }

        /// <inheritdoc />
        public string Description { get; }

        /// <inheritdoc />
        public override string Name => "OpenApiDescriptionValidator";

        /// <inheritdoc />
        public override bool IsValid(ValidationContext<T> context, TProperty value) => true;
    }
}
