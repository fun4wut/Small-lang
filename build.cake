var target = Argument("target", "dll");
var input = Argument("input", "./Source/TextFile1.txt");
var output = Argument("output", "./test.ll");
Task("dll")
    .Does(() =>
{
    StartProcess("cargo", new ProcessSettings 
    {
        Arguments = "build --release --manifest-path ./DLLs/Cargo.toml --target-dir tmp"
    });
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
        Arguments = $"{output} -L ./tmp/release -l libkumiko -o ./tmp/kumiko.exe"
    });
    StartProcess("./tmp/kumiko");
});



RunTarget(target);