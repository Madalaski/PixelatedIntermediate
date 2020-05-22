namespace UnityEditor.Recorder
{
    /// <summary>
    /// Options class for the Recorder
    /// </summary>
    public static class Options
    {
        const string k_VerboseModeMenuItem = RecorderWindow.MenuRoot + "Options/Verbose Mode";
        const string k_ShowRecorderGameObject = RecorderWindow.MenuRoot + "Options/Show Recorder GameObject";
        const string k_ShowLegacyModeMenuItem = RecorderWindow.MenuRoot + "Options/Show Legacy Recorders";

        const string k_ExitPayModeItem = RecorderWindow.MenuRoot + "Options/Exit PlayMode";
        const string k_RecorderPanelWidth = RecorderWindow.MenuRoot + "Options/Recorder Panel Width";
        const string k_SelectedRecorderIndex = RecorderWindow.MenuRoot + "Options/Selected Recorder Index";

        /// <summary>
        /// If true, the recorder will log additional recording steps into the Console.
        /// </summary>
        public static bool verboseMode
        {
            get { return EditorPrefs.GetBool(k_VerboseModeMenuItem, false); }
            set { EditorPrefs.SetBool(k_VerboseModeMenuItem, value); }
        }
        
        /// <summary>
        /// The recoder uses a "Unity-RecorderSessions" GameObject to store Scene references and manage recording sessions.
        /// If true, this GameObject will be visible in the Scene Hierarchy. 
        /// </summary>
        public static bool showRecorderGameObject
        {
            get { return EditorPrefs.GetBool(k_ShowRecorderGameObject, false); }
            set
            {
                EditorPrefs.SetBool(k_ShowRecorderGameObject, value);                
                UnityHelpers.SetGameObjectsVisibility(value);
            }
        }

        /// <summary>
        /// If true, legacy recorders will be displayed in the "Add New Recorder" menu.
        /// Legacy recorders are deprecated and will be removed from the Unity Recorder in future releases.
        /// </summary>
        public static bool showLegacyRecorders
        {
            get { return EditorPrefs.GetBool(k_ShowLegacyModeMenuItem, false); }
            set { EditorPrefs.SetBool(k_ShowLegacyModeMenuItem, value); }
        }
        
        internal static bool exitPlayMode
        {
            get { return EditorPrefs.GetBool(k_ExitPayModeItem, false); }
            set { EditorPrefs.SetBool(k_ExitPayModeItem, value); }
        }

        internal static float recorderPanelWith
        {
            get { return EditorPrefs.GetFloat(k_RecorderPanelWidth, 0); }
            set { EditorPrefs.SetFloat(k_RecorderPanelWidth, value); }
        }

        internal static int selectedRecorderIndex
        {
            get { return EditorPrefs.GetInt(k_SelectedRecorderIndex, 0); }
            set { EditorPrefs.SetInt(k_SelectedRecorderIndex, value); }
        }

        // This variable is used to select how we capture the final image from the
        // render pipeline, with the legacy render pipeline this variable is set to false
        // with the scriptable render pipeline the CameraCaptureBride
        // inside the SRP will reflection to set this variable to true, this will in turn
        // enable using the CameraInput inputStrategy CaptureCallbackInputStrategy 
        public static bool useCameraCaptureCallbacks = false;

        [MenuItem(k_VerboseModeMenuItem, false, RecorderWindow.MenuRootIndex + 200)]
        static void ToggleDebugMode()
        {
            var value = !verboseMode;
            EditorPrefs.SetBool(k_VerboseModeMenuItem, value);
            verboseMode = value;
        }
        
        [MenuItem(k_VerboseModeMenuItem, true)]
        static bool ToggleDebugModeValidate()
        {
            Menu.SetChecked(k_VerboseModeMenuItem, verboseMode);
            return true;
        }
        
        [MenuItem(k_ShowRecorderGameObject, false, RecorderWindow.MenuRootIndex + 200)]
        static void ToggleShowRecorderGameObject()
        {
            var value = !showRecorderGameObject;
            EditorPrefs.SetBool(k_ShowRecorderGameObject, value);
            showRecorderGameObject = value;
        }
        
        [MenuItem(k_ShowRecorderGameObject, true)]
        static bool ToggleShowRecorderGameObjectValidate()
        {
            Menu.SetChecked(k_ShowRecorderGameObject, showRecorderGameObject);
            return true;
        }

        [MenuItem(k_ShowLegacyModeMenuItem, false, RecorderWindow.MenuRootIndex + 200)]
        static void ToggleShowLegacyRecorders()
        {
            var value = !showLegacyRecorders;
            EditorPrefs.SetBool(k_ShowLegacyModeMenuItem, value);
            showLegacyRecorders = value;
        }
        
        [MenuItem(k_ShowLegacyModeMenuItem, true)]
        static bool ToggleShowLegacyRecordersValidate()
        {
            Menu.SetChecked(k_ShowLegacyModeMenuItem, showLegacyRecorders);
            return true;
        }
    }
}
