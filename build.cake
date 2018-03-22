var projectName = "Orc.EntityFramework";
var projectsToPackage = new [] { "Orc.EntityFramework5", "Orc.EntityFramework6" };
var company = "WildGums";
var startYear = 2010;
var defaultRepositoryUrl = string.Format("https://github.com/{0}/{1}", company, projectName);

#l "./deployment/cake/variables.cake"
#l "./deployment/cake/tasks.cake"
