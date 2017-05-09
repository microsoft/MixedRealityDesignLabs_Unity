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

## Ways to use Interactable Objects ##

In the scene **InteractableObject_Examples.unity**, you will be able to find various combinations of 'CompoundButton' scripts. To create your own Interactable Object, you can combine different types of 'CompountButton' scripts. You can find them in **MRDesignLab\HUX\Scripts\Buttons**. It is designed to support various types of Interactable Object in flexible way.

<img src="https://github.com/Microsoft/MRDesignLabs_Unity/blob/master/External/ReadMeImages/InteractibleObject_CompoundButtonScripts.png" alt="Compound Button" width="450px">

### Compound Button ###
<img src="https://github.com/Microsoft/MRDesignLabs_Unity/blob/master/External/ReadMeImages/InteractibleObject_CompoundButton.jpg" alt="Compound Button" width="450px">
This is the base of the button component. You will need this script to build any types of Interactable Objects.

### Compound Button Mesh ###
<img src="https://github.com/Microsoft/MRDesignLabs_Unity/blob/master/External/ReadMeImages/InteractibleObject_CompoundButtonMesh.jpg" alt="Compound Button" width="450px">

Use this script to use various types of custom mesh. You can use your own 3D models imported from 3D modeling software. Using this scripts, you can easily change the scale, offset of the mesh or material properties such as color for the different input interaction states. To create an Interactable Object using script, it is recommended to create an empty GameObject as a container and put the 3D mesh model under it as child component. This will prevent unexpected behavior from different scaling or offset values.

### Icon ###
<img src="https://github.com/Microsoft/MRDesignLabs_Unity/blob/master/External/ReadMeImages/InteractibleObject_CompoundButtonIcon.jpg" alt="Compound Button" width="450px">

Using this scripts, you can use various types of icon provided by Windows fonts. To use this feature, you should have your Windows updated to 'Creators Update'.

### Text ###
<img src="https://github.com/Microsoft/MRDesignLabs_Unity/blob/master/External/ReadMeImages/InteractibleObject_CompoundButtonText.jpg" alt="Compound Button" width="450px">

This scripts helps you manage a TextMesh component to display text on your button. This script can be used in conjunction with a CompoundButtonSpeech component to automatically link your button to spoken keywords.

### Sound ###
<img src="https://github.com/Microsoft/MRDesignLabs_Unity/blob/master/External/ReadMeImages/InteractibleObject_CompoundButtonSounds.jpg" alt="Compound Button" width="450px">

Use this script to add audio feedback for the different input interaction states.

### Speech ###
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

