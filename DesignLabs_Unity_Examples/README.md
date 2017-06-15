**You can find example test scenes for these controls under https://github.com/Microsoft/MRDesignLabs_Unity/tree/master/DesignLabs_Unity_Examples/Assets/MRDL_ControlsExample/Scenes**

## [Interactable Object](https://github.com/Microsoft/MRDesignLabs_Unity/blob/master/DesignLabs_Unity_Examples/Assets/MRDL_ControlsExample/Scenes/InteractableObject_Examples.unity)


### Interactable Object samples ###
You can find the samples in the scene **InteractableObject_Examples.unity**. 

![InteractableObject_Examples.unity scene showing various examples of Interactable Object.](https://github.com/Microsoft/MRDesignLabs_Unity/blob/master/External/ReadMeImages/InteractibleObject_TestScene.jpg)


### Mesh button ###

![Mesh button](https://github.com/Microsoft/MRDesignLabs_Unity/blob/master/External/ReadMeImages/InteractibleObject_MeshButton.jpg)

These are the examples using primitives and imported 3D meshes as Interactable Objects. You can easily assign different scale, offset and colors to respond to different input interaction states.


### Holographic button ###

![Holographic button](https://github.com/Microsoft/MRDesignLabs_Unity/blob/master/External/ReadMeImages/InteractibleObject_HolographicButton.jpg)

This is an example of Holographic button used in the Start menu and [[Holobar and bounding box|HoloBar]]. This example uses Unity's Animation Controller and Animation Clips.


### Toolbar ###

![Toolbar](https://github.com/Microsoft/MRDesignLabs_Unity/blob/master/External/ReadMeImages/InteractibleObject_Toolbar.jpg)

Toolbar is a widely used pattern in mixed reality experiences. It is a simple collection of buttons with additional behavior such as [[Billboarding and tag-a-long]]. This example uses a Billboarding and tag-a-long script from HoloToolkit. You can control the detailed behavior including distance, moving speed and threshold values.


### Traditional button ###

![Traditional button](https://github.com/Microsoft/MRDesignLabs_Unity/blob/master/External/ReadMeImages/InteractibleObject_TraditionalButton.jpg)

This example shows a traditional 2D style button with some dimension. Each input state has a slightly different depth and animation properties.
 


### Other examples ###

![Other examples](https://github.com/Microsoft/MRDesignLabs_Unity/blob/master/External/ReadMeImages/InteractibleObject_PushButton.jpg)

![Other examples](https://github.com/Microsoft/MRDesignLabs_Unity/blob/master/External/ReadMeImages/InteractibleObject_RealLifeObject.jpg)

With HoloLens, you can leverage physical space. Imagine a holographic push button on the physical wall. Or how about a coffee cup on the real table? Using 3D models imported from modeling software, we can create Interactable Object that resembles real life object. Since it is digital object, we can add magical interactions to it.

## Importing and assigning HoloLens symbol icon font file ##
In default, this Unity project does NOT contain required HoloLens symbol font file. You need to download from Microsoft's desgin resources page.(https://docs.microsoft.com/en-us/windows/uwp/design-downloads/index)
Without font file, you will see placeholder icons on the buttons. After importing the font file, find **DefaultButtonIconProfileFont** and click **Auto-assign HoloLens MDL2 Symbols font**
<img src="https://github.com/Microsoft/MRDesignLabs_Unity/blob/master/External/ReadMeImages/HoloLensSymbolFont1.jpg">
<img src="https://github.com/Microsoft/MRDesignLabs_Unity/blob/master/External/ReadMeImages/HoloLensSymbolFont2.jpg">
<img src="https://github.com/Microsoft/MRDesignLabs_Unity/blob/master/External/ReadMeImages/HoloLensSymbolFont3.jpg">


## Ways to use Interactable Objects ##

In the scene **InteractableObject_Examples.unity**, you will be able to find various combinations of 'CompoundButton' scripts. To create your own Interactable Object, you can combine different types of 'CompountButton' scripts. You can find them in **MRDesignLab\HUX\Scripts\Buttons**. It is designed to support various types of Interactable Object in flexible way.

<img src="https://github.com/Microsoft/MRDesignLabs_Unity/blob/master/External/ReadMeImages/InteractibleObject_CompoundButtonScripts.png" alt="Compound Button" width="450px">

### Compound Button ###
<img src="https://github.com/Microsoft/MRDesignLabs_Unity/blob/master/External/ReadMeImages/InteractibleObject_CompoundButton.jpg" alt="Compound Button" width="450px">
This is the base of the button component. You will need this script to build any types of Interactable Objects.

### Compound Button Mesh ###
<img src="https://github.com/Microsoft/MRDesignLabs_Unity/blob/master/External/ReadMeImages/InteractibleObject_CompoundButtonMesh.jpg" alt="Compound Button" width="450px">

Use this script to use various types of custom mesh. You can use your own 3D models imported from 3D modeling software. Using this scripts, you can easily change the scale, offset of the mesh or material properties such as color for the different input interaction states. To create an Interactable Object using script, it is recommended to create an empty GameObject as a container and put the 3D mesh model under it as child component. This will prevent unexpected behavior from different scaling or offset values.

### Compound Button Icon ###
<img src="https://github.com/Microsoft/MRDesignLabs_Unity/blob/master/External/ReadMeImages/InteractibleObject_CompoundButtonIcon.jpg" alt="Compound Button">

Using this scripts, you can use various types of icon provided by Windows fonts. To use this feature, you should have your Windows updated to 'Creators Update'.

### Compound Button Text ###
<img src="https://github.com/Microsoft/MRDesignLabs_Unity/blob/master/External/ReadMeImages/InteractibleObject_CompoundButtonText.jpg" alt="Compound Button" width="450px">

This scripts helps you manage a TextMesh component to display text on your button. This script can be used in conjunction with a CompoundButtonSpeech component to automatically link your button to spoken keywords.

### Compound Button Sound ###
<img src="https://github.com/Microsoft/MRDesignLabs_Unity/blob/master/External/ReadMeImages/InteractibleObject_CompoundButtonSounds.jpg" alt="Compound Button" width="450px">

Use this script to add audio feedback for the different input interaction states.

### Compound Button Speech ###
<img src="https://github.com/Microsoft/MRDesignLabs_Unity/blob/master/External/ReadMeImages/InteractibleObject_CompoundButtonSpeech.jpg" alt="Compound Button" width="450px">

Use this script to automatically register keywords for your button in the Speech Manager (This script is experimental and still being tested).

## [Object Collection](https://github.com/Microsoft/MRDesignLabs_Unity/blob/master/DesignLabs_Unity_Examples/Assets/MRDL_ControlsExample/Scenes/ObjectCollection_Examples.unity)

Object collection is a script which helps you layout an array of objects in predefined three-dimensional shapes. It supports four different surface styles - **plane, cylinder, sphere** and **scatter**. You can adjust the radius, size and the space between the items. Since it supports any object in Unity, you can use it to layout both 2D and 3D objects. 

<img src="https://github.com/Microsoft/MRDesignLabs_Unity/blob/master/External/ReadMeImages/ObjectCollection_Hero.jpg" alt="ObjectCollection">

## Object collection examples ##
Periodic Table of the Elements is an example app that demonstrates how Object collection works. It uses Object collection to layout the 3D element boxes in different shapes.

<img src="https://github.com/Microsoft/MRDesignLabs_Unity/blob/master/External/ReadMeImages/ObjectCollection_Types.jpg" alt="ObjectCollection">

### 3D Objects ###

You can use Object collection to layout imported 3D objects. The example below shows the plane and cylindrical layouts of 3D chair model objects using Object collection.

<img src="https://github.com/Microsoft/MRDesignLabs_Unity/blob/master/External/ReadMeImages/ObjectCollection_3DObjects.jpg" alt="ObjectCollection">

### 2D Objects ###

You can also use 2D images with Object collection. For example, you can easily display multiple images in grid style using Object collection.


<img src="https://github.com/Microsoft/MRDesignLabs_Unity/blob/master/External/ReadMeImages/ObjectCollection_Layout_3DObjects_3.jpg" alt="ObjectCollection">

<img src="https://github.com/Microsoft/MRDesignLabs_Unity/blob/master/External/ReadMeImages/ObjectCollection_Layout_2DImages.jpg" alt="ObjectCollection">

## Ways to use Object collection ##
You can find the examples in the scene **ObjectCollection_Examples.unity**. In this scene, you can find the **ObjectCollection.cs** script under **Assets/MRDesignLab/HUX/Scripts/Collections**

1. To create a collection, simply create an empty GameObject and assign the ObjectCollection.cs script to it. 
2. Then you can add any object(s) as a child of the GameObject. 
3. Once you finished adding a child object, click the **Update Collection** button in the Inspector Panel. 
4. You will then see the object(s) laid out in selected Surface Type. 


<img src="https://github.com/Microsoft/MRDesignLabs_Unity/blob/master/External/ReadMeImages/ObjectCollection_Unity.jpg" alt="ObjectCollection in Unity">

<img src="https://github.com/Microsoft/MRDesignLabs_Unity/blob/master/External/ReadMeImages/ObjectCollection_ExampleScene1.jpg" alt="ObjectCollection in Unity">

<img src="https://github.com/Microsoft/MRDesignLabs_Unity/blob/master/External/ReadMeImages/ObjectCollection_ExampleScene2.jpg" alt="ObjectCollection in Unity">

## [Progress](https://github.com/Microsoft/MRDesignLabs_Unity/blob/master/DesignLabs_Unity_Examples/Assets/MRDL_ControlsExample/Scenes/Progress_Examples.unity)

A progress control provides feedback to the user that a long-running operation is underway. It can mean that the user cannot interact with the app when the progress indicator is visible, and can also indicate how long the wait time might be, depending on the indicator used. 

<img src="https://github.com/Microsoft/MRDesignLabs_Unity/blob/master/External/ReadMeImages/Progress_Hero.jpg" alt="Progress Ring">

You can find Progress.prefab under **Assets/MRDesignLab/HUX/Prefabs/Dialogs/**

<img src="https://github.com/Microsoft/MRDesignLabs_Unity/blob/master/External/ReadMeImages/Progress_Types1.jpg" alt="Progress Examples">

<img src="https://github.com/Microsoft/MRDesignLabs_Unity/blob/master/External/ReadMeImages/Progress_Types2.jpg" alt="Progress Examples">

## [App bar & Bounding box](https://github.com/Microsoft/MRDesignLabs_Unity/blob/master/DesignLabs_Unity_Examples/Assets/MRDL_ControlsExample/Scenes/ManipulationGizmo_Examples.unity)

<img src="https://github.com/Microsoft/MRDesignLabs_Unity/blob/master/External/ReadMeImages/HolobarAndBoundingBox_Hero.jpg">

There are three components to ensure the Holobar and bounding box work inside of your Unity project. The first is the **AppBar.prefab**. It behaves much like a singleton. You only need one in your scene hierarchy. If an object in the scene is interacted with, it will request the App bar and take it from the last interacted object. The AppBar.prefab has a [AppBar.cs](https://github.com/Microsoft/MRDesignLabs_Unity/blob/master/DesignLabs_Unity_Examples/Assets/MRDesignLab/HUX/Scripts/Dialogs/AppBar.cs) script which handles all of the properties of how and what the Holobar should display.

The second is the **BoundingBox.prefab**. This prefab works much like the AppBar.prefab. You only need one bounding box in the scene. If an object in the scene is put into adjust mode, the object will take the bounding box from the last adjusted object. The prefab has two scripts associated with it. The [BoundingBoxManipulate.cs](https://github.com/Microsoft/MRDesignLabs_Unity/blob/master/DesignLabs_Unity_Examples/Assets/MRDesignLab/HUX/Scripts/Interaction/BoundingBoxManipulate.cs) script which does the actual manipulating of the object its currently assigned to. The other script is the  [BoundingBoxGizmo.cs](https://github.com/Microsoft/MRDesignLabs_Unity/blob/master/DesignLabs_Unity_Examples/Assets/MRDesignLab/HUX/Scripts/Interaction/BoundingBoxGizmo.cs) prefab. This script handles the visual representation of the bounding box and renders the transform affordances dynamically.

The last is the **BoundingBoxTarget** [BoundingBoxTarget.cs](https://github.com/Microsoft/MRDesignLabs_Unity/blob/master/DesignLabs_Unity_Examples/Assets/MRDesignLab/HUX/Scripts/Interaction/BoundingBoxTarget.cs) script. This script gives objects in the scene ability to request the Holobar and bounding box by linking the two prefabs together. It essentially is the glue to make all of the MRDesignLab manipulation components talk to each other. Assign BoundingBoxTarget.cs script to any objects that you want to use bounding box and app bar.


Clone and open the project [MRDesignLabs_Unity](https://github.com/Microsoft/MRDesignLabs_Unity) in Unity.



<img src="https://github.com/Microsoft/MRDesignLabs_Unity/blob/master/External/ReadMeImages/NewHLCamera.png" alt="Use the HUX menu to create a new HoloLens camera instance">

The default **HololensCamera** prefab provided in [MRDesignLabs_Unity](https://github.com/Microsoft/MRDesignLabs_Unity) provides global event and input handling required for the Holobar and Bounding box to work. It can be found at [/MRDesignLab/Hux/Prefab/Interface](https://github.com/Microsoft/MRDesignLabs_Unity/tree/master/DesignLabs_Unity/Assets/MRDesignLab/HUX/Prefabs/Interface). Additionally, once **MRDesignLabs** is in your unity project, you can use the menu system to create an instance of the HoloLens default camera.


<img src="https://github.com/Microsoft/MRDesignLabs_Unity/blob/master/External/ReadMeImages/BoundingBoxAppBar-InteractionManager.jpg" alt="Interaction Manager under HoloLensCamera" width="550px">

Please make sure **ManipulationTranslate** is checked in **InteractionManager** in HoloLens object.

<img src="https://github.com/Microsoft/MRDesignLabs_Unity/blob/master/External/ReadMeImages/BoundingBoxAppBar-scene.jpg" alt="The bounding box and app bar prefabs in the scene" width="550px">

You can find the **AppBar.prefab** in [/MRDesignLab/HUX/Prefabs/Dialogs](https://github.com/Microsoft/MRDesignLabs_Unity/tree/master/DesignLabs_Unity/Assets/MRDesignLab/HUX/Prefabs/Dialogs/) and the **BoundingBox.prefab** can be found in [/MRDesignLab/HUX/Examples/Prefabs/](https://github.com/Microsoft/MRDesignLabs_Unity/tree/master/DesignLabs_Unity/MRDesignLab/HUX/Prefabs/Dialogs/Assets/MRDesignLab/HUX/Examples/Prefabs/) Drag both of these prefabs into the scene.


<img src="https://github.com/Microsoft/MRDesignLabs_Unity/blob/master/External/ReadMeImages/BoundingBoxAppBar-TargetInspector.jpg" alt="Bounding Box Target script provides everything needed to enable object manipulation" width="550px">

Apply the **BoundingBoxTarget.cs** [/MRDesignLab/HUX/Scripts/interaction/](https://github.com/Microsoft/MRDesignLabs_Unity/tree/master/DesignLabs_Unity/Assets/MRDesignLab/HUX/Scripts/Interaction/) script to any object in the scene to enable object manipulation. The script provides full customization of how the object can transform. The default set is Drag, ScaleUniform, and RotateY.

