//
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
using UnityEngine;
using System;
using System.Collections.Generic;

namespace HUX.Utility
{
	public class AABBTree<T> where T : class
	{
		#region Private Classes
		/// <summary>
		/// Node class for the tree.  CAn be either a branch or a leaf.
		/// </summary>
		private class AABBNode
		{
			#region Public Static Functions
			/// <summary>
			/// Creates a node containing both bounds.
			/// </summary>
			/// <param name="rightBounds"></param>
			/// <param name="leftBounds"></param>
			/// <returns></returns>
			public static AABBNode CreateNode(Bounds rightBounds, Bounds leftBounds)
			{
				AABBNode newNode = new AABBNode();
				newNode.Bounds = rightBounds.ExpandToContian(leftBounds);
				return newNode;
			}

			/// <summary>
			/// Creates a node with the bounds and data.
			/// </summary>
			/// <param name="bounds"></param>
			/// <param name="data"></param>
			/// <returns></returns>
			public static AABBNode CreateNode(Bounds bounds, T data)
			{
				AABBNode newNode = new AABBNode();
				newNode.Bounds = bounds;
				newNode.UserData = data;

				// Determine if we want a margin bounds;

				return newNode;
			}
			#endregion

			//-----------------------------------------------------------------------------------------------------------

			#region Public Variables
			/// <summary>
			/// Children of this node.
			/// </summary>
			public AABBNode[] Children = new AABBNode[2];

			/// <summary>
			/// The Axis Aligned Bounding Box for this node.
			/// </summary>
			public Bounds Bounds;

			/// <summary>
			/// User Data for this node.
			/// </summary>
			public T UserData;
			#endregion

			//-----------------------------------------------------------------------------------------------------------

			#region Private Variables
			/// <summary>
			/// A weak reference to the parent so the tree will get cleaned up if the root node is no longer referenced.
			/// </summary>
			private WeakReference m_ParentRef;
			#endregion

			//-----------------------------------------------------------------------------------------------------------

			#region Accessors
			/// <summary>
			/// True if this is a branch node with no children assigned.
			/// </summary>
			public bool IsLeaf
			{
				get
				{
					return Children[0] == null && Children[1] == null;
				}
			}

			/// <summary>
			/// Accessor for setting/getting the parent.
			/// </summary>
			public AABBNode Parent
			{
				get
				{
					return m_ParentRef != null && m_ParentRef.IsAlive ? m_ParentRef.Target as AABBNode : null;
				}

				set
				{
					if (value == null)
					{
						m_ParentRef = null;
					}
					else
					{
						m_ParentRef = new WeakReference(value);
					}
				}
			}
			#endregion

			//-----------------------------------------------------------------------------------------------------------

			#region Public Functions
			/// <summary>
			/// Sets the children for this node.
			/// </summary>
			/// <param name="child1"></param>
			/// <param name="child2"></param>
			public void SetChildren(AABBNode child1, AABBNode child2)
			{
				child1.Parent = this;
				child2.Parent = this;

				Children[0] = child1;
				Children[1] = child2;
			}

			/// <summary>
			/// Sets the bounds to the size of both children.
			/// </summary>
			public void RebuildBounds()
			{
				Bounds = Children[0].Bounds.ExpandToContian(Children[1].Bounds);
			}
			#endregion
		}
		#endregion

		//-----------------------------------------------------------------------------------------------------------

		#region Private Variables

		/// <summary>
		/// The root node of the tree.
		/// </summary>
		private AABBNode m_RootNode;

		#endregion

		//-----------------------------------------------------------------------------------------------------------

		#region Public Functions
		/// <summary>
		/// Creates a new node with the provided bounds.
		/// </summary>
		/// <param name="bounds"></param>
		/// <param name="data"></param>
		public void Insert(Bounds bounds, T data)
		{
			AABBNode newNode = AABBNode.CreateNode(bounds, data);
			if (m_RootNode == null)
			{
				m_RootNode = newNode;
			}
			else
			{
				RecursiveInsert(m_RootNode, newNode);
			}
		}

		/// <summary>
		/// Removes the node containing data.
		/// </summary>
		/// <param name="data"></param>
		public void Remove(T data)
		{
			AABBNode node = FindNode(data);
			RemoveNode(node);
		}

		/// <summary>
		/// Removes the node with the bounds.  If two nodes have the exact same bounds only the first one found will be removed.
		/// </summary>
		/// <param name="bounds"></param>
		public void Remove(Bounds bounds)
		{
			AABBNode node = FindNode(bounds);
			RemoveNode(node);
		}

