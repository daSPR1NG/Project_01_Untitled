using dnSR_Coding;
using dnSR_Coding.Utilities;
using UnityEditor;
using UnityEditor.Graphs;
using UnityEngine;
using UnityEngine.UI;

public class DataEditorWindow : EditorWindow
{
    private enum DisplaySelector { Items = 0, Units = 1, Competences = 2 }

    private const float LEFT_PANEL_HEADER_WIDTH = 125;
    private const float MIDDLE_PANEL_HEADER_WIDTH = 175, MIDDLE_PANEL_HEADER_HEIGHT = 25;
    private const float RIGHT_PANEL_HEADER_HEIGHT = 25;

    private DisplaySelector _displaySelector = DisplaySelector.Items;

    private Vector2 _middleSectionScrollPos;
    private int _middleSectionSelectedButtonIndex = 0;

    private const string ITEM_SCRIPTABLE_OBJECTS_CREATION_PATH = "Assets/Utilities/Scriptable Objects/Item/";
    private string _newFileNameFieldRegister = null;

    private string _rightPanelHeaderTitle;

    private Object _activeEditedObject = null;

    private Texture2D _resetIcon = null;
    #region Items variables
    string [] _itemGuids = null;
    int _itemGuidsCountOnOppeningWindow = 0;

    private Item _editedItem = null;
    private SerializedObject _editedItemSOReference = null;

    private SerializedProperty
        /* Infos : */               _editedItemName, _editedItemID, _editedItemDescription,
        /* Visuals : */             _editedItemIcon, _editedItemPrefab,
        /* Setting : */             _editedItemCanBeEquipped, _editedItemRarity, _editedItemLinkedBodyPart,
        /* Setting - Stack : */     _editedItemIsStackable, _editedItemMaxStackSize,
        /* Settings - Stats : */    _editedItemHasStats, _editedItemStrengthStat, _editedItemEnduranceStat, _editedItemDexterityStat;

    private string _editedItemNameField, _editedItemDescriptionField;
    private int? _editedItemIDField;

    private Sprite _editedItemIconField;
    private GameObject _editedItemPrefabField;

    private bool? _editedItemCanBeEquippedField;
    private Rarity? _editedItemRarityField;
    private LinkedBodyPart? _editedItemLinkedBodyPartField;

    private bool? _editedItemIsStackableField;
    private int? _editedItemMaxStackSizeField;

    private bool? _editedItemHasStatsField;
    private int? _editedItemStrengthStatField, _editedItemEnduranceStatField, _editedItemDexterityStatField;

    private string _descriptionSizeInfo;

    #endregion

    [MenuItem( "Window/Custom Windows/Datas" )]
    public static void ShowWindow()
    {
        EditorWindow window = GetWindow( typeof( DataEditorWindow ) );

        window.titleContent = new GUIContent( "Data Manager" );

        // Fixed prefabPreviewSize
        window.minSize = new Vector2( 1350, 600 );
        window.maxSize = new Vector2( 1350, 600 );
    }

    private void OnEnable() => OnOppeningWindow();

    private void OnOppeningWindow()
    {
        _resetIcon = ( Texture2D ) AssetDatabase.LoadAssetAtPath(
            "Assets/Utilities/Editor/Icons/Spr_Editor_Reset_White.png",
            typeof( Texture2D ) );
        Debug.Log( _resetIcon.name );

        _displaySelector = DisplaySelector.Items;

        // Find all the component of type obj and store them as button...
        _itemGuids = AssetDatabase.FindAssets( "t:item", null );
        _itemGuidsCountOnOppeningWindow = _itemGuids.Length;

        // Get the first obj guid...
        string firstItemPath = AssetDatabase.GUIDToAssetPath( _itemGuids [ 0 ] );

        //Convert it to Object and set active context object...
        _activeEditedObject = AssetDatabase.LoadAssetAtPath( firstItemPath, typeof( Object ) );
        OnDisplayingContentOfType( DisplaySelector.Items );
    }

    void OnGUI()
    {
        HandleNullSelection();

        using ( new EditorGUILayout.HorizontalScope() )
        {
            DrawLeftPanel( LEFT_PANEL_HEADER_WIDTH );

            DrawMiddlePanelHeader( LEFT_PANEL_HEADER_WIDTH, _displaySelector );
            DrawMiddlePanel( LEFT_PANEL_HEADER_WIDTH, MIDDLE_PANEL_HEADER_HEIGHT, _displaySelector );

            DrawRightPanelHeader( LEFT_PANEL_HEADER_WIDTH + MIDDLE_PANEL_HEADER_WIDTH );
            DrawRightPanel( LEFT_PANEL_HEADER_WIDTH + MIDDLE_PANEL_HEADER_WIDTH );
        }
    }    

    /// <summary>
    /// Draws the left panel, it shows button to navigate from one display to another.
    /// </summary>
    private void DrawLeftPanel( float width )
    {
        Rect panelSize = new( x: 0, y: 0, width, height: position.height );

        GUILayout.BeginArea( panelSize, GUI.skin.box );
        GUILayout.BeginVertical( GUI.skin.window );

        GUILayout.Space( -15f );

        DrawButtonsInLeftPanel();

        GUILayout.EndVertical();
        GUILayout.EndArea();
    }

    #region MIDDLE PANEL

