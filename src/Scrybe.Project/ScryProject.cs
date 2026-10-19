// SPDX-License-Identifier: LGPL-3.0-only

using System.Collections.ObjectModel;

namespace Scrybe.Project;

/// <summary>
/// Represents an authored Scrybe project.
/// </summary>
public sealed class ScryProject
{
	private readonly List<ScryGraph> _graphs = [];
	private readonly ReadOnlyCollection<ScryGraph> _graphView;

	public Guid Id { get; }
	public string Title { get; }

	/// <summary>
	/// Gets the project's graphs in insertion order.
	/// </summary>
	/// <remarks>
	/// The returned collection cannot be modified directly.
	/// Changes made through the project are reflected in the view.
	/// </remarks>
	public IReadOnlyList<ScryGraph> Graphs => _graphView;

	/// <summary>
	/// Initializes a project with an explicit identity and title.
	/// </summary>
	/// <param name="id">The stable project identifier.</param>
	/// <param name="title">The project title.</param>
	/// <exception cref="ArgumentException">
	/// The identifier is empty or the title is blank.
	/// </exception>
	/// <exception cref="ArgumentNullException">
	/// The title is null.
	/// </exception>
	public ScryProject(Guid id, string title)
	{
		if (id == Guid.Empty)
		{
			throw new ArgumentException(
				"Project identity cannot be empty.",
				nameof(id));
		}

		ArgumentException.ThrowIfNullOrWhiteSpace(title);

		Id = id;
		Title = title;

		_graphView = _graphs.AsReadOnly();
	}

	/// <summary>
	/// Adds a graph to the end of the project's graph collection.
	/// </summary>
	/// <param name="graph">The graph to add.</param>
	/// <exception cref="ArgumentNullException">
	/// The graph is null.
	/// </exception>
	/// <exception cref="ArgumentException">
	/// The graph identifier already exists or the graph belongs to a project.
	/// </exception>
	public void AddGraph(ScryGraph graph)
	{
		ArgumentNullException.ThrowIfNull(graph);

		foreach (var existing in _graphs)
		{
			if (existing.Id == graph.Id)
			{
				throw new ArgumentException(
					"A graph with the same identity already exists.",
					nameof(graph));
			}
		}

		if (graph.IsAttached)
		{
			throw new ArgumentException(
				"Graph is already attached to a project.",
				nameof(graph));
		}

		graph.AttachTo(this);
		_graphs.Add(graph);
	}

	/// <summary>
	/// Removes the graph with the specified identifier.
	/// </summary>
	/// <param name="graphId">The graph identifier.</param>
	/// <returns>
	/// True if the graph was removed; otherwise, false.
	/// </returns>
	/// <exception cref="ArgumentException">
	/// The identifier is empty.
	/// </exception>
	public bool RemoveGraph(Guid graphId)
	{
		if (graphId == Guid.Empty)
		{
			throw new ArgumentException(
				"Graph identity cannot be empty.",
				nameof(graphId));
		}

		for (int index = 0; index < _graphs.Count; index++)
		{
			if (_graphs[index].Id == graphId)
			{
				_graphs[index].DetachFrom(this);
				_graphs.RemoveAt(index);
				return true;
			}
		}

		return false;
	}
}
