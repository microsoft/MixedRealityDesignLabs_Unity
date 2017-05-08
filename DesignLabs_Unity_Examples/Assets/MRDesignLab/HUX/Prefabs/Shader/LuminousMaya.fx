//---------------------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//---------------------------------------------------------------------------

// Default values for non-material parameters.
#define DEFAULT_FLOAT_1 DefaultFloatOne < string UIWidget = "None"; > = 1.0;

// Non-stereo rendering.
#define INSTANCE_COUNT  1
#define NUMBER_MIP_MAPS 0


#define SURFACE_GROUP           "Surface Properties"
#define SURFACE_BASE            100

#define FX_GROUP                "FX: Controls"
#define FX_BASE                 150

#define TEXTURE_GROUP           "FX: Textures"
#define TEXTURE_BASE            200

#define FLARE_GROUP             "FX: Lens Flare"
#define FLARE_BASE              250

#define GLOW_GROUP              "FX: Glow Lines"
#define GLOW_BASE               300

#define CURSOR_GROUP            "FX: Cursor Shadow"
#define CURSOR_BASE             350

#define SHADOW_GROUP            "FX: Icon/Text Shadow"
#define SHADOW_BASE             400

#define REFLECT_GROUP           "FX: Reflections"
#define REFLECT_BASE            450

#define HYDRATION_GROUP         "FX: Hydration"
#define HYDRATION_BASE          500

#define SYSTEM_GROUP            "System Variables"
#define SYSTEM_BASE             550

#define TMP_GROUP               "Tmp Variables"
#define TMP_BASE                600



#define SURFACE_ALBEDO_CNTRL SurfaceAlbedo                          \
    <                                                               \
    string  UIGroup  = SURFACE_GROUP;                               \
    string  UIName   = "Albedo Color";                              \
    string  UIWidget = "ColorPicker";                               \
    int     UIOrder  = SURFACE_BASE + 1;                            \
    > = { 1.0f, 1.0f, 1.0f };

#define ALPHA_ENABLE_CNTRL EnableAlpha                              \
    <                                                               \
    string  UIGroup  = SURFACE_GROUP;                               \
    string  UIName   = "Enable Alpha-based transparency";           \
    int     UIOrder  = SURFACE_BASE + 2;                            \
    > = false;

#define SURFACE_OPACITY_CNTRL SurfaceOpacity                        \
    <                                                               \
    string UIGroup  = SURFACE_GROUP;                                \
    string UIName   = "Alpha";                                      \
    string UIWidget = "Slider";                                     \
    float UIMin     = 0.0;                                          \
    float UISoftMax = 1.0;                                          \
    float UIMax     = 1.0;                                          \
    float UIStep    = 0.01;                                         \
    int UIOrder     = SURFACE_BASE + 4;                             \
    > = 0.99;

#define SURFACE_NORMAL_SCALE_CNTRL SurfaceNormalScale               \
    <                                                               \
    string UIGroup  = SURFACE_GROUP;                                \
    string UIName   = "Normal Scale";                               \
    string UIWidget = "Slider";                                     \
    float UIMin     = 0.0;                                          \
    float UISoftMax = 1.0;                                          \
    float UIMax     = 10.0;                                         \
    float UIStep    = 0.01;                                         \
    int UIOrder     = SURFACE_BASE + 5;                             \
    > = 1.00;




#define FX_WIDTH_CNTRL FXWidth                                      \
    <                                                               \
    string UIGroup  = FX_GROUP;                                     \
    string UIName   = "Shader x-limit";                             \
    string UIWidget = "Slider";                                     \
    float UIMin     = 0.0;                                          \
    float UISoftMax = 0.2;                                          \
    float UIMax     = 100.0;                                        \
    float UIStep    = 0.01;                                         \
    int UIOrder     = FX_BASE + 0;                                  \
    > = 0.05;

#define FX_HEIGHT_CNTRL FXHeight                                    \
    <                                                               \
    string UIGroup  = FX_GROUP;                                     \
    string UIName   = "Shader y-limit";                             \
    string UIWidget = "Slider";                                     \
    float UIMin     = 0.0;                                          \
    float UISoftMax = 0.2;                                          \
    float UIMax     = 100.0;                                        \
    float UIStep    = 0.01;                                         \
    int UIOrder     = FX_BASE + 1;                                  \
    > = 0.05;

