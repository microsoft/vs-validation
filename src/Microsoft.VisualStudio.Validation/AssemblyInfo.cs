using System.Resources;
using System.Security;

#if NETFRAMEWORK || NETSTANDARD2_0
[assembly: NeutralResourcesLanguage("en-US", UltimateResourceFallbackLocation.MainAssembly)]
#else
[assembly: NeutralResourcesLanguage("en-US")]
#endif

[assembly: AllowPartiallyTrustedCallers]