   /// <summary>
   /// Draws the header's title for the middle panel, it is based on displaySelector value.
   /// </summary>
    private void DrawMiddlePanelHeader( float xOffset, DisplaySelector displaySelector )
    {
        float width = MIDDLE_PANEL_HEADER_WIDTH;

        Rect panelSize = new( xOffset, 0, width, height: MIDDLE_PANEL_HEADER_HEIGHT );

        GUIStyle style = new( EditorStyles.boldLabel )
        {
            alignment = TextAnchor.MiddleLeft,
        };

        GUILayout.BeginArea( panelSize, GUI.skin.box );
        GUILayout.BeginHorizontal( GUI.skin.textArea );

        string middlePanelHeader;

        switch ( displaySelector )
        {
            case DisplaySelector.Items:
                middlePanelHeader = "Items";
                GUILayout.Label( middlePanelHeader, style );
                break;
            case DisplaySelector.Units:
                middlePanelHeader = "Units";
                GUILayout.Label( middlePanelHeader, style );
                break;
            case DisplaySelector.Competences:
                middlePanelHeader = "Competences";
                GUILayout.Label( middlePanelHeader, style );
                break;
        }

        GUILayout.EndHorizontal();

        GUILayout.EndArea();
    }

    /// <summary>
    /// Draws the middle panel, it shows all the files correspoding to _displaySelector.
    /// User can navigate from one file to another to edit them properly.
    /// </summary>
    private void DrawMiddlePanel( float xOffset, float yOffset, DisplaySelector displaySelector )
    {
        float width = MIDDLE_PANEL_HEADER_WIDTH;

        Rect panelSize = new( xOffset, yOffset, width, position.height - yOffset );

        GUILayout.BeginArea( panelSize, GUI.skin.box );

        _middleSectionScrollPos = EditorGUILayout.BeginScrollView( _middleSectionScrollPos, GUI.skin.window );

        GUILayout.Space( -15 );

        switch ( displaySelector )
        {
            case DisplaySelector.Items:

                DrawMiddlePanelItemEntries();
                break;

            case DisplaySelector.Units:
                break;

            case DisplaySelector.Competences:
                break;
        }       

        EditorGUILayout.EndScrollView();

        using ( new EditorGUILayout.HorizontalScope() )
        {
            GUIStyle createItemButtonStyle = new( GUI.skin.button )
            {
                fontStyle = FontStyle.Bold,
            };

            GUIContent createItemButtonContent = new()
            {
                text = "+ Create",
            };

            if ( GUILayout.Button( createItemButtonContent, createItemButtonStyle ) )
            {
                Debug.Log( "Create new Item : " + _newFileNameFieldRegister );

                Item newItem = new ( _newFileNameFieldRegister );

                if ( ITEM_SCRIPTABLE_OBJECTS_CREATION_PATH.IsNull() )
                {
                    Debug.LogError( "The folder of path : " + ITEM_SCRIPTABLE_OBJECTS_CREATION_PATH + " has not been found, please create this path before trying to create an item" );
                }

                AssetDatabase.CreateAsset( newItem, ITEM_SCRIPTABLE_OBJECTS_CREATION_PATH + _newFileNameFieldRegister + ".asset" );

                string createdItem = AssetDatabase.GetAssetPath( newItem );

                if ( !createdItem.IsNull() )
                {
                    _middleSectionSelectedButtonIndex = _itemGuids.Length;

                    newItem.CreateStatEntriesInEditor();
                    FocusThisItemAndDisplayItsDatas( newItem );
                }
            }

            // New File label next to the create button
            if ( _newFileNameFieldRegister.IsNull() ) { _newFileNameFieldRegister = "New file Name"; }
            _newFileNameFieldRegister = EditorGUILayout.TextField( _newFileNameFieldRegister );
        }

        GUILayout.EndArea();
    }

    /// <summary>
    /// Draw all the button, as entry, corresponding to every files matching _displaySelector type.
    /// </summary>
    private void DrawMiddlePanelItemEntries()
    {
        // Then set reference for the obj currently edited
        if ( _editedItem.IsNull() ) { SetSelectedItem( ( Item ) _activeEditedObject ); }

        for ( int i = 0; i < _itemGuids.Length; i++ )
        {
            // Get file path for every index...
            string contextualPath = AssetDatabase.GUIDToAssetPath( _itemGuids [ i ] );

            // Convert guids to obj type...
            var itemSO = ( ScriptableObject ) AssetDatabase.LoadAssetAtPath( contextualPath, typeof( ScriptableObject ) );
            Item item = ( Item ) itemSO;

            if ( item.IsNull() ) { continue; }

            using ( new EditorGUI.DisabledGroupScope( _middleSectionSelectedButtonIndex == i ) )
            {
                string buttonText = item.Datas.Name;

                Helper.DrawButton( 
                    new GUIContent() { text = buttonText, },
                    new GUIStyle( GUI.skin.button ) { alignment = TextAnchor.MiddleLeft, },
                    new GUILayoutOption [] { GUILayout.Height( 35 ) },
                    OnClickingButton: () =>
                    {
                        _middleSectionSelectedButtonIndex = i;
                        FocusThisItemAndDisplayItsDatas( item );
                        Selection.activeObject = _activeEditedObject;
                        EditorGUI.FocusTextInControl( null );
                    } );
            }            
        }
    }

    #endregion

    #region RIGHT PANEL