#define FX_FALLOFF_CNTRL FXFalloff                                  \
    <                                                               \
    string UIGroup  = FX_GROUP;                                     \
    string UIName   = "Limits falloff";                             \
    string UIWidget = "Slider";                                     \
    float UIMin     = 0.0;                                          \
    float UISoftMax = 100.0;                                        \
    float UIMax     = 1000.0;                                       \
    float UIStep    = 1;                                            \
    int UIOrder     = FX_BASE + 2;                                  \
    > = 40.0;

#define FX_ENABLE_DEBUG_CNTRL FXEnableDebug                         \
    <                                                               \
    string UIGroup  = FX_GROUP;                                     \
    string UIName   = "Visualize FX (Limits=R, Glow=G, Drop-Shadow=B)";\
    int UIOrder     = FX_BASE + 3;                                  \
    > = false;




#define TEXTURE_MAP_CNTRL TextureMap                                \
    <                                                               \
    string UIGroup      = TEXTURE_GROUP;                            \
    string UIName       = "Texture";                                \
    string ResourceName = "textures\\white.dds";                    \
    string UIWidget     = "FilePicker";                             \
    string ResourceType = "2D";                                     \
    int mipmaplevels    = NUMBER_MIP_MAPS;                          \
    int UIOrder         = TEXTURE_BASE + 0;                         \
    int UVEditorOrder   = 2;                                        \
    >;

#define TEXTURE_ENABLE_NORMALS_CNTRL EnableNormalMap                \
    <                                                               \
    string UIGroup  = TEXTURE_GROUP;                                \
    string UIName   = "Use texture-based normals (R+G channels)";   \
    int UIOrder     = TEXTURE_BASE + 1;                             \
    > = false;

#define TEXTURE_ENABLE_SHADOWS_CNTRL EnableShadowMap                \
    <                                                               \
    string UIGroup  = TEXTURE_GROUP;                                \
    string UIName   = "Use texture-based drop shadows (R+G or B+A channels)";\
    int UIOrder     = TEXTURE_BASE + 2;                             \
    > = false;

#define TEXTURE_SHADOW_MIX_CNTRL TextureShadowMix                   \
    <                                                               \
    string UIGroup  = TEXTURE_GROUP;                                \
    string UIName   = "Crossfade Shadows";                          \
    string UIWidget = "Slider";                                     \
    float UIMin     = 0.0;                                          \
    float UISoftMax = 1.0;                                          \
    float UIMax     = 1.0;                                          \
    float UIStep    = 0.001;                                        \
    int UIOrder     = TEXTURE_BASE + 3;                             \
    > = 0.0;



#define FLARE_ENABLE_CNTRL EnableInteractFlare                      \
    <                                                               \
    string  UIGroup  = FLARE_GROUP;                                 \
    string  UIName   = "Enable Flare";                              \
    int     UIOrder  = FLARE_BASE + 0;                              \
    > = false;


#define FLARE_CNTRL FlareIntensity                                  \
    <                                                               \
    string UIGroup  = FLARE_GROUP;                                  \
    string UIName   = "Flare Intensity";                            \
    string UIWidget = "Slider";                                     \
    float UIMin     = 0.0;                                          \
    float UISoftMax = 1.0;                                          \
    float UIMax     = 10.0;                                         \
    float UIStep    = 0.01;                                         \
    int UIOrder     = FLARE_BASE + 1;                               \
    > = 0.0;

#define FLARE_RADIUS_CNTRL FlareRadius                              \
    <                                                               \
    string UIGroup  = FLARE_GROUP;                                  \
    string UIName   = "Flare Radius";                               \
    string UIWidget = "Slider";                                     \
    float UIMin     = 0.0;                                          \
    float UISoftMax = 0.5;                                          \
    float UIMax     = 100.0;                                        \
    float UIStep    = 0.01;                                         \
    int UIOrder     = FLARE_BASE + 2;                               \
    > = 0.02;

#define FLARE_COLOR_ENABLE_CNTRL EnableInteractFlareColor           \
    <                                                               \
    string  UIGroup  = FLARE_GROUP;                                 \
    string  UIName   = "Use Polynomial Color";                      \
    int     UIOrder  = FLARE_BASE + 3;                              \
    > = false;

#define FLARE_0TH_ORDER_CNTRL Flare0thOrder                         \
    <                                                               \
    string UIGroup  = FLARE_GROUP;                                  \
    string UIWidget = "Slider";                                     \
    string UIName   = "0th Coefficient";                            \
    int UIOrder     = FLARE_BASE + 4;                               \
     > = {0.3913,0.8453,0.9968};

