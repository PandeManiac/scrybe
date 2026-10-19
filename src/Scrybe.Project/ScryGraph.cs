// SPDX-License-Identifier: LGPL-3.0-only

using System.Collections.ObjectModel;

namespace Scrybe.Project;

/// <summary>
/// Represents an authored narrative graph.
/// </summary>
public sealed class ScryGraph
{
	private readonly List<ScryNode> _nodes = [];
	private readonly ReadOnlyCollection<ScryNode> _nodeView;
	private ScryProject? _owner;

	public Guid Id { get; }
	public string Name { get; }

	/// <summary>
	/// Gets the graph's nodes in insertion order.
	/// </summary>
	/// <remarks>
	/// The returned collection cannot be modified directly.
	/// Changes made through the graph are reflected in the view.
	/// </remarks>
	public IReadOnlyList<ScryNode> Nodes => _nodeView;

	internal bool IsAttached => _owner is not null;

	/// <summary>
	/// Initializes a graph with an explicit identity and name.
	/// </summary>
	/// <param name="id">The stable graph identifier.</param>
	/// <param name="name">The graph name.</param>
	/// <exception cref="ArgumentException">
	/// The identifier is empty or the name is blank.
	/// </exception>
	/// <exception cref="ArgumentNullException">
	/// The name is null.
	/// </exception>
	public ScryGraph(Guid id, string name)
	{
		if (id == Guid.Empty)
		{
			throw new ArgumentException(
				"Graph identity cannot be empty.",
				nameof(id));
		}

		ArgumentException.ThrowIfNullOrWhiteSpace(name);

		Id = id;
		Name = name;

		_nodeView = _nodes.AsReadOnly();
	}

	/// <summary>
	/// Adds a node to the end of the graph's node collection.
	/// </summary>
	/// <param name="node">The node to add.</param>
	/// <exception cref="ArgumentNullException">
	/// The node is null.
	/// </exception>
	/// <exception cref="ArgumentException">
	/// The node identifier already exists or the node belongs to a graph.
	/// </exception>
	public void AddNode(ScryNode node)
	{
		ArgumentNullException.ThrowIfNull(node);

		foreach (var existing in _nodes)
		{
			if (existing.Id == node.Id)
			{
				throw new ArgumentException(
					"A node with the same identity already exists.",
					nameof(node));
			}
		}

		if (node.IsAttached)
		{
			throw new ArgumentException(
				"Node is already attached to a graph.",
				nameof(node));
		}

		node.AttachTo(this);
		_nodes.Add(node);
	}

	/// <summary>
	/// Removes the node with the specified identifier.
	/// </summary>
	/// <param name="nodeId">The node identifier.</param>
	/// <returns>
	/// True if the node was removed; otherwise, false.
	/// </returns>
	/// <exception cref="ArgumentException">
	/// The identifier is empty.
	/// </exception>
	public bool RemoveNode(Guid nodeId)
	{
		if (nodeId == Guid.Empty)
		{
			throw new ArgumentException(
				"Node identity cannot be empty.",
				nameof(nodeId));
		}

		for (int index = 0; index < _nodes.Count; index++)
		{
			if (_nodes[index].Id == nodeId)
			{
				_nodes[index].DetachFrom(this);
				_nodes.RemoveAt(index);
				return true;
			}
		}

		return false;
	}

	/// <summary>
	/// Attaches the graph to its owning project.
	/// </summary>
	internal void AttachTo(ScryProject project)
	{
		if (_owner is not null)
		{
			throw new InvalidOperationException(
				"Graph is already attached to a project.");
		}

		_owner = project;
	}

	/// <summary>
	/// Releases ownership of the graph from its current project.
	/// </summary>
	internal void DetachFrom(ScryProject project)
	{
		if (!ReferenceEquals(_owner, project))
		{
			throw new InvalidOperationException(
				"Graph does not belong to the specified project.");
		}

		_owner = null;
	}
}
