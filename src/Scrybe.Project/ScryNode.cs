// SPDX-License-Identifier: LGPL-3.0-only

using System.Collections.ObjectModel;

namespace Scrybe.Project;

/// <summary>
/// Represents an authored narrative node.
/// </summary>
public sealed class ScryNode
{
	private readonly List<ScryPort> _ports = [];
	private readonly ReadOnlyCollection<ScryPort> _portView;
	private ScryGraph? _owner;

	public Guid Id { get; }
	public string TypeId { get; }

	/// <summary>
	/// Gets the node's ports in insertion order.
	/// </summary>
	public IReadOnlyList<ScryPort> Ports => _portView;

	internal bool IsAttached => _owner is not null;

	/// <summary>
	/// Initializes a node with an explicit identity and type identifier.
	/// </summary>
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

		_portView = _ports.AsReadOnly();
	}

	/// <summary>
	/// Adds a port to the end of the node's port collection.
	/// </summary>
	public void AddPort(ScryPort port)
	{
		ArgumentNullException.ThrowIfNull(port);

		foreach (var existing in _ports)
		{
			if (existing.Id == port.Id)
			{
				throw new ArgumentException(
					"A port with the same identity already exists.",
					nameof(port));
			}
		}

		if (port.IsAttached)
		{
			throw new ArgumentException(
				"Port is already attached to a node.",
				nameof(port));
		}

		port.AttachTo(this);
		_ports.Add(port);
	}

	/// <summary>
	/// Removes the port with the specified identifier.
	/// </summary>
	/// <returns>
	/// True if the port was removed; otherwise, false.
	/// </returns>
	public bool RemovePort(Guid portId)
	{
		if (portId == Guid.Empty)
		{
			throw new ArgumentException(
				"Port identity cannot be empty.",
				nameof(portId));
		}

		for (int index = 0; index < _ports.Count; index++)
		{
			if (_ports[index].Id == portId)
			{
				_ports[index].DetachFrom(this);
				_ports.RemoveAt(index);
				return true;
			}
		}

		return false;
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
