![](External/ReadMeImages/MRDL_Logo_Rev.png)

# What is Mixed Reality Design Labs?
The Mixed Reality Design Labs (MRDL) is a collection of well-documented, open-source samples, based on the foundation of **[Mixed Reality Toolkit (MRTK)](https://github.com/Microsoft/MixedRealityToolkit-Unity)**. The goal is to inspire creators and help them build compelling, efficient Mixed Reality experiences. 

MRTK offers building-block components, and MRDL leverages them to offer more complete experiences and samples. As the name suggests, these samples are experimental/’works-in-progress’, driven by experience design, which provide creators with concrete examples of best practices for app experiences, UX, and MRTK implementation. By ‘experimental’, we mean MRDL is not officially supported/maintained by Microsoft (e.g. updated to latest versions of Unity), whereas MRTK is officially supported/maintained.  

<img src="External/ReadMeImages/MixedRealityStack-Apps3.png" width="600"><img src="External/ReadMeImages/MixedRealityStack-MRTK-Unity.png" width="600"><img src="External/ReadMeImages/MixedRealityStack-MRTK.png" width="600"><img src="External/ReadMeImages/MixedRealityStack-UWP.png" width="600">


# Sample apps
MRDL samples have been a successful pipeline for taking app-specific interactions and pushing them back as controls/patterns for MRTK. MRTK/MRDL have blossomed into a symbiotic relationship where iteration on each side has made the other better. This repo is where Microsoft's Windows Mixed Reality Design team publishes sample apps and experiments. If you are looking for the official toolkit, please use **[Mixed Reality Toolkit](https://github.com/Microsoft/MixedRealityToolkit-Unity)**.

| [Sample app - Lunar Module](https://github.com/Microsoft/MRDesignLabs_Unity_LunarModule) | [Sample app - Periodic Table of the Elements](https://github.com/Microsoft/MRDesignLabs_Unity_PeriodicTable) |
|:---------|:--------------|
|<img src="https://github.com/Microsoft/MRDesignLabs_Unity_LunarModule/blob/master/External/ReadMeImages/LM_hero.jpg" alt="Lunar Module sample app"> | <img src="https://github.com/Microsoft/MRDesignLabs_Unity_PeriodicTable/blob/master/External/ReadMeImages/PeriodicTable_Hero.jpg" alt="Periodic Table of the Elements">|
|Lunar Module demonstrates how to extend Hololens' base gestures with two hand tracking and xbox controller input, reactive objects to surface mapping and plane finding, and simple menu systems. You can use this project's components to create your own mixed reality app experience. |Periodic Table of the Elements app demonstrates how to use Object Collection to lay out an array of objects in three-dimensional space with various surface types. You can also find the example of using Fluent Design elements such as light, depth, motion and material.|


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


# Contributing

This project has adopted the [Microsoft Open Source Code of Conduct](https://opensource.microsoft.com/codeofconduct/). For more information see the [Code of Conduct FAQ](https://opensource.microsoft.com/codeofconduct/faq/) or contact [opencode@microsoft.com](mailto:opencode@microsoft.com) with any additional questions or comments.

# Useful resources on Microsoft Windows Dev Center
| ![Academy](https://github.com/Microsoft/MixedRealityToolkit-Unity/blob/master/External/ReadMeImages/icon_academy.png) [Academy](https://developer.microsoft.com/en-us/windows/mixed-reality/academy)| ![Design](https://github.com/Microsoft/MixedRealityToolkit-Unity/blob/master/External/ReadMeImages/icon_design.png) [Design](https://developer.microsoft.com/en-us/windows/mixed-reality/design)| ![Development](https://github.com/Microsoft/MixedRealityToolkit-Unity/blob/master/External/ReadMeImages/icon_development.png) [Development](https://developer.microsoft.com/en-us/windows/mixed-reality/development)| ![Community)](https://github.com/Microsoft/MixedRealityToolkit-Unity/blob/master/External/ReadMeImages/icon_community.png) [Community](https://developer.microsoft.com/en-us/windows/mixed-reality/community)|
| :--------------------- | :----------------- | :------------------ | :------------------------ |
| See code examples.<br/> Do a coding tutorial.<br/> Watch guest lectures.          | Get design guides.<br/> Build user interface.<br/> Learn interactions and input.     | Get development guides.<br/> Learn the technology.<br/> Understand the science.       | Join open source projects.<br/> Ask questions on forums.<br/> Attend events and meetups. |

