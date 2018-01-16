var target = Argument("target", "Default");
var runs= Argument<int>("runs", 10); 
var delay= Argument<int>("delay", 5); // in seconds

var giturl= Argument("giturl", "https://github.com/jamesmontemagno/LocalizationSample.git");

bool ExecuteGit()
{
    var directory= System.IO.Path.GetFileNameWithoutExtension(giturl);
    if (DirectoryExists(directory))
    {
        Information(directory);
        DeleteDirectory(directory, new DeleteDirectorySettings {
            Recursive= true,
            Force= true
        });
    }
    if (DirectoryExists(directory))
        throw new Exception("Dir not empty");
    StartProcess("cmd", new ProcessSettings { Arguments = $"/c git clone {giturl}", WorkingDirectory = MakeAbsolute(Directory("."))});
    return DirectoryExists(directory);
}

void MessageSplit(int count= 60, ConsoleColor color= ConsoleColor.White) =>  Message(new string('-', count), color);

void Message(string text, ConsoleColor color= ConsoleColor.White)
{
    Console.ForegroundColor= color;
    Console.WriteLine(text);
    Console.ForegroundColor= ConsoleColor.White;
}

Task("Default")
.Does(() => {
    MessageSplit();
    Message("Sample git cloing with SSL error's", ConsoleColor.White);
    Message($"./build.ps -runs={runs} -delay={delay} -target={target}", ConsoleColor.Magenta);
    MessageSplit();
    int ok= 0;
    int run= 0;
    for (int localRun= 0; localRun< runs; localRun++)
    {
        run++;
        Message($"Run={run}/{runs}", ConsoleColor.Blue);
        var result= false;
        if (result= ExecuteGit())
            ok++;
        Message($"Result={result} Oks={ok}/{run}", result ? ConsoleColor.Green: ConsoleColor.Red);
        if (localRun< runs-1)
        {
            Message($"Waiting {delay} seconds", ConsoleColor.Yellow);
            System.Threading.Thread.Sleep(TimeSpan.FromSeconds(delay));
        }
    }
    MessageSplit();
    Message($"Ready Runs={run} Oks={ok} Errors={runs-ok}", ConsoleColor.Magenta);
    MessageSplit();
  });

RunTarget(target);