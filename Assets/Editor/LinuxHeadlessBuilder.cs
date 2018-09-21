//place this script in the Editor folder within Assets.
 using UnityEditor;
  
  
 //to be used on the command line:
 //$ Unity -quit -batchmode -executeMethod WebGLBuilder.build
  
 class LinuxHeadlessBuilder {
     static void build() {
         string[] scenes = {"Assets/Main.unity"};
         BuildPipeline.BuildPlayer(scenes, "build\\linux\\linux64headless", BuildTarget.StandaloneLinux64, BuildOptions.EnableHeadlessMode);
     }
 }