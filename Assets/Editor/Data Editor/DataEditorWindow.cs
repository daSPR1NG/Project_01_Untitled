using dnSR_Coding;
using dnSR_Coding.Utilities;
using UnityEditor;
using UnityEngine;

public class DataEditorWindow : EditorWindow
{
    private enum DisplaySelector { Items = 0, Units = 1, Competences = 2 }

    private const float RIGHT_PANEL_HEADER_HEIGHT = 25;
    private const float MIDDLE_PANEL_HEADER_WIDTH = 150, MIDDLE_PANEL_HEADER_HEIGHT = 25;

    private DisplaySelector _displaySelector = DisplaySelector.Items;
    private float _leftPanelWidth = 0;

    private Vector2 _middleSectionScrollPos;
    private int _middleSectionSelectedButtonIndex = 0;

    private string _rightPanelHeaderTitle;

    private Object _activeContextObject = null;

    #region Items variables
    string [] _itemGuids = null;
    int _itemGuidsCountOnOppeningWindow = 0;

    private string _firstItemPath = null;
    private Object _firstItemObject;

    private Item _editedItem = null;
    private SerializedObject _editedItemSOReference = null;

    private SerializedProperty
        /* Infos : */       _editedItemName, _editedItemID, _editedItemDescription,
        /* Visuals : */     _editedItemIcon;

    private string _editedItemNameField, _editedItemDescriptionField;
    private int? _editedItemIDField;

    private Sprite _editedItemIconField;

    private string _descriptionSize;

    #endregion

    [MenuItem( "Window/Custom Windows/Datas" )]
    public static void ShowWindow()
    {
        EditorWindow window = GetWindow( typeof( DataEditorWindow ) );

        window.titleContent = new GUIContent( "Data Manager" );

        window.minSize = new Vector2( 450, 200 );
        window.maxSize = new Vector2( 1350, 600 );
    }

    private void OnEnable()
    {
        OnOppeningWindow();       
    }
    private void OnOppeningWindow()
    {
        _displaySelector = DisplaySelector.Items;

        // Find all the component of type obj and store them as button...
        _itemGuids = AssetDatabase.FindAssets( "t:item", null );
        _itemGuidsCountOnOppeningWindow = _itemGuids.Length;

        // Get the first obj guid...
        _firstItemPath = AssetDatabase.GUIDToAssetPath( _itemGuids [ 0 ] );

        //Convert it to Object and set active context object...
        _firstItemObject = AssetDatabase.LoadAssetAtPath( _firstItemPath, typeof( Object ) );
        OnDisplayingItemContent();
    }

    void OnGUI()
    {
        ApplyWindowDefaultSettings();

        using ( new EditorGUILayout.HorizontalScope( GUILayout.ExpandHeight( true ) ) )
        {
            DrawLeftPanel( _leftPanelWidth );

            DrawMiddlePanelHeader( _leftPanelWidth, _displaySelector );
            DrawMiddlePanel( _leftPanelWidth, MIDDLE_PANEL_HEADER_HEIGHT, _displaySelector );

            DrawRightPanelHeader( _leftPanelWidth + MIDDLE_PANEL_HEADER_WIDTH );
            DrawRightPanel( _leftPanelWidth + MIDDLE_PANEL_HEADER_WIDTH );
        }
    }

    private void ApplyWindowDefaultSettings()
    {
        // Default settings 
        _leftPanelWidth = position.width * .125f;        
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

        OnClickingItemButtonInLeftPanel();
        OnClickingUnitButtonInLeftPanel();
        OnClickingCompetencesButtonInLeftPanel();

        GUILayout.EndVertical();
        GUILayout.EndArea();
    }

    #region Middle Panel

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
        switch ( displaySelector )
        {
            case DisplaySelector.Items:
                GUILayout.Label( "Items", style );
                break;
            case DisplaySelector.Units:
                GUILayout.Label( "Units", style );
                break;
            case DisplaySelector.Competences:
                GUILayout.Label( "Competences", style );
                break;
        }

        GUILayout.EndHorizontal();

