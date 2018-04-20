using System.Resources;
using System.Security;

#if NET45
[assembly: NeutralResourcesLanguage("en-US", UltimateResourceFallbackLocation.MainAssembly)]
#else
[assembly: NeutralResourcesLanguage("en-US")]
#endif

[assembly: SecurityTransparent]
