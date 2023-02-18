//using dnSR_Coding;
//using dnSR_Coding.Utilities;
//using System.Collections.Generic;
//using System.Runtime.Remoting.Metadata.W3cXsd2001;
//using UnityEditor;
//using UnityEngine;

//public class DataEditorWindow : EditorWindow
//{
//    private enum DisplaySelector { Item = 0, Unit = 1, Competence = 2 }
//    private DisplaySelector _displaySelector = DisplaySelector.Item;

//    private const float LEFT_PANEL_HEADER_WIDTH = 125;
//    private const float MIDDLE_PANEL_HEADER_WIDTH = 175, MIDDLE_PANEL_HEADER_HEIGHT = 25;
//    private const float RIGHT_PANEL_HEADER_HEIGHT = 25;

//    private const string ITEM_SCRIPTABLE_OBJECTS_CREATION_PATH = "Assets/Utilities/Scriptable Objects/Item/";
//    private string _newFileNameFieldRegister = null;

//    private Vector2 _middleSectionScrollPos;
//    private int _middleSectionSelectedButtonIndex = 0;

//    private string _rightPanelHeaderTitle;

//    private SerializedObject _editedSerializedObject = null;

//    private Object _activeEditedObject = null;
//    private Texture2D _resetIcon = null;

//    string [] _registeredGuids = null;
//    int _guidsCountOnOppeningWindow = 0;

//    #region Items variables

//    private Item _editedItem = null;

//    private SerializedProperty
//        /* Infos : */               _editedItemName, _editedItemID, _editedItemDescription,
//        /* Visuals : */             _editedItemIcon, _editedItemPrefab,
//        /* Setting : */             _editedItemCanBeEquipped, _editedItemRarity, _editedItemLinkedBodyPart,
//        /* Setting - Stack : */     _editedItemIsStackable, _editedItemMaxStackSize,
//        /* Settings - Stats : */    _editedItemHasStats, _editedItemStrengthStat, _editedItemEnduranceStat, _editedItemDexterityStat;

//    private string _editedItemNameField, _editedItemDescriptionField;
//    private int? _editedItemIDField;

//    private Sprite _editedItemIconField;
//    private GameObject _editedItemPrefabField;

//    private bool? _editedItemCanBeEquippedField;
//    private Rarity? _editedItemRarityField;
//    private LinkedBodyPart? _editedItemLinkedBodyPartField;

//    private bool? _editedItemIsStackableField;
//    private int? _editedItemMaxStackSizeField;

//    private bool? _editedItemHasStatsField;
//    private int? _editedItemStrengthStatField, _editedItemEnduranceStatField, _editedItemDexterityStatField;

//    private string _descriptionSizeInfo;

//    #endregion

//    // Only here for test purpose
//    List<string> statNames = new() { "HP", "DEF ", "ATK", };
//    private string statField;

//[MenuItem( "Window/Custom Windows/Datas" )]
//    public static void ShowWindow()
//    {
//        EditorWindow window = GetWindow( typeof( DataEditorWindow ) );

//        window.titleContent = new GUIContent( "Data Manager" );

//        // Fixed prefabPreviewSize
//        window.minSize = new Vector2( 1350, 600 );
//        window.maxSize = new Vector2( 1350, 600 );
//    }

//    private void OnEnable() => OnOppeningWindow();

//    private void OnOppeningWindow()
//    {
//        _resetIcon = ( Texture2D ) AssetDatabase.LoadAssetAtPath(
//            "Assets/Utilities/Editor/Icons/Spr_Editor_Reset_White.png",
//            typeof( Texture2D ) );
//        Debug.Log( _resetIcon.name );

//        _displaySelector = DisplaySelector.Item;

//        // Find all the component of _type obj and store them as button...
//        FindAssetGuidsOfType( _displaySelector );
//        _guidsCountOnOppeningWindow = _registeredGuids.Length;

//        SetActiveEditedObject( GetFirstGuid() );
//        OnDisplayingContentOfType( _displaySelector );

//        statField = statNames [ 0 ];
//    }

//    void OnGUI()
//    {
//        HandleNullSelection();
        
//        // This is used to create a popup using statNames entries...
//        // 
//        int index = statNames.IndexOf( statField );
//        index = EditorGUILayout.Popup( index, statNames.ToArray() );
//        statField = statNames[ index ];

//        using ( new EditorGUILayout.HorizontalScope() )
//        {
//            DrawLeftPanel( LEFT_PANEL_HEADER_WIDTH );

//            DrawMiddlePanelHeader( LEFT_PANEL_HEADER_WIDTH, _displaySelector );
//            DrawMiddlePanel( LEFT_PANEL_HEADER_WIDTH, MIDDLE_PANEL_HEADER_HEIGHT, _displaySelector );

//            DrawRightPanelHeader( LEFT_PANEL_HEADER_WIDTH + MIDDLE_PANEL_HEADER_WIDTH );
//            DrawRightPanel( LEFT_PANEL_HEADER_WIDTH + MIDDLE_PANEL_HEADER_WIDTH );
//        }
//    }    

//    /// <summary>
//    /// Draws the left panel, it shows button to navigate from one display to another.
//    /// </summary>
//    private void DrawLeftPanel( float width )
//    {
//        Rect panelSize = new( x: 0, y: 0, width, height: position.height );

//        GUILayout.BeginArea( panelSize, GUI.skin.box );
//        GUILayout.BeginVertical( GUI.skin.window );

//        GUILayout.Space( -15f );

//        DrawButtonsInLeftPanel();

//        GUILayout.EndVertical();
//        GUILayout.EndArea();
//    }

//    #region MIDDLE PANEL

//   /// <summary>
//   /// Draws the header's title for the middle panel, it is based on displaySelector value.
//   /// </summary>
//    private void DrawMiddlePanelHeader( float xOffset, DisplaySelector displaySelector )
//    {
//        float width = MIDDLE_PANEL_HEADER_WIDTH;

//        Rect panelSize = new( xOffset, 0, width, height: MIDDLE_PANEL_HEADER_HEIGHT );

