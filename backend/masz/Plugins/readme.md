# Plugins in MASZ

Please follow the existing examples to create your own plugin.

## Requirements

Your service has to

- be in the namespace `masz.Plugins`.
- implement the interface `masz.Plugins.IBasePlugin` and inherit the class `masz.Plugins.BasePlugin`.
- set the `ENABLE_CUSTOM_PLUGINS` env var to `true` in your `.env` file.

## Access masz resources

You can either

- use the current eventArgs to quickly access the current resource.
- use the protected fields of `masz.Plugins.BasePlugin` to access internal masz stuff like the Discord API Wrapper `_discordAPI`.

## Init

The `void Init()` method of your plugin will be called upon startup of masz.\
It is recommended to do all relevant stuff here.

## Errors

Errors in your plugins will not crash masz but it is recommended to dispense the use of error-prone operations.

## Execution time

Your plugin can take as long as you want to handle an incoming event.\
However, be aware that you will block the currently executed web API request or slash command or similiar.\
It is recommded for plugins to only execute small task like logging or creating statistics.\
If you need to do longer lasting tasks, please refer to the `ExampleBackgroundPlugin.cs`.

## Custom service

Your plugin does not need to listen on masz events.\
You can also do your own background stuff like monitoring or cleaning database content.\
Your service is registered as a singleton.

## Events

You can find all subscribable events in the _eventHandler property.\
For example `_eventHandler.OnIdentityRegistered`.
