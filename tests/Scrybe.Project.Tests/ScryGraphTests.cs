// SPDX-License-Identifier: LGPL-3.0-only

using Scrybe.Project;
using Xunit;

namespace Scrybe.Project.Tests;

public sealed class ScryGraphTests
{
	[Fact]
	public void ConstructorPreservesIdentityAndName()
	{
		var id = Guid.Parse("01234567-89ab-cdef-0123-456789abcdef");
		var graph = new ScryGraph(id, "Introduction");

		Assert.Equal(id, graph.Id);
		Assert.Equal("Introduction", graph.Name);
	}

	[Fact]
	public void ConstructorRejectsEmptyIdentity()
	{
		var exception = Assert.Throws<ArgumentException>(
			() => new ScryGraph(Guid.Empty, "Introduction"));

		Assert.Equal("id", exception.ParamName);
	}

	[Fact]
	public void ConstructorRejectsNullName()
	{
		var exception = Assert.Throws<ArgumentNullException>(
			() => new ScryGraph(Guid.NewGuid(), null!));

		Assert.Equal("name", exception.ParamName);
	}

	[Theory]
	[InlineData("")]
	[InlineData(" ")]
	[InlineData("\t")]
	[InlineData("\r\n")]
	public void ConstructorRejectsBlankName(string name)
	{
		var exception = Assert.Throws<ArgumentException>(
			() => new ScryGraph(Guid.NewGuid(), name));

		Assert.Equal("name", exception.ParamName);
	}

	[Fact]
	public void ConstructorPreservesNameWhitespace()
	{
		const string name = "  Introduction  ";
		var graph = new ScryGraph(Guid.NewGuid(), name);
		Assert.Equal(name, graph.Name);
	}
}
