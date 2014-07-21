// include Fake lib
#r @"..\tools\FAKE\tools\FakeLib.dll"
#r @"..\tools\Nyx\Elders.Nyx.exe"

open Fake
open Fake.AssemblyInfoFile
open System
open System.IO
open Elders.Nyx

let projectName = "Elders.Hermes"
let projectSummary = "Elders.Hermes - Unique Id generator"
let projectDescription = "Elders.Hermes - Unique Id generator"
let authors = ["Simeno Dimov"; "Nikolai Mynkow"; "Blagovest Petrov"]

// Directories
let buildDir  = @"..\aaaaa\"
let testDir   = @"..\test\"
let deployDir = @"..\publish\"
let reportDir = @"..\report\"
let nugetDir = @"..\nuget\" 

// version info
let version = ReadFileAsString "version.txt" // or retrieve from CI server

// Targets
Target "Clean" (fun _ -> 
    CleanDirs [buildDir; testDir; deployDir; reportDir; nugetDir]
    RestorePackages()
)

Target "FixOutputPath" (fun _ ->
    let projectDir = new DirectoryInfo(@"..\")
    let solutionFile = new FileInfo(@"..\src\Elders.Hermes.sln")

    Nyx.ModifyOutputPath(projectDir, solutionFile)
    AssemblyInfoModifier.ModifyAssemblyInfo(solutionFile)
)

Target "AssemblyVersionInfo" (fun _ ->
    CreateCSharpAssemblyInfo @"..\src\AssemblyVersionInfo.cs"
            [Attribute.ComVisible false
             Attribute.Company  "Elders"
             Attribute.Product  "Hermes"
             Attribute.InformationalVersion " / "
             Attribute.Version version
             Attribute.FileVersion version]
)

Target "AssemblyCompanyProductInfo" (fun _ ->
    CreateCSharpAssemblyInfo @"..\src\AssemblyCompanyProductInfo.cs"
            [Attribute.ComVisible false
             Attribute.Company  "Elders"
             Attribute.Product  "Hermes"
             Attribute.InformationalVersion " / "
             Attribute.Version version
             Attribute.FileVersion version]
)

Target "AssemblyInfo" (fun _ ->
    CreateCSharpAssemblyInfo @"..\src\AssemblyInfo.cs"
            [Attribute.ComVisible false
             Attribute.Company  "Elders"
             Attribute.Product  "Hermes"
             Attribute.InformationalVersion " / "
             Attribute.Version version
             Attribute.FileVersion version]
)

Target "BuildApp" (fun _ ->
    !! @"..\src\*.sln" 
        |> MSBuildRelease null "Build"
        |> Log "Build-Output: "
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
  ==> "FixOutputPath"
  ==> "AssemblyVersionInfo"
  ==> "AssemblyCompanyProductInfo"
  ==> "AssemblyInfo"
  ==> "BuildApp"
 
// start build
RunParameterTargetOrDefault "target" "BuildApp"