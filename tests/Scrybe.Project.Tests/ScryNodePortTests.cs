// SPDX-License-Identifier: LGPL-3.0-only

using Scrybe.Project;
using Xunit;

namespace Scrybe.Project.Tests;

public sealed class ScryNodePortTests
{
	[Fact]
	public void PortsInitiallyEmpty()
	{
		var node = new ScryNode(Guid.NewGuid(), "Dialogue");
		Assert.Empty(node.Ports);
	}

	[Fact]
	public void AddPortPreservesInsertionOrder()
	{
		var node = new ScryNode(Guid.NewGuid(), "Dialogue");
		var first = CreatePort("In", ScryPortDirection.Input);
		var second = CreatePort("Next", ScryPortDirection.Output);
		var third = CreatePort("Alternative", ScryPortDirection.Output);

		node.AddPort(first);
		node.AddPort(second);
		node.AddPort(third);

		Assert.Equal(3, node.Ports.Count);
		Assert.Same(first, node.Ports[0]);
		Assert.Same(second, node.Ports[1]);
		Assert.Same(third, node.Ports[2]);
	}

	[Fact]
	public void AddPortRejectsNull()
	{
		var node = new ScryNode(Guid.NewGuid(), "Dialogue");

		var exception = Assert.Throws<ArgumentNullException>(
			() => node.AddPort(null!));

		Assert.Equal("port", exception.ParamName);
		Assert.Empty(node.Ports);
	}

	[Fact]
	public void AddPortRejectsDuplicateIdentity()
	{
		var node = new ScryNode(Guid.NewGuid(), "Dialogue");
		var id = Guid.NewGuid();
		var first = CreatePort("First", id: id);
		var second = CreatePort("Second", id: id);

		node.AddPort(first);

		var exception = Assert.Throws<ArgumentException>(
			() => node.AddPort(second));

		Assert.Equal("port", exception.ParamName);
		Assert.Single(node.Ports);
		Assert.Same(first, node.Ports[0]);
	}

	[Fact]
	public void AddPortRejectsSameInstanceTwice()
	{
		var node = new ScryNode(Guid.NewGuid(), "Dialogue");
		var port = CreatePort("Next");

		node.AddPort(port);

		Assert.Throws<ArgumentException>(
			() => node.AddPort(port));

		Assert.Single(node.Ports);
	}

	[Fact]
	public void AddPortAllowsDuplicateNames()
	{
		var node = new ScryNode(Guid.NewGuid(), "Dialogue");
		var first = CreatePort("Next");
		var second = CreatePort("Next");

		node.AddPort(first);
		node.AddPort(second);

		Assert.Equal(2, node.Ports.Count);
		Assert.NotEqual(node.Ports[0].Id, node.Ports[1].Id);
	}

	[Fact]
	public void PortsCannotBeModifiedExternally()
	{
		var node = new ScryNode(Guid.NewGuid(), "Dialogue");
		var port = CreatePort("Next");

		node.AddPort(port);

		var view = Assert.IsAssignableFrom<IList<ScryPort>>(node.Ports);

		Assert.Throws<NotSupportedException>(
			() => view.Add(CreatePort("Unauthorized")));

		Assert.Throws<NotSupportedException>(
			() => view.Clear());

		Assert.Single(node.Ports);
		Assert.Same(port, node.Ports[0]);
	}

	[Fact]
	public void PortsViewReflectsNodeMutations()
	{
		var node = new ScryNode(Guid.NewGuid(), "Dialogue");
		var view = node.Ports;
		var port = CreatePort("Next");

		Assert.Empty(view);

		node.AddPort(port);

		Assert.Single(view);
		Assert.Same(port, view[0]);

		node.RemovePort(port.Id);

		Assert.Empty(view);
		Assert.Same(view, node.Ports);
	}

