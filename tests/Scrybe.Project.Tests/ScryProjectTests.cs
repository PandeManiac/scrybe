// SPDX-License-Identifier: LGPL-3.0-only

using Scrybe.Project;
using Xunit;

namespace Scrybe.Project.Tests;

public sealed class ScryProjectTests
{
	[Fact]
	public void ConstructorPreservesIdentityAndTitle()
	{
		var id = Guid.Parse("01234567-89ab-cdef-0123-456789abcdef");
		var project = new ScryProject(id, "Example Project");

		Assert.Equal(id, project.Id);
		Assert.Equal("Example Project", project.Title);
	}

	[Fact]
	public void ConstructorRejectsEmptyIdentity()
	{
		var exception = Assert.Throws<ArgumentException>(
			() => new ScryProject(Guid.Empty, "Example Project"));

		Assert.Equal("id", exception.ParamName);
	}

	[Fact]
	public void ConstructorRejectsNullTitle()
	{
		var exception = Assert.Throws<ArgumentNullException>(
			() => new ScryProject(Guid.NewGuid(), null!));

		Assert.Equal("title", exception.ParamName);
	}

	[Theory]
	[InlineData("")]
	[InlineData(" ")]
	[InlineData("\t")]
	[InlineData("\r\n")]
	public void ConstructorRejectsBlankTitle(string title)
	{
		var exception = Assert.Throws<ArgumentException>(
			() => new ScryProject(Guid.NewGuid(), title));

		Assert.Equal("title", exception.ParamName);
	}

	[Fact]
	public void ConstructorPreservesTitleWhitespace()
	{
		const string title = "  Example Project  ";
		var project = new ScryProject(Guid.NewGuid(), title);
		Assert.Equal(title, project.Title);
	}
}
