
# Hermes - File Search Tool

Hermes is a command-line utility designed for efficient file search operations across directories and drives. It utilizes the Levenshtein distance algorithm to match file names based on a specified precision level, allowing for fuzzy searches where exact matches might not be possible.


## Features

- **Search Across Directories and Drives:** Hermes can search within a specified directory or across all available drives.
- **Fuzzy Matching:** Utilize the precision feature to find files that are similar but not identical to the search query.
- **Concurrent Search:** Hermes leverages parallel processing to speed up the search across multiple directories.
- **Customizable Output:** Displays the top 10 best-matching files based on search precision.


## Usage

```pwsh
Hermes <path> <file_name> <precision>
```

- **<path>**: The directory or drive to search in. Use ALL_DRIVES to search across all available drives.

- **<file_name>**: The name of the file you want to search for. Wildcards are not supported, but you can specify partial names.

- **<precision>**: An integer between 0 and 100 representing the search precision. A lower precision value allows for more lenient matches.

## Example

To search for a file named document.txt in the C:\Users\ directory with a 75% match precision:

```pwsh
Hermes "C:\Users\" "document.txt" 75%
```

```pwsh
Hermes ALL_DRIVES "document.txt" 50%
```
## Installation

To use Hermes, you need to have .NET installed on your system. Once installed, compile the program using:

```pwsh
git clone https://github.com/VincentBrodin/Hermes.git
dotnet build
```

Add the bin folder to your PATH enviroment varibles to use it.
    