//        GUIStyle style = new( EditorStyles.boldLabel )
//        {
//            alignment = TextAnchor.MiddleLeft,
//        };

//        GUILayout.BeginArea( panelSize, GUI.skin.box );
//        GUILayout.BeginHorizontal( GUI.skin.textArea );

//        string middlePanelHeader;

//        switch ( displaySelector )
//        {
//            case DisplaySelector.Item:
//                middlePanelHeader = "Item";
//                GUILayout.Label( middlePanelHeader, style );
//                break;
//            case DisplaySelector.Unit:
//                middlePanelHeader = "Unit";
//                GUILayout.Label( middlePanelHeader, style );
//                break;
//            case DisplaySelector.Competence:
//                middlePanelHeader = "Competence";
//                GUILayout.Label( middlePanelHeader, style );
//                break;
//        }

//        GUILayout.EndHorizontal();

//        GUILayout.EndArea();
//    }

//    /// <summary>
//    /// Draws the middle panel, it shows all the files correspoding to _displaySelector.
//    /// User can navigate from one file to another to edit them properly.
//    /// </summary>
//    private void DrawMiddlePanel( float xOffset, float yOffset, DisplaySelector displaySelector )
//    {
//        float width = MIDDLE_PANEL_HEADER_WIDTH;

//        Rect panelSize = new( xOffset, yOffset, width, position.height - yOffset );

//        GUILayout.BeginArea( panelSize, GUI.skin.box );

//        _middleSectionScrollPos = EditorGUILayout.BeginScrollView( _middleSectionScrollPos, GUI.skin.window );

//        GUILayout.Space( -15 );

//        switch ( displaySelector )
//        {
//            case DisplaySelector.Item: DrawMiddlePanelItemEntries();
//                break;

//            case DisplaySelector.Unit:
//                break;

//            case DisplaySelector.Competence:
//                break;
//        }       

//        EditorGUILayout.EndScrollView();

//        using ( new EditorGUILayout.HorizontalScope() )
//        {
//            Helper.DrawButton(
//                new GUIContent() { text = "+ Create" },
//                new GUIStyle( GUI.skin.button ) { fontStyle = FontStyle.Bold },
//                OnClickingButton: () => 
//                {
//                    CreateNewItemFile();
//                    _newFileNameFieldRegister = null;
//                } );

//            // New File label next to the create button
//            if ( _newFileNameFieldRegister.IsNull() ) { _newFileNameFieldRegister = "New file Name"; }
//            _newFileNameFieldRegister = EditorGUILayout.TextField( _newFileNameFieldRegister );
//        }

//        GUILayout.EndArea();
//    }    

//    /// <summary>
//    /// Draw all the button, as entry, corresponding to every files matching _displaySelector _type.
//    /// </summary>
//    private void DrawMiddlePanelItemEntries()
//    {
//        // Then set reference for the obj currently edited
//        if ( _editedItem.IsNull() ) { SetSelectedItem( ( Item ) _activeEditedObject ); }

//        for ( int i = 0; i < _registeredGuids.Length; i++ )
//        {
//            // Get file path for every index...
//            string contextualPath = AssetDatabase.GUIDToAssetPath( _registeredGuids [ i ] );

//            // Convert guids to obj _type...
//            var itemSO = ( ScriptableObject ) AssetDatabase.LoadAssetAtPath( contextualPath, typeof( ScriptableObject ) );
//            Item item = ( Item ) itemSO;

//            if ( item.IsNull() ) { continue; }

//            using ( new EditorGUI.DisabledGroupScope( _middleSectionSelectedButtonIndex == i ) )
//            {
//                string buttonText = item.Datas.Name;

//                Helper.DrawButton( 
//                    new GUIContent() { text = buttonText, },
//                    new GUIStyle( GUI.skin.button ) { alignment = TextAnchor.MiddleLeft, },
//                    new GUILayoutOption [] { GUILayout.Height( 35 ) },
//                    OnClickingButton: () =>
//                    {
//                        _middleSectionSelectedButtonIndex = i;
//                        FocusThisItemAndDisplayItsDatas( item );                        
//                    } );
//            }            
//        }
//    }

//    #endregion

//    #region RIGHT PANEL

//    #region Header
//    /// <summary>
//    /// Draw the right panel header title corresponding to _rightPanelHeaderTitle
//    /// </summary>
//    private void DrawRightPanelHeader( float xOffset )
//    {
//        float width = position.width - ( xOffset );

//        Rect panelSize = new( x: xOffset, y: 0, width, height: RIGHT_PANEL_HEADER_HEIGHT );

//        GUILayout.BeginArea( panelSize, GUI.skin.box );
//        GUILayout.BeginHorizontal( GUI.skin.textArea );

//        Helper.DrawLabel( _rightPanelHeaderTitle, false, new GUIStyle( GUI.skin.label )
//        {
//            alignment = TextAnchor.MiddleLeft,
//            fontStyle = FontStyle.Bold,
//            fontSize = 13,
//        } );

//        DrawSelectFileButton( _activeEditedObject );

//        GUILayout.EndHorizontal();
//        GUILayout.EndArea();
//    }
//    private void SetRightPanelHeaderLabel( string newTitleValue )
//    {
//        _rightPanelHeaderTitle = newTitleValue;
//    }
//    #endregion

//    /// <summary>
//    /// Draws the right panel corresponding to _displaySelector _type.
//    /// </summary>
//    private void DrawRightPanel( float xOffset )
//    {
//        switch ( _displaySelector )
//        {
//            case DisplaySelector.Item: DrawItemContent( xOffset );
//                break;
//            case DisplaySelector.Unit:
//                break;
//            case DisplaySelector.Competence:
//                break;
//        }
//    }

//    #endregion

//    #region ITEMS CONTENT

//    #region Item initialization

//    private void InitializeItemProperties( bool withFieldReset = false )
//    {
//        Debug.Log( "InitializeItemProperties" );

//        if ( withFieldReset ) { ResetEditedItemFields(); }

//        #region Infos