    #region Header
    /// <summary>
    /// Draw the right panel header title corresponding to _rightPanelHeaderTitle
    /// </summary>
    private void DrawRightPanelHeader( float xOffset )
    {
        float width = position.width - ( xOffset );

        Rect panelSize = new( x: xOffset, y: 0, width, height: RIGHT_PANEL_HEADER_HEIGHT );

        GUILayout.BeginArea( panelSize, GUI.skin.box );
        GUILayout.BeginHorizontal( GUI.skin.textArea );

        Helper.DrawLabel( _rightPanelHeaderTitle, false, new GUIStyle( GUI.skin.label )
        {
            alignment = TextAnchor.MiddleLeft,
            fontStyle = FontStyle.Bold,
            fontSize = 13,
        } );

        DrawSelectFileButton( _activeEditedObject );

        GUILayout.EndHorizontal();
        GUILayout.EndArea();
    }
    private void SetRightPanelHeaderLabel( string newTitleValue )
    {
        _rightPanelHeaderTitle = newTitleValue;
    }
    #endregion

    /// <summary>
    /// Draws the right panel corresponding to _displaySelector type.
    /// </summary>
    private void DrawRightPanel( float xOffset )
    {
        switch ( _displaySelector )
        {
            case DisplaySelector.Items: DrawItemContent( xOffset );
                break;
            case DisplaySelector.Units:
                break;
            case DisplaySelector.Competences:
                break;
        }
    }

    #endregion

    #region ITEMS CONTENT

    #region Item initialization

    private void InitializeItemProperties( bool withFieldReset = false )
    {
        Debug.Log( "InitializeItemProperties" );

        if ( withFieldReset ) { ResetEditedItemFields(); }

        #region Infos

        // Name
        _editedItemName = _editedItemSOReference.FindProperty( "_name" );
        _editedItemNameField ??= _editedItemName.stringValue;

        // ID
        _editedItemID = _editedItemSOReference.FindProperty( "_id" );
        if ( _editedItemID.intValue != _editedItem.GetInstanceID() )
        {
            _editedItemID.intValue = _editedItem.GetInstanceID();
            _editedItemSOReference.ApplyModifiedProperties();
        }

        // Description
        _editedItemDescription = _editedItemSOReference.FindProperty( "_description" );
        _editedItemDescriptionField ??= _editedItemDescription.stringValue;

        #endregion

        #region Visuals

        // Icon
        _editedItemIcon = _editedItemSOReference.FindProperty( "_icon" );
        _editedItemIconField ??= ( Sprite ) _editedItemIcon.objectReferenceValue;

        // Prefab
        _editedItemPrefab = _editedItemSOReference.FindProperty( "_prefab" );
        _editedItemPrefabField ??= ( GameObject ) _editedItemPrefab.objectReferenceValue;

        #endregion

        #region Settings

        // Rarity
        _editedItemRarity = _editedItemSOReference.FindProperty( "_rarity" );
        _editedItemRarityField ??= ( Rarity? ) _editedItemRarity.enumValueFlag;

        // Can be equipped
        _editedItemCanBeEquipped = _editedItemSOReference.FindProperty( "_canBeEquipped" );
        _editedItemCanBeEquippedField ??= _editedItemCanBeEquipped.boolValue;

        // Linked body part
        _editedItemLinkedBodyPart = _editedItemSOReference.FindProperty( "_linkedBodyPart" );
        _editedItemLinkedBodyPartField ??= ( LinkedBodyPart? ) _editedItemLinkedBodyPart.enumValueIndex;

        // Is Stackable
        _editedItemIsStackable = _editedItemSOReference.FindProperty( "_isStackable" );
        _editedItemIsStackableField ??= _editedItemIsStackable.boolValue;

        // Max Stack Size
        _editedItemMaxStackSize = _editedItemSOReference.FindProperty( "_maxStackSize" );
        _editedItemMaxStackSizeField ??= _editedItemMaxStackSize.intValue;

        // Has Stats
        _editedItemHasStats = _editedItemSOReference.FindProperty( "_hasStats" );
        _editedItemHasStatsField ??= _editedItemHasStats.boolValue;

        //Stats List
        InitializeStatProperty( StatType.Strength );
        InitializeStatProperty( StatType.Endurance );
        InitializeStatProperty( StatType.Dexterity );

        #endregion
    }

    private void InitializeStatProperty( StatType statType )
    {
        switch ( statType )
        {
            case StatType.Strength:
                _editedItemStrengthStat = 
                    _editedItemSOReference.FindProperty( "_stats" ).GetArrayElementAtIndex( 0 ).FindPropertyRelative( "_points" );

                if ( _editedItemStrengthStatField.IsNull() )
                {
                    _editedItemStrengthStatField = _editedItemStrengthStat.intValue;
                }
                break;

            case StatType.Endurance:
                _editedItemEnduranceStat = 
                    _editedItemSOReference.FindProperty( "_stats" ).GetArrayElementAtIndex( 1 ).FindPropertyRelative( "_points" );

                Debug.Log( _editedItemEnduranceStat.intValue );

                if ( _editedItemEnduranceStatField.IsNull() )
                {
                    _editedItemEnduranceStatField = _editedItemEnduranceStat.intValue;
                }
                break;

            case StatType.Dexterity:
                _editedItemDexterityStat =
                    _editedItemSOReference.FindProperty( "_stats" ).GetArrayElementAtIndex( 2 ).FindPropertyRelative( "_points" );

                if ( _editedItemDexterityStatField.IsNull() )
                {
                    _editedItemDexterityStatField = _editedItemDexterityStat.intValue;
                }
                break;
        }
    }

    #endregion

    #region Item drawer methods

