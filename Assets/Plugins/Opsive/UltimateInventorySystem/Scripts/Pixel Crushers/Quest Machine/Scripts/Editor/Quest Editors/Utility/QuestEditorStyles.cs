// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;
using UnityEditor;

namespace PixelCrushers.QuestMachine
{

    /// <summary>
    /// GUI styles, images, etc., used by the custom editors.
    /// </summary>
    public static class QuestEditorStyles
    {

        public const string LogoImagePath = "Quest Machine/questMachineLogo.png";
        public const string TitleImagePath = "Quest Machine/title.png";

        public const string ConnectorImagePath = "Quest Machine/connector.png";
        public const string PassthroughImagePath = "Quest Machine/passthrough.png";

        public const string EntityImagePath = "Quest Machine/entity.png";
        public const string FactionImagePath = "Quest Machine/faction.png";
        public const string DriveImagePath = "Quest Machine/drive.png";
        public const string ActionImagePath = "Quest Machine/action.png";
        public const string UrgencyFunctionImagePath = "Quest Machine/urgencyFunction.png";
        public const string DomainImagePath = "Quest Machine/domain.png";

#if UNITY_2019_1_OR_NEWER
        public const string CollapsibleHeaderButtonStyleName = "Popup";
#else
        public const string CollapsibleHeaderButtonStyleName = "dragtab"; // Alternate choice: "OL Title";
#endif
        public const string CollapsibleSubheaderButtonStyleName = "MiniToolbarButtonLeft"; // Alternate choice: "ObjectFieldThumb";

#if UNITY_5 && !EVALUATION_VERSION
        public const string GroupBoxStyle = "AS TextArea";
#else
        public static GUIStyle GroupBoxStyle = EditorStyles.helpBox;
#endif

        public const string FoldoutOpenArrow = "\u25BC ";
        public const string FoldoutClosedArrow = "\u25BA ";

        private static Texture2D m_logo = null;
        private static Texture2D m_titleImage = null;
        private static Texture2D m_connectorImage = null;
        private static Texture2D m_passthroughImage = null;

        private static GUIStyle m_questNameGUIStyle = null;

        private static Texture2D m_entityImage = null;
        private static Texture2D m_actionImage = null;
        private static Texture2D m_factionImage = null;
        private static Texture2D m_driveImage = null;
        private static Texture2D m_urgencyFunctionImage = null;
        private static Texture2D m_domainImage = null;

        public static Texture2D logo { get { return GetEditorImage(LogoImagePath, ref m_logo); } }
        public static Texture2D titleImage { get { return GetEditorImage(TitleImagePath, ref m_titleImage); } }
        public static Texture2D connectorImage { get { return GetEditorImage(ConnectorImagePath, ref m_connectorImage); } }
        public static Texture2D passthroughImage { get { return GetEditorImage(PassthroughImagePath, ref m_passthroughImage); } }

        public static Texture2D entityImage { get { return GetEditorImage(EntityImagePath, ref m_entityImage); } }
        public static Texture2D factionImage { get { return GetEditorImage(FactionImagePath, ref m_factionImage); } }
        public static Texture2D driveImage { get { return GetEditorImage(DriveImagePath, ref m_driveImage); } }
        public static Texture2D actionImage { get { return GetEditorImage(ActionImagePath, ref m_actionImage); } }
        public static Texture2D urgencyFunctionImage { get { return GetEditorImage(UrgencyFunctionImagePath, ref m_urgencyFunctionImage); } }
        public static Texture2D domainImage { get { return GetEditorImage(DomainImagePath, ref m_domainImage); } }

        private static Texture2D GetImage(string imagePath, ref Texture2D image)
        {
            if (image == null) image = AssetDatabase.LoadAssetAtPath<Texture2D>(imagePath);
            return image;
        }

        private static Texture2D GetEditorImage(string imagePath, ref Texture2D image)
        {
            if (image == null)
            {
                image = EditorGUIUtility.Load(imagePath) as Texture2D;
                if (image == null)
                {
                    image = new Texture2D(1, 1);
                    Debug.LogError("Quest Machine: The file Assets/Editor Default Resources/" + imagePath + " is missing.");
                }
            }
            return image;
        }

        public static void ResetImages()
        {
            if (m_startBarGuiStyle != null) Object.DestroyImmediate(m_startBarGuiStyle.normal.background);
            if (m_successBarGuiStyle != null) Object.DestroyImmediate(m_successBarGuiStyle.normal.background);
            if (m_failureBarGuiStyle != null) Object.DestroyImmediate(m_failureBarGuiStyle.normal.background);
            if (m_defaultBarGuiStyle != null) Object.DestroyImmediate(m_defaultBarGuiStyle.normal.background);
            m_startBarGuiStyle = null;
            m_successBarGuiStyle = null;
            m_failureBarGuiStyle = null;
            m_defaultBarGuiStyle = null;
        }

        private static Color ProSkinCollapsibleHeaderOpenColor = new Color(0.3f, 0.8f, 1f);
        private static Color ProSkinCollapsibleHeaderClosedColor = new Color(0.6f, 0.6f, 0.6f);
        private static Color LightSkinCollapsibleHeaderOpenColor = new Color(0.3f, 0.8f, 1f);
        private static Color LightSkinCollapsibleHeaderClosedColor = new Color(0.6f, 0.6f, 0.6f);
        public static Color collapsibleHeaderOpenColor { get { return EditorGUIUtility.isProSkin ? ProSkinCollapsibleHeaderOpenColor : LightSkinCollapsibleHeaderOpenColor; } }
        public static Color collapsibleHeaderClosedColor { get { return EditorGUIUtility.isProSkin ? ProSkinCollapsibleHeaderClosedColor : LightSkinCollapsibleHeaderClosedColor; } }