//        // Name
//        _editedItemName = _editedSerializedObject.FindProperty( "_name" );
//        _editedItemNameField ??= _editedItemName.stringValue;

//        // ID
//        _editedItemID = _editedSerializedObject.FindProperty( "_id" );
//        if ( _editedItemID.intValue != _editedItem.GetInstanceID() )
//        {
//            _editedItemID.intValue = _editedItem.GetInstanceID();
//            _editedSerializedObject.ApplyModifiedProperties();
//        }

//        // Description
//        _editedItemDescription = _editedSerializedObject.FindProperty( "_description" );
//        _editedItemDescriptionField ??= _editedItemDescription.stringValue;

//        #endregion

//        #region Visuals

//        // Icon
//        _editedItemIcon = _editedSerializedObject.FindProperty( "_icon" );
//        _editedItemIconField ??= ( Sprite ) _editedItemIcon.objectReferenceValue;

//        // Prefab
//        _editedItemPrefab = _editedSerializedObject.FindProperty( "_prefab" );
//        _editedItemPrefabField ??= ( GameObject ) _editedItemPrefab.objectReferenceValue;

//        #endregion

//        #region Settings

//        // Rarity
//        _editedItemRarity = _editedSerializedObject.FindProperty( "_rarity" );
//        _editedItemRarityField ??= ( Rarity? ) _editedItemRarity.enumValueFlag;

//        // Can be equipped
//        _editedItemCanBeEquipped = _editedSerializedObject.FindProperty( "_canBeEquipped" );
//        _editedItemCanBeEquippedField ??= _editedItemCanBeEquipped.boolValue;

//        // Linked body part
//        _editedItemLinkedBodyPart = _editedSerializedObject.FindProperty( "_linkedBodyPart" );
//        _editedItemLinkedBodyPartField ??= ( LinkedBodyPart? ) _editedItemLinkedBodyPart.enumValueIndex;

//        // Is Stackable
//        _editedItemIsStackable = _editedSerializedObject.FindProperty( "_isStackable" );
//        _editedItemIsStackableField ??= _editedItemIsStackable.boolValue;

//        // Max Stack Size
//        _editedItemMaxStackSize = _editedSerializedObject.FindProperty( "_maxStackSize" );
//        _editedItemMaxStackSizeField ??= _editedItemMaxStackSize.intValue;

//        // Has Stats
//        _editedItemHasStats = _editedSerializedObject.FindProperty( "_hasStats" );
//        _editedItemHasStatsField ??= _editedItemHasStats.boolValue;

//        //Stats List
//        InitializeStatProperty( StatType.Strength );
//        InitializeStatProperty( StatType.Endurance );
//        InitializeStatProperty( StatType.Dexterity );

//        #endregion
//    }

//    private void InitializeStatProperty( StatType statType )
//    {
//        switch ( statType )
//        {
//            case StatType.Strength:
//                _editedItemStrengthStat = 
//                    _editedSerializedObject.FindProperty( "_stats" ).GetArrayElementAtIndex( 0 ).FindPropertyRelative( "_points" );

//                if ( _editedItemStrengthStatField.IsNull() )
//                {
//                    _editedItemStrengthStatField = _editedItemStrengthStat.intValue;
//                }
//                break;

//            case StatType.Endurance:
//                _editedItemEnduranceStat = 
//                    _editedSerializedObject.FindProperty( "_stats" ).GetArrayElementAtIndex( 1 ).FindPropertyRelative( "_points" );

//                if ( _editedItemEnduranceStatField.IsNull() )
//                {
//                    _editedItemEnduranceStatField = _editedItemEnduranceStat.intValue;
//                }
//                break;

//            case StatType.Dexterity:
//                _editedItemDexterityStat =
//                    _editedSerializedObject.FindProperty( "_stats" ).GetArrayElementAtIndex( 2 ).FindPropertyRelative( "_points" );

//                if ( _editedItemDexterityStatField.IsNull() )
//                {
//                    _editedItemDexterityStatField = _editedItemDexterityStat.intValue;
//                }
//                break;
//        }
//    }

//    #endregion

//    #region Item drawer methods

//    private void DrawItemContent( float xOffset )
//    {
//        float width = position.width - ( xOffset );
//        float yOffset = RIGHT_PANEL_HEADER_HEIGHT;

//        Rect panelSize = new( x: xOffset, y: yOffset, width, height: position.height - RIGHT_PANEL_HEADER_HEIGHT );

//        GUILayout.BeginArea( panelSize, GUI.skin.box );
//        GUILayout.BeginVertical( GUI.skin.window );

//        DrawItemSubPanels( width );

//        GUILayout.EndVertical();
//        GUILayout.EndArea();        
//    }

//    private void DrawItemSubPanels( float parentWidth )
//    {
//        for ( int i = 0; i < 3; i++ )
//        {
//            string subPanelText = string.Empty;

//            float subPanelXOffset = 0;
//            float subPanelYOffset = 0;

//            float subPanelWidth = 0;
//            float subPanelHeight = 0;

//            float totalSubPanelWidth = parentWidth - 10;
//            float totalSubPanelHeight = position.height - RIGHT_PANEL_HEADER_HEIGHT - ( RIGHT_PANEL_HEADER_HEIGHT + 1 );

//            switch ( i )
//            {
//                case 0: // Upper Left
//                    subPanelXOffset = 2;
//                    subPanelYOffset = 2;

//                    subPanelWidth = totalSubPanelWidth;

//                    subPanelHeight = totalSubPanelHeight;
//                    subPanelHeight += RIGHT_PANEL_HEADER_HEIGHT * 2.5f;
//                    break;

//                case 1: // Upper Right
//                    subPanelXOffset = totalSubPanelWidth / 2;
//                    subPanelXOffset += 3;

//                    subPanelYOffset = 2;

//                    subPanelWidth = totalSubPanelWidth;

//                    subPanelHeight = totalSubPanelHeight;
//                    subPanelHeight += RIGHT_PANEL_HEADER_HEIGHT * 2.5f;
//                    break;

//                case 2: //Lower
//                    subPanelXOffset = 2;