	[Fact]
	public void RemovePortPreservesRemainingOrder()
	{
		var node = new ScryNode(Guid.NewGuid(), "Dialogue");
		var first = CreatePort("First");
		var second = CreatePort("Second");
		var third = CreatePort("Third");

		node.AddPort(first);
		node.AddPort(second);
		node.AddPort(third);

		var removed = node.RemovePort(second.Id);

		Assert.True(removed);
		Assert.Equal(2, node.Ports.Count);
		Assert.Same(first, node.Ports[0]);
		Assert.Same(third, node.Ports[1]);
	}

	[Fact]
	public void RemovePortReturnsFalseForUnknownIdentity()
	{
		var node = new ScryNode(Guid.NewGuid(), "Dialogue");
		var port = CreatePort("Next");

		node.AddPort(port);
		var removed = node.RemovePort(Guid.NewGuid());

		Assert.False(removed);
		Assert.Single(node.Ports);
		Assert.Same(port, node.Ports[0]);
	}

	[Fact]
	public void RemovePortRejectsEmptyIdentity()
	{
		var node = new ScryNode(Guid.NewGuid(), "Dialogue");

		var exception = Assert.Throws<ArgumentException>(
			() => node.RemovePort(Guid.Empty));

		Assert.Equal("portId", exception.ParamName);
	}

	[Fact]
	public void AddPortRejectsPortOwnedByAnotherNode()
	{
		var first = new ScryNode(Guid.NewGuid(), "Dialogue");
		var second = new ScryNode(Guid.NewGuid(), "Dialogue");
		var port = CreatePort("Next");

		first.AddPort(port);

		var exception = Assert.Throws<ArgumentException>(
			() => second.AddPort(port));

		Assert.Equal("port", exception.ParamName);
		Assert.Single(first.Ports);
		Assert.Empty(second.Ports);
	}

	[Fact]
	public void RemovePortAllowsMovingToAnotherNode()
	{
		var first = new ScryNode(Guid.NewGuid(), "Dialogue");
		var second = new ScryNode(Guid.NewGuid(), "Dialogue");
		var port = CreatePort("Next");

		first.AddPort(port);

		Assert.True(first.RemovePort(port.Id));

		second.AddPort(port);

		Assert.Empty(first.Ports);
		Assert.Single(second.Ports);
		Assert.Same(port, second.Ports[0]);
	}

	[Fact]
	public void MovingNodeBetweenGraphsPreservesPorts()
	{
		var first = new ScryGraph(Guid.NewGuid(), "First");
		var second = new ScryGraph(Guid.NewGuid(), "Second");
		var node = new ScryNode(Guid.NewGuid(), "Dialogue");
		var port = CreatePort("Next");

		node.AddPort(port);
		first.AddNode(node);

		Assert.True(first.RemoveNode(node.Id));

		second.AddNode(node);

		Assert.Empty(first.Nodes);
		Assert.Single(second.Nodes);
		Assert.Same(node, second.Nodes[0]);
		Assert.Single(node.Ports);
		Assert.Same(port, node.Ports[0]);
	}

	[Fact]
	public void DifferentNodesAllowMatchingPortIdentifiers()
	{
		var first = new ScryNode(Guid.NewGuid(), "Dialogue");
		var second = new ScryNode(Guid.NewGuid(), "Dialogue");
		var id = Guid.NewGuid();

		first.AddPort(CreatePort("Next", id: id));
		second.AddPort(CreatePort("Next", id: id));

		Assert.Single(first.Ports);
		Assert.Single(second.Ports);
		Assert.Equal(first.Ports[0].Id, second.Ports[0].Id);
		Assert.NotSame(first.Ports[0], second.Ports[0]);
	}

	private static ScryPort CreatePort(
		string name,
		ScryPortDirection direction = ScryPortDirection.Output,
		Guid? id = null)
	{
		return new ScryPort(
			id ?? Guid.NewGuid(),
			name,
			"Flow",
			direction,
			ScryPortCapacity.One);
	}
}
