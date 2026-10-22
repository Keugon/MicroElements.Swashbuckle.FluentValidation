// Copyright (c) MicroElements. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using FluentAssertions;
using FluentValidation.Results;
using MicroElements.AspNetCore.OpenApi.FluentValidation;
using MicroElements.OpenApi.FluentValidation.OpenApiDescription;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Text.Json;
using Xunit;

namespace MicroElements.AspNetCore.OpenApi.FluentValidation.Tests;

/// <summary>
/// Custom property descriptions attached to the rule with '.WithOpenApiDescription(...)'.
/// </summary>
public class OpenApiDescriptionTests : IClassFixture<OpenApiDescriptionTests.TestWebApplicationFactory>
{
  private readonly TestWebApplicationFactory _factory;

  public OpenApiDescriptionTests(TestWebApplicationFactory factory) => this._factory = factory;

  [Fact]
  public async Task WithOpenApiDescription_ShouldBeWrittenToPropertyDescription()
  {
    JsonElement schemas = await this.GetSchemasAsync();

    JsonElement model = schemas.GetProperty("TestDescriptionModel");
    JsonElement invoiceNumber = model.GetProperty("properties").GetProperty("invoiceNumber");

    invoiceNumber.GetProperty("description").GetString()
        .Should().Be("Invoice number, format 'RE-2024-0001'.");
  }

  [Fact]
  public async Task WithOpenApiDescription_ShouldNotAffectOtherConstraints()
  {
    JsonElement schemas = await this.GetSchemasAsync();

    JsonElement model = schemas.GetProperty("TestDescriptionModel");
    JsonElement invoiceNumber = model.GetProperty("properties").GetProperty("invoiceNumber");

    // NotEmpty => minLength: 1
    invoiceNumber.GetProperty("minLength").GetInt32().Should().Be(1);
  }

  [Fact]
  public async Task PropertyWithoutDescription_ShouldNotHaveDescription()
  {
    JsonElement schemas = await this.GetSchemasAsync();

    JsonElement model = schemas.GetProperty("TestDescriptionModel");
    JsonElement plain = model.GetProperty("properties").GetProperty("plain");

    plain.TryGetProperty("description", out _).Should().BeFalse();
  }

  [Fact]
  public async Task TwoDescriptions_ShouldBothBeKept()
  {
    JsonElement schemas = await this.GetSchemasAsync();

    JsonElement model = schemas.GetProperty("TestDescriptionModel");
    JsonElement twoNotes = model.GetProperty("properties").GetProperty("twoNotes");

    twoNotes.GetProperty("description").GetString()
        .Should().Be("First note.\nSecond note.");
  }

  [Fact]
  public async Task WithOpenApiDescription_ShouldBeCopiedToQueryParameter()
  {
    HttpClient client = this._factory.CreateClient();
    HttpResponseMessage response = await client.GetAsync("/openapi/v1.json");
    response.EnsureSuccessStatusCode();
    var doc = JsonDocument.Parse(await response.Content.ReadAsStringAsync());

    JsonElement parameters = doc.RootElement.GetProperty("paths")
            .GetProperty("/api/description-search").GetProperty("get").GetProperty("parameters");

    JsonElement search = parameters.EnumerateArray()
            .First(p => p.GetProperty("name").GetString() == "Search");

    search.GetProperty("schema").GetProperty("description").GetString()
        .Should().Be("Search term.");
  }

    [Fact]
    public async Task WithOpenApiDescription_ShouldBeAppendedToExistingDescription()
    {
        var schemas = await GetSchemasAsync();

        var model = schemas.GetProperty("TestXmlDocDescriptionModel");
        var invoiceNumber = model.GetProperty("properties").GetProperty("invoiceNumber");

        var description = invoiceNumber.GetProperty("description").GetString();

#if NET10_0_OR_GREATER
        // The XML doc summary is in the schema description (Microsoft.AspNetCore.OpenApi reads XML
        // comments starting with ASP.NET Core 10) — the custom text is appended to it.
        description.Should().Be("Die Rechnungsnummer.\nFormat 'RE-2024-0001'.");
#else
        // .NET 9: Microsoft.AspNetCore.OpenApi does not read XML doc comments, so there is no
        // existing description to append to.
        description.Should().Be("Format 'RE-2024-0001'.");
#endif
    }

    [Fact]
    public void WithOpenApiDescription_ShouldNotAffectValidation()
  {
    // The description rule is annotation-only: it must never produce a validation failure.
    var validator = new TestDescriptionModelValidator();

    ValidationResult result = validator.Validate(new TestDescriptionModel
    {
      InvoiceNumber = "RE-2024-0001",
      Plain = "text",
      TwoNotes = "text",
    });

    result.IsValid.Should().BeTrue();
  }

    [Fact]
    public void Append_ShouldCombineDescriptions()
    {
        OpenApiDescriptionBuilder.Append(null, "New text.").Should().Be("New text.");
        OpenApiDescriptionBuilder.Append("Existing text.", "New text.").Should().Be("Existing text.\nNew text.");

        // Idempotent: the same text is not appended twice.
        OpenApiDescriptionBuilder.Append("Existing text.", "Existing text.").Should().Be("Existing text.");
    }

  private async Task<JsonElement> GetSchemasAsync()
  {
    HttpClient client = this._factory.CreateClient();
    HttpResponseMessage response = await client.GetAsync("/openapi/v1.json");
    response.EnsureSuccessStatusCode();
    var json = await response.Content.ReadAsStringAsync();
    var doc = JsonDocument.Parse(json);
    return doc.RootElement.GetProperty("components").GetProperty("schemas");
  }

  // ----- Test App -----

  public class TestWebApplicationFactory : WebApplicationFactory<TestMarker>
  {
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
      builder.UseContentRoot(AppContext.BaseDirectory);
      builder.UseEnvironment("Testing");
    }
  }
}