//                    subPanelYOffset = totalSubPanelHeight / 2 ;
//                    subPanelYOffset += RIGHT_PANEL_HEADER_HEIGHT + 9;

//                    subPanelWidth = totalSubPanelWidth * 2;
//                    subPanelWidth += 2;

//                    subPanelHeight = totalSubPanelHeight;
//                    subPanelHeight -= RIGHT_PANEL_HEADER_HEIGHT + 9;
//                    break;
//            }

//            DrawItemSubPanelsContent( i, subPanelText, subPanelXOffset, subPanelYOffset, subPanelWidth, subPanelHeight );
//        }
//    }

//    private void DrawItemSubPanelsContent( int index, string panelName, float xPos, float yPos, float totalWidth, float totalHeight )
//    {
//        float recalculatedWidth = totalWidth / 2;
//        float recalculatedHeight = totalHeight / 2;

//        var skin = GUI.skin.textField;
//        Rect panelSize = new( 
//            xPos + skin.border.left, 
//            yPos + skin.border.top, 
//            recalculatedWidth, 
//            recalculatedHeight );

//        GUILayout.BeginArea( panelSize, skin );

//        GUILayout.BeginVertical( GUI.skin.box );

//        GUILayout.Space( 2f );

//        GUILayout.Label( panelName, EditorStyles.boldLabel );

//        switch ( index )
//        {
//            case 0:
//                DrawItemInfosContent( panelSize.width, panelSize.height );
//                break;

//            case 1:
//                DrawItemSettingsContent( panelSize.width, panelSize.height );
//                break;

//            case 2:
//                DrawItemVisualsContent( panelSize.width, panelSize.height );                
//                break;
//        }        

//        GUILayout.EndVertical();

//        GUILayout.EndArea();
//    }

//    private void DrawItemInfosContent( float subPanelWidth, float subPanelHeight )
//    {
//        EditorGUI.BeginChangeCheck();

//        Helper.DrawLabel( "Infos", true, new GUIStyle( GUI.skin.label )
//        {
//            fontSize = 14,
//            fontStyle = FontStyle.Bold,
//        },
//        yOffset: -15 );

//        GUILayout.Space( 5 );

//        EditorGUILayout.BeginVertical();
//        EditorGUILayout.BeginHorizontal();

//        Helper.DrawLabel( "Name", false );

//        _editedItemNameField = EditorGUILayout.TextField( _editedItemNameField );

//        Helper.DrawButton(
//            new GUIContent()
//            {
//                image = _resetIcon,
//                tooltip = "ResetExp name",
//            },
//            new GUIStyle( GUI.skin.button )
//            {
//                padding = new RectOffset( 2, 2, 2, 2 ),
//            },
//            new GUILayoutOption []
//            {
//                GUILayout.Width( 18 ),
//                GUILayout.Height( 18 ),
//            },
//            OnClickingButton: () => _editedItemNameField = "[TYPE HERE]" );

//        Helper.DrawLabel( "ID", true );        

//        using ( new EditorGUI.DisabledGroupScope( true ) )
//        {
//            _editedItemIDField = EditorGUILayout.IntField( _editedItem.GetInstanceID() );
//        } 

//        EditorGUILayout.EndHorizontal();

//        GUILayout.Space( 5 );

//        _descriptionSizeInfo = _editedItemDescriptionField.IsNull() ? "" : _editedItemDescriptionField.Length.ToString();
//        string size = "Size : " + _descriptionSizeInfo;
//        GUILayout.Label( "Description - " + size );

//        _editedItemDescriptionField = EditorGUILayout.TextArea( _editedItemDescriptionField, new GUILayoutOption [] 
//        {
//            GUILayout.Width( subPanelWidth - 15 ),
//            GUILayout.Height( subPanelHeight / 1.525f ) 
//        } );

//        GUILayout.Space( 2 );

//        Helper.DrawButton( 
//            new GUIContent() { text = "ResetExp description", },
//            new GUILayoutOption[] { GUILayout.Width( 125 ), },
//            OnClickingButton: () => _editedItemDescriptionField = "[TYPE HERE]" );

//        EditorGUILayout.EndVertical();

//        if ( EditorGUI.EndChangeCheck() )
//        {
//            _descriptionSizeInfo = _editedItemDescriptionField.Length.ToString();

//            _editedItemName.stringValue = _editedItemNameField;
//            _editedItemID.intValue = _editedItemIDField.Value;
//            _editedItemDescription.stringValue = _editedItemDescriptionField;

//            PushDatasToScriptableObject( DisplaySelector.Item );

//            SetRightPanelHeaderLabel( _editedItemName.stringValue );
//        }
//    }

//    #region Item settings - General infos / stack / stats / visuals

//    private void DrawItemSettingsContent( float subPanelWidth, float subPanelHeight )
//    {
//        EditorGUI.BeginChangeCheck();

//        Helper.DrawLabel( "Settings", true, new GUIStyle( GUI.skin.label )
//        {
//            fontSize = 14,
//            fontStyle = FontStyle.Bold,
//        },
//        yOffset: -15 );

//        GUILayout.Space( 5 );

//        #region General Infos -> Is it equippable - Rarity - Linked body part

//        EditorGUILayout.BeginVertical();

//        EditorGUILayout.BeginVertical();

//        GUILayout.Label( "1 - Generalities".ToUpper(), new GUIStyle( GUI.skin.label )
//        {
//            contentOffset = new Vector2( -2, 0 ),
//            alignment = TextAnchor.MiddleLeft,
//            fontSize = 12,
//            fontStyle = FontStyle.Bold,
//        } );

//        GUILayout.Space( 5 );

//        // Can be equipped bool
//        _editedItemCanBeEquippedField = EditorGUILayout.ToggleLeft( " Can Be Equipped ?", _editedItemCanBeEquippedField.Value );

//        GUILayout.Space( 5 );

//        float labelXOffset = 34;

//        using ( new EditorGUI.DisabledGroupScope( !_editedItemCanBeEquippedField.Value ) )
//        {
//            EditorGUILayout.BeginHorizontal();