    private void DrawItemContent( float xOffset )
    {
        float width = position.width - ( xOffset );
        float yOffset = RIGHT_PANEL_HEADER_HEIGHT;

        Rect panelSize = new( x: xOffset, y: yOffset, width, height: position.height - RIGHT_PANEL_HEADER_HEIGHT );

        GUILayout.BeginArea( panelSize, GUI.skin.box );
        GUILayout.BeginVertical( GUI.skin.window );

        DrawItemSubPanels( width );

        GUILayout.EndVertical();
        GUILayout.EndArea();        
    }

    private void DrawItemSubPanels( float parentWidth )
    {
        for ( int i = 0; i < 3; i++ )
        {
            string subPanelText = string.Empty;

            float subPanelXOffset = 0;
            float subPanelYOffset = 0;

            float subPanelWidth = 0;
            float subPanelHeight = 0;

            float totalSubPanelWidth = parentWidth - 10;
            float totalSubPanelHeight = position.height - RIGHT_PANEL_HEADER_HEIGHT - ( RIGHT_PANEL_HEADER_HEIGHT + 1 );

            switch ( i )
            {
                case 0: // Upper Left
                    subPanelXOffset = 2;
                    subPanelYOffset = 2;

                    subPanelWidth = totalSubPanelWidth;

                    subPanelHeight = totalSubPanelHeight;
                    subPanelHeight += RIGHT_PANEL_HEADER_HEIGHT * 2.5f;
                    break;

                case 1: // Upper Right
                    subPanelXOffset = totalSubPanelWidth / 2;
                    subPanelXOffset += 3;

                    subPanelYOffset = 2;

                    subPanelWidth = totalSubPanelWidth;

                    subPanelHeight = totalSubPanelHeight;
                    subPanelHeight += RIGHT_PANEL_HEADER_HEIGHT * 2.5f;
                    break;

                case 2: //Lower
                    subPanelXOffset = 2;

                    subPanelYOffset = totalSubPanelHeight / 2 ;
                    subPanelYOffset += RIGHT_PANEL_HEADER_HEIGHT + 9;

                    subPanelWidth = totalSubPanelWidth * 2;
                    subPanelWidth += 2;

                    subPanelHeight = totalSubPanelHeight;
                    subPanelHeight -= RIGHT_PANEL_HEADER_HEIGHT + 9;
                    break;
            }

            DrawItemSubPanelsContent( i, subPanelText, subPanelXOffset, subPanelYOffset, subPanelWidth, subPanelHeight );
        }
    }

    private void DrawItemSubPanelsContent( int index, string panelName, float xPos, float yPos, float totalWidth, float totalHeight )
    {
        float recalculatedWidth = totalWidth / 2;
        float recalculatedHeight = totalHeight / 2;

        var skin = GUI.skin.textField;
        Rect panelSize = new( 
            xPos + skin.border.left, 
            yPos + skin.border.top, 
            recalculatedWidth, 
            recalculatedHeight );

        GUILayout.BeginArea( panelSize, skin );

        GUILayout.BeginVertical( GUI.skin.box );

        GUILayout.Space( 2f );

        GUILayout.Label( panelName, EditorStyles.boldLabel );

        switch ( index )
        {
            case 0:
                DrawItemInfosContent( panelSize.width, panelSize.height );
                break;

            case 1:
                DrawItemSettingsContent( panelSize.width, panelSize.height );
                break;

            case 2:
                DrawItemVisualsContent( panelSize.width, panelSize.height );                
                break;
        }        

        GUILayout.EndVertical();

        GUILayout.EndArea();
    }

    private void DrawItemInfosContent( float subPanelWidth, float subPanelHeight )
    {
        EditorGUI.BeginChangeCheck();

        Helper.DrawLabel( "Infos", true, new GUIStyle( GUI.skin.label )
        {
            fontSize = 14,
            fontStyle = FontStyle.Bold,
        },
        yOffset: -15 );

        GUILayout.Space( 5 );

        EditorGUILayout.BeginVertical();
        EditorGUILayout.BeginHorizontal();

        Helper.DrawLabel( "Name", false );

        _editedItemNameField = EditorGUILayout.TextField( _editedItemNameField );

        Helper.DrawButton(
            new GUIContent()
            {
                image = _resetIcon,
                tooltip = "Reset name",
            },
            new GUIStyle( GUI.skin.button )
            {
                padding = new RectOffset( 2, 2, 2, 2 ),
            },
            new GUILayoutOption []
            {
                GUILayout.Width( 18 ),
                GUILayout.Height( 18 ),
            },
            OnClickingButton: () => _editedItemNameField = "[TYPE HERE]" );

        Helper.DrawLabel( "ID", true );        

        using ( new EditorGUI.DisabledGroupScope( true ) )
        {
            _editedItemIDField = EditorGUILayout.IntField( _editedItem.GetInstanceID() );
        } 

        EditorGUILayout.EndHorizontal();

        GUILayout.Space( 5 );

        _descriptionSizeInfo = _editedItemDescriptionField.IsNull() ? "" : _editedItemDescriptionField.Length.ToString();
        string size = "Size : " + _descriptionSizeInfo;
        GUILayout.Label( "Description - " + size );

        _editedItemDescriptionField = EditorGUILayout.TextArea( _editedItemDescriptionField, new GUILayoutOption [] 
        {
            GUILayout.Width( subPanelWidth - 15 ),
            GUILayout.Height( subPanelHeight / 1.525f ) 
        } );

        GUILayout.Space( 2 );

        Helper.DrawButton( 
            new GUIContent() { text = "Reset description", },
            new GUILayoutOption[] { GUILayout.Width( 125 ), },
            OnClickingButton: () => _editedItemDescriptionField = "[TYPE HERE]" );

        EditorGUILayout.EndVertical();

        if ( EditorGUI.EndChangeCheck() )
        {
            _descriptionSizeInfo = _editedItemDescriptionField.Length.ToString();

            _editedItemName.stringValue = _editedItemNameField;
            _editedItemID.intValue = _editedItemIDField.Value;
            _editedItemDescription.stringValue = _editedItemDescriptionField;

            PushDatasToScriptableObject( DisplaySelector.Items );

            SetRightPanelHeaderLabel( _editedItemName.stringValue );
        }
    }