        GUILayout.EndArea();
    }

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
        GUILayout.EndArea();
    }

    private void DrawMiddlePanelItemEntries()
    {
        // Then set reference for the obj currently edited
        if ( _editedItem.IsNull() ) { SetSelectedItem( ( Item ) _firstItemObject ); }

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
                GUIStyle buttonStyle = new( GUI.skin.button )
                {
                    alignment = TextAnchor.MiddleLeft,
                };

                string buttonText = "00" + i + " " + item.Datas.Name;

                if ( _middleSectionSelectedButtonIndex == i )
                {
                    buttonText = "00" + i + " " + _editedItemName.stringValue;
                }

                GUIContent buttonContent = new()
                {
                    text = buttonText,
                };

                if ( GUILayout.Button( buttonContent, buttonStyle ) )
                {
                    _middleSectionSelectedButtonIndex = i;

                    SetActiveContextObject( item );
                    SetSelectedItem( item );

                    SetItemSerializedObjectReferences( true );

                    EditorGUI.FocusTextInControl( null );
                }
            }            
        }
    }

    #endregion

    #region Right Panel
    private void DrawRightPanelHeader( float xOffset )
    {
        float width = position.width - ( xOffset );

        Rect panelSize = new( x: xOffset, y: 0, width, height: RIGHT_PANEL_HEADER_HEIGHT );

        GUILayout.BeginArea( panelSize, GUI.skin.box );
        GUILayout.BeginHorizontal( GUI.skin.textArea );

        GUIStyle style = new( GUI.skin.label )
        {
            alignment = TextAnchor.MiddleLeft,
            fontStyle = FontStyle.Bold,
            fontSize = 14,
        };

        GUILayout.Label( _rightPanelHeaderTitle, style );

        DrawJumpToFileButton( _activeContextObject, _activeContextObject.name );

        GUILayout.EndHorizontal();
        GUILayout.EndArea();
    }

    private void DrawRightPanel( float xOffset )
    {
        switch ( _displaySelector )
        {
            case DisplaySelector.Items:
                DrawItemsPanelContent( xOffset );
                break;
            case DisplaySelector.Units:
                break;
            case DisplaySelector.Competences:
                break;
        }
    }

    private void SetRightPanelHeaderLabel( string newTitleValue )
    {
        Debug.Log( _activeContextObject );

        switch ( _displaySelector )
        {
            case DisplaySelector.Items:
                _rightPanelHeaderTitle = newTitleValue;
                break;

            case DisplaySelector.Units:
                break;

            case DisplaySelector.Competences:
                break;
        }
    }

    #endregion

    #region Items Panel Content

    private void DrawItemsPanelContent( float xOffset )
    {
        float width = position.width - ( xOffset );
        float yOffset = RIGHT_PANEL_HEADER_HEIGHT;

        Rect panelSize = new( x: xOffset, y: yOffset, width, height: position.height - RIGHT_PANEL_HEADER_HEIGHT );

        GUILayout.BeginArea( panelSize, GUI.skin.box );
        GUILayout.BeginVertical( GUI.skin.window );

        ItemSubPanelDrawerHelper( width );

        GUILayout.EndVertical();
        GUILayout.EndArea();        
    }

    private void ItemSubPanelDrawerHelper( float parentWidth )
    {
        for ( int i = 0; i < 4; i++ )
        {
            string subPanelText = string.Empty;

            float subPanelXOffset = 0;
            float subPanelYOffset = 0;

            float subPanelWidth = 0;
            float subPanelHeight = 0;

            float totalSubPanelWidth = parentWidth - 10;
            float totalSubPanelHeight = position.height - RIGHT_PANEL_HEADER_HEIGHT - 26;

            switch ( i )
            {
                case 0:
                    //subPanelText = "Upper Left";

                    subPanelXOffset = 2;

                    subPanelYOffset = 2;

                    subPanelWidth = totalSubPanelWidth;
                    subPanelHeight = totalSubPanelHeight / 2;                    
                    break;

                case 1:
                    subPanelText = "Upper Right";

                    subPanelXOffset = totalSubPanelWidth / 2 + 2;
                    subPanelXOffset += 2;

                    subPanelYOffset = 2;

                    subPanelWidth = totalSubPanelWidth;
                    subPanelWidth -= 2;

                    subPanelHeight = totalSubPanelHeight;
                    break;

                case 2:
                    //subPanelText = "Lowest Left";

                    subPanelXOffset = 2;

                    subPanelYOffset = totalSubPanelHeight / 4;
                    subPanelYOffset += 3;

                    subPanelWidth = totalSubPanelWidth;

                    subPanelHeight = position.height - RIGHT_PANEL_HEADER_HEIGHT + totalSubPanelHeight / 2;
                    subPanelHeight += 3;
                    break;

                case 3:
                    subPanelText = "Lowest Right";

                    subPanelXOffset = totalSubPanelWidth / 2 + 2;
                    subPanelXOffset += 2;

                    subPanelYOffset = totalSubPanelHeight / 2;
                    subPanelYOffset += 3;

                    subPanelWidth = totalSubPanelWidth;
                    subPanelWidth -= 2;

                    subPanelHeight = position.height - RIGHT_PANEL_HEADER_HEIGHT;
                    subPanelHeight += 2;
                    break;
            }

            DrawItemsSubPanel( i, subPanelText, subPanelXOffset, subPanelYOffset, subPanelWidth, subPanelHeight );
        }
    }

    private void DrawItemsSubPanel( int index, string panelName, float xPos, float yPos, float totalWidth, float totalHeight )
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
                DrawVisualsContent( panelSize.width, panelSize.height );
                break;

            case 1:
                break;

            case 2:                
                DrawInfosContent( panelSize.width, panelSize.height );
                break;

            case 3:
                break;
        }        

        GUILayout.EndVertical();

        GUILayout.EndArea();
    }

    private void SetItemSerializedObjectReferences( bool withFieldReset = false )
    {
        Debug.Log( "SetItemSerializedObjectReferences" );

        // Set item serializedObject reference
        _editedItemSOReference = new SerializedObject( _editedItem );

        if ( withFieldReset ) 
        { 
            _editedItemNameField = null;
            _editedItemIDField = _editedItem.GetInstanceID();
            _editedItemDescriptionField = null;

            _editedItemIconField = null;
        }

        // Name
        _editedItemName = _editedItemSOReference.FindProperty( "_name" );
        if ( _editedItemNameField.IsNull() ) 
        {
            _editedItemNameField = _editedItemName.stringValue; 
        }

        // ID
        _editedItemID = _editedItemSOReference.FindProperty( "_id" );
        if( _editedItemID.intValue != _editedItem.GetInstanceID() ) 
        {
            _editedItemID.intValue = _editedItem.GetInstanceID();
            _editedItemSOReference.ApplyModifiedProperties(); 
        }

        // Description
        _editedItemDescription = _editedItemSOReference.FindProperty( "_description" );
        if ( _editedItemDescriptionField.IsNull() ) 
        {
            _editedItemDescriptionField = _editedItemDescription.stringValue; 
        }

        // Icon
        _editedItemIcon = _editedItemSOReference.FindProperty( "_icon" );
        if ( _editedItemIconField.IsNull() )
        {
            _editedItemIconField = ( Sprite ) _editedItemIcon.objectReferenceValue;
        }
    }

    private void DrawInfosContent( float subPanelWidth, float subPanelHeight )
    {
        EditorGUI.BeginChangeCheck();

        GUIStyle labelStyle = new( GUI.skin.label )
        {
            fontSize = 14,
            fontStyle = FontStyle.Bold,
        };
        GUIContent labelContent = new()
        {
            text = "Infos".ToUpper(),
        };

        GUILayout.Space( -15 );

        GUILayout.Label( labelContent, labelStyle );

        GUILayout.Space( 5 );

        EditorGUILayout.BeginVertical();
        EditorGUILayout.BeginHorizontal();

        GUILayout.Label( "Name" );

        _editedItemNameField = EditorGUILayout.TextField( _editedItemNameField );        

        GUILayout.Label( "ID" );

        using ( new EditorGUI.DisabledGroupScope( true ) )
        {
            _editedItemIDField = EditorGUILayout.IntField( _editedItem.GetInstanceID() );
        } 

        EditorGUILayout.EndHorizontal();

        GUILayout.Space( 5 );

        _descriptionSize = _editedItemDescriptionField.IsNull() ? "" : _editedItemDescriptionField.Length.ToString();
        string size = "Size : " + _descriptionSize;
        GUILayout.Label( "Description - " + size );

        _editedItemDescriptionField = EditorGUILayout.TextArea( _editedItemDescriptionField, new GUILayoutOption [] 
        {
            GUILayout.Width( subPanelWidth - 15 ),
            GUILayout.Height( subPanelHeight / 1.25f ) 
        } );

        EditorGUILayout.EndVertical();

        if ( EditorGUI.EndChangeCheck() )
        {
            _descriptionSize = _editedItemDescriptionField.Length.ToString();

            _editedItemName.stringValue = _editedItemNameField;
            _editedItemID.intValue = _editedItemIDField.Value;
            _editedItemDescription.stringValue = _editedItemDescriptionField;

            _editedItemSOReference.ApplyModifiedProperties();

            SetRightPanelHeaderLabel( _editedItemName.stringValue );

            _editedItem.Datas = new Item.ItemDatas( _editedItem );
        }
    }

    private void DrawVisualsContent( float subPanelWidth, float subPanelHeight )
    {
        EditorGUI.BeginChangeCheck();

        GUIStyle labelStyle = new( GUI.skin.label )
        {
            fontSize = 14,
            fontStyle = FontStyle.Bold,
        };
        GUIContent labelContent = new()
        {
            text = "Visuals".ToUpper(),
        };

        GUILayout.Space( -15 );

        GUILayout.Label( labelContent, labelStyle );

        GUILayout.Space( 5 );

        EditorGUILayout.BeginVertical();

        GUILayout.FlexibleSpace();

        GUILayout.Label( "Icon" );

        _editedItemIconField = ( Sprite ) EditorGUILayout.ObjectField( _editedItemIconField, typeof( Sprite ), false, new GUILayoutOption []
        {
            GUILayout.Width( subPanelWidth - 15 ),
        } );

        int iconPreviewSize = 100;

        if ( !_editedItemIconField.IsNull() && Event.current.type.Equals( EventType.Repaint ) )
        {
            Graphics.DrawTexture( new Rect( 
                ( subPanelWidth / 2 ) - iconPreviewSize / 2, 
                ( subPanelHeight / 2 ) - iconPreviewSize / 2 - 10, 
                iconPreviewSize, iconPreviewSize ), 
                _editedItemIconField.texture );
        }

        EditorGUILayout.EndVertical();

        if ( EditorGUI.EndChangeCheck() )
        {
            _editedItemIcon.objectReferenceValue = _editedItemIconField;

            _editedItemSOReference.ApplyModifiedProperties();
            _editedItem.Datas = new Item.ItemDatas( _editedItem );

            Debug.Log( "Save Icon" );
        }
    }

    #endregion

    #region Buttons

    private void OnClickingItemButtonInLeftPanel()
    {
        using ( new EditorGUI.DisabledScope( _displaySelector == DisplaySelector.Items ) )
        {
            if ( GUILayout.Button( "Items".ToUpper(), GUILayout.Height( 35 ) ) )
            {
                _displaySelector = DisplaySelector.Items;

                OnDisplayingItemContent();
            }
        }
    }

    private void OnClickingCompetencesButtonInLeftPanel()
    {
        using ( new EditorGUI.DisabledScope( _displaySelector == DisplaySelector.Competences ) )
        {
            if ( GUILayout.Button( "Competences".ToUpper(), GUILayout.Height( 35 ) ) )
            {
                _displaySelector = DisplaySelector.Competences;
            }
        }
    }

    private void OnClickingUnitButtonInLeftPanel()
    {
        using ( new EditorGUI.DisabledScope( _displaySelector == DisplaySelector.Units ) )
        {
            if ( GUILayout.Button( "Units".ToUpper(), GUILayout.Height( 35 ) ) )
            {
                _displaySelector = DisplaySelector.Units;
            }
        }
    }

    private void DrawJumpToFileButton( Object contextObject, string fileName )
    {
        GUIStyle style = new( GUI.skin.button )
        {
            alignment = TextAnchor.MiddleLeft,
            fontStyle = FontStyle.Bold,
        };

        string contentText = "Select File";
        GUIContent content = new()
        {
            text = contentText.ToUpper(),
        };

        Vector2 contentSize = style.CalcSize( content );

        if ( GUILayout.Button( content, style, GUILayout.Width( contentSize.x + 5 ) ) )
        {
            Selection.activeObject = contextObject;
        }

        GUILayout.FlexibleSpace();
    }

    #endregion

    #region Helpers

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
        if ( _activeContextObject != obj )
        {
            _activeContextObject = obj;
            Debug.Log( "Active contect object : " + _activeContextObject.name );
        }
    }

    private void SetSelectedItem( Item item )
    {
        if ( _editedItem != item )
        {
            _editedItem = item;
            Debug.Log( "Item currently edited : " + _editedItem.Datas.Name );
        }
    }

    private void OnDisplayingItemContent()
    {
        SetActiveContextObject( _firstItemObject );

        Item firstItem = ( Item ) _firstItemObject;

        SetSelectedItem( firstItem );
        SetItemSerializedObjectReferences();

        SetRightPanelHeaderLabel( _editedItemName.stringValue );
    }

    void OnInspectorUpdate()
    {
        TryToRefreshItemGuids();
        Repaint();
    }

    #endregion
}