//            GUIStyle rarityLabelStyle = new( GUI.skin.label )
//            {
//                contentOffset = new Vector2( labelXOffset, 2 ),
//            };
//            GUILayout.Label( "Rarity", rarityLabelStyle );

//            // Rarity enum
//            _editedItemRarityField = !_editedItemCanBeEquippedField.Value ?
//                ( Rarity? ) EditorGUILayout.EnumPopup( Rarity.Unassigned, GUILayout.Width( subPanelWidth / 1.45f ) )
//                : ( Rarity? ) EditorGUILayout.EnumPopup( _editedItemRarityField.Value, GUILayout.Width( subPanelWidth / 1.45f ) );

//            EditorGUILayout.EndHorizontal();
//        }

//        EditorGUILayout.EndVertical();

//        EditorGUILayout.BeginHorizontal();

//        LinkedBodyPart selectedBodyPart = _editedItemLinkedBodyPartField.Value;

//        string bodyPartsIconFolderPath = "Assets/Utilities/Editor/Data Editor/Icons/LinkedBodyPart/White/";
//        string iconFileName = "";

//        switch ( selectedBodyPart )
//        {
//            case LinkedBodyPart.Head:
//                iconFileName = bodyPartsIconFolderPath + "Spr_Head_White.png";
//                break;
//            case LinkedBodyPart.Chest:
//                iconFileName = bodyPartsIconFolderPath + "Spr_Chest_White.png";
//                break;
//            case LinkedBodyPart.Legs:
//                iconFileName = bodyPartsIconFolderPath + "Spr_Legs_White.png";
//                break;
//            case LinkedBodyPart.Hands:
//                iconFileName = bodyPartsIconFolderPath + "Spr_Hands_White.png";
//                break;
//            case LinkedBodyPart.Weapon_Left:
//                iconFileName = bodyPartsIconFolderPath + "Spr_Weapon_Left_White.png";
//                break;
//            case LinkedBodyPart.Weapon_Right:
//                iconFileName = bodyPartsIconFolderPath + "Spr_Weapon_Right_White.png";
//                break;
//        }

//        using ( new EditorGUI.DisabledGroupScope( !_editedItemCanBeEquippedField.Value ) )
//        {
//            Texture2D bodyPartIcon = selectedBodyPart == LinkedBodyPart.Unassigned
//                ? EditorGUIUtility.FindTexture( "d_Unlinked@2x" )
//                : ( Texture2D ) AssetDatabase.LoadAssetAtPath( iconFileName, typeof( Texture2D ) );

//            // Draw Icon of body part
//            if ( !bodyPartIcon.IsNull() && Event.current.type.Equals( EventType.Repaint ) )
//            {
//                Graphics.DrawTexture( new Rect( 5, 91, 32, 32 ), bodyPartIcon );
//            }

//            GUIStyle linkedBodyPartLabelStyle = new( GUI.skin.label )
//            {
//                contentOffset = new Vector2( labelXOffset, 0 ),
//            };
//            GUILayout.Label( "Linked Body Part", linkedBodyPartLabelStyle );

//            // Linked body part
//            _editedItemLinkedBodyPartField = !_editedItemCanBeEquippedField.Value ?
//                ( LinkedBodyPart? ) EditorGUILayout.EnumPopup( LinkedBodyPart.Unassigned, GUILayout.Width( subPanelWidth / 1.45f ) )
//                : ( LinkedBodyPart? ) EditorGUILayout.EnumPopup( _editedItemLinkedBodyPartField.Value, GUILayout.Width( subPanelWidth / 1.45f ) );
//        }

//        EditorGUILayout.EndHorizontal();

//        #endregion

//        GUILayout.Space( 15 );

//        #region Stack infos

//        GUILayout.Label( "2 - Stack infos".ToUpper(), new GUIStyle( GUI.skin.label )
//        {
//            contentOffset = new Vector2( -2, 0 ),
//            alignment = TextAnchor.MiddleLeft,
//            fontSize = 12,
//            fontStyle = FontStyle.Bold,
//        } );

//        GUILayout.Space( 5 );

//        using ( new EditorGUILayout.HorizontalScope() )
//        {
//            // isStackable
//            _editedItemIsStackableField = EditorGUILayout.ToggleLeft( " Is the item stackable ?", _editedItemIsStackableField.Value );

//            using ( new EditorGUI.DisabledGroupScope( !_editedItemIsStackableField.Value ) )
//            {
//                GUILayout.Label( "Max stack size", new GUIStyle( GUI.skin.label )
//                {
//                    contentOffset = new Vector2( -2, 2 ),
//                } );

//                // Max stack size int range
//                _editedItemMaxStackSizeField = 
//                    EditorGUILayout.IntField( 
//                        !_editedItemIsStackableField.Value ? 1 : _editedItemMaxStackSizeField.Value, 
//                        new GUIStyle( GUI.skin.textField ) { contentOffset = new Vector2( 0, 0 ) } );
//            }
//        }

//        #endregion

//        GUILayout.Space( 15 );

//        #region Stats infos

//        GUILayout.Label( "3 - Stats infos".ToUpper(), new GUIStyle( GUI.skin.label )
//        {
//            contentOffset = new Vector2( -2, 0 ),
//            alignment = TextAnchor.MiddleLeft,
//            fontSize = 12,
//            fontStyle = FontStyle.Bold,
//        } );

//        GUILayout.Space( 5 );

//        GUILayout.BeginHorizontal();

//        // Stats - END FOR DEX
//        // Has Stats ?
//        _editedItemHasStatsField = EditorGUILayout.ToggleLeft( " Does it have stats ?", _editedItemHasStatsField.Value );

//        Helper.DrawButton( 
//            new GUIContent() { text = "ResetExp All Stats" },
//            new GUILayoutOption[] { GUILayout.Width( 105 ), },
//            OnClickingButton: () =>
//            {
//                if ( EditorUtility.DisplayDialog(
//                   "ResetExp all stats point ?",
//                   "Are you sure you want to reset all stats point ?",
//                   "Yes, ResetExp all",
//                   "Cancel" ) )
//                {
//                    ResetItemStatPoint( StatType.Strength );
//                    ResetItemStatPoint( StatType.Endurance );
//                    ResetItemStatPoint( StatType.Dexterity );
//                }
//            } );


