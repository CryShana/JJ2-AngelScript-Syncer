global using static System.Console;
using System.Text;
using System.Text.RegularExpressions;

const string SCRIPT_FILENAME = "global.as";

var current_directory = AppDomain.CurrentDomain.BaseDirectory;
Environment.CurrentDirectory = current_directory;

// ensure we are in same directory as Jazz2
if (!File.Exists("Jazz2.exe"))
{
    WriteLine("Jazz2.exe not found! Make sure to place this executable in the same directory as Jazz2.exe!");
    return;
}

// ensure global script exists
if (!File.Exists(SCRIPT_FILENAME))
{
    WriteLine($"{SCRIPT_FILENAME} not found! Please create a global AngelScript called '{SCRIPT_FILENAME}' and place it in the same directory as this executable!");
    WriteLine("Any world you want excluded, make sure to use comments '// EXCLUDE filename_pattern'");
    return;
}

// load global script and parse exclusions
var global_as = File.ReadAllText(SCRIPT_FILENAME);

var exclusion_rgx = new Regex(@"\/\/\s?EXCLUDE\s(.*)");
var exclusions = new List<string>();
foreach (Match match in exclusion_rgx.Matches(global_as))
{
    var exclusion = match.Groups[1].Value;
    exclusions.Add(exclusion);
}

// load worlds
var worlds = Directory.GetFiles(".", "*.j2l");

WriteLine($"Detected {worlds.Length} worlds");
WriteLine($"Detected {exclusions.Count} exclusions: {string.Join(", ", exclusions)}");

// filter worlds based on exclusion pattern - they can use '*' as wildcard
var removed_worlds = new List<string>();
var filtered_worlds = worlds.Where(world =>
{
    var filename = Path.GetFileNameWithoutExtension(world);
    foreach (var exclusion in exclusions)
    {
        if (Regex.IsMatch(filename, exclusion.Replace("*", ".*")))
        {
            removed_worlds.Add(filename);
            return false;
        }
    }
    return true;
}).ToArray();

if (exclusions.Count > 0)
{
    WriteLine($"Excluded {removed_worlds.Count} world(s): {string.Join(", ", removed_worlds)}");
}

// confirmation
Write("This will create/override 'j2as' files for the included worlds. Please type [Y/y] to continue: ");
var input = ReadLine();
if (input != "Y" && input != "y")
{
    WriteLine("Aborted!");
    return;
}

// remove all '// EXCLUDE ...' comments from global.as contents
global_as = exclusion_rgx.Replace(global_as, "");
global_as = $"// This file was automatically generated from '{SCRIPT_FILENAME}' - modify that file and rerun the generator\n" 
    + $"// Generated at: {DateTime.Now:yyyy-MM-dd HH:mm:ss} | Contact: cryshana@cryshana.me\n\n"
    + global_as;

// create j2as files
int index = 0;
foreach (var world in filtered_worlds)
{
    index++;

    Write($" [{index,3}/{filtered_worlds.Length}] {Path.GetFileNameWithoutExtension(world), -18}");

    var j2as_path = Path.ChangeExtension(world, "j2as");
    File.WriteAllText(j2as_path, global_as, Encoding.UTF8);

    WriteLine("done");
}

WriteLine("Sync finished!");