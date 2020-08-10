var target = Argument("target", "run");
var input = Argument("input", "./Source/TextFile1.txt");
var output = Argument("output", "./test.ll");
var isUnix = IsRunningOnUnix();
var exeName = isUnix ? "./tmp/kumiko" : "./tmp/kumiko.exe";

Task("dll")
    .Does(() =>
{
    StartProcess("cargo", new ProcessSettings 
    {
        Arguments = "build --release --manifest-path ./DLLs/Cargo.toml --target-dir tmp"
    });
    if (!isUnix)
    {
        CopyFile("tmp/release/kumiko.dll.lib", "tmp/kumiko.lib");
        CopyFile("tmp/release/kumiko.dll", "tmp/kumiko.dll");
    }
    else
    {
        CopyFile("tmp/release/libkumiko.so", "tmp/libkumiko.so");
    }
});

Task("compile")
    .IsDependentOn("dll")
    .Does(() =>
{
    DotNetCoreRun("Source", $"{input} {output}");
});

Task("run")
    .IsDependentOn("compile")
    .Does(() =>
{
    StartProcess("clang", new ProcessSettings 
    {
        Arguments = $"{output} -L ./tmp -l kumiko -o {exeName}"
    });
    StartProcess("./tmp/kumiko", new ProcessSettings
    {
        EnvironmentVariables = new Dictionary<string, string> { { "LD_LIBRARY_PATH", "./tmp" } }
    });
});

Task("clean")
    .Does(() =>
{
    CleanDirectory("tmp");
    DotNetCoreClean("");
});


RunTarget(target);