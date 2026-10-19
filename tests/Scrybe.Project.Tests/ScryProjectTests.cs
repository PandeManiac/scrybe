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

	[Fact]
	public void GraphsInitiallyEmpty()
	{
		var project = new ScryProject(Guid.NewGuid(), "Example Project");
		Assert.Empty(project.Graphs);
	}

	[Fact]
	public void AddGraphPreservesInsertionOrder()
	{
		var project = new ScryProject(Guid.NewGuid(), "Example Project");
		var first = new ScryGraph(Guid.NewGuid(), "Introduction");
		var second = new ScryGraph(Guid.NewGuid(), "Conversation");
		var third = new ScryGraph(Guid.NewGuid(), "Conclusion");

		project.AddGraph(first);
		project.AddGraph(second);
		project.AddGraph(third);

		Assert.Equal(3, project.Graphs.Count);

		Assert.Same(first, project.Graphs[0]);
		Assert.Same(second, project.Graphs[1]);
		Assert.Same(third, project.Graphs[2]);
	}

	[Fact]
	public void AddGraphRejectsNull()
	{
		var project = new ScryProject(Guid.NewGuid(), "Example Project");

		var exception = Assert.Throws<ArgumentNullException>(
			() => project.AddGraph(null!));

		Assert.Equal("graph", exception.ParamName);
		Assert.Empty(project.Graphs);
	}

	[Fact]
	public void AddGraphRejectsDuplicateIdentity()
	{
		var project = new ScryProject(Guid.NewGuid(), "Example Project");
		var id = Guid.NewGuid();
		var first = new ScryGraph(id, "Introduction");
		var second = new ScryGraph(id, "Different Name");

		project.AddGraph(first);

		var exception = Assert.Throws<ArgumentException>(
			() => project.AddGraph(second));

		Assert.Equal("graph", exception.ParamName);

		Assert.Single(project.Graphs);
		Assert.Same(first, project.Graphs[0]);
	}

	[Fact]
	public void AddGraphRejectsSameInstanceTwice()
	{
		var project = new ScryProject(Guid.NewGuid(), "Example Project");
		var graph = new ScryGraph(Guid.NewGuid(), "Introduction");

		project.AddGraph(graph);

		Assert.Throws<ArgumentException>(
			() => project.AddGraph(graph));

		Assert.Single(project.Graphs);
	}

	[Fact]
	public void AddGraphAllowsDuplicateNames()
	{
		var project = new ScryProject(Guid.NewGuid(), "Example Project");
		var first = new ScryGraph(Guid.NewGuid(), "Conversation");
		var second = new ScryGraph(Guid.NewGuid(), "Conversation");

		project.AddGraph(first);
		project.AddGraph(second);

		Assert.Equal(2, project.Graphs.Count);

		Assert.NotEqual(project.Graphs[0].Id, project.Graphs[1].Id);
		Assert.Equal(project.Graphs[0].Name, project.Graphs[1].Name);
	}

	[Fact]
	public void GraphsCannotBeModifiedExternally()
	{
		var project = new ScryProject(Guid.NewGuid(), "Example Project");
		var graph = new ScryGraph(Guid.NewGuid(), "Introduction");

		project.AddGraph(graph);

		var externalView = Assert.IsAssignableFrom<IList<ScryGraph>>(
			project.Graphs);

		Assert.Throws<NotSupportedException>(
			() => externalView.Add(
				new ScryGraph(Guid.NewGuid(), "Unauthorized")));

		Assert.Throws<NotSupportedException>(
			() => externalView.Clear());

		Assert.Single(project.Graphs);
		Assert.Same(graph, project.Graphs[0]);
	}

	[Fact]
	public void GraphsViewReflectsProjectMutations()
	{
		var project = new ScryProject(Guid.NewGuid(), "Example Project");
		var view = project.Graphs;
		var graph = new ScryGraph(Guid.NewGuid(), "Introduction");

		Assert.Empty(view);

		project.AddGraph(graph);

		Assert.Single(view);
		Assert.Same(graph, view[0]);

		project.RemoveGraph(graph.Id);

		Assert.Empty(view);
		Assert.Same(view, project.Graphs);
	}

	[Fact]
	public void RemoveGraphRemovesMatchingIdentity()
	{
		var project = new ScryProject(Guid.NewGuid(), "Example Project");
		var first = new ScryGraph(Guid.NewGuid(), "Introduction");
		var second = new ScryGraph(Guid.NewGuid(), "Conversation");

		project.AddGraph(first);
		project.AddGraph(second);

		var removed = project.RemoveGraph(first.Id);

		Assert.True(removed);
		Assert.Single(project.Graphs);
		Assert.Same(second, project.Graphs[0]);
	}

	[Fact]
	public void RemoveGraphPreservesRemainingOrder()
	{
		var project = new ScryProject(Guid.NewGuid(), "Example Project");
		var first = new ScryGraph(Guid.NewGuid(), "Introduction");
		var second = new ScryGraph(Guid.NewGuid(), "Conversation");
		var third = new ScryGraph(Guid.NewGuid(), "Conclusion");

		project.AddGraph(first);
		project.AddGraph(second);
		project.AddGraph(third);

		var removed = project.RemoveGraph(second.Id);

		Assert.True(removed);
		Assert.Equal(2, project.Graphs.Count);

		Assert.Same(first, project.Graphs[0]);
		Assert.Same(third, project.Graphs[1]);
	}

	[Fact]
	public void RemoveGraphReturnsFalseForUnknownIdentity()
	{
		var project = new ScryProject(Guid.NewGuid(), "Example Project");
		var graph = new ScryGraph(Guid.NewGuid(), "Introduction");

		project.AddGraph(graph);
		var removed = project.RemoveGraph(Guid.NewGuid());

		Assert.False(removed);
		Assert.Single(project.Graphs);
		Assert.Same(graph, project.Graphs[0]);
	}

	[Fact]
	public void RemoveGraphRejectsEmptyIdentity()
	{
		var project = new ScryProject(Guid.NewGuid(), "Example Project");

		var exception = Assert.Throws<ArgumentException>(
			() => project.RemoveGraph(Guid.Empty));

		Assert.Equal("graphId", exception.ParamName);
	}
}