    #region Item settings - General infos / stack / stats / visuals

    private void DrawItemSettingsContent( float subPanelWidth, float subPanelHeight )
    {
        EditorGUI.BeginChangeCheck();

        Helper.DrawLabel( "Settings", true, new GUIStyle( GUI.skin.label )
        {
            fontSize = 14,
            fontStyle = FontStyle.Bold,
        },
        yOffset: -15 );

        GUILayout.Space( 5 );

        #region General Infos -> Is it equippable - Rarity - Linked body part

        EditorGUILayout.BeginVertical();

        EditorGUILayout.BeginVertical();

        GUILayout.Label( "1 - Generalities".ToUpper(), new GUIStyle( GUI.skin.label )
        {
            contentOffset = new Vector2( -2, 0 ),
            alignment = TextAnchor.MiddleLeft,
            fontSize = 12,
            fontStyle = FontStyle.Bold,
        } );

        GUILayout.Space( 5 );

        // Can be equipped bool
        _editedItemCanBeEquippedField = EditorGUILayout.ToggleLeft( " Can Be Equipped ?", _editedItemCanBeEquippedField.Value );

        GUILayout.Space( 5 );

        float labelXOffset = 34;

        using ( new EditorGUI.DisabledGroupScope( !_editedItemCanBeEquippedField.Value ) )
        {
            EditorGUILayout.BeginHorizontal();

            GUIStyle rarityLabelStyle = new( GUI.skin.label )
            {
                contentOffset = new Vector2( labelXOffset, 2 ),
            };
            GUILayout.Label( "Rarity", rarityLabelStyle );

            // Rarity enum
            _editedItemRarityField = !_editedItemCanBeEquippedField.Value ?
                ( Rarity? ) EditorGUILayout.EnumPopup( Rarity.Unassigned, GUILayout.Width( subPanelWidth / 1.45f ) )
                : ( Rarity? ) EditorGUILayout.EnumPopup( _editedItemRarityField.Value, GUILayout.Width( subPanelWidth / 1.45f ) );

            EditorGUILayout.EndHorizontal();
        }

        EditorGUILayout.EndVertical();

        EditorGUILayout.BeginHorizontal();

        LinkedBodyPart selectedBodyPart = _editedItemLinkedBodyPartField.Value;

        string bodyPartsIconFolderPath = "Assets/Utilities/Editor/Data Editor/Icons/LinkedBodyPart/White/";
        string iconFileName = "";

        switch ( selectedBodyPart )
        {
            case LinkedBodyPart.Head:
                iconFileName = bodyPartsIconFolderPath + "Spr_Head_White.png";
                break;
            case LinkedBodyPart.Chest:
                iconFileName = bodyPartsIconFolderPath + "Spr_Chest_White.png";
                break;
            case LinkedBodyPart.Legs:
                iconFileName = bodyPartsIconFolderPath + "Spr_Legs_White.png";
                break;
            case LinkedBodyPart.Hands:
                iconFileName = bodyPartsIconFolderPath + "Spr_Hands_White.png";
                break;
            case LinkedBodyPart.Weapon_Left:
                iconFileName = bodyPartsIconFolderPath + "Spr_Weapon_Left_White.png";
                break;
            case LinkedBodyPart.Weapon_Right:
                iconFileName = bodyPartsIconFolderPath + "Spr_Weapon_Right_White.png";
                break;
        }

        using ( new EditorGUI.DisabledGroupScope( !_editedItemCanBeEquippedField.Value ) )
        {
            Texture2D bodyPartIcon = selectedBodyPart == LinkedBodyPart.Unassigned
                ? EditorGUIUtility.FindTexture( "d_Unlinked@2x" )
                : ( Texture2D ) AssetDatabase.LoadAssetAtPath( iconFileName, typeof( Texture2D ) );

            // Draw Icon of body part
            if ( !bodyPartIcon.IsNull() && Event.current.type.Equals( EventType.Repaint ) )
            {
                Graphics.DrawTexture( new Rect( 5, 91, 32, 32 ), bodyPartIcon );
            }

            GUIStyle linkedBodyPartLabelStyle = new( GUI.skin.label )
            {
                contentOffset = new Vector2( labelXOffset, 0 ),
            };
            GUILayout.Label( "Linked Body Part", linkedBodyPartLabelStyle );

            // Linked body part
            _editedItemLinkedBodyPartField = !_editedItemCanBeEquippedField.Value ?
                ( LinkedBodyPart? ) EditorGUILayout.EnumPopup( LinkedBodyPart.Unassigned, GUILayout.Width( subPanelWidth / 1.45f ) )
                : ( LinkedBodyPart? ) EditorGUILayout.EnumPopup( _editedItemLinkedBodyPartField.Value, GUILayout.Width( subPanelWidth / 1.45f ) );
        }

        EditorGUILayout.EndHorizontal();

        #endregion

        GUILayout.Space( 15 );

        #region Stack infos

        GUILayout.Label( "2 - Stack infos".ToUpper(), new GUIStyle( GUI.skin.label )
        {
            contentOffset = new Vector2( -2, 0 ),
            alignment = TextAnchor.MiddleLeft,
            fontSize = 12,
            fontStyle = FontStyle.Bold,
        } );

        GUILayout.Space( 5 );

        using ( new EditorGUILayout.HorizontalScope() )
        {
            // isStackable
            _editedItemIsStackableField = EditorGUILayout.ToggleLeft( " Is the item stackable ?", _editedItemIsStackableField.Value );

            using ( new EditorGUI.DisabledGroupScope( !_editedItemIsStackableField.Value ) )
            {
                GUILayout.Label( "Max stack size", new GUIStyle( GUI.skin.label )
                {
                    contentOffset = new Vector2( -2, 2 ),
                } );

                // Max stack size int range
                _editedItemMaxStackSizeField = 
                    EditorGUILayout.IntField( 
                        !_editedItemIsStackableField.Value ? 1 : _editedItemMaxStackSizeField.Value, 
                        new GUIStyle( GUI.skin.textField ) { contentOffset = new Vector2( 0, 0 ) } );
            }
        }

        #endregion

        GUILayout.Space( 15 );

        #region Stats infos

        GUILayout.Label( "3 - Stats infos".ToUpper(), new GUIStyle( GUI.skin.label )
        {
            contentOffset = new Vector2( -2, 0 ),
            alignment = TextAnchor.MiddleLeft,
            fontSize = 12,
            fontStyle = FontStyle.Bold,
        } );

        GUILayout.Space( 5 );

        GUILayout.BeginHorizontal();

        // Stats - END FOR DEX
        // Has Stats ?
        _editedItemHasStatsField = EditorGUILayout.ToggleLeft( " Does it have stats ?", _editedItemHasStatsField.Value );

        Helper.DrawButton( 
            new GUIContent() { text = "Reset All Stats" },
            new GUILayoutOption[] { GUILayout.Width( 105 ), },
            OnClickingButton: () =>
            {
                if ( EditorUtility.DisplayDialog(
                   "Reset all stats point ?",
                   "Are you sure you want to reset all stats point ?",
                   "Yes, Reset all",
                   "Cancel" ) )
                {
                    ResetItemStatPoint( StatType.Strength );
                    ResetItemStatPoint( StatType.Endurance );
                    ResetItemStatPoint( StatType.Dexterity );
                }
            } );


        GUILayout.EndHorizontal();

        using ( new EditorGUI.DisabledGroupScope( !_editedItemHasStatsField.Value ) )
        {
            GUILayout.BeginVertical();

            int statAmount = System.Enum.GetValues( typeof( StatType ) ).Length - 1;
            int statTypeIndex = 1;

            for ( int i = 0; i < statAmount; i++ )
            {
                DrawItemStatEntries( ( StatType ) Helper.GetEnumToArray( typeof( StatType ) ).GetValue( statTypeIndex ), subPanelWidth );
                statTypeIndex++;
            }            

            GUILayout.EndVertical();
        }

        #endregion

        EditorGUILayout.EndVertical();

        if ( EditorGUI.EndChangeCheck() )
        {
            _editedItemCanBeEquipped.boolValue = _editedItemCanBeEquippedField.Value;
            _editedItemRarity.enumValueFlag = ( int ) _editedItemRarityField.Value;
            _editedItemLinkedBodyPart.enumValueFlag = ( int ) _editedItemLinkedBodyPartField.Value;

            _editedItemIsStackable.boolValue = _editedItemIsStackableField.Value;
            _editedItemMaxStackSize.intValue = _editedItemMaxStackSizeField.Value;

            _editedItemHasStats.boolValue = _editedItemHasStatsField.Value;

            _editedItemStrengthStat.intValue = _editedItemStrengthStatField.Value;
            _editedItemEnduranceStat.intValue = _editedItemEnduranceStatField.Value;
            _editedItemDexterityStat.intValue = _editedItemDexterityStatField.Value;

            PushDatasToScriptableObject( DisplaySelector.Items );
        }
    }

