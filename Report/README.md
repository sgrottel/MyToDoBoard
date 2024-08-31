# MyToDoBoard™ Report Tool
This is a _local_, _offline_ Desktop App for a personal, _non-shared_ ToDo Board.

https://github.com/sgrottel/MyToDoBoard

MyToDoReport is a command line utility to read the `.mytodo` data file and to produce a report of it's content.

Version ...

## Usage
The tool `MyToDoReport.exe` can print a help description via the `-h` command line option.

A typical command line will read:
```
MyToDoReport.exe C:\path\to\your\file.mytodo -f
```

This will generate (or overwrite, because of `-f`) the report file `C:\path\to\your\file.html`.

## Build Instructions
This tool is part of the [MyToDoBoard solution](https://github.com/sgrottel/MyToDoBoard).

1. Checkout the git repository, or obtain a complete copy of the source code via other means.
2. Open `MyToDoBoard.sln` in a recent Visual Studio
   - Community Edition is sufficient
   - Ensure the CSharp sdk/tools/etc. are available, as well as Nuget support
3. Restore dependencies of the solution via Nuget
4. Build the `Report` project.
   - It's dependencies should build automatically as well
5. (Optionally) publish the `Report` project.
   - The final binaries are then available in the `bin` subdirectory.

## License
This project is freely available under the terms of the Apache License v.2.0

> Copyright 2022-2024 SGrottel
> 
> Licensed under the Apache License, Version 2.0 (the "License");
> you may not use this file except in compliance with the License.
> You may obtain a copy of the License at
>
> http://www.apache.org/licenses/LICENSE-2.0
>
> Unless required by applicable law or agreed to in writing, software
> distributed under the License is distributed on an "AS IS" BASIS,
> WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
> See the License for the specific language governing permissions and
> limitations under the License.

For more information see the [LICENSE](./LICENSE) file.

