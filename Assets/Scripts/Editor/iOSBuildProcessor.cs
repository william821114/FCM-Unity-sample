#if UNITY_IOS
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEditor;
using UnityEditor.iOS.Xcode;
using System.IO;

public class iOSBuildProcessor : IPostprocessBuildWithReport // Will execute after XCode project is built
{
    public int callbackOrder { get { return 0; } }

    public void OnPostprocessBuild(BuildReport report)
    {
        if (report.summary.platform == BuildTarget.iOS) // Check if the build is for iOS 
        {
            iOSPushNotificationProcess(report.summary.outputPath); // Setup for push notification
        }
    }

    private void iOSPushNotificationProcess(string path)
    {
        string projPath = PBXProject.GetPBXProjectPath(path);

        PBXProject proj = new PBXProject();
        proj.ReadFromFile(projPath);

        // Add user notifications framework
        string targetGUID = proj.GetUnityMainTargetGuid(); //GetUnityFrameworkTargetGuid
        proj.AddFrameworkToProject(targetGUID, "UserNotifications.framework", false);

        File.WriteAllText(projPath, proj.WriteToString()); //proj.WriteToFile(path);

        // Enable push notifications capability
        ProjectCapabilityManager manager = new ProjectCapabilityManager(
            projPath,
            "Entitlements.entitlements",
            targetGuid: proj.GetUnityMainTargetGuid()
            );

        manager.AddPushNotifications(true);
        manager.AddBackgroundModes(BackgroundModesOptions.RemoteNotifications);

        manager.WriteToFile();
    }
}
#endif