    private void DrawItemStatEntries( StatType statType, float entryWidth )
    {
        GUILayout.BeginHorizontal();

        // Set label content according to statType
        string statTypeAbreviation = "";
        switch ( statType )
        {
            case StatType.Strength:
                statTypeAbreviation = " - STR";
                break;

            case StatType.Endurance:
                statTypeAbreviation = " - END";
                break;

            case StatType.Dexterity:
                statTypeAbreviation = " - DEX";
                break;
        }

        string labelContent = statType.ToString().ToUpper() + statTypeAbreviation;
        Helper.DrawLabel( labelContent, true, new GUIStyle( GUI.skin.label )
        {
            fontSize = 12,
        } );

        float entryFieldWidth = entryWidth / 6.225f;

        // Set objectField point according to statType
        switch ( statType )
        {
            case StatType.Strength:
                GUILayout.FlexibleSpace();
                _editedItemStrengthStatField = 
                    EditorGUILayout.IntField( _editedItemStrengthStatField.Value,
            new GUILayoutOption [] { GUILayout.Width( entryFieldWidth ), } );
                break;

            case StatType.Endurance:
                GUILayout.FlexibleSpace();
                _editedItemEnduranceStatField = EditorGUILayout.IntField( _editedItemEnduranceStatField.Value,
            new GUILayoutOption [] { GUILayout.Width( entryFieldWidth ), } );
                break;

            case StatType.Dexterity:
                GUILayout.FlexibleSpace();
                _editedItemDexterityStatField = EditorGUILayout.IntField( _editedItemDexterityStatField.Value,
            new GUILayoutOption [] { GUILayout.Width( entryFieldWidth ), } );
                break;
        }

        Helper.DrawButton(
            new GUIContent()
            {
                image = _resetIcon,
                tooltip = "Reset points of stat " + statType.ToString(),
            },
            new GUIStyle( GUI.skin.button )
            {
                padding = new RectOffset( 2, 2, 2, 2 ),
            },
            new GUILayoutOption []
            {
                GUILayout.Width( 18 ),
                GUILayout.Height( 18 ),
            },
            OnClickingButton: () => ResetItemStatPoint( statType ) );

        GUILayout.EndHorizontal();
    }

