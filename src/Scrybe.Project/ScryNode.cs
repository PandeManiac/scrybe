// SPDX-License-Identifier: LGPL-3.0-only

namespace Scrybe.Project;

/// <summary>
/// Represents an authored narrative node.
/// </summary>
public sealed class ScryNode
{
	private ScryGraph? _owner;

	public Guid Id { get; }
	public string TypeId { get; }

	internal bool IsAttached => _owner is not null;

	/// <summary>
	/// Initializes a node with an explicit identity and type identifier.
	/// </summary>
	/// <param name="id">The stable node identifier.</param>
	/// <param name="typeId">The node type identifier.</param>
	/// <exception cref="ArgumentException">
	/// The identifier is empty or the type identifier is blank.
	/// </exception>
	/// <exception cref="ArgumentNullException">
	/// The type identifier is null.
	/// </exception>
	public ScryNode(Guid id, string typeId)
	{
		if (id == Guid.Empty)
		{
			throw new ArgumentException(
				"Node identity cannot be empty.",
				nameof(id));
		}

		ArgumentException.ThrowIfNullOrWhiteSpace(typeId);

		Id = id;
		TypeId = typeId;
	}

	/// <summary>
	/// Attaches the node to its owning graph.
	/// </summary>
	internal void AttachTo(ScryGraph graph)
	{
		if (_owner is not null)
		{
			throw new InvalidOperationException(
				"Node is already attached to a graph.");
		}

		_owner = graph;
	}

	/// <summary>
	/// Releases ownership of the node from its current graph.
	/// </summary>
	internal void DetachFrom(ScryGraph graph)
	{
		if (!ReferenceEquals(_owner, graph))
		{
			throw new InvalidOperationException(
				"Node does not belong to the specified graph.");
		}

		_owner = null;
	}
}
