//=======================================================
// DEFINE PARAMETERS
//=======================================================

// Define the required parameters
var Parameters = new Dictionary<string, object>();
Parameters["SolutionName"] = "Orc.EntityFramework";
Parameters["Company"] = "WildGums";
Parameters["RepositoryUrl"] = string.Format("https://github.com/{0}/{1}", GetBuildServerVariable("Company"), GetBuildServerVariable("SolutionName"));
Parameters["StartYear"] = "2014";

// Note: the rest of the variables should be coming from the build server,
// see `/deployment/cake/*-variables.cake` for customization options
// 
// If required, more variables can be overridden by specifying them via the 
// Parameters dictionary, but the build server variables will always override
// them if defined by the build server. For example, to override the code
// sign wild card, add this to build.cake
//
// Parameters["CodeSignWildcard"] = "Orc.EntityFramework";

Parameters["CodeSignWildcard"] = "Orc.EntityFramework";

//=======================================================
// DEFINE COMPONENTS TO BUILD / PACKAGE
//=======================================================

var ComponentsToBuild = new string[]
{
    "Orc.EntityFramework5",
    "Orc.EntityFramework6"
};

//=======================================================
// DEFINE WEB APPS TO BUILD / PACKAGE
//=======================================================

var WebAppsToBuild = new string[]
{

};

//=======================================================
// DEFINE WPF APPS TO BUILD / PACKAGE
//=======================================================

var WpfAppsToBuild = new string[]
{

};

//=======================================================
// DEFINE UWP APPS TO BUILD / PACKAGE
//=======================================================

var UwpAppsToBuild = new string[]
{

};

//=======================================================
// DEFINE TEST PROJECTS TO BUILD
//=======================================================

var TestProjectsToBuild = new string[]
{
    "Orc.EntityFramework5.Tests",
    "Orc.EntityFramework6.Tests"
};

//=======================================================
// REQUIRED INITIALIZATION, DO NOT CHANGE
//=======================================================

// Now all variables are defined, include the tasks, that
// script will take care of the rest of the magic

#l "./deployment/cake/tasks.cake"