#define FLARE_0TH_ORDER_ALPHA_CNTRL Flare0thOrderAlpha              \
    <                                                               \
    string UIGroup  = FLARE_GROUP;                                  \
    string UIName   = "0th Alpha";                                  \
    string UIWidget = "Slider";                                     \
    float UIMin     = -10.0;                                        \
    float UISoftMax = 10;                                           \
    float UIMax     = 100.0;                                        \
    float UIStep    = 0.01;                                         \
    int UIOrder     = FLARE_BASE + 5;                               \
    > = 0.0146;

#define FLARE_1ST_ORDER_CNTRL Flare1stOrder                         \
    <                                                               \
    string UIGroup  = FLARE_GROUP;                                  \
    string UIWidget = "Slider";                                     \
    string UIName   = "1st Coefficient";                            \
    int UIOrder     = FLARE_BASE + 6;                               \
     > = {0.5573,0.6098,-0.2163};

#define FLARE_1ST_ORDER_ALPHA_CNTRL Flare1stOrderAlpha              \
    <                                                               \
    string UIGroup  = FLARE_GROUP;                                  \
    string UIName   = "1st Alpha";                                  \
    string UIWidget = "Slider";                                     \
    float UIMin     = -10.0;                                        \
    float UISoftMax = 10;                                           \
    float UIMax     = 100.0;                                        \
    float UIStep    = 0.01;                                         \
    int UIOrder     = FLARE_BASE + 7;                               \
    > = 0.7105;

#define FLARE_2ND_ORDER_CNTRL Flare2ndOrder                         \
    <                                                               \
    string UIGroup  = FLARE_GROUP;                                  \
    string UIWidget = "Slider";                                     \
    string UIName   = "2nd Coefficient";                            \
    int UIOrder     = FLARE_BASE + 8;                               \
     > = {2.7267,-2.4174,-4.186};

#define FLARE_2ND_ORDER_ALPHA_CNTRL Flare2ndOrderAlpha              \
    <                                                               \
    string UIGroup  = FLARE_GROUP;                                  \
    string UIName   = "2nd Alpha";                                  \
    string UIWidget = "Slider";                                     \
    float UIMin     = -10.0;                                        \
    float UISoftMax = 10;                                           \
    float UIMax     = 100.0;                                        \
    float UIStep    = 0.01;                                         \
    int UIOrder     = FLARE_BASE + 9;                               \
    > = 0.4171;

#define FLARE_3RD_ORDER_CNTRL Flare3rdOrder                         \
    <                                                               \
    string UIGroup  = FLARE_GROUP;                                  \
    string UIWidget = "Slider";                                     \
    string UIName   = "3rd Coefficient";                            \
    int UIOrder     = FLARE_BASE + 10;                              \
     > = {-4.9305,2.1202,9.1073};

#define FLARE_3RD_ORDER_ALPHA_CNTRL Flare3rdOrderAlpha              \
    <                                                               \
    string UIGroup  = FLARE_GROUP;                                  \
    string UIName   = "3rd Alpha";                                  \
    string UIWidget = "Slider";                                     \
    float UIMin     = -10.0;                                        \
    float UISoftMax = 10;                                           \
    float UIMax     = 100.0;                                        \
    float UIStep    = 0.01;                                         \
    int UIOrder     = FLARE_BASE + 11;                              \
    > = -0.3155;

#define FLARE_4TH_ORDER_CNTRL Flare4thOrder                         \
    <                                                               \
    string UIGroup  = FLARE_GROUP;                                  \
    string UIWidget = "Slider";                                     \
    string UIName   = "4th Coefficient";                            \
    int UIOrder     = FLARE_BASE + 12;                              \
     > = {2.0836,-0.3467,-4.9097};

#define FLARE_4TH_ORDER_ALPHA_CNTRL Flare4thOrderAlpha              \
    <                                                               \
    string UIGroup  = FLARE_GROUP;                                  \
    string UIName   = "4th Alpha";                                  \
    string UIWidget = "Slider";                                     \
    float UIMin     = -10.0;                                        \
    float UISoftMax = 10;                                           \
    float UIMax     = 100.0;                                        \
    float UIStep    = 0.01;                                         \
    int UIOrder     = FLARE_BASE + 13;                              \
    > = 0.1399;





#define GLOW_ENABLE_CNTRL EnableGlow                                \
    <                                                               \
    string  UIGroup  = GLOW_GROUP;                                  \
    string  UIName   = "Enable Glow";                               \
    int     UIOrder  = GLOW_BASE + 0;                               \
    > = false;

