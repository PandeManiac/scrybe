// SPDX-License-Identifier: LGPL-3.0-only

using Scrybe.Project;
using Xunit;

namespace Scrybe.Project.Tests;

public sealed class ScryPortTests
{
	[Fact]
	public void ConstructorPreservesMetadata()
	{
		var id = Guid.NewGuid();
		var port = new ScryPort(
			id,
			"Next",
			"Flow",
			ScryPortDirection.Output,
			ScryPortCapacity.Many);

		Assert.Equal(id, port.Id);
		Assert.Equal("Next", port.Name);
		Assert.Equal("Flow", port.TypeId);
		Assert.Equal(ScryPortDirection.Output, port.Direction);
		Assert.Equal(ScryPortCapacity.Many, port.Capacity);
	}

	[Fact]
	public void ConstructorRejectsEmptyIdentity()
	{
		var exception = Assert.Throws<ArgumentException>(
			() => CreatePort(id: Guid.Empty));

		Assert.Equal("id", exception.ParamName);
	}

	[Fact]
	public void ConstructorRejectsNullName()
	{
		var exception = Assert.Throws<ArgumentNullException>(
			() => CreatePort(name: null!));

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
			() => CreatePort(name: name));

		Assert.Equal("name", exception.ParamName);
	}

	[Fact]
	public void ConstructorRejectsNullTypeId()
	{
		var exception = Assert.Throws<ArgumentNullException>(
			() => CreatePort(typeId: null!));

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
			() => CreatePort(typeId: typeId));

		Assert.Equal("typeId", exception.ParamName);
	}

	[Fact]
	public void ConstructorPreservesWhitespace()
	{
		var port = CreatePort(name: "  Next  ", typeId: "  Flow  ");

		Assert.Equal("  Next  ", port.Name);
		Assert.Equal("  Flow  ", port.TypeId);
	}

	[Fact]
	public void ConstructorRejectsInvalidDirection()
	{
		var exception = Assert.Throws<ArgumentOutOfRangeException>(
			() => CreatePort(direction: (ScryPortDirection)123));

		Assert.Equal("direction", exception.ParamName);
	}

	[Fact]
	public void ConstructorRejectsInvalidCapacity()
	{
		var exception = Assert.Throws<ArgumentOutOfRangeException>(
			() => CreatePort(capacity: (ScryPortCapacity)123));

		Assert.Equal("capacity", exception.ParamName);
	}

	private static ScryPort CreatePort(
		Guid? id = null,
		string name = "Next",
		string typeId = "Flow",
		ScryPortDirection direction = ScryPortDirection.Output,
		ScryPortCapacity capacity = ScryPortCapacity.One)
	{
		return new ScryPort(
			id ?? Guid.NewGuid(),
			name,
			typeId,
			direction,
			capacity);
	}
}