        private static GUIStyle m_startBarGuiStyle = null;
        private static GUIStyle m_successBarGuiStyle = null;
        private static GUIStyle m_failureBarGuiStyle = null;
        private static GUIStyle m_defaultBarGuiStyle = null;
        private static GUIStyle m_nodeTextGuiStyle = null;
        private static GUIStyle m_nodeWindowOnGuiStyle = null;

        private static Color StartBarColor = Color.gray;
        private static Color SuccessBarColor = new Color(0.2f, 0.5f, 0.2f, 1);
        private static Color FailureBarColor = new Color(0.5f, 0, 0, 1);
        private static Color DefaultBarColor = new Color(0, 0.5f, 0.7f, 1);

        public static Color ConnectorColor = Color.white;
        public static Color NewConnectorColor = Color.yellow;
        public static Color ChildRelationConnectorColor = Color.yellow;
        public static Color ParentRelationConnectorColor = new Color(0.65f, 0.16f, 0.15f, 1); // brown
        //--- public static Color InactiveNodeColor = (Use skin's default color)
        public static Color ActiveNodeColor = new Color(0.2f, 0.5f, 1, 1);
        public static Color TrueNodeColor = Color.green;

        public static float nodeWidth { get { return QuestNode.DefaultNodeWidth; } }
        public static float nodeHeight { get { return QuestNode.DefaultNodeHeight; } }
        public static float shortNodeHeight { get { return QuestNode.ShortNodeHeight; } }
        public static float nodeBarHeight { get { return 16; } }

        public static GUIStyle nodeTextGUIStyle { get { return GetNodeGUIStyle(false, Color.white, ref m_nodeTextGuiStyle); } }

        public static GUIStyle GetNodeBarGUIStyle(QuestNodeType questNodeType)
        {
            switch (questNodeType)
            {
                case QuestNodeType.Start:
                    return GetNodeGUIStyle(true, StartBarColor, ref m_startBarGuiStyle);
                case QuestNodeType.Success:
                    return GetNodeGUIStyle(true, SuccessBarColor, ref m_successBarGuiStyle);
                case QuestNodeType.Failure:
                    return GetNodeGUIStyle(true, FailureBarColor, ref m_failureBarGuiStyle);
                default:
                    return GetNodeGUIStyle(true, DefaultBarColor, ref m_defaultBarGuiStyle);
            }
        }

        private static GUIStyle GetNodeGUIStyle(bool isBar, Color barColor, ref GUIStyle guiStyle)
        {
            if (guiStyle == null)
            {
                guiStyle = new GUIStyle();
                guiStyle.name = barColor.ToString();
                guiStyle.normal.textColor = isBar ? Color.black : Color.white;
                if (isBar) guiStyle.normal.background = MakeTexture((int)nodeWidth, (int)nodeHeight, barColor);
                guiStyle.fontStyle = FontStyle.Bold;
                guiStyle.alignment = TextAnchor.MiddleCenter;
                guiStyle.clipping = TextClipping.Clip;
            }
            else if (isBar && guiStyle.normal.background == null)
            {
                // Some operations cause Unity to unload dynamically-created textures.
                // If this has happened, recreate the texture:
                guiStyle.normal.background = MakeTexture((int)nodeWidth, (int)nodeHeight, barColor);
            }
            return guiStyle;
        }

        public static GUIStyle questNodeWindowGUIStyle
        {
            get
            {
                if (m_nodeWindowOnGuiStyle == null)
                {
                    m_nodeWindowOnGuiStyle = new GUIStyle(GUI.skin.window);
#if UNITY_2019_3_OR_NEWER
                    var normalTextureName = "Quest Machine/" + (EditorGUIUtility.isProSkin ? "windowDark.png" : "windowLight.png");
                    var normalTexture = EditorGUIUtility.Load(normalTextureName) as Texture2D;
                    if (normalTexture != null)
                    {
                        m_nodeWindowOnGuiStyle.normal.background = normalTexture;
                    }
#endif
                    var highlightedTextureName = "Quest Machine/" + (EditorGUIUtility.isProSkin ? "windowOnDark.png" : "windowOnLight.png");
                    var highlightedTexture = EditorGUIUtility.Load(highlightedTextureName) as Texture2D;
                    if (highlightedTexture != null)
                    {
                        m_nodeWindowOnGuiStyle.onNormal.background = highlightedTexture;
                    }
                }
                return m_nodeWindowOnGuiStyle;
            }
        }

        private static Texture2D MakeTexture(int width, int height, Color color)
        {
            Color[] pixels = new Color[width * height];
            for (int i = 0; i < pixels.Length; i++)
                pixels[i] = color;
            Texture2D texture = new Texture2D(width, height);
            texture.SetPixels(pixels);
            texture.Apply();
            return texture;
        }

        public static GUIStyle questNameGUIStyle
        {
            get
            {
                if (m_questNameGUIStyle == null)
                {
                    m_questNameGUIStyle = new GUIStyle();
                    m_questNameGUIStyle.fontStyle = FontStyle.Bold;
                    m_questNameGUIStyle.fontSize = 20;
                    m_questNameGUIStyle.normal.textColor = EditorGUIUtility.isProSkin ? Color.gray : new Color(0.2f, 0.2f, 0.2f, 1);
                }
                return m_questNameGUIStyle;
            }
        }
    }
}