#define GLOW_ENABLE_SCREENSPACE_LINES_CNTRL EnableScreenspaceLines  \
    <                                                               \
    string  UIGroup  = GLOW_GROUP;                                  \
    string  UIName   = "Enable Screenspace Lines";                  \
    int     UIOrder  = GLOW_BASE + 1;                               \
    > = false;

#define GLOW_BASE_CNTRL GlowBase                                    \
    <                                                               \
    string UIGroup  = GLOW_GROUP;                                   \
    string UIName   = "Ambient Intensity";                          \
    string UIWidget = "Slider";                                     \
    float UIMin     = 0.0;                                          \
    float UISoftMax = 1.0;                                          \
    float UIMax     = 10.0;                                         \
    float UIStep    = 0.01;                                         \
    int UIOrder     = GLOW_BASE + 2;                                \
    > = 0.0;

#define GLOW_FLARE_CNTRL GlowFlare                                  \
    <                                                               \
    string UIGroup  = GLOW_GROUP;                                   \
    string UIName   = "Flare Intensity";                            \
    string UIWidget = "Slider";                                     \
    float UIMin     = 0.0;                                          \
    float UISoftMax = 1.0;                                          \
    float UIMax     = 10.0;                                         \
    float UIStep    = 0.01;                                         \
    int UIOrder     = GLOW_BASE + 3;                                \
    > = 0.0;

#define GLOW_WIDTH_CNTRL GlowWidth                                  \
    <                                                               \
    string UIGroup  = GLOW_GROUP;                                   \
    string UIName   = "Line Width (uv|pix)";                        \
    string UIWidget = "Slider";                                     \
    float UIMin     = 0.0;                                          \
    float UISoftMax = 1.0;                                          \
    float UIMax     = 100.0;                                        \
    float UIStep    = 0.01;                                         \
    int UIOrder     = GLOW_BASE + 4;                                \
    > = 0.48;

#define GLOW_SOFTNESS_CNTRL GlowSoftness                            \
    <                                                               \
    string UIGroup  = GLOW_GROUP;                                   \
    string UIName   = "Line Softness";                              \
    string UIWidget = "Slider";                                     \
    float UIMin     = 0.0;                                          \
    float UISoftMax = 1000.0;                                       \
    float UIMax     = 10000.0;                                      \
    float UIStep    = 1.0;                                          \
    int UIOrder     = GLOW_BASE + 5;                                \
    > = 100.0;




#define CURSOR_ENABLE_CNTRL EnableCursor                            \
    <                                                               \
    string  UIGroup  = CURSOR_GROUP;                                \
    string  UIName   = "Enable Cursor";                             \
    int     UIOrder  = CURSOR_BASE + 0;                             \
    > = false;

#define CURSOR_INTENSITY_CNTRL CursorIntensity                      \
    <                                                               \
    string UIGroup  = CURSOR_GROUP;                                 \
    string UIName   = "Cursor Intensity";                           \
    string UIWidget = "Slider";                                     \
    float UIMin     = 0.0;                                          \
    float UISoftMax = 1;                                            \
    float UIMax     = 10.0;                                         \
    float UIStep    = 0.01;                                         \
    int UIOrder     = CURSOR_BASE + 1;                              \
    > = 1.0;

#define CURSOR_INNER_RADIUS_CNTRL CursorInnerRadius                 \
    <                                                               \
    string UIGroup  = CURSOR_GROUP;                                 \
    string UIName   = "Inner Radius";                               \
    string UIWidget = "Slider";                                     \
    float UIMin     = 0.0;                                          \
    float UISoftMax = 0.0001;                                       \
    float UIMax     = 0.001;                                        \
    float UIStep    = 0.00001;                                      \
    int UIOrder     = CURSOR_BASE + 2;                              \
    > = 0.00001;

#define CURSOR_OUTER_RADIUS_CNTRL CursorOuterRadius                 \
    <                                                               \
    string UIGroup  = CURSOR_GROUP;                                 \
    string UIName   = "Outer Radius";                               \
    string UIWidget = "Slider";                                     \
    float UIMin     = 0.0;                                          \
    float UISoftMax = 0.0001;                                       \
    float UIMax     = 0.001;                                        \
    float UIStep    = 0.00001;                                      \
    int UIOrder     = CURSOR_BASE + 3;                              \
    > = 0.00005;

