# EssentialsPlus #

![](https://img.shields.io/badge/Version-1.3.0-blue.svg) ![](https://img.shields.io/badge/API-2.1-green.svg)

Essentials+ is a combination of things from Essentials and things from MoreAdminCommands made better. All commands run asynchronously.
It does not include Sign Commands.

## Commands ##

    /find -> takes a variety of subcommands:
        -command -> Searches for a specific command based on input, returning matching commands and their permissions.
        -item -> Searches for a specific item based on input, returning matching items and their IDs.
        -tile -> Searches for a specific tile based on input, returning matching tiles and their IDs.
        -wall -> Searches for a specific wall based on input, returning matching walls and their IDs.
    /freezetime -> Freezes and unfreezes time.
    /delhome <home name> -> Deletes a home specified by <home name>.
    /sethome <home name> -> Sets a home named <home name>.
    /myhome <home name> -> Teleports you to your house named <home name>.
    /kickall <flag> <reason> -> Kicks every player for <reason>. Valid flag: -nosave -> kick doesn't save SSC inventory.
    /= -> Repeats your last entered command (not including other iterations of /=).
	/more -> Maximizes item stack of held item. Subcommands:
		all -> Maximizes all stackable items in the player's inventory
    /mute -> Overwrites TShock's /mute. Has subcommands:
        add <name> <time> -> Adds a mute on user with name <name> for <time>
        delete <name> -> Removes a mute on user with name <name>
        help -> Outputs command info
    /pvp -> Enables/disables PvP status
    /ruler [1|2] -> Measures distance between points 1 and 2.
    /sudo [flag] <player> <command> -> Attempts to make <player> execute <command>. Valid flag: -force -> forces the command to be run, independent of <player>'s permissions. Players with the essentials.sudo.super permission can use /sudo on anyone.
    /timecmd [flag] <time> <command> -> Makes <command> execute after <time>. Valid flag: -repeat -> Makes <command> repeat every <time>
    /back [steps] -> Takes you back to your previous position. If [steps] is supplied, attempts to take you back to your positions [steps] steps ago.
    /down [levels] -> Attempts to move you down the map. If [levels] is specified, attempts to move you down [levels] times.
    /left [levels] -> Same as /down [levels], but to the left.
    /right [levels] -> Same as /down [levels], but to the right.
    /up [levels] -> Same as /down [levels], but upwards.

## Permissions ##

	essentials.find -> Grants access to the /find command.
	essentials.freezetime -> Grants access to the /freezetime command.
	essentials.home.delete -> Grants access to the /delhome and /sethome commands.
	essentials.home.tp -> Grants access to the /myhome command.
	essentials.kickall -> Grants access to the /kickall command.
	essentials.lastcommand -> Grants access to the /= command.
	essentials.more -> Grants access to the /more command.
	essentials.mute -> Grants access to the improved /mute command.
	essentials.pvp -> Grants access to the /pvp command.
	essentials.ruler -> Grants access to the /ruler command.
	essentials.send -> Grants access to the /send command.
	essentials.sudo -> Grants access to the /sudo command.
	essentials.sudo.force -> Extends the capabilities of sudo.
	essentials.sudo.super -> Allows sudo to be used on anyone.
	essentials.sudo.invisible -> Causes sudo'd commands to be executed invisibly.
	essentials.timecmd -> Grants access to the /timecmd command.
	essentials.tp.back -> Grants access to the /back command.
	essentials.tp.down -> Grants access to the /down command.
	essentials.tp.left -> Grants access to the /left command.
	essentials.tp.right -> Grants access to the /right command.
	essentials.tp.up -> Grants access to the /up command.
