// include Fake lib
#r @"..\..\tools\FAKE\tools\FakeLib.dll"

open Fake
open Fake.AssemblyInfoFile
open System
open System.IO

let projectName = "Elders.Hermes"
let projectSummary = "Elders.Hermes - Unique Id generator"
let projectDescription = "Elders.Hermes - Unique Id generator"
let authors = ["Simeno Dimov"; "Nikolai Mynkow"; "Blagovest Petrov"]

// Directories
let buildDir  = @"..\..\aaaaa\"
let testDir   = @"..\..\test\"
let deployDir = @"..\..\Publish\"
let reportDir = @"..\..\report\"
let nugetDir = @"..\..\nuget\" 
let packagesDir = @"..\packages\"

// version info
let version = ReadFileAsString "version.txt" // or retrieve from CI server

// Targets
Target "Clean" (fun _ -> 
    CleanDirs [buildDir; testDir; deployDir; reportDir; nugetDir]
    RestorePackages()
)

Target "AssemblyInfo" (fun _ ->
    CreateCSharpAssemblyInfo @"..\Elders.Hermes\Properties\AssemblyInfo.cs"
            [Attribute.Title "Elders.Hermes"
             Attribute.Description "Elders.Hermes - Unique Id generator"
             Attribute.Guid "e53468fd-9f29-422c-9499-28e516aa41b1"
             Attribute.Product "Elders.Hermes"
             Attribute.Version version
             Attribute.FileVersion version]            
)

Target "BuildApp" (fun _ ->
    !! @"..\..\src\*.sln" 
        |> MSBuildRelease null "Build"
        |> Log "Build-Output: "
)

Target "Test" (fun _ ->
let dir = new DirectoryInfo(@"..\..\")
Nyx.ModifyOutputPath(dir.FullName, @"D:\Projects\OSS\Elders\Hermes\src\Elders.Hermes.sln")
)

//Target "CreateNuget" (fun _ ->
//    XCopy @"..\..\build\" (nugetDir @@ "Elders.Hermes")
//    XCopy @"..\..\tools\FAKE\" (nugetDir @@ "tools\FAKE")
//
//    "Elders.Hermes.nuspec"
//      |> NuGet (fun p -> 
//            {p with               
//                Authors = authors
//                Project = projectName
//                Version = version
//                NoPackageAnalysis = true
//                Description = projectDescription                               
//                ToolPath = @"..\..\tools\NuGet\NuGet.exe"                             
//                OutputPath = nugetDir })
//)
//
//Target "Publish" (fun _ ->     
//    !! (nugetDir + "*.nupkg") 
//      |> Copy deployDir
//)

// Dependencies
"Clean"
  ==> "Test"
  ==> "AssemblyInfo"
  ==> "BuildApp"
 
// start build
RunParameterTargetOrDefault "target" "BuildApp"