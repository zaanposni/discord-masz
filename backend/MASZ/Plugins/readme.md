# Plugins in MASZ

Please follow the existing examples to create your own plugin.

## Requirements

Your service has to

- be in the namespace `MASZ.Plugins`.
- implement the interface `MASZ.Plugins.IBasePlugin` and inherit the class `MASZ.Plugins.BasePlugin`.
- set the `ENABLE_CUSTOM_PLUGINS` env var to `true` in your `.env` file.

## Access MASZ resources

You can either

- use the current eventArgs to quickly access the current resource.
- use the protected fields of `MASZ.Plugins.BasePlugin` to access internal MASZ stuff like the Discord API Wrapper `_discordAPI`.

## Init

The `void Init()` method of your plugin will be called upon startup of MASZ.\
It is recommended to do all relevant stuff here.

## Errors

Errors in your plugins will not crash masz but it is recommended to dispense the use of error-prone operations.

## Execution time

Your plugin can take as long as you want to handle an incoming event.\
As your plugin/the event subscriptions are invoked in a different thread, MASZ wont be blocked.

## Custom service

Your plugin does not need to listen on MASZ events.\
You can also do your own background stuff like monitoring or cleaning database content.\
Your service is registered as a singleton.

## Events

You can find all subscribable events in the _eventHandler property.\
For example `_eventHandler.OnIdentityRegistered`.

## Deployment

You have to rebuild your backend image when using custom plugins.\
Either overwrite the image tag or use `docker-compose -f docker-compose-dev.yml up --force-recreate --build` to locally build images.\
Be sure to set `ENABLE_CUSTOM_PLUGINS=true` in your `.env` file.
