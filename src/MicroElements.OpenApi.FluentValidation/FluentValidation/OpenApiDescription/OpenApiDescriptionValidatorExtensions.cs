// Copyright (c) MicroElements. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using FluentValidation;

namespace MicroElements.OpenApi.FluentValidation.OpenApiDescription
{
    /// <summary>
    /// FluentValidation rule builder extensions for attaching a custom OpenAPI description to a property.
    /// </summary>
    public static class OpenApiDescriptionValidatorExtensions
    {
        /// <summary>
        /// Attaches a custom description to the validated property. The text is written into the
        /// <c>description</c> of the property schema in the generated OpenAPI document.
        /// </summary>
        /// <remarks>
        /// The rule is annotation-only: it never fails validation and does not change the runtime behaviour.
        /// </remarks>
        /// <typeparam name="T">Validated object type.</typeparam>
        /// <typeparam name="TProperty">Type of the validated property.</typeparam>
        /// <param name="ruleBuilder">Rule builder.</param>
        /// <param name="description">Description text.</param>
        /// <returns>Rule builder options for chaining.</returns>
        public static IRuleBuilderOptions<T, TProperty> WithOpenApiDescription<T, TProperty>(
            this IRuleBuilder<T, TProperty> ruleBuilder,
            string description)
        {
            if (ruleBuilder is null)
                throw new ArgumentNullException(nameof(ruleBuilder));

            if (string.IsNullOrWhiteSpace(description))
                throw new ArgumentException("Description must not be empty.", nameof(description));

            return ruleBuilder.SetValidator(new OpenApiDescriptionValidator<T, TProperty>(description));
        }
    }
}