#define CURSOR_SOFTNESS_CNTRL CursorSoftness                        \
    <                                                               \
    string UIGroup  = CURSOR_GROUP;                                 \
    string UIName   = "Cursor Softness";                            \
    string UIWidget = "Slider";                                     \
    float UIMin     = 0.0;                                          \
    float UISoftMax = 500000;                                       \
    float UIMax     = 1000000.0;                                    \
    float UIStep    = 10000;                                        \
    int UIOrder     = CURSOR_BASE + 4;                              \
    > = 100000.0;








#define SHADOW_PROC_ENABLE_CNTRL EnableShadowProc                   \
    <                                                               \
    string UIGroup  = SHADOW_GROUP;                                 \
    string UIName   = "Enable Procedural Shadows";                  \
    int UIOrder     = SHADOW_BASE + 0;                              \
    > = false;

#define SHADOW_CONTRAST_CNTRL MaskContrast                          \
    <                                                               \
    string UIGroup  = SHADOW_GROUP;                                 \
    string UIName   = "Contrast";                                   \
    string UIWidget = "Slider";                                     \
    float UIMin     = 0.0;                                          \
    float UISoftMax = 1.0;                                          \
    float UIMax     = 1.0;                                          \
    float UIStep    = 0.001;                                        \
    int UIOrder     = SHADOW_BASE + 8;                              \
    > = 0.0;

#define SHADOW_TEXT_OFFSET_CNTRL MaskTextOffset                     \
    <                                                               \
    string UIGroup  = SHADOW_GROUP;                                 \
    string UIName   = "Text Vertical Offset";                       \
    string UIWidget = "Slider";                                     \
    float UIMin     = -1.0;                                         \
    float UISoftMax = 1.0;                                          \
    float UIMax     = 100.0;                                        \
    float UIStep    = 0.001;                                        \
    int UIOrder     = SHADOW_BASE + 11;                             \
    > = -0.2;

#define SHADOW_TEXT_SOFTNESS_CNTRL MaskTextSoftness                 \
    <                                                               \
    string UIGroup  = SHADOW_GROUP;                                 \
    string UIName   = "Text Softness";                              \
    string UIWidget = "Slider";                                     \
    float UIMin     = 0.0;                                          \
    float UISoftMax = 1000.0;                                       \
    float UIMax     = 10000.0;                                      \
    float UIStep    = 1;                                            \
    int UIOrder     = SHADOW_BASE + 12;                             \
    > = 300.0;

#define SHADOW_TEXT_WIDTH_CNTRL MaskTextWidth                       \
    <                                                               \
    string UIGroup  = SHADOW_GROUP;                                 \
    string UIName   = "Text Width";                                 \
    string UIWidget = "Slider";                                     \
    float UIMin     = 0.0;                                          \
    float UISoftMax = 0.50;                                         \
    float UIMax     = 10.0;                                         \
    float UIStep    = 1;                                            \
    int UIOrder     = SHADOW_BASE + 13;                             \
    > = 0.20;

#define SHADOW_TEXT_HEIGHT_CNTRL MaskTextHeight                     \
    <                                                               \
    string UIGroup  = SHADOW_GROUP;                                 \
    string UIName   = "Text Height";                                \
    string UIWidget = "Slider";                                     \
    float UIMin     = 0.0;                                          \
    float UISoftMax = 0.050;                                        \
    float UIMax     = 10.0;                                         \
    float UIStep    = 1;                                            \
    int UIOrder     = SHADOW_BASE + 14;                             \
    > = 0.020;

#define SHADOW_ICON_SOFTNESS_CNTRL MaskIconSoftness                 \
    <                                                               \
    string UIGroup  = SHADOW_GROUP;                                 \
    string UIName   = "Icon Softness";                              \
    string UIWidget = "Slider";                                     \
    float UIMin     = 0.0;                                          \
    float UISoftMax = 1000.0;                                       \
    float UIMax     = 10000.0;                                      \
    float UIStep    = 1;                                            \
    int UIOrder     = SHADOW_BASE + 22;                             \
    > = 300.0;

#define SHADOW_ICON_WIDTH_CNTRL MaskIconWidth                       \
    <                                                               \
    string UIGroup  = SHADOW_GROUP;                                 \
    string UIName   = "Icon Width";                                 \
    string UIWidget = "Slider";                                     \
    float UIMin     = 0.0;                                          \
    float UISoftMax = 0.50;                                         \
    float UIMax     = 10.0;                                         \
    float UIStep    = 1;                                            \
    int UIOrder     = SHADOW_BASE + 23;                             \
    > = 0.20;

