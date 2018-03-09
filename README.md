![](External/ReadMeImages/MRDL_Logo_Rev.png)

# Mixed Reality Design Labs
This repo is where Microsoft's Windows Mixed Reality Design team publishes sample apps and experiments. If you are looking for official toolkit, please use **[Mixed Reality Toolkit](https://github.com/Microsoft/MixedRealityToolkit-Unity)**

**MRDL is based on [HUX](https://github.com/Microsoft/MRDesignLabs_Unity_Tools) which has an input system that is not compatible with Mixed Reality Toolkit.**

# Supported Unity version: 2017.1.0f3
The current supported version of Unity is [**Unity 2017.1.0f3**](https://unity3d.com/get-unity/download?thank-you=update&download_nid=47505&os=Win).  If you are looking to have support for previous versions of Unity please check under **[Releases](https://github.com/Microsoft/MRDesignLabs_Unity/releases)**.

# Important: Requires submodule MRDesignLab
As soon as you clone the repo, init and update submodule with git command:
```
cd MRDesignLabs_Unity
git submodule init
git submodule update
```

To update the submodules you'll still need to pull from master by either going into the individual submodule directory and doing a git pull or by doing the following command to do pulls on all the submodules:
```
git submodule foreach git pull
```

This will add [HUX and related tools](https://github.com/Microsoft/MRDesignLabs_Unity_tools) under Assets/MRDesignLab/ folder

# Mixed Reality Design Labs(MRDL) and [Mixed Reality Toolkit(MRTK)](https://github.com/Microsoft/MixedRealityToolkit-Unity)
We are in process of migrating foundational UI controls and building blocks from MRDL to MRTK. Below you can find the list of the components that have been merged.  

## Ported UI controls and building blocks, now available on MRTK
- [Bounding Box and App Bar](https://github.com/Microsoft/MixedRealityToolkit-Unity/blob/Dev_Working_Branch/Assets/MixedRealityToolkit-Examples/UX/Readme/README_BoundingBoxGizmoExample.md)
- [Buttons(Interactable Object)](https://github.com/Microsoft/MixedRealityToolkit-Unity/blob/master/Assets/HoloToolkit-Examples/UX/Readme/README_InteractableObjectExample.md)  
- [Interaction Receivers](https://github.com/Microsoft/MixedRealityToolkit-Unity/blob/master/Assets/HoloToolkit-Examples/UX/Readme/README_InteractableObjectExample.md)
- [Object Collection](https://github.com/Microsoft/MixedRealityToolkit-Unity/blob/master/Assets/HoloToolkit-Examples/UX/Readme/README_ObjectCollection.md)
- [Line creation / Rendering classes for parabolic pointers](https://github.com/Microsoft/MixedRealityToolkit-Unity/pull/1305)
- [Multi-step pointer for parabolic pointers](https://github.com/Microsoft/MixedRealityToolkit-Unity/pull/1332)
- [Solvers](https://github.com/Microsoft/MixedRealityToolkit-Unity/tree/Dev_Working_Branch/Assets/MixedRealityToolkit/Utilities/Scripts/Solvers)
- [Dialog and Progress (in PR)](https://github.com/Microsoft/MixedRealityToolkit-Unity/pull/1718)

## Sample app - Periodic Table of the Elements ##
<img src="https://github.com/Microsoft/MRDesignLabs_Unity_PeriodicTable/blob/master/External/ReadMeImages/PeriodicTable_Hero.jpg" alt="Periodic Table of the Elements">
https://github.com/Microsoft/MRDesignLabs_Unity_PeriodicTable

Periodic Table of the Elements is a open-source sample app from Microsoft's Mixed Reality Design Lab. With this project, you can learn how to layout an array of objects in 3D space with various surface types using Object collection. Also learn how to create interactable objects that respond to standard inputs from HoloLens. You can use this project's components to create your own mixed reality app experiences.

## Sample app - Lunar Module ##
<img src="https://github.com/Microsoft/MRDesignLabs_Unity_LunarModule/blob/master/External/ReadMeImages/LM_hero.jpg" alt="Lunar Module sample app">
https://github.com/Microsoft/MRDesignLabs_Unity_LunarModule

Lunar Module is a open-source sample app from Microsoft's Mixed Reality Design Labs, it is a spiritual sequel to the 1979 Atari classic, *Lunar Lander*. This sample app will demonstrate how to extend Hololens' base gestures with two hand tracking and xbox controller input, reactive objects to surface mapping and plane finding, and simple menu systems. You can use this project's components to create your own mixed reality app experience. 


# Contributing

This project has adopted the [Microsoft Open Source Code of Conduct](https://opensource.microsoft.com/codeofconduct/). For more information see the [Code of Conduct FAQ](https://opensource.microsoft.com/codeofconduct/faq/) or contact [opencode@microsoft.com](mailto:opencode@microsoft.com) with any additional questions or comments.

# Useful resources on Microsoft Windows Dev Center
| ![Academy](https://github.com/Microsoft/MixedRealityToolkit-Unity/blob/master/External/ReadMeImages/icon_academy.png) [Academy](https://developer.microsoft.com/en-us/windows/mixed-reality/academy)| ![Design](https://github.com/Microsoft/MixedRealityToolkit-Unity/blob/master/External/ReadMeImages/icon_design.png) [Design](https://developer.microsoft.com/en-us/windows/mixed-reality/design)| ![Development](https://github.com/Microsoft/MixedRealityToolkit-Unity/blob/master/External/ReadMeImages/icon_development.png) [Development](https://developer.microsoft.com/en-us/windows/mixed-reality/development)| ![Community)](https://github.com/Microsoft/MixedRealityToolkit-Unity/blob/master/External/ReadMeImages/icon_community.png) [Community](https://developer.microsoft.com/en-us/windows/mixed-reality/community)|
| :--------------------- | :----------------- | :------------------ | :------------------------ |
| See code examples.<br/> Do a coding tutorial.<br/> Watch guest lectures.          | Get design guides.<br/> Build user interface.<br/> Learn interactions and input.     | Get development guides.<br/> Learn the technology.<br/> Understand the science.       | Join open source projects.<br/> Ask questions on forums.<br/> Attend events and meetups. |