    private void ResetItemStatPoint( StatType statType )
    {
        switch ( statType )
        {
            case StatType.Strength:
                _editedItemStrengthStat.intValue = 0;
                _editedItemStrengthStatField = _editedItemStrengthStat.intValue;
                break;

            case StatType.Endurance:
                _editedItemEnduranceStat.intValue = 0;
                _editedItemEnduranceStatField = _editedItemEnduranceStat.intValue;
                break;

            case StatType.Dexterity:
                _editedItemDexterityStat.intValue = 0;
                _editedItemDexterityStatField = _editedItemDexterityStat.intValue;
                break;
        }
    }

    private void DrawItemVisualsContent( float subPanelWidth, float subPanelHeight )
    {
        EditorGUI.BeginChangeCheck();

        Helper.DrawLabel( "Visuals", true, new GUIStyle( GUI.skin.label )
        {
            fontSize = 14,
            fontStyle = FontStyle.Bold,
        }, 
        yOffset: -15);

        GUILayout.Space( 5 );

        using ( new EditorGUILayout.HorizontalScope() )
        {
            #region Icon field

            EditorGUILayout.BeginVertical();

            GUILayout.FlexibleSpace();

            GUILayout.BeginHorizontal();

            GUILayout.FlexibleSpace();

            GUILayout.Label( "Icon" );

            _editedItemIconField = ( Sprite ) EditorGUILayout.ObjectField( _editedItemIconField, typeof( Sprite ), false, new GUILayoutOption []
            {
                GUILayout.Width( subPanelWidth / 6 ),
            } );

            using ( new EditorGUI.DisabledScope( _editedItemIconField.IsNull() ) )
            {
                Helper.DrawButton(
                    new GUIContent()
                    {
                        image = EditorGUIUtility.FindTexture( "d_winbtn_win_close@2x" ),
                        tooltip = "Erase the selection for this icon field",
                    },
                    new GUIStyle( GUI.skin.button )
                    {
                        padding = new RectOffset( 0, 0, 0, 0 ),
                    },
                    new GUILayoutOption[]
                    {
                        GUILayout.Width( 18 ),
                        GUILayout.Height( 18 ),
                    },
                    Color.red,
                    OnClickingButton: () => _editedItemIconField = null );
            }

            GUILayout.EndHorizontal();

            int iconPreviewSize = 150;

            if ( !_editedItemIconField.IsNull() && Event.current.type.Equals( EventType.Repaint ) )
            {

                Helper.DrawTexture(
                    _editedItemIconField.texture,
                    iconPreviewSize,
                    subPanelWidth / 4,
                    subPanelHeight / 2,
                    0,
                    10 );
            }
            else
            {
                GUI.Label( new Rect(
                        x: subPanelWidth / 4 - iconPreviewSize / 4,
                        y: ( subPanelHeight / 2 ) - iconPreviewSize / 2,
                        width: iconPreviewSize,
                        height: iconPreviewSize ),
                    "No icon set." );
            }

            EditorGUILayout.EndVertical();

            #endregion

            GUILayout.FlexibleSpace();

            #region Prefab field

            EditorGUILayout.BeginVertical();

            GUILayout.FlexibleSpace();

            GUILayout.BeginHorizontal();

            GUILayout.FlexibleSpace();

            GUILayout.Label( "Prefab" );

            _editedItemPrefabField = ( GameObject ) EditorGUILayout.ObjectField( _editedItemPrefabField, typeof( GameObject ), false, new GUILayoutOption []
            {
                GUILayout.Width( subPanelWidth / 6 ),
            } );

            using ( new EditorGUI.DisabledScope( _editedItemPrefabField.IsNull() ) )
            {
                Helper.DrawButton( 
                    new GUIContent()
                    {
                        image = EditorGUIUtility.FindTexture( "d_winbtn_win_close@2x" ),
                        tooltip = "Erase the selection for this prefab field",
                    },
                    new GUIStyle( GUI.skin.button )
                    {
                        padding = new RectOffset( 0, 0, 0, 0 ),
                    },
                    new GUILayoutOption []
                    {
                        GUILayout.Width( 18 ),
                        GUILayout.Height( 18 ),
                    },
                    Color.red,
                    OnClickingButton: () => _editedItemPrefabField = null );

            }

            GUILayout.EndHorizontal();

            int prefabPreviewSize = 150;

            Texture prefabTexture = _editedItemPrefabField.IsNull()
                ? Texture2D.whiteTexture
                : AssetPreview.GetAssetPreview( _editedItemPrefabField );

            if ( !_editedItemPrefabField.IsNull() && Event.current.type.Equals( EventType.Repaint ) )
            {

                Helper.DrawTexture(
                    prefabTexture,
                    prefabPreviewSize,
                    subPanelWidth,
                    subPanelHeight / 2,
                    subPanelWidth / 4,
                    10 );
            }
            else
            {
                GUI.Label( new Rect(
                        x: subPanelWidth - prefabPreviewSize / 2 - subPanelWidth / 4 + prefabPreviewSize / 4,
                        y: ( subPanelHeight / 2 ) - prefabPreviewSize / 2,
                        width: prefabPreviewSize,
                        height: prefabPreviewSize ),
                    "No prefab set." );
            }

            EditorGUILayout.EndVertical();

            #endregion

            GUILayout.FlexibleSpace();
        }

        if ( EditorGUI.EndChangeCheck() )
        {
            _editedItemIcon.objectReferenceValue = _editedItemIconField;
            _editedItemPrefab.objectReferenceValue = _editedItemPrefabField;

            PushDatasToScriptableObject( DisplaySelector.Items );
        }
    }