#define SHADOW_ICON_HEIGHT_CNTRL MaskIconHeight                     \
    <                                                               \
    string UIGroup  = SHADOW_GROUP;                                 \
    string UIName   = "Icon Height";                                \
    string UIWidget = "Slider";                                     \
    float UIMin     = 0.0;                                          \
    float UISoftMax = 0.050;                                        \
    float UIMax     = 10.0;                                         \
    float UIStep    = 1;                                            \
    int UIOrder     = SHADOW_BASE + 24;                             \
    > = 0.020;



#define REFLECT_ENABLE_CNTRL EnableReflect                          \
    <                                                               \
    string  UIGroup  = REFLECT_GROUP;                               \
    string  UIName   = "Enable Reflections";                        \
    int     UIOrder  = REFLECT_BASE + 0;                            \
    > = false;

#define REFLECT_00_CNTRL ReflectSph00                               \
    <                                                               \
    string UIGroup  = REFLECT_GROUP;                                \
    string UIWidget = "Slider";                                     \
    string UIName   = "SpH 00";                                     \
    int UIOrder     = REFLECT_BASE + 1;                             \
     > = {1.91,1.172,1.202};

#define REFLECT_1M1_CNTRL ReflectSph1m1                             \
    <                                                               \
    string UIGroup  = REFLECT_GROUP;                                \
    string UIWidget = "Slider";                                     \
    string UIName   = "SpH 1m1";                                    \
    int UIOrder     = REFLECT_BASE + 2;                             \
     > = {-0.231,0.222,0.216};

#define REFLECT_10_CNTRL ReflectSph10                               \
    <                                                               \
    string UIGroup  = REFLECT_GROUP;                                \
    string UIWidget = "Slider";                                     \
    string UIName   = "SpH 10";                                     \
    int UIOrder     = REFLECT_BASE + 3;                             \
     > = {-0.234,0.218,0.207};

#define REFLECT_11_CNTRL ReflectSph11                               \
    <                                                               \
    string UIGroup  = REFLECT_GROUP;                                \
    string UIWidget = "Slider";                                     \
    string UIName   = "SpH 11";                                     \
    int UIOrder     = REFLECT_BASE + 4;                             \
     > = {0.0,0.0,-0.325};

#define REFLECT_INTENSITY_CNTRL ReflectIntensity                    \
    <                                                               \
    string UIGroup  = REFLECT_GROUP;                                \
    string UIName   = "Reflect Intensity";                          \
    string UIWidget = "Slider";                                     \
    float UIMin     = 0.0;                                          \
    float UISoftMax = 0.50;                                         \
    float UIMax     = 50.0;                                         \
    float UIStep    = 0.1;                                          \
    int UIOrder     = REFLECT_BASE + 5;                             \
    > = 1.0;

#define REFLECT_FREQ_CNTRL ReflectFreq                              \
    <                                                               \
    string UIGroup  = REFLECT_GROUP;                                \
    string UIName   = "Reflect Frequency";                          \
    string UIWidget = "Slider";                                     \
    float UIMin     = 0.0;                                          \
    float UISoftMax = 0.50;                                         \
    float UIMax     = 50.0;                                         \
    float UIStep    = 0.1;                                          \
    int UIOrder     = REFLECT_BASE + 6;                             \
    > = 4.0;

#define REFLECT_FRESNEL_CNTRL ReflectFresnel                        \
    <                                                               \
    string UIGroup  = REFLECT_GROUP;                                \
    string UIName   = "Reflect Fresnel";                            \
    string UIWidget = "Slider";                                     \
    float UIMin     = 0.0;                                          \
    float UISoftMax = 1.0;                                          \
    float UIMax     = 50.0;                                         \
    float UIStep    = 0.1;                                          \
    int UIOrder     = REFLECT_BASE + 7;                             \
    > = 0.0;



#define HYDRATION_ENABLE_CNTRL EnableHydration                      \
    <                                                               \
    string  UIGroup  = HYDRATION_GROUP;                             \
    string  UIName   = "Enable Hydration";                          \
    int     UIOrder  = HYDRATION_BASE + 0;                          \
    > = false;

#define HYDRATION_PLANE_DIR_CNTRL HydrationPlaneDir                 \
    <                                                               \
    string UIGroup  = HYDRATION_GROUP;                              \
    string UIName   = "Plane Normal";                               \
    int UIOrder     = HYDRATION_BASE + 1;                           \
     >;