//        GUILayout.EndHorizontal();

//        using ( new EditorGUI.DisabledGroupScope( !_editedItemHasStatsField.Value ) )
//        {
//            GUILayout.BeginVertical();

//            int statAmount = System.Enum.GetValues( typeof( StatType ) ).Length - 1;
//            int statTypeIndex = 1;

//            for ( int i = 0; i < statAmount; i++ )
//            {
//                DrawItemStatEntries( ( StatType ) Helper.GetEnumToArray( typeof( StatType ) ).GetValue( statTypeIndex ), subPanelWidth );
//                statTypeIndex++;
//            }            

//            GUILayout.EndVertical();
//        }

//        #endregion

//        EditorGUILayout.EndVertical();

//        if ( EditorGUI.EndChangeCheck() )
//        {
//            _editedItemCanBeEquipped.boolValue = _editedItemCanBeEquippedField.Value;
//            _editedItemRarity.enumValueFlag = ( int ) _editedItemRarityField.Value;
//            _editedItemLinkedBodyPart.enumValueFlag = ( int ) _editedItemLinkedBodyPartField.Value;

//            _editedItemIsStackable.boolValue = _editedItemIsStackableField.Value;
//            _editedItemMaxStackSize.intValue = _editedItemMaxStackSizeField.Value;

//            _editedItemHasStats.boolValue = _editedItemHasStatsField.Value;

//            _editedItemStrengthStat.intValue = _editedItemStrengthStatField.Value;
//            _editedItemEnduranceStat.intValue = _editedItemEnduranceStatField.Value;
//            _editedItemDexterityStat.intValue = _editedItemDexterityStatField.Value;

//            PushDatasToScriptableObject( DisplaySelector.Item );
//        }
//    }

//    private void DrawItemStatEntries( StatType statType, float entryWidth )
//    {
//        GUILayout.BeginHorizontal();

//        // Set label content according to statType
//        string statTypeAbreviation = "";
//        switch ( statType )
//        {
//            case StatType.Strength:
//                statTypeAbreviation = " - STR";
//                break;

//            case StatType.Endurance:
//                statTypeAbreviation = " - END";
//                break;

//            case StatType.Dexterity:
//                statTypeAbreviation = " - DEX";
//                break;
//        }

//        string labelContent = statType.ToString().ToUpper() + statTypeAbreviation;
//        Helper.DrawLabel( labelContent, true, new GUIStyle( GUI.skin.label )
//        {
//            fontSize = 12,
//        } );

//        float entryFieldWidth = entryWidth / 6.225f;

//        // Set objectField point according to statType
//        switch ( statType )
//        {
//            case StatType.Strength:
//                GUILayout.FlexibleSpace();
//                _editedItemStrengthStatField = 
//                    EditorGUILayout.IntField( _editedItemStrengthStatField.Value,
//            new GUILayoutOption [] { GUILayout.Width( entryFieldWidth ), } );
//                break;

//            case StatType.Endurance:
//                GUILayout.FlexibleSpace();
//                _editedItemEnduranceStatField = EditorGUILayout.IntField( _editedItemEnduranceStatField.Value,
//            new GUILayoutOption [] { GUILayout.Width( entryFieldWidth ), } );
//                break;

//            case StatType.Dexterity:
//                GUILayout.FlexibleSpace();
//                _editedItemDexterityStatField = EditorGUILayout.IntField( _editedItemDexterityStatField.Value,
//            new GUILayoutOption [] { GUILayout.Width( entryFieldWidth ), } );
//                break;
//        }

//        Helper.DrawButton(
//            new GUIContent()
//            {
//                image = _resetIcon,
//                tooltip = "ResetExp points of stat " + statType.ToString(),
//            },
//            new GUIStyle( GUI.skin.button )
//            {
//                padding = new RectOffset( 2, 2, 2, 2 ),
//            },
//            new GUILayoutOption []
//            {
//                GUILayout.Width( 18 ),
//                GUILayout.Height( 18 ),
//            },
//            OnClickingButton: () => ResetItemStatPoint( statType ) );

//        GUILayout.EndHorizontal();
//    }

//    private void ResetItemStatPoint( StatType statType )
//    {
//        switch ( statType )
//        {
//            case StatType.Strength:
//                _editedItemStrengthStat.intValue = 0;
//                _editedItemStrengthStatField = _editedItemStrengthStat.intValue;
//                break;

//            case StatType.Endurance:
//                _editedItemEnduranceStat.intValue = 0;
//                _editedItemEnduranceStatField = _editedItemEnduranceStat.intValue;
//                break;

//            case StatType.Dexterity:
//                _editedItemDexterityStat.intValue = 0;
//                _editedItemDexterityStatField = _editedItemDexterityStat.intValue;
//                break;
//        }
//    }

//    private void DrawItemVisualsContent( float subPanelWidth, float subPanelHeight )
//    {
//        EditorGUI.BeginChangeCheck();

//        Helper.DrawLabel( "Visuals", true, new GUIStyle( GUI.skin.label )
//        {
//            fontSize = 14,
//            fontStyle = FontStyle.Bold,
//        }, 
//        yOffset: -15);

//        GUILayout.Space( 5 );

//        using ( new EditorGUILayout.HorizontalScope() )
//        {
//            #region Icon field

//            EditorGUILayout.BeginVertical();

//            GUILayout.FlexibleSpace();

//            GUILayout.BeginHorizontal();

//            GUILayout.FlexibleSpace();

//            GUILayout.Label( "Icon" );

//            _editedItemIconField = ( Sprite ) EditorGUILayout.ObjectField( _editedItemIconField, typeof( Sprite ), false, new GUILayoutOption []
//            {
//                GUILayout.Width( subPanelWidth / 6 ),
//            } );

