# TriggersTools.DiscordBots.Analyzer

This is an optional analyzer that corrects the usage of `[Require(Something)]` `PreconditionAttributes` and changes it to the `[Requires(Something)]` `PreconditionAttribute`.

The attributes with the added `s` now return a `PreconditionAttributeResult` which returns the failing attribute.