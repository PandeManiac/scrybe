// SPDX-License-Identifier: LGPL-3.0-only

namespace Scrybe.Project;

/// <summary>
/// Represents a typed input or output port on a narrative node.
/// </summary>
public sealed class ScryPort
{
	private ScryNode? _owner;

	public Guid Id { get; }
	public string Name { get; }
	public string TypeId { get; }
	public ScryPortDirection Direction { get; }
	public ScryPortCapacity Capacity { get; }

	internal bool IsAttached => _owner is not null;

	/// <summary>
	/// Initializes a port with explicit identity and structural metadata.
	/// </summary>
	public ScryPort(
		Guid id,
		string name,
		string typeId,
		ScryPortDirection direction,
		ScryPortCapacity capacity)
	{
		if (id == Guid.Empty)
		{
			throw new ArgumentException(
				"Port identity cannot be empty.",
				nameof(id));
		}

		ArgumentException.ThrowIfNullOrWhiteSpace(name);
		ArgumentException.ThrowIfNullOrWhiteSpace(typeId);

		if (!Enum.IsDefined(direction))
		{
			throw new ArgumentOutOfRangeException(nameof(direction));
		}

		if (!Enum.IsDefined(capacity))
		{
			throw new ArgumentOutOfRangeException(nameof(capacity));
		}

		Id = id;
		Name = name;
		TypeId = typeId;
		Direction = direction;
		Capacity = capacity;
	}

	/// <summary>
	/// Attaches the port to its owning node.
	/// </summary>
	internal void AttachTo(ScryNode node)
	{
		if (_owner is not null)
		{
			throw new InvalidOperationException(
				"Port is already attached to a node.");
		}

		_owner = node;
	}

	/// <summary>
	/// Releases ownership of the port from its current node.
	/// </summary>
	internal void DetachFrom(ScryNode node)
	{
		if (!ReferenceEquals(_owner, node))
		{
			throw new InvalidOperationException(
				"Port does not belong to the specified node.");
		}

		_owner = null;
	}
}
