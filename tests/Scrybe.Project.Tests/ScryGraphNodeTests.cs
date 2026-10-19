// SPDX-License-Identifier: LGPL-3.0-only

using Scrybe.Project;
using Xunit;

namespace Scrybe.Project.Tests;

public sealed class ScryGraphNodeTests
{
	[Fact]
	public void NodesInitiallyEmpty()
	{
		var graph = new ScryGraph(Guid.NewGuid(), "Introduction");
		Assert.Empty(graph.Nodes);
	}

	[Fact]
	public void AddNodePreservesInsertionOrder()
	{
		var graph = new ScryGraph(Guid.NewGuid(), "Introduction");
		var first = new ScryNode(Guid.NewGuid(), "Start");
		var second = new ScryNode(Guid.NewGuid(), "Dialogue");
		var third = new ScryNode(Guid.NewGuid(), "End");

		graph.AddNode(first);
		graph.AddNode(second);
		graph.AddNode(third);

		Assert.Equal(3, graph.Nodes.Count);
		Assert.Same(first, graph.Nodes[0]);
		Assert.Same(second, graph.Nodes[1]);
		Assert.Same(third, graph.Nodes[2]);
	}

	[Fact]
	public void AddNodeRejectsNull()
	{
		var graph = new ScryGraph(Guid.NewGuid(), "Introduction");

		var exception = Assert.Throws<ArgumentNullException>(
			() => graph.AddNode(null!));

		Assert.Equal("node", exception.ParamName);
		Assert.Empty(graph.Nodes);
	}

	[Fact]
	public void AddNodeRejectsDuplicateIdentity()
	{
		var graph = new ScryGraph(Guid.NewGuid(), "Introduction");
		var id = Guid.NewGuid();
		var first = new ScryNode(id, "Dialogue");
		var second = new ScryNode(id, "Choice");

		graph.AddNode(first);

		var exception = Assert.Throws<ArgumentException>(
			() => graph.AddNode(second));

		Assert.Equal("node", exception.ParamName);
		Assert.Single(graph.Nodes);
		Assert.Same(first, graph.Nodes[0]);
	}

	[Fact]
	public void AddNodeRejectsSameInstanceTwice()
	{
		var graph = new ScryGraph(Guid.NewGuid(), "Introduction");
		var node = new ScryNode(Guid.NewGuid(), "Dialogue");

		graph.AddNode(node);

		Assert.Throws<ArgumentException>(
			() => graph.AddNode(node));

		Assert.Single(graph.Nodes);
	}

	[Fact]
	public void AddNodeAllowsDuplicateTypeIds()
	{
		var graph = new ScryGraph(Guid.NewGuid(), "Introduction");
		var first = new ScryNode(Guid.NewGuid(), "Dialogue");
		var second = new ScryNode(Guid.NewGuid(), "Dialogue");

		graph.AddNode(first);
		graph.AddNode(second);

		Assert.Equal(2, graph.Nodes.Count);
		Assert.NotEqual(graph.Nodes[0].Id, graph.Nodes[1].Id);
		Assert.Equal(graph.Nodes[0].TypeId, graph.Nodes[1].TypeId);
	}

	[Fact]
	public void NodesCannotBeModifiedExternally()
	{
		var graph = new ScryGraph(Guid.NewGuid(), "Introduction");
		var node = new ScryNode(Guid.NewGuid(), "Dialogue");

		graph.AddNode(node);

		var externalView = Assert.IsAssignableFrom<IList<ScryNode>>(
			graph.Nodes);

		Assert.Throws<NotSupportedException>(
			() => externalView.Add(
				new ScryNode(Guid.NewGuid(), "Unauthorized")));

		Assert.Throws<NotSupportedException>(
			() => externalView.Clear());

		Assert.Single(graph.Nodes);
		Assert.Same(node, graph.Nodes[0]);
	}

	[Fact]
	public void NodesViewReflectsGraphMutations()
	{
		var graph = new ScryGraph(Guid.NewGuid(), "Introduction");
		var view = graph.Nodes;
		var node = new ScryNode(Guid.NewGuid(), "Dialogue");

		Assert.Empty(view);

		graph.AddNode(node);

		Assert.Single(view);
		Assert.Same(node, view[0]);

		graph.RemoveNode(node.Id);

		Assert.Empty(view);
		Assert.Same(view, graph.Nodes);
	}