#define HYDRATION_PROGRESS_CNTRL HydrationProgress                  \
    <                                                               \
    string UIGroup  = HYDRATION_GROUP;                              \
    string UIName   = "Progress";                                   \
    string UIWidget = "Slider";                                     \
    float UIMin     = 0.0;                                          \
    float UISoftMax = 1.0;                                          \
    float UIMax     = 100.0;                                        \
    float UIStep    = 0.01;                                         \
    int UIOrder     = HYDRATION_BASE + 2;                           \
    > = 0.48;

#define HYDRATION_GLOW_WIDTH_CNTRL HydrationGlowWidth               \
    <                                                               \
    string UIGroup  = HYDRATION_GROUP;                              \
    string UIName   = "Effect Width";                               \
    string UIWidget = "Slider";                                     \
    float UIMin     = 0.0;                                          \
    float UISoftMax = 1.0;                                          \
    float UIMax     = 100.0;                                        \
    float UIStep    = 0.01;                                         \
    int UIOrder     = HYDRATION_BASE + 3;                           \
    > = 0.48;

#define HYDRATION_FLASH_START_CNTRL HydrationFlashStart             \
    <                                                               \
    string UIGroup  = HYDRATION_GROUP;                              \
    string UIName   = "Flash Start";                                \
    string UIWidget = "Slider";                                     \
    float UIMin     = 0.0;                                          \
    float UISoftMax = 1.0;                                          \
    float UIMax     = 1.0;                                          \
    float UIStep    = 0.01;                                         \
    int UIOrder     = HYDRATION_BASE + 4;                           \
    > = 0.48;

#define HYDRATION_FLASH_DURATION_CNTRL HydrationFlashDuration       \
    <                                                               \
    string UIGroup  = HYDRATION_GROUP;                              \
    string UIName   = "Flash Duration";                             \
    string UIWidget = "Slider";                                     \
    float UIMin     = 0.0;                                          \
    float UISoftMax = 1.0;                                          \
    float UIMax     = 1.0;                                          \
    float UIStep    = 0.01;                                         \
    int UIOrder     = HYDRATION_BASE + 5;                           \
    > = 0.48;

#define HYDRATION_FLASH_INTENSITY_CNTRL HydrationFlashIntensity     \
    <                                                               \
    string UIGroup  = HYDRATION_GROUP;                              \
    string UIName   = "Flash Intensity";                            \
    string UIWidget = "Slider";                                     \
    float UIMin     = 0.0;                                          \
    float UISoftMax = 1.0;                                          \
    float UIMax     = 10.0;                                         \
    float UIStep    = 0.01;                                         \
    int UIOrder     = HYDRATION_BASE + 6;                           \
    > = 0.48;




#define SYSTEM_GAZE_POSITION_CNTRL SystemGazePosition               \
   <                                                                \
    string UIGroup  = SYSTEM_GROUP;                                 \
    string UIName   = "Gaze Pos";                                   \
    int UIOrder     = SYSTEM_BASE + 0;                              \
    >;

#define SYSTEM_GAZE_DIRECTION_CNTRL SystemGazeDirection             \
    <                                                               \
    string UIGroup  = SYSTEM_GROUP;                                 \
    string UIName   = "Gaze Dir";                                   \
    int UIOrder     = SYSTEM_BASE + 1;                              \
     >;

#define SYSTEM_CURSOR_SCALE_CNTRL SystemCursorScale                 \
    <                                                               \
    string UIGroup  = SYSTEM_GROUP;                                 \
    string UIName   = "Cursor 1/sqrt Scale";                        \
    int UIOrder     = SYSTEM_BASE + 2;                              \
    > = 1.0;

#define SYSTEM_INTERACTION_SCALE_CNTRL InteractionScale             \
    <                                                               \
    string UIGroup  = SYSTEM_GROUP;                                 \
    string UIName   = "Interaction Scale (x,1/x^2,1)";              \
    int UIOrder     = SYSTEM_BASE + 3;                              \
    > = { 1.0f, 1.0f, 1.0f };

#define SYSTEM_OBJECT_BBX_CNTRL SystemObjectBBX                     \
    <                                                               \
    string UIGroup  = SYSTEM_GROUP;                                 \
    string UIName   = "Bounding Box";                                \
    int UIOrder     = SYSTEM_BASE + 4;                              \
    > = { 1.0f, 1.0f, 1.0f };




