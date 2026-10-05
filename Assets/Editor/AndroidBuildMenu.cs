using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

// Builds the Android APK without BuildOptions.StrictMode.
// The Build Settings window always uses StrictMode, which turns the Vuforia preprocess errors
// (VuforiaEngine.dll is not available in the Linux editor) into a failed build.
public static class AndroidBuildMenu
{
    const string OUTPUT_PATH = "Builds/MoleculAR_Android_Updated.apk";

    [MenuItem("MoleculAR/Build Android APK")]
    public static void BuildAndroidApk()
    {
        if (EditorUserBuildSettings.activeBuildTarget != BuildTarget.Android)
        {
            EditorUtility.DisplayDialog("Build Android APK", "Switch the platform to Android in File > Build Settings first.", "OK");
            return;
        }

        string[] scenes = EditorBuildSettings.scenes.Where(s => s.enabled).Select(s => s.path).ToArray();
        Directory.CreateDirectory(Path.GetDirectoryName(OUTPUT_PATH));
        EditorUserBuildSettings.buildAppBundle = false;

        BuildPlayerOptions options = new BuildPlayerOptions
        {
            scenes = scenes,
            locationPathName = OUTPUT_PATH,
            target = BuildTarget.Android,
            targetGroup = BuildTargetGroup.Android,
            options = BuildOptions.None
        };

        BuildReport report = BuildPipeline.BuildPlayer(options);
        BuildSummary summary = report.summary;
        if (summary.result == BuildResult.Succeeded)
        {
            Debug.Log($"APK built: {OUTPUT_PATH} ({new FileInfo(OUTPUT_PATH).Length / (1024f * 1024f):0.0} MB) in {summary.totalTime:mm\\:ss}");
            EditorUtility.RevealInFinder(OUTPUT_PATH);
        }
        else
        {
            Debug.LogError($"APK build {summary.result} with {summary.totalErrors} error(s). Check the Console.");
        }
    }
}