	[Fact]
	public void RemoveNodeRemovesMatchingIdentity()
	{
		var graph = new ScryGraph(Guid.NewGuid(), "Introduction");
		var first = new ScryNode(Guid.NewGuid(), "Start");
		var second = new ScryNode(Guid.NewGuid(), "Dialogue");

		graph.AddNode(first);
		graph.AddNode(second);

		var removed = graph.RemoveNode(first.Id);

		Assert.True(removed);
		Assert.Single(graph.Nodes);
		Assert.Same(second, graph.Nodes[0]);
	}

	[Fact]
	public void RemoveNodePreservesRemainingOrder()
	{
		var graph = new ScryGraph(Guid.NewGuid(), "Introduction");
		var first = new ScryNode(Guid.NewGuid(), "Start");
		var second = new ScryNode(Guid.NewGuid(), "Dialogue");
		var third = new ScryNode(Guid.NewGuid(), "End");

		graph.AddNode(first);
		graph.AddNode(second);
		graph.AddNode(third);

		var removed = graph.RemoveNode(second.Id);

		Assert.True(removed);
		Assert.Equal(2, graph.Nodes.Count);
		Assert.Same(first, graph.Nodes[0]);
		Assert.Same(third, graph.Nodes[1]);
	}

	[Fact]
	public void RemoveNodeReturnsFalseForUnknownIdentity()
	{
		var graph = new ScryGraph(Guid.NewGuid(), "Introduction");
		var node = new ScryNode(Guid.NewGuid(), "Dialogue");

		graph.AddNode(node);
		var removed = graph.RemoveNode(Guid.NewGuid());

		Assert.False(removed);
		Assert.Single(graph.Nodes);
		Assert.Same(node, graph.Nodes[0]);
	}

	[Fact]
	public void RemoveNodeRejectsEmptyIdentity()
	{
		var graph = new ScryGraph(Guid.NewGuid(), "Introduction");

		var exception = Assert.Throws<ArgumentException>(
			() => graph.RemoveNode(Guid.Empty));

		Assert.Equal("nodeId", exception.ParamName);
	}

	[Fact]
	public void RemoveNodeAllowsIdentityReuse()
	{
		var graph = new ScryGraph(Guid.NewGuid(), "Introduction");
		var id = Guid.NewGuid();
		var first = new ScryNode(id, "Dialogue");
		var second = new ScryNode(id, "Choice");

		graph.AddNode(first);
		Assert.True(graph.RemoveNode(id));

		graph.AddNode(second);

		Assert.Single(graph.Nodes);
		Assert.Same(second, graph.Nodes[0]);
	}

	[Fact]
	public void AddNodeRejectsNodeOwnedByAnotherGraph()
	{
		var first = new ScryGraph(Guid.NewGuid(), "First");
		var second = new ScryGraph(Guid.NewGuid(), "Second");
		var node = new ScryNode(Guid.NewGuid(), "Dialogue");

		first.AddNode(node);

		var exception = Assert.Throws<ArgumentException>(
			() => second.AddNode(node));

		Assert.Equal("node", exception.ParamName);
		Assert.Single(first.Nodes);
		Assert.Empty(second.Nodes);
		Assert.Same(node, first.Nodes[0]);
	}

	[Fact]
	public void RemoveNodeAllowsMovingToAnotherGraph()
	{
		var first = new ScryGraph(Guid.NewGuid(), "First");
		var second = new ScryGraph(Guid.NewGuid(), "Second");
		var node = new ScryNode(Guid.NewGuid(), "Dialogue");

		first.AddNode(node);

		Assert.True(first.RemoveNode(node.Id));

		second.AddNode(node);

		Assert.Empty(first.Nodes);
		Assert.Single(second.Nodes);
		Assert.Same(node, second.Nodes[0]);
	}

	[Fact]
	public void DifferentGraphsAllowMatchingNodeIdentifiers()
	{
		var first = new ScryGraph(Guid.NewGuid(), "First");
		var second = new ScryGraph(Guid.NewGuid(), "Second");
		var id = Guid.NewGuid();

		var firstNode = new ScryNode(id, "Dialogue");
		var secondNode = new ScryNode(id, "Choice");

		first.AddNode(firstNode);
		second.AddNode(secondNode);

		Assert.Single(first.Nodes);
		Assert.Single(second.Nodes);
		Assert.Equal(first.Nodes[0].Id, second.Nodes[0].Id);
		Assert.NotSame(first.Nodes[0], second.Nodes[0]);
	}
}
