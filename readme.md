Sitecore.Mvc.TestingHotfix
==========================

A/B and MV testing don't work in Sitecore 7.2 when using MVC.  You will always see the original content, not the variations configured for your test.  I logged this issue the Sitecore support and they confirmed that it is a bug.  This is an unofficial hotfix.

There is really only one line of code that needed changing, but because that line was in a pipeline processor that was inherited by another processor, both had to be fixed.  The problem is in the `Sitecore.Mvc.Analytics.Pipelines.Response.CustomizeRendering.SelectVariation` class.
The evaluate method tries to get the test variable item with this line of code:

```C#
Item variableItem = args.PageContext.Database.GetItem(renderingReference.Settings.MultiVariateTest);
```
However, renderingReference.Settings.MultiVariateTest is not a simple ID, it is actually a query string language names as keys and IDs as values.
The variable item is always null as a result and the method aborts.  This problem can be fixed by using the APIs from the core library:

```C#
Item variableItem = args.PageContext.Database.GetItem(renderingReference.Settings.GetMultiVariateTestForLanguage(Context.Language));
```

[Download the package](Sitecore.Mvc.TestingHotfix-1.0.zip?raw=true)
