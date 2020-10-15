#r "paket:
nuget Fake.Core.Target
nuget Fake.DotNet.Cli
nuget Fake.DotNet.Testing.DotCover
nuget Fake.IO.FileSystem
//"

#load "./.fake/build.fsx/intellisense.fsx"

open System.IO
open Fake.Core
open Fake.DotNet
open Fake.DotNet.Testing
open Fake.IO
open Fake.IO.FileSystemOperators
open Fake.IO.Globbing.Operators

let binDir = "./bin/."
let buildConfiguration = DotNet.BuildConfiguration.Release

Target.create "Clean" (fun _ ->
  Shell.cleanDirs [binDir]
)

Target.create "Build" (fun _ ->
  let setDotNetBuildOptions : (DotNet.BuildOptions -> DotNet.BuildOptions) =
    fun (dotNetBuildOptions:DotNet.BuildOptions) ->
      { dotNetBuildOptions with
          Configuration = buildConfiguration
          Common        = { dotNetBuildOptions.Common with CustomParams = Some "" }
          // Common       = { dotNetBuildOptions.Common with CustomParams = Some "--no-restore" }
      }

  DotNet.build setDotNetBuildOptions "./src/SpecificationFor.sln"
)

Target.create "Test" (fun _ ->
  let setDotNetOptions (projectDirectory:string) : (DotNet.TestOptions-> DotNet.TestOptions) =
    fun (dotNetTestOptions:DotNet.TestOptions) ->
    { dotNetTestOptions with
        Common        = { dotNetTestOptions.Common with WorkingDirectory = projectDirectory }
        NoBuild       = true
        Configuration = buildConfiguration
    }

  !!("**/*.Tests/*.Unit.Tests.dll")
  |> DotCover.runNUnit3
      (fun dotCoverOptions -> { dotCoverOptions with
                                  Output = binDir @@ "CoverageSnapshot.dcvr"})
      (fun nUnit3Options -> { nUnit3Options with
                                  ShadowCopy = false })
//  !! (buildDir @@ "release" @@ "/*.Unit.Tests.dll") 
//    |> DotCover.runNUnit 
//        (fun dotCoverOptions -> { dotCoverOptions with 
//                Output = artifactsDir @@ "NUnitDotCoverSnapshot.dcvr" }) 
//        (fun nUnitOptions -> { nUnitOptions with
//                DisableShadowCopy = true })
  !!("**/*.Tests.csproj")
  |> Seq.toArray
  |> Array.Parallel.iter (
    fun fullCsProjName -> 
      let projectDirectory = Path.GetDirectoryName(fullCsProjName)
      DotNet.test (setDotNetOptions projectDirectory) ""
  )

)

Target.create "Deploy" (fun _ ->
  Trace.log " --- Deploying app --- "
)

open Fake.Core.TargetOperators

// *** Define Dependencies ***
"Clean"
  ==> "Build"
  ==> "Test"
  ==> "Deploy"

// *** Start Build ***
Target.runOrDefault "Deploy"