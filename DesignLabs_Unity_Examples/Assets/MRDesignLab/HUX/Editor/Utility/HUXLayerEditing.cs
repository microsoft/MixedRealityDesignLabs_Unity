//
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;

public class HUXLayerEditing
{
	/// <summary>
	/// Skip the build in layers by starting on index 8
	/// </summary>
	protected const int START_LAYER_INDEX = 8;

	/// <summary>
	/// Adds missing layers and updates the prefabs in prefabFolder changing layers from the original mapping to the new layer numbers.
	/// </summary>
	/// <param name="originalMapping"></param>
	/// <param name="prefabFolder"></param>
	public static void UpdateLayers(Dictionary<string, int> originalMapping, string prefabFolder)
	{
		SerializedObject tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);

		SerializedProperty layers = tagManager.FindProperty("layers");
		if (layers != null)
		{
			CreateMissingLayers(layers, new List<string>(originalMapping.Keys));
			tagManager.ApplyModifiedProperties();
			Dictionary<int, int> newMappings = CreateLayerMapping(layers, originalMapping);

			DirectoryInfo dir = new DirectoryInfo("Assets/MRDesignLabs");
			FileInfo[] infos = dir.GetFiles("*.prefab", SearchOption.AllDirectories);
			for (int index = 0; index < infos.Length; index++)
			{
				FileInfo info = infos[index];
				string assetPath = "Assets/" + info.FullName.Substring(Application.dataPath.Length + 1);

				GameObject prefab = AssetDatabase.LoadAssetAtPath(assetPath, typeof(GameObject)) as GameObject;
				if (prefab != null)
				{
					UpdateObjectLayers(prefab, newMappings);
				}
			}

			AssetDatabase.SaveAssets();
		}
		else
		{
			Debug.Log("Unable to access layers.  Please make sure ProjectSettings/TagManager.assets exists.");
		}

	}

	/// <summary>
	/// Creates any layers in layerNames that do not already exist in the layers list.
	/// </summary>
	/// <param name="layers"></param>
	/// <param name="layerNames"></param>
	protected static void CreateMissingLayers(SerializedProperty layers, List<string> layerNames)
	{
		for (int index = 0; index < layerNames.Count; index++)
		{
			SerializedProperty firstUnused = null;
			bool foundLayer = false;

			for (int layerIndex = START_LAYER_INDEX; layerIndex < layers.arraySize; layerIndex++)
			{
				SerializedProperty element = layers.GetArrayElementAtIndex(layerIndex);

				if (firstUnused == null && string.IsNullOrEmpty(element.stringValue))
				{
					firstUnused = element;
				}
				else if (element.stringValue.Equals(layerNames[index]))
				{
					foundLayer = true;
					break;
				}
			}

			if (!foundLayer)
			{
				if (firstUnused != null)
				{
					firstUnused.stringValue = layerNames[index];
				}
				else
				{
					Debug.Log("Unable to fit missing layers!  No more free layers.");
					break;
				}
			}
		}
	}

	/// <summary>
	/// Crates a mapping of original layer number to layer number based on the layer names and original layer numbers passed in.
	/// </summary>
	/// <param name="layers"></param>
	/// <param name="originalLayers"></param>
	/// <returns>A dictionary of original layer numbers to new layer numbers.</returns>
	protected static Dictionary<int, int> CreateLayerMapping(SerializedProperty layers, Dictionary<string, int> originalLayers)
	{
		Dictionary<int, int> newMapping = new Dictionary<int, int>();

		foreach (KeyValuePair<string, int> origLayer in originalLayers)
		{
			int foundLayerIndex = -1;

			for (int layerIndex = START_LAYER_INDEX; layerIndex < layers.arraySize; layerIndex++)
			{
				SerializedProperty element = layers.GetArrayElementAtIndex(layerIndex);

				if (element.stringValue.Equals(origLayer.Key))
				{
					foundLayerIndex = layerIndex;
				}
			}

			if (foundLayerIndex != -1)
			{
				newMapping[origLayer.Value] = foundLayerIndex;
			}
		}

		return newMapping;
	}

	/// <summary>
	/// Updates the layer of an object and its children based on the layer mapping passed on.  If the objects layer is no in the mapping then it does not change.
	/// </summary>
	/// <param name="obj"></param>
	/// <param name="layerMapping">A mapping of layer number to change.  eg 1 {key} -> 2 {value} </param>
	public static void UpdateObjectLayers(GameObject obj, Dictionary<int, int> layerMapping)
	{
		if (layerMapping.ContainsKey(obj.layer))
		{
			obj.layer = layerMapping[obj.layer];
		}

		for (int index = 0; index < obj.transform.childCount; index++)
		{
			UpdateObjectLayers(obj.transform.GetChild(index).gameObject, layerMapping);
		}
	}

	/// <summary>
	/// Updates the default collisions so that only the collision pairs described in validLayerCollisions are set to true.
	/// </summary>
	/// <param name="validLayerCollisions">An Array of layer names collision pairs.
	/// eg. { {"Layer Name 1", "Layer Name 2"}, {"Layer Name 3", "Layer Name 2"} }</param>
	public static void UpdatePhysicsCollisions(string[,] validLayerCollisions)
	{
		int[,] layerNums = new int[validLayerCollisions.GetLength(0), validLayerCollisions.GetLength(1)];

		for (int row = 0; row < validLayerCollisions.GetLength(0); row++)
		{
			for (int col = 0; col < validLayerCollisions.GetLength(1); col++)
			{
				layerNums[row, col] = LayerMask.NameToLayer(validLayerCollisions[row, col]);
			}
		}

		UpdatePhysicsCollisions(layerNums);
	}

	/// <summary>
	/// Updates the default collisions so that only the collision pairs described in validLayerCollisions are set to true.
	/// </summary>
	/// <param name="validLayerCollisions">An Array of layer number collision pairs.  eg. { {2, 4}, {5, 2} }</param>
	public static void UpdatePhysicsCollisions(int[,] validLayerCollisions)
	{
		for (int layer1 = 0; layer1 < 32; layer1++)
		{
			for (int layer2 = 0; layer2 < 32; layer2++)
			{
				bool shouldCollide = false;
				for (int collisions = 0; collisions < validLayerCollisions.GetLength(0); collisions++)
				{
					if ((layer1 == validLayerCollisions[collisions, 0] && layer2 == validLayerCollisions[collisions, 1])
						|| (layer2 == validLayerCollisions[collisions, 0] && layer1 == validLayerCollisions[collisions, 1])
						)
					{
						shouldCollide = true;
					}
				}
				Physics.IgnoreLayerCollision(layer1, layer2, !shouldCollide);
			}
		}
	}
}
