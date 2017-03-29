# Redis.NetCore
--
### Async all the way .Net Core client for Redis
Why another .Net Redis client?  Well I was having issues with some of the others when using Async and .Net Core... so I created one (of cource).  The goal is to be unencumbered by the past, specifically targetting Async only and .Net Core.  So please do not ask to add sync versions of APIs or test on anything other than .Net Core projects (although I have done some light testing with .Net 4.62 full framework).

Installation
--

Redis.NetCore is on nuget:

[https://www.nuget.org/packages/Redis.NetCore](https://www.nuget.org/packages/Redis.NetCore)

    Install-Package Redis.NetCore -Pre 

Features
--

- Async all the way
- Connection pool with multiplexed connections
- Pipelines
- Current support of redis feature set (Strings, Keys, Connection) (working on adding full support over time.)


Basic Usage
--

Coming soon...

Contributing
--

1. Please submit an issue first!
2. If appropriate, submit a pull request.