#define TMP0_CNTRL Tmp0                                             \
   <                                                                \
    string UIGroup  = TMP_GROUP;                                    \
    string UIName   = "Tmp0";                                       \
    int UIOrder     = TMP_BASE + 0;                                 \
    >;

#define TMP1_CNTRL Tmp1                                             \
   <                                                                \
    string UIGroup  = TMP_GROUP;                                    \
    string UIName   = "Tmp1";                                       \
    int UIOrder     = TMP_BASE + 1;                                 \
    >;

#define TMP2_CNTRL Tmp2                                             \
   <                                                                \
    string UIGroup  = TMP_GROUP;                                    \
    string UIName   = "Tmp2";                                       \
    int UIOrder     = TMP_BASE + 2;                                 \
    >;

#define TMP3_CNTRL Tmp3                                             \
   <                                                                \
    string UIGroup  = TMP_GROUP;                                    \
    string UIName   = "Tmp3";                                       \
    int UIOrder     = TMP_BASE + 3;                                 \
    >;

#define TMP4_CNTRL Tmp4                                             \
   <                                                                \
    string UIGroup  = TMP_GROUP;                                    \
    string UIName   = "Tmp4";                                       \
    int UIOrder     = TMP_BASE + 4;                                 \
    >;





extern uniform bool bSwatch : MayaSwatchRender < string UIWidget = "None"; >;

// The world matrix is a part of the instance data in the shell.
// cbuffer UpdatePerMesh
// {
//     // World matrices
//     extern uniform float4x4 mWorld         : World                 < string UIWidget = "None"; >;
//     extern uniform float4x4 mWorldInvTrans : WorldInverseTranspose < string UIWidget = "None"; >;
// }


//------------------------------------
// Per Frame parameters
//------------------------------------
cbuffer UpdatePerFrame : register(b0)
{
    float4x4 viewInv        : ViewInverse           < string UIWidget = "None"; >;   
    float4x4 view           : View                  < string UIWidget = "None"; >;
    float4x4 prj            : Projection            < string UIWidget = "None"; >;
    float4x4 viewPrj        : ViewProjection        < string UIWidget = "None"; >;

    // A shader may wish to do different actions when Maya is rendering the preview swatch (e.g. disable displacement)
    // This value will be true if Maya is rendering the swatch
    bool IsSwatchRender     : MayaSwatchRender      < string UIWidget = "None"; > = false;

    // If the user enables viewport gamma correction in Maya's global viewport rendering settings, the shader should not do gamma again
    bool MayaFullScreenGamma : MayaGammaCorrection < string UIWidget = "None"; > = false;

   // World matrices
    extern uniform float4x4 mWorld         : World                 < string UIWidget = "None"; >;
    extern uniform float4x4 mWorldInvTrans : WorldInverseTranspose < string UIWidget = "None"; >;

}

// cbuffer UpdatePerObject : register(b1)
// {
//     extern uniform float4x4 world      : World                     < string UIWidget = "None"; >;
//     extern uniform float4x4 worldIT    : WorldInverseTranspose     < string UIWidget = "None"; >;
// }

// Constant matrix to transform cube map
// lookup to maya space.
const float4x4 mToMayaCube = {
    1, 0, 0, 0,
    0, 0, 1, 0,
    0,-1, 0, 0,
    0, 0, 0, 1};

// define the new #defines for the vertex shader
#define VS_INPUT_NORMAL
// Include the pipeline shaders.
#include <shaders\Luminous.vs>
#include <shaders\Luminous.gs>
#include <shaders\Luminous.ps>


#include <MayaStates.fx>


technique11 Interact_Over
<
/* Refer to AutodeskUberShader.fx for transparency sorting info */
int isTransparent = 3;
string transparencyTest = "fAlphaScale < 1.0";
>
{
    pass P0
    {
        SetBlendState(alphaBlend, float4(0,0,0,1), 0xffffffff);
        SetVertexShader(CompileShader(vs_5_0, VsMain()));
        SetGeometryShader(CompileShader(gs_5_0, GsMain()));
        SetPixelShader(CompileShader(ps_5_0, PsMain()));
    }
}

technique11 Interact_Additive
<
    int isTransparent = 3;
    string transparencyTest = "fAlphaScale < 1.0";
>
{
    pass P0
    {
        SetBlendState(additiveBlend, float4(0,0,0,1), 0xffffffff);
        SetVertexShader(CompileShader(vs_5_0, VsMain()));
        SetGeometryShader(CompileShader(gs_5_0, GsMain()));
        SetPixelShader(CompileShader(ps_5_0, PsMain()));
    }
}
