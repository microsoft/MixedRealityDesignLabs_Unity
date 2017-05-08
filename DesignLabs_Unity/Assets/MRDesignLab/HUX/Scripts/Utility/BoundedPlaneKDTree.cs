//
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
using System.Collections.Generic;
using HoloToolkit.Unity.SpatialMapping;
using UnityEngine;

namespace HUX.Utility
{
    public class BoundedPlaneKDTree<T> 
		where T : class
	{
		#region Private Classes
		/// <summary>
		/// Tree node class.
		/// </summary>
		private class Node
		{
			#region Enums
			/// <summary>
			/// Enum for placing behind or infront of the node for its children.
			/// </summary>
			public enum ChildPath
			{
				Behind,
				InfrontOrEqual
			}
			#endregion

			//---------------------------------------------------------------------------------------

			#region public variables
			/// <summary>
			/// The bounded plane this node is using.
			/// </summary>
			public BoundedPlane Plane;

			/// <summary>
			/// User supplied data.
			/// </summary>
			public T UserData;

			/// <summary>
			/// Parent of this node.  If null we are the root.
			/// </summary>
			public Node Parent;

			/// <summary>
			/// Children of this node.
			/// </summary>
			public Node[] Children = new Node[2];
			#endregion

			//---------------------------------------------------------------------------------------

			#region Constructors
			/// <summary>
			///  Duplicates the plane and user data of the other node but leaves blank parent and children data.
			/// </summary>
			/// <param name="otherNode"></param>
			public Node(Node otherNode)
			{
				// Just copy the Plane and user data
				Plane = otherNode.Plane;
				UserData = otherNode.UserData;
			}

			/// <summary>
			/// Creates a new node with the provided plane and data.
			/// </summary>
			/// <param name="plane"></param>
			/// <param name="data"></param>
			public Node(BoundedPlane plane, T data)
			{
				Plane = plane;
				UserData = data;
			}
			#endregion

			//---------------------------------------------------------------------------------------

			#region Public Functions
			/// <summary>
			/// Returns true if the user data and the extents, center and rotation of the bounding plane are the same
			/// </summary>
			/// <param name="otherNode"></param>
			/// <returns>True if the user data and the extents, center and rotation of the bounding plane are the same</returns>
			public bool IsSameNode(Node otherNode)
			{
				return this.UserData.Equals(otherNode.UserData)
				&& this.Plane.Bounds.Center == otherNode.Plane.Bounds.Center
				&& this.Plane.Bounds.Extents == otherNode.Plane.Bounds.Extents
				&& this.Plane.Bounds.Rotation == otherNode.Plane.Bounds.Rotation;
			}

			public bool HasOtherChild(Node child)
			{
				return this.Children[0] == child ? this.Children[1] != null : this.Children[0] != null;
			}

			public Node GetOtherChild(Node child)
			{
				return this.Children[0] == child ? this.Children[1] : this.Children[0];
			}
			#endregion
		}
		#endregion

		//---------------------------------------------------------------------------------------

		#region Private Variables
		/// <summary>
		/// The root of the tree.
		/// </summary>
		private Node m_RootNode;
		#endregion

		//---------------------------------------------------------------------------------------

		#region Public Functions
		/// <summary>
		/// Adds a bounding plane and user data to the tree.  (Note if they are already in the tree they will not be added again as they already exist.)
		/// </summary>
		/// <param name="plane"></param>
		/// <param name="data"></param>
		public void Add(BoundedPlane plane, T data)
		{
			Node newNode = new Node(plane, data);
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
		/// Removes all node that reference the passed in user data.
		/// </summary>
		/// <param name="data"></param>
		public void Remove(T data)
		{
			List<Node> toRemove = FindAllNodesWithData(data);
			while (toRemove.Count > 0)
			{
				RemoveNode(toRemove[0]);
				toRemove.RemoveAt(0);
			}
		}

		/// <summary>
		/// Removes all planes from the tree that match the bounded plane passed in.
		/// </summary>
		/// <param name="plane"></param>
		public void Remove(BoundedPlane plane)
		{
			List<Node> toRemove = FindAllNodesWithPlane(plane);
			while (toRemove.Count > 0)
			{
				RemoveNode(toRemove[0]);
				toRemove.RemoveAt(0);
			}
		}

		/// <summary>
		/// Finds the closest bounding plane and its assosiated data.
		/// </summary>
		/// <param name="worldPos"></param>
		/// <param name="foundPlane"></param>
		/// <param name="foundData"></param>
		/// <returns></returns>
		/// <remarks>If two exactly the same bounding planes exist with different data the data returned is unspecified.</remarks>
		public bool FindClosestBoundedPlane(Vector3 worldPos, out BoundedPlane foundPlane, out T foundData)
		{
			float bestSqDistance = float.MaxValue;
			Node closestNode = null;
			if (m_RootNode != null)
			{
				closestNode = GetClosestNode(m_RootNode, worldPos, out bestSqDistance);
			}

			if (closestNode != null)
			{
				foundPlane = closestNode.Plane;
				foundData = closestNode.UserData;
			}
			else
			{
				foundPlane = new BoundedPlane();
				foundData = null;
			}

			return closestNode != null;
		}

		#endregion

		//---------------------------------------------------------------------------------------

		#region Private Functions
		/// <summary>
		/// Inserts a node into the tree.
		/// </summary>
		/// <param name="currentNode"></param>
		/// <param name="newNode"></param>
		/// <remarks>
		/// This works on the following process.
		/// 
		/// 1. If we are on the positive side of the infinite plane described by the current node then
		///    add us to the child tree of Node.ChildPath.InfrontOrEqual
		///    
		/// 2. If we are on the negative side of the infinite plane described by the current node then
		///    add us to the child tree of Node.ChildPath.Behind.
		///    
		/// 3. If we span across the infinite plane described by the current node then duplicate the
		///    node and add us to both of the child trees.
		/// </remarks>
		private void RecursiveInsert(Node currentNode, Node newNode)
		{
			if (currentNode.IsSameNode(newNode))
			{
				//These are the same nodes we do not need to continue the insert as the node is already here.
				return;
			}

			Vector3 extents = newNode.Plane.Bounds.Extents;
			Vector3 worldTopLeft = (newNode.Plane.Bounds.Rotation * new Vector3(-extents.x, extents.y)) + newNode.Plane.Bounds.Center;
			Vector3 worldTopRight = (newNode.Plane.Bounds.Rotation * new Vector3(extents.x, extents.y)) + newNode.Plane.Bounds.Center;
			Vector3 worldBottomLeft = (newNode.Plane.Bounds.Rotation * new Vector3(-extents.x, -extents.y)) + newNode.Plane.Bounds.Center;
			Vector3 worldBottomRight = (newNode.Plane.Bounds.Rotation * new Vector3(extents.x, -extents.y)) + newNode.Plane.Bounds.Center;

			bool hasPos = currentNode.Plane.Plane.GetSide(worldTopLeft) || currentNode.Plane.Plane.GetSide(worldTopRight) || currentNode.Plane.Plane.GetSide(worldBottomLeft) || currentNode.Plane.Plane.GetSide(worldBottomRight);
			bool hasNeg = !currentNode.Plane.Plane.GetSide(worldTopLeft) || !currentNode.Plane.Plane.GetSide(worldTopRight) || !currentNode.Plane.Plane.GetSide(worldBottomLeft) || !currentNode.Plane.Plane.GetSide(worldBottomRight);

			if (hasPos && hasNeg)
			{
				//The planes intersect split the new node in two and do inserts on left and right.
				AddToChildTree(currentNode, Node.ChildPath.InfrontOrEqual, newNode);
				AddToChildTree(currentNode, Node.ChildPath.Behind, new Node(newNode));
			}
			else if (hasNeg && !hasPos)
			{
				//It is behind the current plane
				AddToChildTree(currentNode, Node.ChildPath.Behind, newNode);
			}
			else
			{
				AddToChildTree(currentNode, Node.ChildPath.InfrontOrEqual, newNode);
			}
		}

		/// <summary>
		/// Adds the child node to the child tree of the current node on the path provided.
		/// </summary>
		/// <param name="currentNode"></param>
		/// <param name="path"></param>
		/// <param name="childNode"></param>
		/// <remarks>
		/// We do the following:
		/// 
		/// 1. If the child node on path for the current node is null assign childNode to it.
		/// 
		/// 2. Other wise continue to try a recursive insert on our child node on the path.
		/// </remarks>
		private void AddToChildTree(Node currentNode, Node.ChildPath path, Node childNode)
		{
			if (currentNode.Children[(int)path] == null)
			{
				currentNode.Children[(int)path] = childNode;
				childNode.Parent = currentNode;
			}
			else
			{
				RecursiveInsert(currentNode.Children[(int)path], childNode);
			}
		}

		/// <summary>
		/// Uses a depth first search of the tree to look for every node that has the data provided.
		/// </summary>
		/// <param name="data"></param>
		/// <returns></returns>
		private List<Node> FindAllNodesWithData(T data)
		{
			List<Node> found = new List<Node>();
			List<Node> toSearch = new List<Node>();

			if (m_RootNode != null)
			{
				toSearch.Add(m_RootNode);
			}

			while (toSearch.Count > 0)
			{
				Node currentNode = toSearch[0];
				toSearch.RemoveAt(0);
				if (currentNode.Children[0] != null)
				{
					toSearch.Add(currentNode.Children[0]);
				}

				if (currentNode.Children[1] != null)
				{
					toSearch.Add(currentNode.Children[1]);
				}

				if (currentNode.UserData.Equals(data))
				{
					found.Add(currentNode);
				}
			}

			return found;
		}

		/// <summary>
		/// Looks at every node using a depth first search to see if the bounding plane they describe is the same as the one passed in.
		/// </summary>
		/// <param name="plane"></param>
		/// <returns></returns>
		private List<Node> FindAllNodesWithPlane(BoundedPlane plane)
		{
			List<Node> found = new List<Node>();
			List<Node> toSearch = new List<Node>();

			if (m_RootNode != null)
			{
				toSearch.Add(m_RootNode);
			}

			while (toSearch.Count > 0)
			{
				Node currentNode = toSearch[0];
				toSearch.RemoveAt(0);
				if (currentNode.Children[0] != null)
				{
					toSearch.Add(currentNode.Children[0]);
				}

				if (currentNode.Children[1] != null)
				{
					toSearch.Add(currentNode.Children[1]);
				}

				if (	currentNode.Plane.Bounds.Center == plane.Bounds.Center
					&&	currentNode.Plane.Bounds.Extents == plane.Bounds.Extents
					&&	currentNode.Plane.Bounds.Rotation == plane.Bounds.Rotation)
				{
					found.Add(currentNode);
				}
			}

			return found;
		}

		/// <summary>
		/// Removes a node from the tree.
		/// </summary>
		/// <param name="node"></param>
		/// <remarks>
		/// We remove a node using the following algorithm:
		/// 
		/// 1. If the node has no children set the parents child to null.
		/// 
		/// 2. If we have a left child but no right child set the parents child to the left child.
		/// 
		/// 2. If we have a right child but no left child set the parents child to the right child.
		/// 
		/// 3. If we have both children set out parents child to our Node.ChildPath.Behind child.
		///    Do a depth first insert of all children of the current node to rebuild the tree with 
		///    Node.ChildPath.Behind as the current parent.
		/// </remarks>
		private void RemoveNode(Node node)
		{
			Node newChild = null;
			if (node.Children[(int)Node.ChildPath.Behind] != null && node.Children[(int)Node.ChildPath.InfrontOrEqual] != null)
			{
				List<Node> toInsert = new List<Node>();
				newChild = node.Children[(int)Node.ChildPath.Behind];

				List<Node> toCheck = new List<Node>();
				while (toCheck.Count > 0)
				{
					Node currentNode = toCheck[0];
					toCheck.RemoveAt(0);

					if (currentNode != node && currentNode != newChild)
					{
						toInsert.Add(currentNode);
					}

					if (currentNode.Children[(int)Node.ChildPath.Behind] != null)
					{
						toCheck.Add(currentNode.Children[(int)Node.ChildPath.Behind]);
					}

					if (currentNode.Children[(int)Node.ChildPath.InfrontOrEqual] != null)
					{
						toCheck.Add(currentNode.Children[(int)Node.ChildPath.InfrontOrEqual]);
					}
				}

				newChild.Parent = null;
				newChild.Children[(int)Node.ChildPath.Behind] = null;
				newChild.Children[(int)Node.ChildPath.InfrontOrEqual] = null;

				foreach (Node oldNode in toInsert)
				{
					//Break the old child links.

					oldNode.Parent = null;
					oldNode.Children[0] = null;
					oldNode.Children[1] = null;

					RecursiveInsert(newChild, oldNode);
				}
			}
			else if (node.Children[(int)Node.ChildPath.Behind] == null && node.Children[(int)Node.ChildPath.InfrontOrEqual] != null)
			{
				newChild = node.Children[(int)Node.ChildPath.InfrontOrEqual];
			}
			else if (node.Children[(int)Node.ChildPath.Behind] != null && node.Children[(int)Node.ChildPath.InfrontOrEqual] == null)
			{
				newChild = node.Children[(int)Node.ChildPath.Behind];
			}

			if (node.Parent != null)
			{
				Node.ChildPath side = node.Parent.Children[(int)Node.ChildPath.Behind] == node ? Node.ChildPath.Behind : Node.ChildPath.InfrontOrEqual;
				node.Parent.Children[(int)side] = newChild;

				if (newChild != null)
				{
					newChild.Parent = node.Parent;
				}
			}
			else
			{
				if (newChild != null)
				{
					newChild.Parent = null;
				}
				m_RootNode = newChild;
			}
		}

		/// <summary>
		/// Gets the closest node to a point in the tree from rootNode down.
		/// </summary>
		/// <param name="rootNode"></param>
		/// <param name="worldPos"></param>
		/// <param name="bestSqrDistance"></param>
		/// <returns></returns>
		private Node GetClosestNode(Node rootNode, Vector3 worldPos, out float bestSqrDistance)
		{
			Node currentNode = FindLeaf(rootNode, worldPos);

			//Start the search up.
			float bestSqrDist = float.MaxValue;
			Node currentBest = null;
			Node previousNode = null;

			while (currentNode != rootNode.Parent)
			{
				float sqDist = currentNode.Plane.GetSqrDistance(worldPos);
				if (sqDist < bestSqrDist)
				{
					bestSqrDist = sqDist;
					currentBest = currentNode;
				}

				if (previousNode != null && currentNode.HasOtherChild(previousNode))
				{
					float distanceToPlane = currentNode.Plane.Plane.GetDistanceToPoint(worldPos);
					if (distanceToPlane <= Mathf.Sqrt(bestSqrDist))
					{
						float bestOtherSqDist = float.MaxValue;
						Node otherBest = GetClosestNode(currentNode.GetOtherChild(previousNode), worldPos, out bestOtherSqDist);
						if (bestOtherSqDist < bestSqrDist)
						{
							bestSqrDist = bestOtherSqDist;
							currentBest = otherBest;
						}
					}
				}

				previousNode = currentNode;
				currentNode = currentNode.Parent;
			}

			bestSqrDistance = bestSqrDist;
			return currentBest;
		}

		/// <summary>
		/// Find the leaf node for world pos.
		/// </summary>
		/// <param name="rootNode"></param>
		/// <param name="worldPos"></param>
		/// <returns></returns>
		/// <remarks>
		/// We search for a leaf node.  If root node is not a leaf node the the following happens.
		/// 
		/// 1. If we have both nodes we search down the child node that the point falls on for the root node plane.  So + would be InfrontOrEqual.
		/// 
		/// 2. Otherwise we search down the child node that exists.
		/// </remarks>
		private Node FindLeaf(Node rootNode, Vector3 worldPos)
		{
			if (rootNode.Children[(int)Node.ChildPath.Behind] == null && rootNode.Children[(int)Node.ChildPath.InfrontOrEqual] == null)
			{
				return rootNode;
			}
			else if (rootNode.Children[(int)Node.ChildPath.Behind] == null && rootNode.Children[(int)Node.ChildPath.InfrontOrEqual] != null)
			{
				return FindLeaf(rootNode.Children[(int)Node.ChildPath.InfrontOrEqual], worldPos);
			}
			else if (rootNode.Children[(int)Node.ChildPath.Behind] != null && rootNode.Children[(int)Node.ChildPath.InfrontOrEqual] == null)
			{
				return FindLeaf(rootNode.Children[(int)Node.ChildPath.Behind], worldPos);
			}
			else if (rootNode.Plane.Plane.GetSide(worldPos))
			{
				return FindLeaf(rootNode.Children[(int)Node.ChildPath.InfrontOrEqual], worldPos);
			}
			else
			{
				return FindLeaf(rootNode.Children[(int)Node.ChildPath.Behind], worldPos);
			}
		}
		#endregion
	}
}