//            using ( new EditorGUI.DisabledScope( _editedItemIconField.IsNull() ) )
//            {
//                Helper.DrawButton(
//                    new GUIContent()
//                    {
//                        image = EditorGUIUtility.FindTexture( "d_winbtn_win_close@2x" ),
//                        tooltip = "Erase the selection for this icon field",
//                    },
//                    new GUIStyle( GUI.skin.button )
//                    {
//                        padding = new RectOffset( 0, 0, 0, 0 ),
//                    },
//                    new GUILayoutOption[]
//                    {
//                        GUILayout.Width( 18 ),
//                        GUILayout.Height( 18 ),
//                    },
//                    Color.red,
//                    OnClickingButton: () => _editedItemIconField = null );
//            }

//            GUILayout.EndHorizontal();

//            int iconPreviewSize = 150;

//            if ( !_editedItemIconField.IsNull() && Event.current.type.Equals( EventType.Repaint ) )
//            {

//                Helper.DrawTexture( _editedItemIconField.texture, iconPreviewSize,
//                    totalWidth: subPanelWidth / 4,
//                    totalHeight: subPanelHeight / 2,
//                    xOffset: 0,
//                    yOffset: 10 );
//            }
//            else
//            {
//                GUI.Label( new Rect(
//                        x: subPanelWidth / 4 - iconPreviewSize / 4,
//                        y: ( subPanelHeight / 2 ) - iconPreviewSize / 2,
//                        width: iconPreviewSize,
//                        height: iconPreviewSize ),
//                    "No icon set." );
//            }

//            EditorGUILayout.EndVertical();

//            #endregion

//            GUILayout.FlexibleSpace();

//            #region Prefab field

//            EditorGUILayout.BeginVertical();

//            GUILayout.FlexibleSpace();

//            GUILayout.BeginHorizontal();

//            GUILayout.FlexibleSpace();

//            GUILayout.Label( "Prefab" );

//            _editedItemPrefabField = ( GameObject ) EditorGUILayout.ObjectField( _editedItemPrefabField, typeof( GameObject ), false, new GUILayoutOption []
//            {
//                GUILayout.Width( subPanelWidth / 6 ),
//            } );

//            using ( new EditorGUI.DisabledScope( _editedItemPrefabField.IsNull() ) )
//            {
//                Helper.DrawButton( 
//                    new GUIContent()
//                    {
//                        image = EditorGUIUtility.FindTexture( "d_winbtn_win_close@2x" ),
//                        tooltip = "Erase the selection for this prefab field",
//                    },
//                    new GUIStyle( GUI.skin.button )
//                    {
//                        padding = new RectOffset( 0, 0, 0, 0 ),
//                    },
//                    new GUILayoutOption []
//                    {
//                        GUILayout.Width( 18 ),
//                        GUILayout.Height( 18 ),
//                    },
//                    Color.red,
//                    OnClickingButton: () => _editedItemPrefabField = null );

//            }

//            GUILayout.EndHorizontal();

//            int prefabPreviewSize = 150;

//            Texture prefabTexture = _editedItemPrefabField.IsNull()
//                ? Texture2D.whiteTexture
//                : AssetPreview.GetAssetPreview( _editedItemPrefabField );

//            if ( !_editedItemPrefabField.IsNull() && Event.current.type.Equals( EventType.Repaint ) )
//            {
//                Helper.DrawTexture( prefabTexture, prefabPreviewSize,
//                    totalWidth: subPanelWidth, 
//                    totalHeight: subPanelHeight / 2,
//                    xOffset: subPanelWidth / 4, 
//                    yOffset: 10 );
//            }
//            else
//            {
//                GUI.Label( new Rect(
//                        x: subPanelWidth - prefabPreviewSize / 2 - subPanelWidth / 4 + prefabPreviewSize / 4,
//                        y: ( subPanelHeight / 2 ) - prefabPreviewSize / 2,
//                        width: prefabPreviewSize,
//                        height: prefabPreviewSize ),
//                    "No prefab set." );
//            }

//            EditorGUILayout.EndVertical();

//            #endregion

//            GUILayout.FlexibleSpace();
//        }

//        if ( EditorGUI.EndChangeCheck() )
//        {
//            _editedItemIcon.objectReferenceValue = _editedItemIconField;
//            _editedItemPrefab.objectReferenceValue = _editedItemPrefabField;

//            PushDatasToScriptableObject( DisplaySelector.Item );
//        }
//    }

//    #endregion

//    #endregion    

//    #endregion

//    #region BUTTONS

//    private void DrawButtonsInLeftPanel()
//    {
//        int selectionTypeAmount = Helper.GetEnumLength( typeof( DisplaySelector ) );

//        for ( int i = 0; i < selectionTypeAmount; i++ )
//        {
//            string buttonText = ( ( DisplaySelector ) i ).ToString().ToUpper();
//            string buttonTooltip = "Display " + buttonText + " content";

//            using ( new EditorGUI.DisabledScope( _displaySelector == ( DisplaySelector ) i ) )
//            {
//                Helper.DrawButton( new GUIContent()
//                {
//                    text = buttonText,
//                    tooltip = buttonTooltip,
//                },
//                new GUILayoutOption [] { GUILayout.Height( 35 ), },
//                OnClickingButton: () => OnDisplayingContentOfType( ( DisplaySelector ) i ) );
//            }
//        }
//    }

//    private void DrawSelectFileButton( Object contextObject )
//    {
//        Helper.DrawButton( new GUIContent()
//        {
//            text = "Select File".ToUpper(),
//            tooltip = "Select the file you are currently editing.",
//        },
//        new GUIStyle( GUI.skin.button )
//        {
//            alignment = TextAnchor.MiddleLeft,
//            fontStyle = FontStyle.Bold,
//        },
//        widthOffset: 5,
//        OnClickingButton: () => Selection.activeObject = contextObject );

//        GUILayout.FlexibleSpace();
//    }

//    #endregion

//    #region HELPERS

//    #region Item related

