using System;
using System.IO;
using UnrealBuildTool;

public class MediaPipe : ModuleRules
{
    public MediaPipe(ReadOnlyTargetRules Target) : base(Target)
    {
        PCHUsage = PCHUsageMode.NoPCHs; // UseExplicitOrSharedPCHs;
        bUseUnity = false;
        bUseRTTI = true;

        PublicDependencyModuleNames.AddRange(
            new string[] {
                "Core",
                "CoreUObject",
                "Engine",
                "InputCore",
                "Slate",
                "SlateCore",
                "RHI",
                "RenderCore"
            }
        );

        PrivateDependencyModuleNames.AddRange(
            new string[] {
                "Projects",
            }
        );

        string BinArch = Target.Platform.ToString();
        string LibExt = "";
        string DllExt = ".so";

        if (Target.Platform == UnrealTargetPlatform.Win64)
        {
            LibExt = ".lib";
            DllExt = ".dll";
        }

        string BinaryOutputDir = "$(BinaryOutputDir)";
        string ThirdPartyDir = Path.Combine(ModuleDirectory, "..", "..", "ThirdParty");

        // protobuf deps

        if (Target.Platform == UnrealTargetPlatform.Win64)
        {
            PublicDefinitions.Add("PROTOBUF_USE_DLLS=1");
        }

        PublicIncludePaths.Add(Path.Combine(ThirdPartyDir, "protobuf", "Include"));
        PublicAdditionalLibraries.Add(Path.Combine(ThirdPartyDir, "protobuf", "Lib", BinArch, "libprotobuf" + LibExt));

        string ProtobufBinDir = Path.Combine(ThirdPartyDir, "protobuf", "Binaries", BinArch);
        AddDep("libprotobuf", DllExt, ProtobufBinDir, BinaryOutputDir);

        // mediapipe deps

        string MPBinDir = Path.Combine(ThirdPartyDir, "mediapipe", "Binaries", BinArch);
        string MPDataDir = Path.Combine(ThirdPartyDir, "mediapipe", "Data");

        //var EngineDir = Path.GetFullPath(Target.RelativeEnginePath);
        //string OpenCVPath = Path.Combine(EngineDir, "Plugins", "Runtime", "OpenCV", "Binaries", "ThirdParty", BinArch);

        //AddDep("opencv_world455", DllExt, OpenCVPath, BinaryOutputDir);
        //RuntimeDependencies.Add(Path.Combine(EngineDir, "Plugins", "Runtime", "OpenCV", "opencv_world455.dll"));
        AddDep("ump_shared", DllExt, MPBinDir, BinaryOutputDir);

        if (Target.Type != TargetType.Editor)
            AddDepFolder(MPDataDir, BinaryOutputDir);
    }

    private void AddDep(string FileName, string Ext, string SourceDir, string TargetDir)
    {
        RuntimeDependencies.Add(Path.Combine(TargetDir, FileName + Ext), Path.Combine(SourceDir, FileName + Ext));
    }

    private void AddDepFolder(string SourceDir, string TargetDir)
    {
        foreach (string filePath in Directory.GetFiles(SourceDir, "*.*", SearchOption.AllDirectories))
        {
            string dst = filePath.Replace(SourceDir, TargetDir);
            RuntimeDependencies.Add(dst, filePath);
        }
    }
}
