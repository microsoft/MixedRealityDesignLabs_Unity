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

This is an example of Holographic button used in the Start menu and ![App Bar](https://developer.microsoft.com/en-us/windows/mixed-reality/app_bar_and_bounding_box). This example uses Unity's Animation Controller and Animation Clips.


### Toolbar ###

![Toolbar](https://github.com/Microsoft/MRDesignLabs_Unity/blob/master/External/ReadMeImages/InteractibleObject_Toolbar.jpg)

Toolbar is a widely used pattern in mixed reality experiences. It is a simple collection of buttons with additional behavior such as Billboarding and tag-a-long. This example uses a Billboarding and tag-a-long script from HoloToolkit. You can control the detailed behavior including distance, moving speed and threshold values.


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

There are three components to ensure the Holobar and bounding box work inside of your Unity project.

The first is the **BoundingBoxTarget.cs** component. If an InteractableObject with a [BoundingBoxTarget.cs](https://github.com/Microsoft/MRDesignLabs_Unity/blob/master/DesignLabs_Unity_Examples/Assets/MRDesignLab/HUX/Scripts/Interaction/BoundingBoxTarget.cs) component is interacted with, it will request the App bar and take it from the last interacted object. It essentially is the glue to make all of the MRDesignLab manipulation components talk to each other. Assign BoundingBoxTarget.cs script to any objects that you want to use bounding box and app bar.

The [ManipulationManager.cs](https://github.com/Microsoft/MRDesignLabs_Unity_Tools/blob/master/HUX/Scripts/Interaction/ManipulationManager.cs) component makes it easy to ensure that the prefabs necessary for [BoundingBoxTarget.cs](https://github.com/Microsoft/MRDesignLabs_Unity/blob/master/DesignLabs_Unity_Examples/Assets/MRDesignLab/HUX/Scripts/Interaction/BoundingBoxTarget.cs) to function are present in your scene. This singleton instantiates **AppBar.prefab** and **BoundingBoxShell.prefab** on startup and provides properties to access them both.

The second, **AppBar.prefab**, behaves much like a singleton. You only need one in your scene hierarchy. The AppBar.prefab has a [AppBar.cs](https://github.com/Microsoft/MRDesignLabs_Unity/blob/master/DesignLabs_Unity_Examples/Assets/MRDesignLab/HUX/Scripts/Dialogs/AppBar.cs) script which handles all of the properties of how and what the Holobar should display.

The third is the **BoundingBoxShell.prefab**. This prefab works much like the AppBar.prefab. You only need one bounding box in the scene. If an object in the scene is put into adjust mode, the object will take the bounding box from the last adjusted object. The prefab has two scripts associated with it. The [BoundingBoxManipulate.cs](https://github.com/Microsoft/MRDesignLabs_Unity/blob/master/DesignLabs_Unity_Examples/Assets/MRDesignLab/HUX/Scripts/Interaction/BoundingBoxManipulate.cs) script which does the actual manipulating of the object its currently assigned to. The other script is the  [BoundingBoxGizmoShell.cs](https://github.com/Microsoft/MRDesignLabs_Unity/blob/master/DesignLabs_Unity_Examples/Assets/MRDesignLab/HUX/Scripts/Interaction/BoundingBoxGizmoShell.cs) prefab. This script handles the visual representation of the bounding box and renders the transform affordances dynamically.

This is only one way to use BoundingBox components. If you want a custom manipulation style or a unique look we recommend you extend the BoundingBox.cs and BoundingBoxGizmo.cs base classes to create your own BoundingBox prefabs.

Clone and open the project [MRDesignLabs_Unity](https://github.com/Microsoft/MRDesignLabs_Unity) in Unity.



<img src="https://github.com/Microsoft/MRDesignLabs_Unity/blob/master/External/ReadMeImages/NewHLCamera.png" alt="Use the HUX menu to create a new HoloLens camera instance">

The default **HololensCamera** prefab provided in [MRDesignLabs_Unity](https://github.com/Microsoft/MRDesignLabs_Unity) provides global event and input handling required for the Holobar and Bounding box to work. It can be found at [/MRDesignLab/Hux/Prefab/Interface](https://github.com/Microsoft/MRDesignLabs_Unity/tree/master/DesignLabs_Unity/Assets/MRDesignLab/HUX/Prefabs/Interface). Additionally, once **MRDesignLabs** is in your unity project, you can use the menu system to create an instance of the HoloLens default camera.


<img src="https://github.com/Microsoft/MRDesignLabs_Unity/blob/master/External/ReadMeImages/BoundingBoxAppBar-InteractionManager.jpg" alt="Interaction Manager under HoloLensCamera" width="550px">

Please make sure **ManipulationTranslate** is checked in **InteractionManager** in HoloLens object.

<img src="https://github.com/Microsoft/MRDesignLabs_Unity/blob/master/External/ReadMeImages/BoundingBoxAppBar-scene.jpg" alt="The bounding box and app bar prefabs in the scene" width="550px">

You can find the **AppBar.prefab** in [/MRDesignLab/HUX/Prefabs/Dialogs](https://github.com/Microsoft/MRDesignLabs_Unity/tree/master/DesignLabs_Unity/Assets/MRDesignLab/HUX/Prefabs/Dialogs/) and the **BoundingBox.prefab** can be found in [/MRDesignLab/HUX/Examples/Prefabs/](https://github.com/Microsoft/MRDesignLabs_Unity/tree/master/DesignLabs_Unity/MRDesignLab/HUX/Prefabs/Dialogs/Assets/MRDesignLab/HUX/Examples/Prefabs/) Drag both of these prefabs into the scene.


<img src="https://github.com/Microsoft/MRDesignLabs_Unity/blob/master/External/ReadMeImages/BoundingBoxAppBar-TargetInspector.jpg" alt="Bounding Box Target script provides everything needed to enable object manipulation" width="550px">

Apply the **BoundingBoxTarget.cs** [/MRDesignLab/HUX/Scripts/interaction/](https://github.com/Microsoft/MRDesignLabs_Unity/tree/master/DesignLabs_Unity/Assets/MRDesignLab/HUX/Scripts/Interaction/) script to any object in the scene to enable object manipulation. The script provides full customization of how the object can transform. The default set is Drag, ScaleUniform, and RotateY.


## Billboarding and tag-along

### What is billboarding? ###

Billboarding is a behavioral concept that can be applied to objects in mixed reality. Objects with billboarding always orient themselves to face the user. This is especially helpful with text and menuing systems where world-locked objects would be otherwise obscured or unreadable if a user were to move around their environment.   

Objects with billboarding enabled can rotate freely, or on the Y axis depending on where they may be placed in the environment. Keep in mind, billboarded objects may clip or occlude themselves if they are placed too close to other objects, or in HoloLens, too close scanned surfaces. To avoid this, think about the total footprint an object may produce when rotated on the axis enabled for billboarding.

### How to use billboarding ###

<img src="https://github.com/Microsoft/MRDesignLabs_Unity/blob/master/External/ReadMeImages/Billboarding-fragments.gif" alt="Billboarding menu system in Fragments">

Billboarding can be applied to any world-locked object. It simply creates a directional vector that points from the user to the object. On update, the script applies a LookRotation Quaternion that is the negative value of the directional vector to the object’s rotation transform. To use billboarding, follow the steps below: 

1. Clone and open the project MRDesignLabs_Unity in Unity. In this project, you can find the Billboard.cs script under /Assets/HoloToolKit/Utilities/Scripts.


<img src="https://github.com/Microsoft/MRDesignLabs_Unity/blob/master/External/ReadMeImages/billboard-dialogue.png" alt="Billboarding properties in the inspector">
2. To create a enable billboarding behavior, assign the Billboard.cs script to any GameObject or prefab in the scene. 

3. Once the script is applied to an object, you can choose between free and Y for the pivot axis of the billboard.


<img src="https://github.com/Microsoft/MRDesignLabs_Unity/blob/master/External/ReadMeImages/NewHLCamera.png" alt="Use the HUX menu to create a new HoloLens camera instance">
The Billboard.cs script is calculated based on the camera with the tag MainCamera. To make things easy just use the Default HoloLens camera Prefab provided in MRDesignLabs_Unity as your camera. It can be found at /MRDesignLab/Hux/Prefab/Interface. Additionally, once MRDesignLabs is in your unity project, you can use the menu system to create an instance of the HoloLens default camera. 

### What is tag-along? ###

Tag-along is a behavioral concept that can be added to holograms, including billboarded objects. This interaction is a more natural and friendly way of achieving the effect of head-locked content. A tag-along object attempts to never leave the user's view. This enables freely interact with what is front of them while also still seeing the holograms outside their direct view.

<img src="https://github.com/Microsoft/MRDesignLabs_Unity/blob/master/External/ReadMeImages/TagAlong.jpg" alt="Tag-along behavior in the pins panel">

Tag-along objects have parameters which can fine-tune the way they behave. Content can be in or out of the user’s line of sight as desired while the user moves around their environment. As the user moves, the content will attempt to stay within the user’s periphery by sliding towards the edge of the view, depending on how quickly a user moves may leave the content temporarily out of view. When the user gazes towards the tag-along object, it comes more fully into view. Think of content always being "a glance away" so users never forget what direction their content is in.

Additional parameters can make the tag-along object feel attached to the user's head by a rubber band. Dampening acceleration or deceleration gives weight to the object making it feel more physically present. This spring behavior is an affordance that helps the user build an accurate mental model of how tag-along works. Audio helps provide additional affordances for when users have objects in tag-along mode. Audio should reinforce the speed of movement; a fast head turn should provide a more noticeable sound effect while walking at a natural speed should have minimal audio if any effects at all.

Just like truly head-locked content, tag-along objects can prove overwhelming or nauseating if they move wildly or spring too much in the user’s view. As users look around and then quickly stop, the user’s senses tell them they have stopped. Their balance informs them their head has stopped turning as well as their vision sees the world stop turning. However, if tag-along keeps on moving when the user has stopped, it may confuse their senses.

### How to use tag-along ###

The simplest way to create the effect of tag-along behavior is to cast a cone from the user's camera. As the user's head turns, the tag-along object is calculated on update to stay within the bounds of the base of the cone. This functionality can be applied to your scene below.

1. Clone and open the project MRDesignLabs_Unity in Unity.
2. You can find the RadialViewSolver prefab under /MRDesignLab/HUX/Prefabs/Spatial/Solvers/.
 
<img src="https://github.com/Microsoft/MRDesignLabs_Unity/blob/master/External/ReadMeImages/radialViewSolverApplied.PNG" alt="Unity scene hierarchy with Radial view solver applied">
To apply the tag-along behavior to an object in the scene, add a RadialViewSolver prefab to the scene. Drag any object or prefab the onto RadialViewSolver so it becomes a child. 


<img src="https://github.com/Microsoft/MRDesignLabs_Unity/blob/master/External/ReadMeImages/NewHLCamera.png" alt="Use the HUX menu to create a new HoloLens camera instance">
The RadialViewSolver's scripts are calculated based on the Default HololensCamera prefab provided in MRDesignLabs_Unity as your camera. It can be found at /MRDesignLab/Hux/Prefab/Interface. Additionally, once MRDesignLabs is in your unity project, you can use the menu system to create an instance of the HoloLens default camera. 

MRDesignLabs also provides a RectViewSolver script which behaves the same as RadialViewSolver but casts a pyramid instead of a cone. /MRDesignLab/HUX/Scripts/Spatial/Solvers/. Replace the RadialViewSolver.cs with this script.
