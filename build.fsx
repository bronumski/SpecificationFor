#r "paket:
nuget Fake.Core.Target
nuget Fake.DotNet.Cli
nuget Fake.IO.FileSystem
//"

#load "./.fake/build.fsx/intellisense.fsx"

open System.IO
open Fake.Core
open Fake.DotNet
open Fake.IO
open Fake.IO.Globbing.Operators

let buildDir = "./build/."
let buildConfiguration = DotNet.BuildConfiguration.Release

let setDotNetBuildOptions : (DotNet.BuildOptions -> DotNet.BuildOptions) =
  fun (dotNetBuildOptions:DotNet.BuildOptions) ->
    { dotNetBuildOptions with
        Configuration = buildConfiguration
        Common        = { dotNetBuildOptions.Common with CustomParams = Some "--no-restore" }
    }
    
let setDotNetTestOptions (projFilePath:string) : (DotNet.TestOptions-> DotNet.TestOptions) =
  let projectDirectory = Path.GetDirectoryName projFilePath
  fun (dotNetTestOptions:DotNet.TestOptions) ->
    { dotNetTestOptions with
        Common        = { dotNetTestOptions.Common with WorkingDirectory = projectDirectory }
        NoBuild       = true
        Configuration = buildConfiguration
    }

// *** Define Targets ***
Target.create "Clean" (fun _ ->
  Shell.cleanDirs [buildDir]
)

Target.create "Build" (fun _ ->
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