		/// <summary>
		/// Destroys all nodes in the tree.
		/// </summary>
		public void Clear()
		{
			// All we need to do is remove the root node reference.  The garbage collector will do the rest.
			m_RootNode = null;
		}

		#endregion

		//-----------------------------------------------------------------------------------------------------------

		#region Private Functions
		/// <summary>
		/// Recursively Insert the new Node until we hit a leaf node.  Then branch and insert both nodes.
		/// </summary>
		/// <param name="currentNode"></param>
		/// <param name="newNode"></param>
		private void RecursiveInsert(AABBNode currentNode, AABBNode newNode)
		{
			AABBNode branch = currentNode;
			if (currentNode.IsLeaf)
			{
				branch = AABBNode.CreateNode(currentNode.Bounds, newNode.Bounds);
				branch.Parent = currentNode.Parent;
				if (currentNode == m_RootNode)
				{
					m_RootNode = branch;
				}
				else
				{
					branch.Parent.Children[branch.Parent.Children[0] == currentNode ? 0 : 1] = branch;
				}

				branch.SetChildren(currentNode, newNode);
			}
			else
			{
				Bounds withChild1 = branch.Children[0].Bounds.ExpandToContian(newNode.Bounds);
				Bounds withChild2 = branch.Children[1].Bounds.ExpandToContian(newNode.Bounds);

				float volume1 = withChild1.Volume();
				float volume2 = withChild2.Volume();
				RecursiveInsert((volume1 <= volume2) ? branch.Children[0] : branch.Children[1], newNode);
			}

			branch.RebuildBounds();
		}

		/// <summary>
		/// Finds the node that has the assigned user data.
		/// </summary>
		/// <param name="userData"></param>
		/// <returns></returns>
		private AABBNode FindNode(T userData)
		{
			AABBNode foundNode = null;
			List<AABBNode> nodesToSearch = new List<AABBNode>();
			nodesToSearch.Add(m_RootNode);

			while (nodesToSearch.Count > 0)
			{
				AABBNode currentNode = nodesToSearch[0];
				nodesToSearch.RemoveAt(0);

				if (currentNode.UserData == userData)
				{
					foundNode = currentNode;
					break;
				}
				else if (!currentNode.IsLeaf)
				{
					nodesToSearch.AddRange(currentNode.Children);
				}
			}

			return foundNode;
		}

		/// <summary>
		/// Finds the leaf node that matches bounds.
		/// </summary>
		/// <param name="bounds"></param>
		/// <returns></returns>
		private AABBNode FindNode(Bounds bounds)
		{
			AABBNode foundNode = null;
			AABBNode currentNode = m_RootNode;

			while (currentNode != null)
			{
				if (currentNode.IsLeaf)
				{
					foundNode = currentNode.Bounds == bounds ? currentNode : null;
					break;
				}
				else
				{
					//Which child node if any would the bounds be in?
					if (currentNode.Children[0].Bounds.ContainsBounds(bounds))
					{
						currentNode = currentNode.Children[0];
					}
					else if (currentNode.Children[1].Bounds.ContainsBounds(bounds))
					{
						currentNode = currentNode.Children[1];
					}
					else
					{
						currentNode = null;
					}
				}
			}

			return foundNode;
		}

		/// <summary>
		/// Removes a node from the tree
		/// </summary>
		/// <param name="node"></param>
		private void RemoveNode(AABBNode node)
		{
			AABBNode nodeParent = node.Parent;
			if (node == m_RootNode)
			{
				m_RootNode = null;
			}
			else
			{
				AABBNode otherChild = nodeParent.Children[0] == node ? nodeParent.Children[1] : nodeParent.Children[0];
				if (nodeParent.Parent == null)
				{
					m_RootNode = otherChild;
					otherChild.Parent = null;
				}
				else
				{
					int childIndex = nodeParent.Parent.Children[0] == nodeParent ? 0 : 1;
					nodeParent.Parent.Children[childIndex] = otherChild;
					otherChild.Parent = nodeParent.Parent;
				}
				UpdateNodeBoundUp(otherChild.Parent);
			}
		}

		/// <summary>
		/// Updates the bounds nonleaf node object moving up the Parent tree to Root.
		/// </summary>
		/// <param name="node"></param>
		private void UpdateNodeBoundUp(AABBNode node)
		{
			if (node != null)
			{
				if (!node.IsLeaf)
				{
					node.RebuildBounds();
				}

				UpdateNodeBoundUp(node.Parent);
			}
		}
		#endregion
	}
}
