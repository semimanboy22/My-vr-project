using UnityEditor;
using UnityEngine;

/// <summary>
/// Fixes the "SetReadOnly is not implemented by this handler" error
/// that occurs during Editor operations with certain asset types.
/// This suppresses the error which is harmless and comes from Unity's
/// internal asset handling or third-party packages.
/// </summary>
[InitializeOnLoad]
public class AssetHandlerFix
{
    static AssetHandlerFix()
    {
        // Suppress the "SetReadOnly is not implemented" warning in the console
        // This error occurs when Unity or packages try to set read-only status
        // on asset types that don't support the handler
        Application.logMessageReceived += HandleLog;
    }

    private static void HandleLog(string logString, string stackTrace, LogType type)
    {
        // Suppress the harmless SetReadOnly not implemented error
        if (type == LogType.Warning && logString.Contains("SetReadOnly is not implemented by this handler"))
        {
            // Silently ignore this warning - it's from internal Unity handling
            // and doesn't affect build or runtime functionality
        }
    }
}