//    private void SetSelectedItem( Item item )
//    {
//        if ( _editedItem != item )
//        {
//            _editedItem = item;
//            Debug.Log( "Item currently edited : " + _editedItem.Datas.Name );
//        }
//    }

//    private void FocusThisItemAndDisplayItsDatas( Object obj )
//    {
//        Item item = ( Item ) obj;
//        SetSerializedObjectReference( item );

//        SetActiveEditedObject( item );
//        SetSelectedItem( item );

//        InitializeItemProperties( true );
//        SetRightPanelHeaderLabel( item.Datas.Name );        
//    }

//    private void ResetEditedItemFields()
//    {
//        _editedItemNameField = null;
//        _editedItemIDField = _editedItem.GetInstanceID();
//        _editedItemDescriptionField = null;

//        _editedItemIconField = null;
//        _editedItemPrefabField = null;

//        _editedItemCanBeEquippedField = null;
//        _editedItemRarityField = null;
//        _editedItemLinkedBodyPartField = null;

//        _editedItemIsStackableField = null;
//        _editedItemMaxStackSizeField = null;

//        _editedItemHasStatsField = null;

//        _editedItemStrengthStatField = null;
//        _editedItemEnduranceStatField = null;
//        _editedItemDexterityStatField = null;
//    }

//    private void CreateNewItemFile()
//    {
//        Debug.Log( "Create new Item : " + _newFileNameFieldRegister );

//        Item newItem = new( _newFileNameFieldRegister );

//        if ( ITEM_SCRIPTABLE_OBJECTS_CREATION_PATH.IsNull() )
//        {
//            Debug.LogError( "The folder of path : " + ITEM_SCRIPTABLE_OBJECTS_CREATION_PATH + " has not been found, please create this path before trying to create an item" );
//        }

//        AssetDatabase.CreateAsset( newItem, ITEM_SCRIPTABLE_OBJECTS_CREATION_PATH + _newFileNameFieldRegister + ".asset" );

//        string createdItem = AssetDatabase.GetAssetPath( newItem );

//        if ( !createdItem.IsNull() )
//        {
//            RefreshGuidsOnAssetCreation();

//            //_middleSectionSelectedButtonIndex = _registeredGuids.Length;
//            _middleSectionSelectedButtonIndex = GetGuidIndex( createdItem );



//            FocusThisItemAndDisplayItsDatas( newItem );
//        }
//    }

//    #endregion

//    private void FindAssetGuidsOfType( DisplaySelector displaySelector )
//    {
//        string filter = "t:" + displaySelector.ToString().ToLower();
//        _registeredGuids = AssetDatabase.FindAssets( filter, null );
//    }

//    private Object GetFirstGuid()
//    {
//        // Get the first obj guid...
//        string firstItemPath = AssetDatabase.GUIDToAssetPath( _registeredGuids [ 0 ] );

//        //Convert it to Object and set active context object...
//        return AssetDatabase.LoadAssetAtPath( firstItemPath, typeof( Object ) );
//    }

//    private void HandleNullSelection()
//    {
//        if ( _activeEditedObject.IsNull() ) 
//        {
//            FindAssetGuidsOfType( _displaySelector );

//            _middleSectionSelectedButtonIndex = 0;

//            SetActiveEditedObject( GetFirstGuid() );
//            SetSerializedObjectReference( _activeEditedObject );
//        }

//        switch ( _displaySelector )
//        {
//            case DisplaySelector.Item:
//                if ( _editedItem.IsNull() )
//                {
//                    FocusThisItemAndDisplayItsDatas( _activeEditedObject );
//                }
//                break;

//            case DisplaySelector.Unit:
//                break;

//            case DisplaySelector.Competence:
//                break;
//        }
//    }

//    private void RefreshGuidsOnAssetCreation()
//    {
//        string filter = "t:" + _displaySelector.ToString().ToLower();

//        if ( AssetDatabase.FindAssets( filter, null ).Length > _guidsCountOnOppeningWindow )
//        {
//            Debug.Log( "An Item has been created or removed, item guids need to be updated." );
//            FindAssetGuidsOfType( _displaySelector );
//        }
//    }

//    private void SetActiveEditedObject( Object obj )
//    {
//        if ( _activeEditedObject != obj )
//        {
//            _activeEditedObject = obj;
//            Selection.activeObject = obj;
//            EditorGUI.FocusTextInControl( null );
            
//            //Debug.Log( "Active contect object : " + _activeEditedObject.name );
//        }
//    }

//    private void SetSerializedObjectReference( Object obj )
//    {
//        _editedSerializedObject = new SerializedObject( obj );
//    }

//    private void OnDisplayingContentOfType( DisplaySelector displaySelector )
//    {
//        _displaySelector = displaySelector;

//        switch ( _displaySelector )
//        {
//            case DisplaySelector.Item: FocusThisItemAndDisplayItsDatas( _activeEditedObject );
//                break;

//            case DisplaySelector.Unit:
//                break;

//            case DisplaySelector.Competence:
//                break;
//        }
//    }

//    private void PushDatasToScriptableObject( DisplaySelector displaySelector )
//    {
//        switch ( displaySelector )
//        {
//            case DisplaySelector.Item:
//                _editedSerializedObject.ApplyModifiedProperties();
//                _editedItem.Datas = new Item.ItemInfos( _editedItem );
//                break;

//            case DisplaySelector.Unit:
//                break;

//            case DisplaySelector.Competence:
//                break;
//        }       
//    }

//    private int GetGuidIndex( string dataPath )
//    {
//        Debug.Log( "Compared guidPath : " + dataPath );

//        for ( int i = 0; i < _registeredGuids.Length; i++ )
//        {
//            string guidDataPath = AssetDatabase.GUIDToAssetPath ( _registeredGuids [ i ] );
//            Debug.Log ( guidDataPath );

//            if ( dataPath == guidDataPath )
//            {
//                Debug.Log( dataPath + " / " + guidDataPath );
//                return i;
//            }
//        }

//        return 0;
//    }

//    void OnInspectorUpdate()
//    {
//        RefreshGuidsOnAssetCreation();
//        Repaint();
//    }

//    #endregion
//}