//=======================================================
// DEFINE PARAMETERS
//=======================================================

// Define the required parameters
var Parameters = new Dictionary<string, object>();
Parameters["SolutionName"] = "Orc.EntityFramework";
Parameters["Company"] = "WildGums";
Parameters["RepositoryUrl"] = string.Format("https://github.com/{0}/{1}", GetBuildServerVariable("Company"), GetBuildServerVariable("SolutionName"));
Parameters["StartYear"] = "2014";
Parameters["UseVisualStudioPrerelease"] = "false";

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

Dependencies.Add("Orc.EntityFramework5.Tests.DbContext");
Dependencies.Add("Orc.EntityFramework5.Tests.ObjectContext");
Dependencies.Add("Orc.EntityFramework6.Tests.DbContext");

Components.Add("Orc.EntityFramework5");
Components.Add("Orc.EntityFramework6");

TestProjects.Add("Orc.EntityFramework5.Tests");
TestProjects.Add("Orc.EntityFramework6.Tests");

//=======================================================
// REQUIRED INITIALIZATION, DO NOT CHANGE
//=======================================================

// Now all variables are defined, include the tasks, that
// script will take care of the rest of the magic

#l "./deployment/cake/tasks.cake"