    #endregion

    #endregion    

    #endregion

    #region BUTTONS

    private void DrawButtonsInLeftPanel()
    {
        int selectionTypeAmount = Helper.GetEnumLength( typeof( DisplaySelector ) );

        for ( int i = 0; i < selectionTypeAmount; i++ )
        {
            string buttonText = ( ( DisplaySelector ) i ).ToString().ToUpper();
            string buttonTooltip = "Display " + buttonText + " content";

            using ( new EditorGUI.DisabledScope( _displaySelector == ( DisplaySelector ) i ) )
            {
                Helper.DrawButton( new GUIContent()
                {
                    text = buttonText,
                    tooltip = buttonTooltip,
                },
                new GUILayoutOption [] { GUILayout.Height( 35 ), },
                OnClickingButton: () => OnDisplayingContentOfType( ( DisplaySelector ) i ) );
            }
        }
    }

    private void DrawSelectFileButton( Object contextObject )
    {
        Helper.DrawButton( new GUIContent()
        {
            text = "Select File".ToUpper(),
            tooltip = "Select the file you are currently editing.",
        },
        new GUIStyle( GUI.skin.button )
        {
            alignment = TextAnchor.MiddleLeft,
            fontStyle = FontStyle.Bold,
        },
        widthOffset: 5,
        OnClickingButton: () => Selection.activeObject = contextObject );

        GUILayout.FlexibleSpace();
    }

    #endregion

    #region HELPERS

    #region Item related

    private void SetSelectedItem( Item item )
    {
        if ( _editedItem != item )
        {
            _editedItem = item;
            Debug.Log( "Item currently edited : " + _editedItem.Datas.Name );
        }
    }

    private void FocusThisItemAndDisplayItsDatas( Item item )
    {
        _editedItemSOReference = new SerializedObject( item );

        SetActiveContextObject( item );
        SetSelectedItem( item );

        InitializeItemProperties( true );
        SetRightPanelHeaderLabel( item.Datas.Name );
    }

    private void ResetEditedItemFields()
    {
        _editedItemNameField = null;
        _editedItemIDField = _editedItem.GetInstanceID();
        _editedItemDescriptionField = null;

        _editedItemIconField = null;
        _editedItemPrefabField = null;

        _editedItemCanBeEquippedField = null;
        _editedItemRarityField = null;
        _editedItemLinkedBodyPartField = null;

        _editedItemIsStackableField = null;
        _editedItemMaxStackSizeField = null;

        _editedItemHasStatsField = null;

        _editedItemStrengthStatField = null;
        _editedItemEnduranceStatField = null;
        _editedItemDexterityStatField = null;
    }

    #endregion

    private void HandleNullSelection()
    {
        if ( _activeEditedObject.IsNull() ) { _middleSectionSelectedButtonIndex = 0; }

        switch ( _displaySelector )
        {
            case DisplaySelector.Items:
                if ( _editedItem.IsNull() )
                {
                    Item firstItem = ( Item ) _activeEditedObject;
                    FocusThisItemAndDisplayItsDatas( firstItem );
                }
                break;

            case DisplaySelector.Units:
                break;

            case DisplaySelector.Competences:
                break;
        }
    }

    private void TryToRefreshItemGuids()
    {
        if ( AssetDatabase.FindAssets( "t:item", null ).Length > _itemGuidsCountOnOppeningWindow )
        {
            Debug.Log( "An Item has been created or removed, item guids need to be updated." );
            _itemGuids = AssetDatabase.FindAssets( "t:item", null );            
        }
    }

    private void SetActiveContextObject( Object obj )
    {
        if ( _activeEditedObject != obj )
        {
            _activeEditedObject = obj;
            Debug.Log( "Active contect object : " + _activeEditedObject.name );
        }
    }

    private void OnDisplayingContentOfType( DisplaySelector displaySelector )
    {
        _displaySelector = displaySelector;

        switch ( _displaySelector )
        {
            case DisplaySelector.Items:
                Item firstItem = ( Item ) _activeEditedObject;
                FocusThisItemAndDisplayItsDatas( firstItem );
                break;

            case DisplaySelector.Units:
                break;

            case DisplaySelector.Competences:
                break;
        }
    }

    private void PushDatasToScriptableObject( DisplaySelector displaySelector )
    {
        switch ( displaySelector )
        {
            case DisplaySelector.Items:
                _editedItemSOReference.ApplyModifiedProperties();
                _editedItem.Datas = new Item.ItemDatas( _editedItem );
                break;

            case DisplaySelector.Units:
                break;

            case DisplaySelector.Competences:
                break;
        }       
    }

    void OnInspectorUpdate()
    {
        TryToRefreshItemGuids();
        Repaint();
    }

    #endregion
}