// SPDX-License-Identifier: LGPL-3.0-only

using Scrybe.Project;
using Xunit;

namespace Scrybe.Project.Tests;

public sealed class ScryNodeTests
{
	[Fact]
	public void ConstructorPreservesIdentityAndTypeId()
	{
		var id = Guid.Parse("01234567-89ab-cdef-0123-456789abcdef");
		var node = new ScryNode(id, "Dialogue");

		Assert.Equal(id, node.Id);
		Assert.Equal("Dialogue", node.TypeId);
	}

	[Fact]
	public void ConstructorRejectsEmptyIdentity()
	{
		var exception = Assert.Throws<ArgumentException>(
			() => new ScryNode(Guid.Empty, "Dialogue"));

		Assert.Equal("id", exception.ParamName);
	}

	[Fact]
	public void ConstructorRejectsNullTypeId()
	{
		var exception = Assert.Throws<ArgumentNullException>(
			() => new ScryNode(Guid.NewGuid(), null!));

		Assert.Equal("typeId", exception.ParamName);
	}

	[Theory]
	[InlineData("")]
	[InlineData(" ")]
	[InlineData("\t")]
	[InlineData("\r\n")]
	public void ConstructorRejectsBlankTypeId(string typeId)
	{
		var exception = Assert.Throws<ArgumentException>(
			() => new ScryNode(Guid.NewGuid(), typeId));

		Assert.Equal("typeId", exception.ParamName);
	}

	[Fact]
	public void ConstructorPreservesTypeIdWhitespace()
	{
		const string typeId = "  Dialogue  ";
		var node = new ScryNode(Guid.NewGuid(), typeId);
		Assert.Equal(typeId, node.TypeId);
	}
}
