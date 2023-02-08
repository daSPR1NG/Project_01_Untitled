using UnityEngine;
using UnityEditor;
using dnSR_Coding.Utilities;

namespace dnSR_Coding
{
	[CustomEditor( typeof( AudioEvent ), true )]
	public class AudioEventEditor : Editor
	{
		[SerializeField] private AudioSource _previewer;

		bool _showFields;

		public void OnEnable()
		{
			_previewer = EditorUtility.CreateGameObjectWithHideFlags( "Audio preview", HideFlags.HideAndDontSave, typeof( AudioSource ) ).GetComponent<AudioSource>();

			_showFields = false;
		}

		public void OnDisable()
		{
			if ( _previewer ) _previewer.gameObject.DestroyInRuntimeOrEditor();
		}

		public override void OnInspectorGUI()
		{
			DrawDefaultInspector();

			//GUILayout.Space( 20 );

			AudioEvent audioEvent = ( AudioEvent ) target;

            SimpleAudioEvent simpleAudioEvent = null;
			if ( audioEvent.GetType() == typeof( SimpleAudioEvent ) ) simpleAudioEvent = ( SimpleAudioEvent ) target;

			bool simpleAudioHasNoClip = simpleAudioEvent && simpleAudioEvent.Clips.IsEmpty();

			_showFields = EditorGUILayout.ToggleLeft( "Show Other Buttons", _showFields );

			GUILayout.Space( 5 );

			EditorGUILayout.BeginFadeGroup( 1 );
			#region Add clip entry button section

			GUIStyle clipManagementLabelstyle = new( GUI.skin.label )
			{
				fontSize = 12,
				fontStyle = FontStyle.Bold
			};

			if ( _showFields ) GUILayout.Label( "Clips List Section", clipManagementLabelstyle );

			if ( ColorUtility.TryParseHtmlString( "#455aff", out Color addClipBgColor ) )
			{
				GUI.backgroundColor = addClipBgColor;
			}

			GUILayout.FlexibleSpace();

			GUIStyle clipManagementStyle = new( GUI.skin.button )
			{
				fontSize = 12,
				fontStyle = FontStyle.Bold
			};

			if ( _showFields && GUILayout.Button( "ADD CLIP ENTRY", clipManagementStyle, GUILayout.Height( 30f ) ) )
			{
				simpleAudioEvent.Clips.Add( new SimpleAudioEvent.ClipData() );
			}

			GUILayout.FlexibleSpace();

			#endregion

			#region Remove all entries that are unreferenced button section

			if ( ColorUtility.TryParseHtmlString( "#455aff", out Color removeLastEntryBgColor ) )
			{
				GUI.backgroundColor = removeLastEntryBgColor;
			}

			GUILayout.FlexibleSpace();

			if ( _showFields && simpleAudioEvent.Clips.Count > 0 && GUILayout.Button( "REMOVE All UNREFERENCED CLIPS", clipManagementStyle, GUILayout.Height( 30 ) ) )
			{
				for ( int i = simpleAudioEvent.Clips.Count - 1; i >= 0; i-- )
				{
					if ( simpleAudioEvent.Clips [ i ].Clip.IsNull() )
					{
						simpleAudioEvent.Clips.RemoveAt( i );
					}
				}
			}

			GUILayout.FlexibleSpace();

			#endregion

			#region Remove all entries button section

			if ( ColorUtility.TryParseHtmlString( "#ff0000", out Color removeAllEntriesBgColor ) )
			{
				GUI.backgroundColor = removeAllEntriesBgColor;
			}

			GUILayout.FlexibleSpace();

			if ( _showFields && simpleAudioEvent.Clips.Count > 0 && GUILayout.Button( "REMOVE ALL CLIPS", clipManagementStyle, GUILayout.Height( 30 ) ) )
			{
				bool decision = EditorUtility.DisplayDialog(
					"REMOVE ALL CLIPS",
					"Are you sure want to remove all existing listed clips for " + simpleAudioEvent.name + "?",
					"Yes, remove these clips.",
					"Don't and cancel." );

				if ( decision ) simpleAudioEvent.Clips.Clear();
			}

			GUILayout.FlexibleSpace();

			#endregion
			EditorGUILayout.EndFadeGroup();

			GUILayout.Space( 10f );

			using ( new GUILayout.VerticalScope() )
            {
				#region Preview button section

				bool cantUsePreviewButton = 
					simpleAudioHasNoClip 
					|| OneOrMoreClipsAreNull( simpleAudioEvent ) 
					|| !_previewer.IsNull() && _previewer.isPlaying;

				Repaint();

				GUIStyle clipPlayLabelstyle = new( GUI.skin.label )
				{
					fontSize = 12,
					fontStyle = FontStyle.Bold
				};

				GUILayout.Label( "Preview Section", clipPlayLabelstyle );

				using ( new EditorGUI.DisabledScope( cantUsePreviewButton ) )
				{
					GUILayout.FlexibleSpace();

					GUIContent previewContent = !_previewer.IsNull() && _previewer.isPlaying 
						? new( "Currently Playing...".ToUpper() ) : new( "Preview".ToUpper() );

					GUIStyle previewStyle = new( GUI.skin.button )
					{
						fontSize = 14,
						fontStyle = FontStyle.Bold
					};

					if ( ColorUtility.TryParseHtmlString( "#455aff", out Color previewBgColor ) )
					{
						GUI.backgroundColor = cantUsePreviewButton ? Color.black : previewBgColor;
					}

					if ( GUILayout.Button( previewContent, previewStyle, GUILayout.Height( 35 ) ) )
					{
						if ( _previewer.IsNull() )
						{
							_previewer = EditorUtility.CreateGameObjectWithHideFlags(
								"Audio preview", HideFlags.HideAndDontSave, typeof( AudioSource ) ).GetComponent<AudioSource>();
						}

						audioEvent.Play( _previewer );
                    }

					GUILayout.FlexibleSpace();
				}

                #endregion

                #region Stop button section

                using ( new EditorGUI.DisabledScope( _previewer.IsNull() || !_previewer.isPlaying ) )
                {
					GUI.backgroundColor = Color.red;

					GUILayout.FlexibleSpace();

					if ( GUILayout.Button( "STOP", GUILayout.Height( 25 ) ) )
					{
						_previewer.Stop();
						_previewer.gameObject.DestroyInRuntimeOrEditor();
					}

					GUILayout.FlexibleSpace();
				}

				#endregion
			}

			GUILayout.Space( 10f );

			#region Is type simple audio event

			if ( !simpleAudioEvent.IsNull() )
            {
				if ( OneOrMoreClipsAreNull( simpleAudioEvent ) )
				{
					GUI.backgroundColor = Color.yellow;

					GUIContent warningContent = new( "One or more than one clip(s) are not referenced." );

					EditorGUILayout.HelpBox( warningContent.text, MessageType.Warning, true );
				}

				if ( simpleAudioHasNoClip )
				{
					GUI.backgroundColor = Color.red;

					GUIContent errorContent = new( "There is no clip referenced." );

					EditorGUILayout.HelpBox( errorContent.text, MessageType.Error, true );
				}
			}

			#endregion

			GUILayout.Space( 10f );

            #region Log Section

			GUI.backgroundColor = Color.white;

			EditorGUILayout.HelpBox(  "Clips count: " + simpleAudioEvent.Clips.Count, MessageType.None );

			GUILayout.Space( 2f );

			if ( !simpleAudioEvent.Clips.IsEmpty() && !simpleAudioEvent.ChosenClip.IsNull() && !string.IsNullOrEmpty( simpleAudioEvent.ChosenClip.GetName() ) )
            {
				EditorGUILayout.HelpBox( "Last recorded settings in memory" + '\n' +
				simpleAudioEvent.ChosenClip.GetName() +
				 " Pitch: " + simpleAudioEvent.GetLastPitchValue + " / " +
				 " Volume: " + simpleAudioEvent.GetLastVolumeValue,

				 MessageType.Info);
			}

            #endregion
		}

		private bool OneOrMoreClipsAreNull( SimpleAudioEvent simpleAudioEvent )
        {
			if ( simpleAudioEvent.Clips.IsEmpty() ) return false;

            for ( int i = simpleAudioEvent.Clips.Count - 1; i >= 0; i-- )
            {
				//Debug.Log( simpleAudioEvent.Clips [ i ].name);
				if ( simpleAudioEvent.Clips [ i ].Clip.IsNull() ) return true;
            }

			return false;
        }
	}
}