// SPDX-License-Identifier: LGPL-3.0-only

using Scrybe.Project;
using Xunit;

namespace Scrybe.Project.Tests;

public sealed class ScryProjectOwnershipTests
{
	[Fact]
	public void AddGraphRejectsGraphOwnedByAnotherProject()
	{
		var first = new ScryProject(Guid.NewGuid(), "First Project");
		var second = new ScryProject(Guid.NewGuid(), "Second Project");
		var graph = new ScryGraph(Guid.NewGuid(), "Introduction");

		first.AddGraph(graph);

		var exception = Assert.Throws<ArgumentException>(
			() => second.AddGraph(graph));

		Assert.Equal("graph", exception.ParamName);
		Assert.Single(first.Graphs);
		Assert.Empty(second.Graphs);
		Assert.Same(graph, first.Graphs[0]);
	}

	[Fact]
	public void RemoveGraphAllowsMovingToAnotherProject()
	{
		var first = new ScryProject(Guid.NewGuid(), "First Project");
		var second = new ScryProject(Guid.NewGuid(), "Second Project");
		var graph = new ScryGraph(Guid.NewGuid(), "Introduction");

		first.AddGraph(graph);

		Assert.True(first.RemoveGraph(graph.Id));

		second.AddGraph(graph);

		Assert.Empty(first.Graphs);
		Assert.Single(second.Graphs);
		Assert.Same(graph, second.Graphs[0]);
	}

	[Fact]
	public void MovingGraphPreservesItsNodes()
	{
		var first = new ScryProject(Guid.NewGuid(), "First Project");
		var second = new ScryProject(Guid.NewGuid(), "Second Project");
		var graph = new ScryGraph(Guid.NewGuid(), "Introduction");
		var node = new ScryNode(Guid.NewGuid(), "Dialogue");

		graph.AddNode(node);
		first.AddGraph(graph);

		Assert.True(first.RemoveGraph(graph.Id));

		second.AddGraph(graph);

		Assert.Single(second.Graphs);
		Assert.Single(graph.Nodes);
		Assert.Same(node, graph.Nodes[0]);

		Assert.Throws<ArgumentException>(
			() => new ScryGraph(Guid.NewGuid(), "Other").AddNode(node));
	}
}
