# BBK Solutions - ASP.NET Core Version
* Start from here: [https://aspnetboilerplate.com/Pages/Documents](https://aspnetboilerplate.com/Pages/Documents)

# Installation for development
* Visual Studio 2022
* node, npm, yarn, gulp

# First build/run
* Create an appsettings.json (copy from appsettings.development.json)
* Go to Development PowerShell (Ctrl + `)
* Ensure all package were loaded from nuget by run command: dotnet restore
* Ensure all node_modules were loaded: yarn install
* Build all javascript, css bundles: guilp buildDev 
