/* Copyright(C) Przemysław Karpiński - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 */
 

using UnityEditor;

using SpritesheetSaver;

public class SpritesheetSaverWindow : EditorWindow
{
    // Add menu named "My Window" to the Window menu
    [MenuItem("Window/Spritesheet Saver")]
    static void Init()
    {
        // Get existing open window or if none, make a new one:
        SpritesheetSaverWindow window = (SpritesheetSaverWindow)EditorWindow.GetWindow(typeof(SpritesheetSaverWindow));
        window.Show();
    }

    private SpritesheetSaver.Saver saver;

    public void OnGUI()
    {
        if (saver == null) saver = new SpritesheetSaver.Saver();
        saver.OnGUI();
    }


    public void Update()
    {
        if (saver != null)
        {
            saver.Update();
        }
        Repaint();
    }

}
